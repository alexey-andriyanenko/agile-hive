var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
        
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.MapGrpcService<BoardService.Application.Services.BoardService>();
app.MapGrpcService<BoardColumnService>();

await app.RunAsync();
