using MassTransit;
using OrganizationMessages.Commands;
using OrganizationMessages.Messages;
using OrganizationMessages.Topics;
using OrganizationService.Application.Consumers;

namespace OrganizationService.API.DI;

public static class ServiceExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<TenantProvisioningConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                
                cfg.ReceiveEndpoint("organization-service__tenant-provisioning-messages-queue", e =>
                {
                    e.ConfigureConsumer<TenantProvisioningConsumer>(context);

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