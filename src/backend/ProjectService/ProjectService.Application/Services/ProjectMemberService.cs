using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IdentityService.Contracts;
using Microsoft.EntityFrameworkCore;
using ProjectService.Application.Mappings;
using ProjectService.Contracts;
using ProjectService.Infrastructure.Data;

namespace ProjectService.Application.Services;

public class ProjectMemberService(ApplicationDbContext dbContext, UserService.UserServiceClient userServiceClient) : Contracts.ProjectMemberService.ProjectMemberServiceBase
{
    public override async Task<ProjectMemberDto> Get(GetProjectMemberRequest request, ServerCallContext context)
    {
        var projectId = Guid.Parse(request.ProjectId);
        var userId = Guid.Parse(request.UserId);
        
        var projectMember = await dbContext.ProjectMembers
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.ProjectId == projectId && x.UserId == userId);
        
        if (projectMember is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Project member with Project ID '{request.ProjectId}' and User ID '{request.UserId}' not found."));
        }

        return projectMember.ToDto();
    }

    public override async Task<GetManyProjectMembersResponse> GetMany(GetManyProjectMembersRequest request, ServerCallContext context)
    {
        var query = dbContext.ProjectMembers.AsNoTracking().Where(x => x.ProjectId == Guid.Parse(request.ProjectId));
        
        if (request.UserIds.Count > 0)
        {
            var userIds = request.UserIds.Select(Guid.Parse).ToList();
            query = query.Where(x => userIds.Contains(x.UserId));
        }
        
        var projectMembers = await query.ToListAsync();
        
        return new GetManyProjectMembersResponse()
        {
            Members =
            {
                projectMembers.Select(x => x.ToDto())
            }
        };
    }

    public override async Task<Empty> AddToProject(AddUserToProjectRequest request, ServerCallContext context)
    {
        var addManyRequest = new AddManyUsersToProjectRequest
        {
            ProjectId = request.ProjectId,
            Users =
            {
                new AddUserToProjectMessage()
                {
                    UserId = request.UserId,
                    Role = request.Role
                }
            }
        };

        return await AddManyToProject(addManyRequest, context);
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
        var removeManyRequest = new RemoveManyUsersFromProjectRequest
        {
            ProjectId = request.ProjectId,
            UserIds = { request.UserId }
        };
        
        return await RemoveManyFromProject(removeManyRequest, context);
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
