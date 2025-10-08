using Microsoft.Extensions.DependencyInjection;

namespace TagService.Application.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<Services.TagService>();

        return services;
    }
}