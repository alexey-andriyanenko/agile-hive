using FluentValidation;
using Grpc.Core;
using IdentityService.Application.Exceptions;
using IdentityService.Contracts.Protos;
using IdentityService.Domain.Constants;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Services;

public class AuthService(ApplicationDbContext dbContext,
    JwtTokenGenerator jwtTokenGenerator,
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

        var token = jwtTokenGenerator.GenerateToken(user);
        
        return new RegisterResponse()
        {
            UserId = user.Id.ToString(),
            Token = token
        };
    }

    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var validationResult = loginRequestValidator.Validate(request);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var passwordHasher = new PasswordHasher<User>();
        
        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Email == request.Email);

        if (user is null)
        {
            throw new UserNotFoundException(request.Email);
        }
        
        
        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedException();
        }
        
        var  token = jwtTokenGenerator.GenerateToken(user);

        return new LoginResponse()
        {
            UserId = user.Id.ToString(),
            Token = token
        };
    }
}
