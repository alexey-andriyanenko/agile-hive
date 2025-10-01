using Microsoft.Extensions.DependencyInjection;

namespace TaskAggregatorService.Application.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<Services.TaskAggregatorService>();
        
        return services;
    }
}