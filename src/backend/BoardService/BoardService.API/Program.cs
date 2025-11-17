using BoardService.Application.DI;
using BoardService.Application.Services;
using BoardService.Infrastructure;
using BoardService.Infrastructure.Data;
using BoardService.Infrastructure.DI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<TenantMiddleware>();
        
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.MapGrpcService<BoardService.Application.Services.BoardService>();
app.MapGrpcService<BoardColumnService>();

await app.RunAsync();
