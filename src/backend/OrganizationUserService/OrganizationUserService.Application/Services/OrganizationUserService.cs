using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IdentityService.Contracts;
using OrganizationService.Contracts;
using OrganizationUserService.Contracts;

namespace OrganizationUserService.Application.Services;

public class OrganizationUserService(
        UserService.UserServiceClient userServiceClient,
        OrganizationMemberService.OrganizationMemberServiceClient organizationMemberServiceClient
    ) : Contracts.OrganizationUserService.OrganizationUserServiceBase
{
    public override async Task<OrganizationUserDto> GetById(GetOrganizationUserByIdRequest request, ServerCallContext context)
    {
        var result = await GetManyByIds(new GetManyOrganizationUsersByIdsRequest()
        {
            OrganizationId = request.OrganizationId,
            UserIds = { request.UserId }
        }, context);
        
        return result.Users.Single();
    }
    
    private static  OrganizationUserDto GetOrganizationUserDto(UserDto user, OrganizationMemberDto member)
    {
        return new OrganizationUserDto()
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Role = (Contracts.OrganizationUserRole)member.Role,
        };
    }
    
    public override async Task<GetManyOrganizationUsersByIdsResponse> GetManyByIds(GetManyOrganizationUsersByIdsRequest request, ServerCallContext context)
    {
        var members = await organizationMemberServiceClient.GetManyByIdsAsync(
            new GetManyOrganizationMembersByIdsRequest()
            {
                OrganizationId = request.OrganizationId,
                UserIds = { request.UserIds }
            }).ResponseAsync;
        var notFoundMemberIds = request.UserIds.Except(members.Members.Select(m => m.UserId)).ToList();
        
        if (notFoundMemberIds.Count > 0)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Organization members not found: {string.Join(", ", notFoundMemberIds)}"));
        }

        var memberDict = members.Members.ToDictionary(m => m.UserId, m => m);
        var usersResponse = await userServiceClient.GetManyByIdsAsync(new GetManyUsersByIdsRequest()
        {
            UserIds = { request.UserIds }
        });
        
        return new GetManyOrganizationUsersByIdsResponse()
        {
            Users =
            {
                usersResponse.Users.Select(u => GetOrganizationUserDto(u, memberDict[u.Id]))
            }
        };
    }
    
    public override async Task<GetManyOrganizationUsersResponse> GetMany(GetManyOrganizationUsersRequest request, ServerCallContext context)
    {
        var members = await organizationMemberServiceClient.GetManyAsync(new GetManyOrganizationMembersRequest()
        {
            OrganizationId = request.OrganizationId
        }).ResponseAsync;

        var userIds = members.Members.Select(m => m.UserId).ToList();
        if (userIds.Count == 0)
        {
            return new GetManyOrganizationUsersResponse();
        }
        
        var usersResponse = await userServiceClient.GetManyByIdsAsync(new GetManyUsersByIdsRequest()
        {
            UserIds = { userIds }
        });
        
        var memberDict = members.Members.ToDictionary(m => m.UserId, m => m);
        
        return new GetManyOrganizationUsersResponse()
        {
            Users =
            {
                usersResponse.Users.Select(u => GetOrganizationUserDto(u, memberDict[u.Id]))
            }
        };
    }
    public override async Task<OrganizationUserDto> Create(CreateOrganizationUserRequest request, ServerCallContext context)
    {
        var user = await userServiceClient.CreateAsync(new CreateUserRequest()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.UserName
        }).ResponseAsync;
        
        await organizationMemberServiceClient.AddToOrganizationAsync(new AddUserToOrganizationRequest()
        {
            OrganizationId = request.OrganizationId,
            UserId = user.Id,
            Role = (OrganizationService.Contracts.OrganizationMemberRole)request.Role
        }).ResponseAsync;
        
        return await GetById(new GetOrganizationUserByIdRequest()
        {
            OrganizationId = request.OrganizationId,
            UserId = user.Id
        }, context);
    }

    public override async Task<OrganizationUserDto> Update(UpdateOrganizationUserRequest request, ServerCallContext context)
    {
        var user = await userServiceClient.UpdateAsync(new UpdateUserRequest()
        {
            UserId = request.UserId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.UserName
        }).ResponseAsync;

        var member = await organizationMemberServiceClient.UpdateRoleAsync(new UpdateUserRoleInOrganizationRequest()
        {
            OrganizationId = request.OrganizationId,
            UserId = request.UserId,
            Role = (OrganizationService.Contracts.OrganizationMemberRole)request.Role
        }).ResponseAsync;
        
        return GetOrganizationUserDto(user, member);
    }

    public override async Task<Empty> Remove(Contracts.RemoveUserFromOrganizationRequest request, ServerCallContext context)
    {
        return await organizationMemberServiceClient.RemoveFromOrganizationAsync(new OrganizationService.Contracts.RemoveUserFromOrganizationRequest()
        {
            OrganizationId = request.OrganizationId,
            UserId = request.UserId
        }).ResponseAsync;
    }
    
    public override async Task<Empty> RemoveMany(Contracts.RemoveManyUsersFromOrganizationRequest request, ServerCallContext context)
    {
        return await organizationMemberServiceClient.RemoveManyFromOrganizationAsync(new OrganizationService.Contracts.RemoveManyUsersFromOrganizationRequest()
        {
            OrganizationId = request.OrganizationId,
            UserIds = { request.UserIds }
        }).ResponseAsync;
    }
}