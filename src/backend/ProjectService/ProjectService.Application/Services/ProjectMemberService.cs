using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IdentityService.gRPC;
using Microsoft.EntityFrameworkCore;
using ProjectService.gRPC;
using ProjectService.Infrastructure.Data;

namespace ProjectService.Application.Services;

public class ProjectMemberService(ApplicationDbContext dbContext, UserService.UserServiceClient userServiceClient) : gRPC.ProjectMemberService.ProjectMemberServiceBase
{
    public override async Task<Empty> AddToProject(AddUserToProjectRequest request, ServerCallContext context)
    {
        var projectId = Guid.Parse(request.ProjectId);
        var project = await dbContext.Projects
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == projectId);
        
        if (project is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Project with ID '{request.ProjectId}' not found."));
        }
        
        var userId = Guid.Parse(request.UserId);
        var user = await userServiceClient.GetByIdAsync(new GetUserByIdRequest { UserId = request.UserId });
        
        if (user is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"User with ID '{request.UserId}' not found."));
        }
        
        var projectMember = new Domain.Entities.ProjectMember
        {
            ProjectId = projectId,
            UserId = userId,
            Role = (Domain.Enums.ProjectMemberRole)request.Role,
        };
        
        project.Members.Add(projectMember);
        
        await dbContext.SaveChangesAsync();
        
        return new Empty();
    }

    public override async Task<Empty> AddManyToProject(AddManyUsersToProjectRequest request, ServerCallContext context)
    {
        var projectId = Guid.Parse(request.ProjectId);
        var userIds = request.Users.Select(user => Guid.Parse(user.UserId)).ToList();
        var project = await dbContext.Projects
            .Include(p => p.Members)
            .SingleOrDefaultAsync(x => x.Id == Guid.Parse(request.ProjectId));

        if (project is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Project with ID '{request.ProjectId}' not found."));
        }
        
        var users = userServiceClient.GetManyByIds(new GetManyUsersByIdsRequest
        {
            UserIds =
            {
                request.Users.Select(user => user.UserId)
            }
        });
        var usersById = users.Users.ToDictionary(u => Guid.Parse(u.Id), u => u);
        var usersToAddById = request.Users.ToDictionary(u => Guid.Parse(u.UserId), u => u);
        
        foreach (var userId in userIds)
        {
            if (!usersById.ContainsKey(userId))
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"User with ID '{userId}' not found."));
            }
            
            var projectMember = new Domain.Entities.ProjectMember
            {
                ProjectId = projectId,
                UserId = userId,
                Role = (Domain.Enums.ProjectMemberRole)usersToAddById[userId].Role,
            };
            
            project.Members.Add(projectMember);
        }
        
        dbContext.Projects.Update(project);
        
        await dbContext.SaveChangesAsync();
        
        return new Empty();
    }

    public override async Task<Empty> RemoveFromProject(RemoveUserFromProjectRequest request, ServerCallContext context)
    {
        var projectId = Guid.Parse(request.ProjectId);
        var userId = Guid.Parse(request.UserId);
        
        var projectMember = dbContext.ProjectMembers
            .SingleOrDefault(x => x.ProjectId == projectId && x.UserId == userId);
        
        if (projectMember is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Project member with User ID '{request.UserId}' not found in Project ID '{request.ProjectId}'."));
        }
        
        dbContext.ProjectMembers.Remove(projectMember);
        
        await dbContext.SaveChangesAsync();
        
        return new Empty();
    }

    public override async Task<Empty> RemoveManyFromProject(RemoveManyUsersFromProjectRequest request, ServerCallContext context)
    {
        var projectId = Guid.Parse(request.ProjectId);
        var userIds = request.UserIds.Select(Guid.Parse).ToList();
        
        var projectMembers = await dbContext.ProjectMembers
            .Where(x => x.ProjectId == projectId && userIds.Contains(x.UserId))
            .ToListAsync();
        
        var notFoundMemberIds = userIds
            .Where(userId => projectMembers.All(pm => pm.UserId != userId))
            .ToList();
        
        if (notFoundMemberIds.Count > 0)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Project members with User IDs '{string.Join(", ", notFoundMemberIds)}' not found in Project ID '{request.ProjectId}'."));
        }
        
        dbContext.ProjectMembers.RemoveRange(projectMembers);
        
        await dbContext.SaveChangesAsync();
        
        return new Empty();
    }
}
