using Grpc.Core;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrganizationMessages.Messages;
using TenantContextService.Application.Extensions;
using TenantContextService.Application.Mappings;
using TenantContextService.Contracts;
using TenantContextService.Domain.Entities;
using TenantContextService.Infrastructure;
using TenantContextService.Infrastructure.Data;
using TenantProvisioning.Messages;

namespace TenantContextService.Application.Services;

public class TenantContextService(
    ILogger<TenantContextService> logger,
    ApplicationDbContext applicationDbContext,
    IHttpContextAccessor httpContextAccessor,
    IPublishEndpoint publishEndpoint) : Contracts.TenantContextService.TenantContextServiceBase
{
    public override Task<TenantContextResponse> GetTenantContext(GetTenantContextRequest request,
        ServerCallContext context)
    {
        var tenantDb = applicationDbContext.TenantDbs
            .FirstOrDefault(x => x.TenantId == Guid.Parse(request.TenantId) && x.ServiceName == request.ServiceName);

        if (tenantDb == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Tenant context not found"));
        }

        var response = new TenantContextResponse
        {
            ServiceName = tenantDb.ServiceName,
            DbConnectionString = tenantDb.ConnectionString
        };

        return Task.FromResult(response);
    }

    private IReadOnlyList<TenantDbEntity> GenerateTenantDbs(Guid tenantId)
    {
        return new List<TenantDbEntity>
        {
            new TenantDbEntity()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ServiceName = "OrganizationService",
                ConnectionString =
                    $"User ID=postgres;Password=root;Host=localhost;Port=5432;Database=organization-db__{tenantId};Include Error Detail=true;"
            },
            new TenantDbEntity
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ServiceName = "ProjectService",
                ConnectionString =
                    $"User ID=postgres;Password=root;Host=localhost;Port=5432;Database=project-db__{tenantId};Include Error Detail=true;"
            },
            new TenantDbEntity
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ServiceName = "TagService",
                ConnectionString =
                    $"User ID=postgres;Password=root;Host=localhost;Port=5432;Database=tag-db__{tenantId};Include Error Detail=true;"
            },
            new TenantDbEntity()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ServiceName = "BoardService",
                ConnectionString =
                    $"User ID=postgres;Password=root;Host=localhost;Port=5432;Database=board-db__{tenantId};Include Error Detail=true;"
            },
            new TenantDbEntity()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ServiceName = "TaskService",
                ConnectionString =
                    $"User ID=postgres;Password=root;Host=localhost;Port=5432;Database=task-db__{tenantId};Include Error Detail=true;"
            }
        };
    }

    public override async Task<TenantResponse> CreateTenant(CreateTenantRequest request, ServerCallContext context)
    {
        var userContext = (UserContext)httpContextAccessor.HttpContext!.Items["UserContext"]!;

        try
        {
            var organization = new TenantEntity()
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Slug = request.Name.ToSlug(),
            };
            var organizationMember = new TenantMemberReadEntity()
            {
                TenantId = organization.Id,
                UserId = userContext.UserId,
                Role = Domain.Enum.TenantMemberRole.Owner,
            };
            var tenantDbs = GenerateTenantDbs(organization.Id);

            applicationDbContext.Tenants.Add(organization);
            applicationDbContext.TenantMembers.Add(organizationMember);
            applicationDbContext.TenantDbs.AddRange(tenantDbs);

            await applicationDbContext.SaveChangesAsync();
        await Parallel.ForEachAsync(tenantDbs,
            new ParallelOptions { MaxDegreeOfParallelism = 4, CancellationToken = context.CancellationToken },
            async (tenantDb, ct) =>
            {
                try
                {
                    await publishEndpoint.Publish(new TenantDatabaseCreationRequested
                    {
                        TenantId = tenantDb.TenantId,
                        CreatedByUserId = userContext.UserId,
                        ServiceName = tenantDb.ServiceName,
                        DbConnectionString = tenantDb.ConnectionString
                    }, ct);

                    logger.LogInformation("Published TenantDatabaseCreationRequested for service {ServiceName} and tenant {TenantId}",
                        tenantDb.ServiceName, tenantDb.TenantId);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to publish TenantDatabaseCreationRequested for tenant {TenantId} / service {ServiceName}",
                        tenantDb.TenantId, tenantDb.ServiceName);

                    try
                    {
                        await publishEndpoint.Publish(new OrganizationCreationFailedMessage
                        {
                            ErrorMessage = $"Failed to publish DB creation for {tenantDb.ServiceName}: {ex.Message}"
                        }, ct);
                    }
                    catch (Exception inner)
                    {
                        logger.LogError(inner, "Failed to publish OrganizationCreationFailedMessage for tenant {TenantId}", tenantDb.TenantId);
                    }
                }
            });

            return organization.ToDto(organizationMember, tenantDbs);
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

    public override async Task<TenantResponse> GetTenantById(GetTenantByIdRequest request, ServerCallContext context)
    {
        var userContext = (UserContext)httpContextAccessor.HttpContext!.Items["UserContext"]!;
        var organization = await applicationDbContext.Tenants
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == Guid.Parse(request.Id));

        if (organization is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Organization with ID '{request.Id}' not found."));
        }

        var organizationDbs = await applicationDbContext.TenantDbs
            .AsNoTracking()
            .Where(x => x.TenantId == organization.Id)
            .ToListAsync();

        var organizationMember = await applicationDbContext.TenantMembers
            .AsNoTracking()
            .SingleOrDefaultAsync(x =>
                x.TenantId == organization.Id && x.UserId == userContext.UserId);

        if (organizationMember is null)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied,
                $"User does not have access to organization with ID '{request.Id}'."));
        }


        return organization.ToDto(organizationMember, organizationDbs);
    }

    public override async Task<TenantResponse> GetTenantBySlug(GetTenantBySlugRequest request,
        ServerCallContext context)
    {
        var userContext = (UserContext)httpContextAccessor.HttpContext!.Items["UserContext"]!;
        var organization = await applicationDbContext.Tenants
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Slug == request.Slug);
        var organizationDbs = await applicationDbContext.TenantDbs
            .AsNoTracking()
            .Where(x => x.TenantId == organization!.Id)
            .ToListAsync();

        if (organization is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Organization with slug '{request.Slug}' not found."));
        }

        var organizationMember = await applicationDbContext.TenantMembers
            .AsNoTracking()
            .SingleOrDefaultAsync(x =>
                x.TenantId == organization.Id && x.UserId == userContext.UserId);

        if (organizationMember is null)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied,
                $"User does not have access to organization with slug '{request.Slug}'."));
        }

        return organization.ToDto(organizationMember, organizationDbs);
    }

    public override async Task<GetManyTenantsResponse> GetManyTenants(GetManyTenantsRequest request,
        ServerCallContext context)
    {
        var userContext = (UserContext)httpContextAccessor.HttpContext!.Items["UserContext"]!;

        var organizationIdsAssociatedWithUser = await applicationDbContext.TenantMembers
            .AsNoTracking()
            .Where(x => x.UserId == userContext.UserId)
            .Select(x => x.TenantId)
            .ToListAsync();
        var organizationIds = request.Ids.Count > 0
            ? request.Ids.Select(Guid.Parse).Intersect(organizationIdsAssociatedWithUser).ToList()
            : organizationIdsAssociatedWithUser;

        var organizations = await applicationDbContext.Tenants.Where(x => organizationIds.Contains(x.Id))
            .ToListAsync();

        var organizationMembers = await applicationDbContext.TenantMembers
            .AsNoTracking()
            .Where(x => x.UserId == userContext.UserId && organizationIds.Contains(x.TenantId))
            .ToDictionaryAsync(x => x.TenantId, x => x);

        var organizationDbs = await applicationDbContext.TenantDbs
            .AsNoTracking()
            .Where(x => organizationIds.Contains(x.TenantId))
            .ToListAsync();
        var organizationDbsByOrganizationId = organizationDbs
            .GroupBy(x => x.TenantId)
            .ToDictionary(g => g.Key, g => g.ToList());

        return new GetManyTenantsResponse()
        {
            Tenants =
            {
                organizations.Select(x => x.ToDto(organizationMembers[x.Id],
                    organizationDbsByOrganizationId.TryGetValue(x.Id, out var value)
                        ? value
                        : new List<TenantDbEntity>()))
            }
        };
    }

    public override async Task<TenantResponse> UpdateTenant(UpdateTenantRequest request, ServerCallContext context)
    {
        var organizationId = Guid.Parse(request.Id);
        var organization = await applicationDbContext.Tenants.SingleOrDefaultAsync(x => x.Id == organizationId);

        if (organization is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Organization with ID '{request.Id}' not found."));
        }
        
        var organizationDbs = await applicationDbContext.TenantDbs
            .AsNoTracking()
            .Where(x => x.TenantId == organization.Id)
            .ToListAsync();

        var userContext = (UserContext)httpContextAccessor.HttpContext!.Items["UserContext"]!;
        var organizationMember = await applicationDbContext.TenantMembers
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.TenantId == organizationId && x.UserId == userContext.UserId);

        if (organizationMember is null || organizationMember.Role != Domain.Enum.TenantMemberRole.Owner)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied,
                $"User does not have permission to update organization with ID '{request.Id}'."));
        }

        if (organization.Name == request.Name)
        {
            return organization.ToDto(organizationMember, organizationDbs);
        }

        try
        {
            organization.Name = request.Name;
            organization.Slug = request.Name.ToSlug();

            await applicationDbContext.SaveChangesAsync();

            return organization.ToDto(organizationMember, organizationDbs);
        }
        catch (ArgumentException e)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
        }
    }

    public override async Task<TenantMemberResponse> GetTenantMember(GetTenantMemberRequest request, ServerCallContext context)
    {
        var userContext = (UserContext)httpContextAccessor.HttpContext!.Items["UserContext"]!;
        var tenantMember = await applicationDbContext.TenantMembers
            .AsNoTracking()
            .SingleOrDefaultAsync(x =>
                x.TenantId == Guid.Parse(request.TenantId) && x.UserId == userContext.UserId);

        if (tenantMember is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Tenant member not found for tenant ID '{request.TenantId}' and user ID '{userContext.UserId}'."));
        }

        return new TenantMemberResponse()
        {
            UserId = tenantMember.UserId.ToString(),
            Role = (TenantMemberRole)tenantMember.Role
        };
    }
}