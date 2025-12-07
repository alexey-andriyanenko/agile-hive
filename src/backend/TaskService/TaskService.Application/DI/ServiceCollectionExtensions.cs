using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using TaskService.Application.Consumers;

namespace TaskService.Application.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<Services.TaskService>();
        services.AddScoped<Services.CommentService>();
        
        services.AddMassTransit(x =>
        {
            x.AddConsumer<TenantProvisioningConsumer>();
            x.AddConsumer<BoardMessagesConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("task-service__tenant-provisioning-messages-queue", e =>
                {
                    e.ConfigureConsumer<TenantProvisioningConsumer>(context);

                    e.UseMessageRetry(r =>
                    {
                        r.Interval(3, TimeSpan.FromSeconds(5));
                    });
                });
                
                cfg.ReceiveEndpoint("task-service__board-messages-queue", e =>
                {
                    e.ConfigureConsumer<BoardMessagesConsumer>(context);

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