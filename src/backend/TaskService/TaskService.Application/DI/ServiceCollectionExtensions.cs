using Microsoft.Extensions.DependencyInjection;

namespace TaskService.Application.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<Services.TaskService>();
        services.AddScoped<Services.TagService>();
        services.AddScoped<Services.CommentService>();
        
        return services;
    }
}