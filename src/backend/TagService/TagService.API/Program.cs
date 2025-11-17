using Microsoft.EntityFrameworkCore;
using TagService.Application.DI;
using TagService.Infrastructure;
using TagService.Infrastructure.Data;
using TagService.Infrastructure.DI;

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

app.MapGrpcService<TagService.Application.Services.TagService>();

await app.RunAsync();
