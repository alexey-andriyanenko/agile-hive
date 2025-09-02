
namespace Web.API.DelegatingHandlers;

public class AuthHeaderHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Add("Authorization", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}