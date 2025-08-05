using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
                // rider.AddConsumer<KafkaMessageConsumer>();

                rider.UsingKafka((context, k) =>
                {
                    k.Host("localhost:9092");

                    // k.TopicEndpoint<KafkaMessage>("topic-name", "consumer-group-name", e =>
                    // {
                    //     e.ConfigureConsumer<KafkaMessageConsumer>(context);
                    // });
                });
            });
        });
        
        return services;
    }
}