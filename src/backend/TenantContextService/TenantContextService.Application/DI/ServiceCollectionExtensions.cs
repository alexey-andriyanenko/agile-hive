using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using TenantContextService.Application.Consumers;

namespace TenantContextService.Application.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<Services.TenantContextService>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<TenantProvisioningMessagesConsumer>();
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                
                cfg.ReceiveEndpoint("tenant-provisioning-queue", e =>
                {
                    e.ConfigureConsumer<TenantProvisioningMessagesConsumer>(context);

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