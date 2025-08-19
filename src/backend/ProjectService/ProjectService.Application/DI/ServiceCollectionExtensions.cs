using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProjectService.Application.Validations;
using ProjectService.gRPC;

namespace ProjectService.Application.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateProjectRequest>, CreateProjectRequestValidator>();
        services.AddScoped<IValidator<UpdateProjectRequest>, UpdateProjectRequestValidator>();

        services.AddScoped<Services.ProjectService>();
        
        return services;
    }
}