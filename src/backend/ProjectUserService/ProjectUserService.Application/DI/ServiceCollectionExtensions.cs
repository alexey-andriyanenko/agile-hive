using Microsoft.Extensions.DependencyInjection;

namespace ProjectUserService.Application.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<Services.ProjectUserService>();
        return services;
    }
}