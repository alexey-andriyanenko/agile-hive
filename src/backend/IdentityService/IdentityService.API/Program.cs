using IdentityService.API.DI;
using IdentityService.Application.DI;
using IdentityService.Infrastructure.Data;
using IdentityService.Infrastructure.DI;
using IdentityService.Infrastructure.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddGrpc()
            .AddServiceOptions<Application.Services.UserService>(options =>
            {
                options.Interceptors.Add<AuthInterceptor>();
            });
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApiServices(builder.Configuration);
        builder.Services.AddApplicationServices();
        
        var app = builder.Build();
        
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapGrpcService<Application.Services.AuthService>();
        app.MapGrpcService<Application.Services.TokenService>();
        app.MapGrpcService<Application.Services.UserService>();
        
        await app.RunAsync();
    }
}