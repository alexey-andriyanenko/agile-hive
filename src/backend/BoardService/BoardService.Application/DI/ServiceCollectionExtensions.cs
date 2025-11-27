using BoardService.Application.Consumers;
using BoardService.Application.Services;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace BoardService.Application.DI;

public static class ServiceCollectionExtensions
{
   public static IServiceCollection AddApplicationServices(this IServiceCollection services)
   {
      services.AddScoped<Services.BoardService>();
      services.AddScoped<BoardColumnService>();

      services.AddMassTransit(x =>
      {
         x.AddConsumer<ProjectMessagesConsumer>();

         x.UsingRabbitMq((context, cfg) =>
         {
            cfg.Host("localhost", "/", h =>
            {
               h.Username("guest");
               h.Password("guest");
            });

                
            cfg.ReceiveEndpoint("board-service__project-messages-queue", e =>
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