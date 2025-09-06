using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OrganizationMessages.Messages;
using OrganizationService.Application.Extensions;
using OrganizationService.Application.Mapping;
using OrganizationService.Contracts;
using OrganizationService.Domain.Entities;
using OrganizationService.Infrastructure;
using OrganizationService.Infrastructure.Data;

namespace OrganizationService.Application.Services;

public class OrganizationService(
    ApplicationDbContext dbContext,
    IPublishEndpoint publishEndpoint,
    IHttpContextAccessor httpContextAccessor
) : Contracts.OrganizationService.OrganizationServiceBase
{
    public override async Task<OrganizationDto> Create(CreateOrganizationRequest request, ServerCallContext context)
    {
        var userContext = (UserContext)httpContextAccessor.HttpContext!.Items["UserContext"]!;

        try
        {
            var organization = new Organization(Guid.NewGuid())
            {
                Name = request.OrganizationName,
                Slug = request.OrganizationName.ToSlug(),
            };

            var organizationUser = new OrganizationMember
            {
                UserId = userContext.UserId,
                OrganizationId = organization.Id,
                Role = Domain.Enums.OrganizationMemberRole.Owner
            };

            dbContext.Organizations.Add(organization);
            dbContext.OrganizationMembers.Add(organizationUser);

            await dbContext.SaveChangesAsync();

            await publishEndpoint.Publish(new OrganizationCreationSucceededMessage()
            {
                OrganizationId = organization.Id,
                OrganizationName = organization.Name,
            });

            return organization.ToDto(organizationUser);
        }
        catch (DbUpdateException e)
        {
            await publishEndpoint.Publish(new OrganizationCreationFailedMessage()
            {
                ErrorMessage = e.Message
            });

            throw new RpcException(new Status(StatusCode.Internal, "Database update failed.", e));
        }
        catch (Exception e)
        {
            await publishEndpoint.Publish(new OrganizationCreationFailedMessage()
            {
                ErrorMessage = e.Message,
            });

            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while creating the organization.",
                e));
        }
    }

    public override async Task<OrganizationDto> GetById(GetOrganizationByIdRequest request, ServerCallContext context)
    {
        var userContext = (UserContext)httpContextAccessor.HttpContext!.Items["UserContext"]!;
        var organization = await dbContext.Organizations
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == Guid.Parse(request.OrganizationId));
        
        if (organization is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Organization with ID '{request.OrganizationId}' not found."));
        }
        
        var organizationMember = await dbContext.OrganizationMembers
            .AsNoTracking()
            .SingleOrDefaultAsync(x =>
                x.OrganizationId == organization.Id && x.UserId == userContext.UserId);
        
        if (organizationMember is null)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied,
                $"User does not have access to organization with ID '{request.OrganizationId}'."));
        }

        return organization.ToDto(organizationMember);
    }

    public override async Task<OrganizationDto> GetBySlug(GetOrganizationBySlugRequest request, ServerCallContext context)
    {
        var userContext = (UserContext)httpContextAccessor.HttpContext!.Items["UserContext"]!;
        var organization = await dbContext.Organizations
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Slug == request.OrganizationSlug);
        
        if (organization is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Organization with slug '{request.OrganizationSlug}' not found."));
        }
        
        var organizationMember = await dbContext.OrganizationMembers
            .AsNoTracking()
            .SingleOrDefaultAsync(x =>
                x.OrganizationId == organization.Id && x.UserId == userContext.UserId);
        
        if (organizationMember is null)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied,
                $"User does not have access to organization with slug '{request.OrganizationSlug}'."));
        }

        return organization.ToDto(organizationMember);
    }

    public override async Task<GetManyOrganizationsResponse> GetMany(GetManyOrganizationsRequest request,
        ServerCallContext context)
    {
        var userContext = (UserContext)httpContextAccessor.HttpContext!.Items["UserContext"]!;

        var organizationIdsAssociatedWithUser = await dbContext.OrganizationMembers
            .AsNoTracking()
            .Where(x => x.UserId == userContext.UserId)
            .Select(x => x.OrganizationId)
            .ToListAsync();
        var organizationIds = request.OrganizationIds.Count > 0
            ? request.OrganizationIds.Select(Guid.Parse).Intersect(organizationIdsAssociatedWithUser).ToList()
            : organizationIdsAssociatedWithUser;

        var organizations = await dbContext.Organizations.Where(x => organizationIds.Contains(x.Id))
            .ToListAsync();
        
        var organizationMembers = await dbContext.OrganizationMembers
            .AsNoTracking()
            .Where(x => x.UserId == userContext.UserId && organizationIds.Contains(x.OrganizationId))
            .ToDictionaryAsync(x => x.OrganizationId, x => x);

        return new GetManyOrganizationsResponse()
        {
            Organizations =
            {
                organizations.Select(x => x.ToDto(organizationMembers[x.Id]))
            }
        };
    }

    public override async Task<OrganizationDto> Update(UpdateOrganizationRequest request, ServerCallContext context)
    {
        var organizationId = Guid.Parse(request.OrganizationId);
        var organization = await dbContext.Organizations.SingleOrDefaultAsync(x => x.Id == organizationId);

        if (organization is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Organization with ID '{request.OrganizationId}' not found."));
        }
        
        var userContext = (UserContext)httpContextAccessor.HttpContext!.Items["UserContext"]!;
        var organizationMember = await dbContext.OrganizationMembers
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.OrganizationId == organizationId && x.UserId == userContext.UserId);
        
        if (organizationMember is null || organizationMember.Role != Domain.Enums.OrganizationMemberRole.Owner)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied,
                $"User does not have permission to update organization with ID '{request.OrganizationId}'."));
        }
        
        if (organization.Name == request.OrganizationName)
        {
            return organization.ToDto(organizationMember);
        }

        try
        {
            organization.Name = request.OrganizationName;
            organization.Slug = request.OrganizationName.ToSlug();

            dbContext.Organizations.Update(organization);
            await dbContext.SaveChangesAsync();

            return organization.ToDto(organizationMember);
        }
        catch (ArgumentException e)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
        }
    }

    public override Task<Empty> Delete(DeleteOrganizationRequest request, ServerCallContext context)
    {
        throw new RpcException(new Status(StatusCode.Unimplemented, "Delete organization is not implemented yet."));
    }

    public override Task<Empty> DeleteMany(DeleteManyOrganizationsRequest request, ServerCallContext context)
    {
        throw new RpcException(
            new Status(StatusCode.Unimplemented, "Delete many organizations is not implemented yet."));
    }
}