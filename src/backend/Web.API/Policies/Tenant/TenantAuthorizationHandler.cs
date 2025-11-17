using OrganizationUserService.Contracts;
using TenantContextService.Contracts;

namespace Web.API.Policies.Tenant;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class TenantAuthorizationHandler(
    TenantContextService.Contracts.TenantContextService.TenantContextServiceClient tenantContextServiceClient,
    IHttpContextAccessor httpContextAccessor)
    : AuthorizationHandler<TenantRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TenantRequirement requirement)
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext == null)
        {
            context.Fail();
            return;
        }

        if (!httpContext.Request.RouteValues.TryGetValue("organizationId", out var tenantObj))
        {
            context.Fail();
            return;
        }

        var tenantIdStr = tenantObj?.ToString();
        if (!Guid.TryParse(tenantIdStr, out var tenantId))
        {
            context.Fail();
            return;
        }

        var userIdStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                        context.User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdStr, out var userId))
        {
            context.Fail();
            return;
        }

        try
        {
            await tenantContextServiceClient.GetTenantMemberAsync(new GetTenantMemberRequest()
            {
                UserId = userId.ToString(),
                TenantId = tenantId.ToString()
            });
            
            context.Succeed(requirement);
        }
        catch (Grpc.Core.RpcException)
        {
            context.Fail();
        }
    }
}
