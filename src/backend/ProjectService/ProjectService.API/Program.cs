using Microsoft.EntityFrameworkCore;
using ProjectService.API.DI;
using ProjectService.Application.DI;
using ProjectService.Infrastructure.Data;
using ProjectService.Infrastructure.DI;

namespace ProjectService.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices();
        builder.Services.AddApiServices();

        var app = builder.Build();

        app.UseAuthentication();
        app.UseAuthorization();
        
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();
        }
        
        app.MapGrpcService<Application.Services.ProjectService>();
        app.MapGrpcService<Application.Services.ProjectMemberService>();

        await app.RunAsync();
    }
}