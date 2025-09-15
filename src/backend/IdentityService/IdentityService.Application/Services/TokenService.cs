using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Grpc.Core;
using IdentityService.Application.Dtos;
using IdentityService.Application.Exceptions;
using IdentityService.Contracts;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Application.Services;

public class TokenService(IConfiguration config, ApplicationDbContext dbContext) : Contracts.TokenService.TokenServiceBase
{
    public async Task<GenerateTokensResult> GenerateTokensAsync(User user)
    {
        var accessToken = GenerateAccessToken(GetClaims(user));
        var refreshToken = GenerateRefreshToken();

        dbContext.RefreshTokens.Add(new RefreshToken()
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
        });
        
        await dbContext.SaveChangesAsync();

        return new GenerateTokensResult()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        var token = await dbContext.RefreshTokens
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.Token == request.RefreshToken);

        if (token == null)
        {
            throw new RefreshTokenNotFoundException();
        }

        if (token.IsExpired)
        {
            throw new RefreshTokenExpiredException();
        }

        if (token.IsRevoked)
        {
            throw new RefreshTokenAlreadyRevokedException();
        }

        if (token.User is null)
        {
            throw new UserNotFoundException(token.UserId);
        }
        
        var newAccessToken = GenerateAccessToken(GetClaims(token.User));
        
        var newRefreshToken = new RefreshToken()
        {
            UserId = token.UserId,
            Token = GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
        };
        
        token.RevokedAt = DateTime.UtcNow;
        token.ReplacedByToken = newRefreshToken.Token;
        
        dbContext.RefreshTokens.Add(newRefreshToken);
        await dbContext.SaveChangesAsync();

        return new RefreshTokenResponse()
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
        };
    }

    private IReadOnlyList<Claim> GetClaims(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.UserName ?? "")
        };
        
        return claims;
    }
    
    private string GenerateAccessToken(IReadOnlyList<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["JwtSettings:Issuer"],
            audience: config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(config["JwtSettings:ExpiresInMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        
        return Convert.ToBase64String(randomBytes);
    }
}