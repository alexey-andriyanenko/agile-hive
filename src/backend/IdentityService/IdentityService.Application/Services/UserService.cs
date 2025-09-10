using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IdentityService.Application.Exceptions;
using IdentityService.Contracts;
using IdentityService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Services;

public class UserService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor) : Contracts.UserService.UserServiceBase
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

        return new UserDto()
        {
            Id = user.Id.ToString(),
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
        };
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
        
        var userDtos = users.Select(u => new UserDto
        {
            Id = u.Id.ToString(),
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email
        }).ToList();

        return new GetManyUsersByIdsResponse()
        {
            Users = { userDtos }
        };
    }
}