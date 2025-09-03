using IdentityService.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectService.Infrastructure.Data;
using ProjectService.Infrastructure.Interceptors;

namespace ProjectService.Infrastructure.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("ProjectDb")));
        
        services.AddAuthentication("Bearer") 
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "https://localhost:5202";
            });
        services.AddAuthorization();
        
        services.AddHttpContextAccessor();
        
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<AuthInterceptor>();
            options.Interceptors.Add<ValidationInterceptor>();
        });
        
        services.AddGrpcClient<UserService.UserServiceClient>(options =>
        {
            options.Address = new Uri("http://localhost:5201");
        });
        
        return services;
    }
}