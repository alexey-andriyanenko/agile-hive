using IdentityService.gRPC;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrganizationService.Infrastructure.Data;

namespace OrganizationService.Infrastructure.DI;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("OrganizationDb")));

        services.AddGrpcClient<UserService.UserServiceClient>(options =>
        {
            options.Address = new Uri("http://localhost:5243");
        });
        
        return services;
    }
}