using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TenantContextService.Contracts;

namespace ProjectService.Infrastructure;

public class TenantMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, TenantContext tenantContext,
        TenantContextService.Contracts.TenantContextService.TenantContextServiceClient orgClient, IConfiguration cfg)
    {
        if (context.Request.Headers.TryGetValue("x-tenant-id", out var tenantHeader))
        {
            var tenantId = Guid.Parse(tenantHeader!);

            if (tenantId != Guid.Empty)
            {
                tenantContext.TenantId = tenantId;

                var tenantContextResult = await orgClient.GetTenantContextAsync(new GetTenantContextRequest()
                {
                    TenantId = tenantContext.TenantId.ToString(),
                    ServiceName = "ProjectService"
                }).ResponseAsync;

                tenantContext.DbConnectionString = tenantContextResult.DbConnectionString;
            }
        }
        else
        {
            tenantContext.TenantId = Guid.Empty;
            tenantContext.DbConnectionString = cfg.GetConnectionString("ProjectDb")!;
        }

        await next(context);
    }
}