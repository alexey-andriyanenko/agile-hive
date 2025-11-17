using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using TagService.Application.Consumers;

namespace TagService.Application.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<Services.TagService>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<TenantProvisioningConsumer>();
            x.AddConsumer<ProjectMessagesConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("tag-service__tenant-provisioning-queue", e =>
                {
                    e.ConfigureConsumer<TenantProvisioningConsumer>(context);

                    e.UseMessageRetry(r =>
                    {
                        r.Interval(3, TimeSpan.FromSeconds(5));
                    });
                });
                
                cfg.ReceiveEndpoint("project-messages-queue", e =>
                {
                    e.ConfigureConsumer<ProjectMessagesConsumer>(context);

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