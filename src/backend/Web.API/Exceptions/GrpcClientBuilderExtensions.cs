using Web.API.Services;

namespace Web.API.Exceptions;

public static class GrpcClientBuilderExtensions
{
    public static IHttpClientBuilder AddJwtCallCredentials(this IHttpClientBuilder builder)
    {
        builder.AddCallCredentials(async (context, metadata, serviceProvider) =>
        {
            var provider = serviceProvider.GetRequiredService<TokenProvider>();
            var token = await provider.GetTokenAsync(context.CancellationToken);

            metadata.Add("Authorization", token);
        });

        return builder;
    }
}
