using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BoardService.Infrastructure;

public static class GrpcClientBuilderExtensions
{
    public static IHttpClientBuilder AddJwtCallCredentials(this IHttpClientBuilder builder)
    {
        builder.AddCallCredentials((context, metadata, serviceProvider) =>
        {
            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(token))
            {
                metadata.Add("Authorization", token);
            }
            
            return Task.CompletedTask;
        });

        return builder;
    }
}
