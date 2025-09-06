using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IdentityService.Contracts;
using Microsoft.EntityFrameworkCore;
using OrganizationService.Application.Mapping;
using OrganizationService.Contracts;
using OrganizationService.Domain.Entities;
using OrganizationService.Infrastructure.Data;

namespace OrganizationService.Application.Services;

public class OrganizationMemberService(
    ApplicationDbContext dbContext,
    UserService.UserServiceClient userServiceClient
) : Contracts.OrganizationMemberService.OrganizationMemberServiceBase
{
    public override async Task<OrganizationMemberDto> Get(GetOrganizationMemberRequest request, ServerCallContext context)
    {
        var organizationId = Guid.Parse(request.OrganizationId);
        var userId = Guid.Parse(request.UserId);
        
        var organizationMember = await dbContext.OrganizationMembers
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.OrganizationId == organizationId && x.UserId == userId);
        
        if (organizationMember is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Organization member with Organization ID '{request.OrganizationId}' and User ID '{request.UserId}' not found."));
        }

        return organizationMember.ToDto();
    }

    public override async Task<GetManyOrganizationMembersResponse> GetMany(GetManyOrganizationMembersRequest request, ServerCallContext context)
    { 
        var organizationId = Guid.Parse(request.OrganizationId);
        var userIds = request.UserIds.Select(Guid.Parse).ToList();
        
        List<OrganizationMember> organizationMembers;
        
        if (userIds.Count > 0)
        {
            organizationMembers = await dbContext.OrganizationMembers
                .AsNoTracking()
                .Where(x => x.OrganizationId == organizationId && userIds.Contains(x.UserId))
                .ToListAsync();
            var organizationMemberUserIds = organizationMembers.Select(x => x.UserId).ToList();
            var notFoundUserIds = userIds
                .Where(id => !organizationMemberUserIds.Contains(id))
                .ToList();
            
            if (notFoundUserIds.Count > 0) 
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Organization members with User IDs '{string.Join(", ", notFoundUserIds)}' not found in Organization ID '{request.OrganizationId}'."));
            }
        }
        else
        {
            organizationMembers = await dbContext.OrganizationMembers
                .AsNoTracking()
                .Where(x => x.OrganizationId == organizationId)
                .ToListAsync();
        }
        
        return new GetManyOrganizationMembersResponse
        {
            Members =
            {
                organizationMembers.Select(m => m.ToDto())
            }
        };
    }

    public override async Task<Empty> AddToOrganization(AddUserToOrganizationRequest request, ServerCallContext context)
    {
        var addManyRequest = new AddManyUsersToOrganizationRequest
        {
            OrganizationId = request.OrganizationId,
            Users =
            {
                new AddUserToOrganizationMessage()
                {
                    UserId = request.UserId,
                    Role = request.Role
                }
            }
        };

        return await AddManyToOrganization(addManyRequest, context);
    }

    public override async Task<Empty> AddManyToOrganization(AddManyUsersToOrganizationRequest request,
        ServerCallContext context)
    {
        var organizationId = Guid.Parse(request.OrganizationId);
        var organization = await dbContext.Organizations.SingleOrDefaultAsync(x => x.Id == organizationId);

        if (organization is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Organization with ID '{request.OrganizationId}' not found."));
        }
        
        var users = await userServiceClient.GetManyByIdsAsync(new GetManyUsersByIdsRequest
        {
            UserIds =
            {
                request.Users.Select(user => user.UserId)
            }
        });
        var userIds = users.Users.Select(u => Guid.Parse(u.Id)).ToList();

        var notFoundUserIds = request.Users
            .Select(u => Guid.Parse(u.UserId))
            .Where(id => !userIds.Contains(id))
            .ToList();

        if (notFoundUserIds.Count > 0)
        {
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Users with IDs '{string.Join(", ", notFoundUserIds)}' not found."));
        }

        var existingMembers = await dbContext.OrganizationMembers
            .AsNoTracking()
            .Where(m => m.OrganizationId == organizationId && userIds.Contains(m.UserId))
            .ToListAsync();

        if (existingMembers.Count > 0)
        {
            var existingMemberIds = existingMembers.Select(m => m.UserId).ToList();
            throw new RpcException(new Status(StatusCode.AlreadyExists,
                $"Users with IDs '{string.Join(", ", existingMemberIds)}' are already members of organization with ID '{request.OrganizationId}'."));
        }

        var usersToAddById = request.Users.ToDictionary(u => Guid.Parse(u.UserId), u => u);

        foreach (var userId in userIds)
        {
            var organizationMember = new Domain.Entities.OrganizationMember
            {
                OrganizationId = organizationId,
                UserId = userId,
                Role = (Domain.Enums.OrganizationMemberRole)usersToAddById[userId].Role,
            };

            organization.Members.Add(organizationMember);
        }

        await dbContext.SaveChangesAsync();

        return new Empty();
    }
    
    public override async Task<Empty> RemoveFromOrganization(RemoveUserFromOrganizationRequest request, ServerCallContext context)
    {
        var removeManyRequest = new RemoveManyUsersFromOrganizationRequest
        {
            OrganizationId = request.OrganizationId,
            UserIds = { request.UserId }
        };
        
        return await RemoveManyFromOrganization(removeManyRequest, context);
    }

    public override async Task<Empty> RemoveManyFromOrganization(RemoveManyUsersFromOrganizationRequest request, ServerCallContext context)
    {
        var organizationId = Guid.Parse(request.OrganizationId);
        var userIds = request.UserIds.Select(Guid.Parse).ToList();
        
        var organization = await dbContext.Organizations.SingleOrDefaultAsync(x => x.Id == organizationId);
        if (organization is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Organization with ID '{request.OrganizationId}' not found."));
        }
        
        var organizationMembers = dbContext.OrganizationMembers
            .Where(x => x.OrganizationId == organizationId && userIds.Contains(x.UserId))
            .ToList();
        var organizationMemberIds = organizationMembers.Select(x => x.UserId).ToList();
        var notFoundMemberIds = userIds
            .Where(userId => !organizationMemberIds.Contains(userId))
            .ToList();

        if (notFoundMemberIds.Count > 0)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Organization members with User IDs '{string.Join(", ", notFoundMemberIds)}' not found in Organization ID '{request.OrganizationId}'."));
        }
        
        dbContext.OrganizationMembers.RemoveRange(organizationMembers);
        
        await dbContext.SaveChangesAsync();
        
        return new Empty();
    }
}