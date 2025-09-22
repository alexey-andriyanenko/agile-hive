using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IdentityService.Contracts;
using Microsoft.EntityFrameworkCore;
using ProjectService.Application.Mappings;
using ProjectService.Contracts;
using ProjectService.Infrastructure;
using ProjectService.Infrastructure.Data;

namespace ProjectService.Application.Services;

public class ProjectMemberService(ApplicationDbContext dbContext, UserService.UserServiceClient userServiceClient) : Contracts.ProjectMemberService.ProjectMemberServiceBase
{
    public override async Task<ProjectMemberDto> GetById(GetProjectMemberByIdRequest request, ServerCallContext context)
    {
        var res = await GetManyByIds(new GetManyProjectMembersByIdsRequest()
        {
            ProjectId = request.ProjectId,
            UserIds = { request.UserId }
        }, context);

        return res.Members.Single();
    }

    public override async Task<GetManyProjectMembersByIdsResponse> GetManyByIds(GetManyProjectMembersByIdsRequest request, ServerCallContext context)
    {
        var project = await dbContext.Projects
            .Include(p => p.Members)
            .SingleOrDefaultAsync(x => x.Id == Guid.Parse(request.ProjectId));
        
        if (project is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Project with ID '{request.ProjectId}' not found."));
        }
        
        var members = project.Members
            .Where(m => request.UserIds.Contains(m.UserId.ToString()))
            .ToList();
        var notFoundMemberIds = request.UserIds
            .Where(userId => members.All(m => m.UserId.ToString() != userId))
            .ToList();
        
        if (notFoundMemberIds.Count > 0)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Project members with User IDs '{string.Join(", ", notFoundMemberIds)}' not found in Project ID '{request.ProjectId}'."));
        }
        
        return new GetManyProjectMembersByIdsResponse() 
        { 
            Members = { members.Select(m => m.ToDto()) } 
        };
    }
    
    public override async Task<GetManyProjectMembersResponse> GetMany(GetManyProjectMembersRequest request, ServerCallContext context)
    {
        var project = await dbContext.Projects
            .Include(p => p.Members)
            .SingleOrDefaultAsync(x => x.Id == Guid.Parse(request.ProjectId));
        
        if (project is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Project with ID '{request.ProjectId}' not found."));
        }
        
        return new GetManyProjectMembersResponse
        {
            Members = { project.Members.Select(m => m.ToDto()) }
        };
    }

    public override async Task<ProjectMemberDto> AddToProject(AddUserToProjectRequest request, ServerCallContext context)
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

        var result = await AddManyToProject(addManyRequest, context);

        return result.Members.Single();
    }

    public override async Task<AddManyUsersToProjectResponse> AddManyToProject(AddManyUsersToProjectRequest request, ServerCallContext context)
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
        
        await dbContext.SaveChangesAsync();

        return new AddManyUsersToProjectResponse()
        {
            Members =
            {
                project.Members.Select(m => m.ToDto())
            }
        };
    }

    public override async Task<ProjectMemberDto> UpdateRole(UpdateProjectMemberRoleRequest request, ServerCallContext context)
    {
        var userContext = (UserContext)context.GetHttpContext()!.Items["UserContext"]!;
        
        var projectId = Guid.Parse(request.ProjectId);
        var userId = Guid.Parse(request.UserId);
        
        if (userId == userContext.UserId)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "Users cannot change their own project member role."));
        }
        
        var projectMember = await dbContext.ProjectMembers
            .SingleOrDefaultAsync(x => x.ProjectId == projectId && x.UserId == userId);
        
        if (projectMember is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Project member with User ID '{request.UserId}' not found in Project ID '{request.ProjectId}'."));
        }
        
        if (projectMember.Role == (Domain.Enums.ProjectMemberRole)request.Role)
        {
            return projectMember.ToDto();
        }
        
        projectMember.Role = (Domain.Enums.ProjectMemberRole)request.Role;
        
        dbContext.ProjectMembers.Update(projectMember);
        
        await dbContext.SaveChangesAsync();
        
        return projectMember.ToDto();
    }

    public override async Task<Empty> RemoveFromProject(RemoveUserFromProjectRequest request, ServerCallContext context)
    {
        var userContext = (UserContext)context.GetHttpContext()!.Items["UserContext"]!;
        
        if (Guid.Parse(request.UserId) == userContext.UserId)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "Users cannot remove themselves from a project."));
        }
        
        var removeManyRequest = new RemoveManyUsersFromProjectRequest
        {
            ProjectId = request.ProjectId,
            UserIds = { request.UserId }
        };
        
        return await RemoveManyFromProject(removeManyRequest, context);
    }

    public override async Task<Empty> RemoveManyFromProject(RemoveManyUsersFromProjectRequest request, ServerCallContext context)
    {
        var userContext = (UserContext)context.GetHttpContext()!.Items["UserContext"]!;
        
        var projectId = Guid.Parse(request.ProjectId);
        var userIds = request.UserIds.Select(Guid.Parse).ToList();
        
        if (userIds.Contains(userContext.UserId))
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "Users cannot remove themselves from a project."));
        }
        
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
