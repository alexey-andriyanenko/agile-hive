using FluentValidation;
using Grpc.Core;
using IdentityService.Application.Exceptions;
using IdentityService.Domain.Constants;
using IdentityService.Domain.Entities;
using IdentityService.gRPC;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Services;

public class AuthService(ApplicationDbContext dbContext,
    TokenService tokenService,
    IValidator<RegisterRequest> registerRequestValidator,
    IValidator<LoginRequest> loginRequestValidator) : Auth.AuthBase {
    public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        var validationResult = await registerRequestValidator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var passwordHasher = new PasswordHasher<User>();
        
        var tenant = new Tenant()
        {
            Id = Guid.NewGuid(),
            Name = request.TenantName
        };

        var user = new User()
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.UserName,
            RoleId = AppRoles.Admin.Id
        };
        
        var passwordHash = passwordHasher.HashPassword(user, request.Password);
        user.PasswordHash = passwordHash;
        
        dbContext.Users.Add(user);
        dbContext.Tenants.Add(tenant);
        
        await dbContext.SaveChangesAsync();

        var tokens = await tokenService.GenerateTokensAsync(user);
        
        return new RegisterResponse()
        {
            UserId = user.Id.ToString(),
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
        };
    }

    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var validationResult = await loginRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var passwordHasher = new PasswordHasher<User>();
        
        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Email == request.Email);

        if (user is null)
        {
            throw new UserNotFoundByEmailException(request.Email);
        }
        
        
        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedException();
        }
        
        var tokens = await tokenService.GenerateTokensAsync(user);

        return new LoginResponse()
        {
            UserId = user.Id.ToString(),
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
        };
    }
}
