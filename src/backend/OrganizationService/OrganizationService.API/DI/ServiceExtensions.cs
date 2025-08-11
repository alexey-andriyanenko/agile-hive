using IdentityMessages.Topics;
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
            x.UsingInMemory();

            x.AddRider(rider =>
            {
                rider.AddConsumer<OrganizationCommandsConsumer>();
                rider.AddProducer<OrganizationCreatedMessage>(OrganizationTopics.OrganizationMessages);

                rider.UsingKafka((context, k) =>
                {
                    k.Host("localhost:9092");

                    // TODO: figure out where to move message bus configuration and where to keep groupId names?
                    k.TopicEndpoint<CreateOrganizationByOwnerUserCommand>(OrganizationTopics.OrganizationCommands, "organization-commands-consumers", e =>
                    {
                        e.ConfigureConsumer<OrganizationCommandsConsumer>(context);
                    });
                });
            });
        });
        
        return services;
    }
}