using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using TenantContextService.Contracts;

namespace ProjectService.Infrastructure;

public class TenantMiddleware(RequestDelegate next)
{
    private const string ServiceName = "ProjectService";
    
    public async Task InvokeAsync(HttpContext context, TenantContext tenantContext,
        TenantContextService.Contracts.TenantContextService.TenantContextServiceClient orgClient, IConfiguration cfg, IMemoryCache memoryCache)
    {
        if (context.Request.Headers.TryGetValue("x-tenant-id", out var tenantHeader))
        {
            var tenantId = Guid.Parse(tenantHeader!);

            if (tenantId != Guid.Empty)
            {
                tenantContext.TenantId = tenantId;

                var cacheKey = $"tenantcontext:{tenantId}:{ServiceName}";

                var tenantContextResult = await memoryCache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

                    var resp = await orgClient.GetTenantContextAsync(new GetTenantContextRequest()
                    {
                        TenantId = tenantId.ToString(),
                        ServiceName = ServiceName
                    }).ResponseAsync;

                    return new TenantContext()
                    {
                        TenantId = tenantId,
                        DbConnectionString = resp.DbConnectionString,
                        ServiceName = ServiceName
                    };
                });
                
                tenantContext.DbConnectionString = tenantContextResult!.DbConnectionString;
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