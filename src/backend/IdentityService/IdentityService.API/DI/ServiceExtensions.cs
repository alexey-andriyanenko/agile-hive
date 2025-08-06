using System.Text;
using IdentityMessages.Messages;
using IdentityMessages.Topics;
using IdentityService.Application.Consumers;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OrganizationMessages.Messages;
using OrganizationMessages.Topics;

namespace IdentityService.API.DI;

public static class ServiceExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.UsingInMemory();

            x.AddRider(rider =>
            {
                rider.AddConsumer<OrganizationMessagesConsumer>();
                rider.AddProducer<UserCreatedMessage>(IdentityTopics.IdentityMessages);

                rider.UsingKafka((context, k) =>
                {
                    k.Host("localhost:9092");

                    k.TopicEndpoint<OrganizationCreatedMessage>(OrganizationTopics.OrganizationMessages, "organization-messages-consumers", e =>
                    {
                        e.ConfigureConsumer<OrganizationMessagesConsumer>(context);
                    });
                });
            });
        });
        
        return services;
    }
}