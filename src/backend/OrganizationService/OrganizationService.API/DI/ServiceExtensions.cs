using IdentityMessages.Topics;
using MassTransit;
using OrganizationMessages.Commands;
using OrganizationMessages.Messages;
using OrganizationMessages.Topics;
using OrganizationService.Application.Consumers;

namespace OrganizationService.API.DI;

public static class ServiceExtensions
{
    public static IServiceCollection AddOrganizationService(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.UsingInMemory();

            x.AddRider(rider =>
            {
                rider.AddConsumer<IdentityCommandsConsumer>();
                rider.AddProducer<OrganizationCreatedMessage>(OrganizationTopics.OrganizationMessages);

                rider.UsingKafka((context, k) =>
                {
                    k.Host("localhost:9092");

                    // TODO: figure out where to move message bus configuration and where to keep groupId names?
                    k.TopicEndpoint<CreateOrganizationByOwnerUserCommand>(IdentityTopics.IdentityMessages, "identity-commands-consumers", e =>
                    {
                        e.ConfigureConsumer<IdentityCommandsConsumer>(context);
                    });
                });
            });
        });
        
        return services;
    }
}