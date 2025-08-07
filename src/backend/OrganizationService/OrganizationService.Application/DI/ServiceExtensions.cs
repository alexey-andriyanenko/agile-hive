using Microsoft.Extensions.DependencyInjection;

namespace OrganizationService.Application.DI;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<Services.OrganizationService>();
        
        return services;
    }
}