using Microsoft.AspNetCore.Http;

namespace TaskAggregatorService.Infrastructure.DelegatingHandlers;

public class TenantMessageHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var tenantId = httpContextAccessor.HttpContext?.Request.Headers["x-tenant-id"].FirstOrDefault();
        
        if (!string.IsNullOrEmpty(tenantId))
        {
            request.Headers.Add("x-tenant-id", tenantId);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
