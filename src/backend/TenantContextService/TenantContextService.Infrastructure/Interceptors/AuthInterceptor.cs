using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;

namespace TenantContextService.Infrastructure.Interceptors;

public class AuthInterceptor(IHttpContextAccessor httpContextAccessor) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        if (context.Method.EndsWith("GetTenantContext"))
        {
            return await continuation(request, context);
        }
        
        var authHeader = context.RequestHeaders
            .FirstOrDefault(h => h.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase));

        if (authHeader == null || string.IsNullOrWhiteSpace(authHeader.Value))
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Authorization header is missing."));
        }

        var token = authHeader.Value.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? authHeader.Value.Substring("Bearer ".Length).Trim()
            : authHeader.Value.Trim();

        ClaimsPrincipal? principal;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            
            principal = new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims, "jwt"));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, $"Invalid token: {ex.Message}"));
        }

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            httpContext.User = principal;
            httpContext.Items["UserContext"] = new UserContext(principal);
        }

        return await continuation(request, context);
    }
}