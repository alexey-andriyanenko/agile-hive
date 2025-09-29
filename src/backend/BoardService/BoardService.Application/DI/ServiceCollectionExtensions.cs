using BoardService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BoardService.Application.DI;

public static class ServiceCollectionExtensions
{
   public static IServiceCollection AddApplicationServices(this IServiceCollection services)
   {
      services.AddScoped<Services.BoardService>();
      services.AddScoped<BoardColumnService>();

      return services;
   } 
}