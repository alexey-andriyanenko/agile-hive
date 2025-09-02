using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;

namespace IdentityService.Infrastructure.Interceptors;

public class AuthInterceptor(IHttpContextAccessor httpContextAccessor) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            httpContext.Items["UserContext"] = new UserContext(httpContext.User);
        }
        
        return await continuation(request, context);
    }
}