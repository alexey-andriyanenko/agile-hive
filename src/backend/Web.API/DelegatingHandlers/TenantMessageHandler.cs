namespace Web.API.DelegatingHandlers;

public class TenantMessageHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    private const string HeaderName = "X-Tenant-Id";

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var tenantId = httpContextAccessor.HttpContext?.Request.RouteValues["organizationId"]?.ToString();

        if (!string.IsNullOrEmpty(tenantId))
        {
            if (request.Headers.Contains(HeaderName))
            {
                request.Headers.Remove(HeaderName);
            }

            request.Headers.Add(HeaderName, tenantId);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}