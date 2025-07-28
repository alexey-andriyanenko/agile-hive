using IdentityService.API.DI;
using IdentityService.Application.DI;
using IdentityService.Application.Services;
using IdentityService.Infrastructure.DI;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddGrpc();
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApiServices(builder.Configuration);
        builder.Services.AddApplicationServices();
        
        var app = builder.Build();
        
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();
        }
        
        app.MapGrpcService<AuthService>();
        app.MapGrpcService<TokenService>();
        
        await app.RunAsync();
    }
}