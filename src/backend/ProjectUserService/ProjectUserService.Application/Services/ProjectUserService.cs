using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IdentityService.Contracts;
using ProjectService.Contracts;
using ProjectUserService.Contracts;
using AddManyUsersToProjectRequest = ProjectUserService.Contracts.AddManyUsersToProjectRequest;
using AddUserToProjectRequest = ProjectUserService.Contracts.AddUserToProjectRequest;
using RemoveManyUsersFromProjectRequest = ProjectUserService.Contracts.RemoveManyUsersFromProjectRequest;
using RemoveUserFromProjectRequest = ProjectUserService.Contracts.RemoveUserFromProjectRequest;

namespace ProjectUserService.Application.Services;

public class ProjectUserService(
    UserService.UserServiceClient userServiceClient,
    ProjectMemberService.ProjectMemberServiceClient projectMemberServiceClient
    ) : Contracts.ProjectUserService.ProjectUserServiceBase
{
    public override async Task<ProjectUserDto> GetById(GetProjectUserByIdRequest request, ServerCallContext context)
    {
        var result = await GetManyByIds(new GetManyProjectUsersByIdsRequest()
        {
            ProjectId = request.ProjectId,
            UserIds = { request.UserId }
        }, context);
        
        return result.Users.Single();
    }
    
    public override async Task<GetManyProjectUsersByIdsResponse> GetManyByIds(GetManyProjectUsersByIdsRequest request, ServerCallContext context)
    {
        var members = await projectMemberServiceClient.GetManyByIdsAsync(
            new GetManyProjectMembersByIdsRequest()
            {
                ProjectId = request.ProjectId,
                UserIds = { request.UserIds }
            }).ResponseAsync;
        var notFoundMemberIds = request.UserIds.Except(members.Members.Select(m => m.UserId)).ToList();
        
        if (notFoundMemberIds.Count > 0)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Project members not found: {string.Join(", ", notFoundMemberIds)}"));
        }

        var memberDict = members.Members.ToDictionary(m => m.UserId, m => m);
        var usersResponse = await userServiceClient.GetManyByIdsAsync(new GetManyUsersByIdsRequest()
        {
            UserIds = { request.UserIds }
        });
        
        return new GetManyProjectUsersByIdsResponse()
        {
            Users =
            {
                usersResponse.Users.Select(user => new ProjectUserDto()
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Role = (Contracts.ProjectUserRole)memberDict[user.Id].Role,
                })
            }
        };
    }

    public override async Task<GetManyProjectUsersResponse> GetMany(GetManyProjectUsersRequest request, ServerCallContext context)
    {
        var members = await projectMemberServiceClient.GetManyAsync(
            new GetManyProjectMembersRequest()
            {
                ProjectId = request.ProjectId
            }).ResponseAsync;
        
        var userIds = members.Members.Select(m => m.UserId).ToList();
        var usersResponse = await userServiceClient.GetManyByIdsAsync(new GetManyUsersByIdsRequest()
        {
            UserIds = { userIds }
        });
        var usersById = usersResponse.Users.ToDictionary(u => u.Id, u => u);
        
        var result = new GetManyProjectUsersResponse();
        foreach (var member in members.Members)
        {
            if (usersById.ContainsKey(member.UserId))
            {
                var user = usersById[member.UserId];
                
                result.Users.Add(new ProjectUserDto()
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Role = (Contracts.ProjectUserRole)member.Role,
                });
            }
        }

        return result;
    }

    public override async Task<ProjectUserDto> Add(AddUserToProjectRequest request, ServerCallContext context)
    {
        var member = await projectMemberServiceClient.AddToProjectAsync(new ProjectService.Contracts.AddUserToProjectRequest()
        {
            ProjectId = request.ProjectId,
            UserId = request.UserId,
            Role = (ProjectService.Contracts.ProjectMemberRole)request.Role
        });
        
        var user = await userServiceClient.GetByIdAsync(new GetUserByIdRequest()
        {
            UserId = request.UserId
        });
        
        return new ProjectUserDto()
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Role = (Contracts.ProjectUserRole)member.Role,
        };
    }

    public override async Task<Contracts.AddManyUsersToProjectResponse> AddMany(AddManyUsersToProjectRequest request, ServerCallContext context)
    {
        if (request.Users.Count == 0)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Users list cannot be empty."));
        }
        
        var membersResponse = await projectMemberServiceClient.AddManyToProjectAsync(new ProjectService.Contracts.AddManyUsersToProjectRequest()
        {
            ProjectId = request.ProjectId,
            Users =
            {
                request.Users.Select(user => new ProjectService.Contracts.AddUserToProjectMessage()
                {
                    UserId = user.UserId,
                    Role = (ProjectService.Contracts.ProjectMemberRole)user.Role
                })
            }
        });
        
        var userIds = request.Users.Select(u => u.UserId).ToList();
        var usersResponse = await userServiceClient.GetManyByIdsAsync(new GetManyUsersByIdsRequest()
        {
            UserIds = { userIds }
        });
        
        var membersById = membersResponse.Members.ToDictionary(m => m.UserId, m => m);
        var usersById = usersResponse.Users.ToDictionary(u => u.Id, u => u);
        
        
        var result = new Contracts.AddManyUsersToProjectResponse();
        
        foreach (var userId in userIds)
        {
            if (usersById.ContainsKey(userId) && membersById.ContainsKey(userId))
            {
                var user = usersById[userId];
                var member = membersById[userId];
                
                result.Users.Add(new ProjectUserDto()
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Role = (Contracts.ProjectUserRole)member.Role,
                });
            }
        }

        return result;
    }

    public override async Task<ProjectUserDto> Update(UpdateProjectUserRequest request, ServerCallContext context)
    {
        var member = await projectMemberServiceClient.UpdateRoleAsync(new UpdateProjectMemberRoleRequest()
        {
            ProjectId = request.ProjectId,
            UserId = request.UserId,
            Role = (ProjectService.Contracts.ProjectMemberRole)request.Role
        }).ResponseAsync;
        
        var user = await userServiceClient.GetByIdAsync(new GetUserByIdRequest()
        {
            UserId = request.UserId
        });
        
        return new ProjectUserDto()
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Role = (Contracts.ProjectUserRole)member.Role,
        };
    }

    public override async Task<Empty> Remove(RemoveUserFromProjectRequest request, ServerCallContext context)
    {
        return await projectMemberServiceClient.RemoveFromProjectAsync(new ProjectService.Contracts.RemoveUserFromProjectRequest()
        {
            ProjectId = request.ProjectId,
            UserId = request.UserId
        });
    }

    public override async Task<Empty> RemoveMany(RemoveManyUsersFromProjectRequest request, ServerCallContext context)
    {
        return await projectMemberServiceClient.RemoveManyFromProjectAsync(new ProjectService.Contracts.RemoveManyUsersFromProjectRequest()
        {
            ProjectId = request.ProjectId,
            UserIds = { request.UserIds }
        });
    }
}