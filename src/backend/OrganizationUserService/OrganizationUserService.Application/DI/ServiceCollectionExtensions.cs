using Microsoft.Extensions.DependencyInjection;

namespace OrganizationUserService.Application.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<Services.OrganizationUserService>();
        return services;
    }
}