using FluentValidation;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using ProjectService.Application.Validations;
using ProjectService.Contracts;

namespace ProjectService.Application.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateProjectRequest>, CreateProjectRequestValidator>();
        services.AddScoped<IValidator<UpdateProjectRequest>, UpdateProjectRequestValidator>();

        services.AddScoped<Services.ProjectService>();
        services.AddScoped<Services.ProjectMemberService>();
        
        services.AddMassTransit(x =>
        {

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });
        });
        
        return services;
    }
}