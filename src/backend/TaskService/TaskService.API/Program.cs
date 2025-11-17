using Microsoft.EntityFrameworkCore;
using TaskService.Application.DI;
using TaskService.Infrastructure;
using TaskService.Infrastructure.Data;
using TaskService.Infrastructure.DI;

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

app.MapGrpcService<TaskService.Application.Services.CommentService>();
app.MapGrpcService<TaskService.Application.Services.TaskService>();

await app.RunAsync();
