using FluentValidation;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using ProjectService.Application.Consumers;
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
            x.AddConsumer<IdentityMessagesConsumer>();
            x.AddConsumer<OrganizationMessagesConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                
                cfg.ReceiveEndpoint("project-service__organization-messages-queue", e =>
                {
                    e.ConfigureConsumer<OrganizationMessagesConsumer>(context);

                    e.UseMessageRetry(r =>
                    {
                        r.Interval(3, TimeSpan.FromSeconds(5));
                    });
                });
                
                cfg.ReceiveEndpoint("project-service__identity-messages-queue", e =>
                {
                    e.ConfigureConsumer<IdentityMessagesConsumer>(context);

                    e.UseMessageRetry(r =>
                    {
                        r.Interval(3, TimeSpan.FromSeconds(5));
                    });
                });
            });
        });
        
        return services;
    }
}