using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using PermissionService.Application.Mappings;
using PermissionService.Contracts;
using PermissionService.Infrastructure.Data;

namespace PermissionService.Application.Services;

public class PermissionsService(ApplicationDbContext dbContext) : Contracts.PermissionsService.PermissionsServiceBase
{
    public override async Task<GetPermissionsResponse> GetMany(GetPermissionsRequest request, ServerCallContext context)
    {
        var tenantId = Guid.Parse(request.TenantId);
        var permissions = await dbContext.Permissions.Where(x => x.TenantId == tenantId && x.Role == request.Role).ToListAsync();
        
        return new GetPermissionsResponse()
        {
            Permissions =
            {
                permissions.Select(x => x.ToDto()).ToList()
            }
        };
    }

    public override async Task<CheckIfAllowedResponse> IsAllowed(CheckIfAllowedRequest request, ServerCallContext context)
    {
        var permission = await dbContext.Permissions.FirstOrDefaultAsync(x =>
            x.TenantId == Guid.Parse(request.TenantId) &&
            x.Role == request.Role &&
            x.Resource.ToString() == request.Resource &&
            x.Operation.ToString() == request.Operation
        );
        
        if (permission == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Permission not found for Role '{request.Role}', Resource '{request.Resource}', Operation '{request.Operation}' in Tenant ID '{request.TenantId}'."));
        }
        
        return new CheckIfAllowedResponse()
        {
            Allowed = permission.Allowed
        };
    }
}