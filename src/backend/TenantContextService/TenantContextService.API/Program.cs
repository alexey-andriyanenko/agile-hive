using Microsoft.EntityFrameworkCore;
using TenantContextService.Application.DI;
using TenantContextService.Infrastructure.Data;
using TenantContextService.Infrastructure.DI;

namespace TenantContextService;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices();

        var app = builder.Build();
        
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        app.MapGrpcService<TenantContextService.Application.Services.TenantContextService>();

        await app.RunAsync();
    }
}