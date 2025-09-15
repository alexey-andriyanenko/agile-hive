using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IdentityMessages.Messages;
using IdentityService.Application.Exceptions;
using IdentityService.Application.Mappings;
using IdentityService.Contracts;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure;
using IdentityService.Infrastructure.Data;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Services;

public class UserService(
    ApplicationDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    IPublishEndpoint publishEndpoint,
    IValidator<CreateUserRequest> createUserValidator,
    IValidator<UpdateUserRequest> updateUserValidator) : Contracts.UserService.UserServiceBase
{
    public override Task<UserDto> GetMe(Empty request, ServerCallContext context)
    {
        var userContext = (UserContext)httpContextAccessor.HttpContext!.Items["UserContext"]!;
        return GetById(new GetUserByIdRequest()
        {
            UserId = userContext.UserId.ToString()
        }, context);
    }

    public override async Task<UserDto> GetById(GetUserByIdRequest request, ServerCallContext context)
    {
        var userId = Guid.Parse(request.UserId);
        var user = await dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            throw new UserNotFoundException(userId);
        }

        return user.ToDto();
    }

    public override async Task<GetManyUsersByIdsResponse> GetManyByIds(GetManyUsersByIdsRequest request, ServerCallContext context)
    {
        var userIds = request.UserIds.Select(Guid.Parse);
        var users = await dbContext.Users
            .AsNoTracking()
            .Where(x => userIds.Contains(x.Id))
            .ToListAsync();
        
        var notFoundUserIds = userIds.Except(users.Select(u => u.Id)).ToList();
        
        if (notFoundUserIds.Count > 0)
        {
            throw new UserNotFoundException(notFoundUserIds.First());
        }
        
        var userDtos = users.Select(u => u.ToDto()).ToList();

        return new GetManyUsersByIdsResponse()
        {
            Users = { userDtos }
        };
    }

    public override async Task<UserDto> Create(CreateUserRequest request, ServerCallContext context)
    { 
        var validationResult = await createUserValidator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.UserName,
        };

        var passwordHasher = new PasswordHasher<User>();
        var passwordHash = passwordHasher.HashPassword(user, request.Password);
        user.PasswordHash = passwordHash;
        
        dbContext.Users.Add(user);

        try
        {
            await dbContext.SaveChangesAsync();
            
            await publishEndpoint.Publish(new UserCreationSucceededMessage()
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
            });

            return user.ToDto();
        }
        catch (DbUpdateException e)
        {
            await publishEndpoint.Publish(new UserCreationFailedMessage()
            {
                ErrorMessage = e.Message,
            });

            throw;
        }
    }

    public override async Task<UserDto> Update(UpdateUserRequest request, ServerCallContext context)
    {
        var validationResult = await updateUserValidator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var userId = Guid.Parse(request.UserId);
        var user = await dbContext.Users
            .SingleOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            throw new UserNotFoundException(userId);
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.UserName = request.UserName;
        
        var passwordHasher = new PasswordHasher<User>();
        var passwordHash = passwordHasher.HashPassword(user, request.Password);
        user.PasswordHash = passwordHash;

        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();

        return user.ToDto();
    }
}