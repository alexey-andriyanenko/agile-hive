using FluentValidation;
using Grpc.Core;
using IdentityMessages.Messages;
using IdentityService.Application.Exceptions;
using IdentityService.Domain.Constants;
using IdentityService.Domain.Entities;
using IdentityService.gRPC;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrganizationMessages.Commands;

namespace IdentityService.Application.Services;

public class AuthService(ApplicationDbContext dbContext,
    TokenService tokenService,
    IValidator<RegisterRequest> registerRequestValidator,
    IValidator<LoginRequest> loginRequestValidator,
    ITopicProducer<CreateOrganizationByOwnerUserCommand> createOrganizationByOwnerUserCommandProducer,
    ITopicProducer<UserCreationSucceededMessage> userCreationSucceededMessageProducer,
    ITopicProducer<UserCreationFailedMessage> userCreationFailedMessageProducer) : Auth.AuthBase {
    public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        var validationResult = await registerRequestValidator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var role = await dbContext.Roles
            .SingleOrDefaultAsync(x => x.Id == AppRoles.Admin.Id);

        if (role is null)
        {
            throw new Exception("Admin role not found. Please ensure the application is seeded with roles.");
        }
        
        var user = new User()
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.UserName,
            RoleId = role.Id,
        };
        
        var passwordHasher = new PasswordHasher<User>();
        var passwordHash = passwordHasher.HashPassword(user, request.Password);
        user.PasswordHash = passwordHash;
        
        dbContext.Users.Add(user);

        try
        {
            await dbContext.SaveChangesAsync();
            
            await userCreationSucceededMessageProducer.Produce(new UserCreationSucceededMessage()
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
            });
        }
        catch (DbUpdateException e)
        {
            await userCreationFailedMessageProducer.Produce(new UserCreationFailedMessage()
            {
                ErrorMessage = e.Message,
            });

            throw;
        }

        if (!string.IsNullOrEmpty(request.OrganizationName))
        {
            await createOrganizationByOwnerUserCommandProducer.Produce(new CreateOrganizationByOwnerUserCommand()
            {
                OwnerUserId = user.Id,
                OrganizationName = request.OrganizationName
            });
        }

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
