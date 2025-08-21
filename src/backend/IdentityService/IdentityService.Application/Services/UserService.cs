using Grpc.Core;
using IdentityService.Application.Exceptions;
using IdentityService.gRPC;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Services;

public class UserService(ApplicationDbContext dbContext) : gRPC.UserService.UserServiceBase
{
    public override async Task<UserDto> GetById(GetUserByIdRequest request, ServerCallContext context)
    {
        if (Guid.TryParse(request.UserId, out var userId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"UserId '{request.UserId}' is not a valid GUID."));
        }
        
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
        var userIds = new List<Guid>();
        
        foreach (var userId in request.UserIds)
        {
            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"UserId '{userId}' is not a valid GUID."));
            }
            
            userIds.Add(parsedUserId);
        }

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