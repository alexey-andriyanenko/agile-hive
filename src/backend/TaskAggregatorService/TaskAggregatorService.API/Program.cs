using TaskAggregatorService.Application.DI;
using TaskAggregatorService.Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<TaskAggregatorService.Application.Services.TaskAggregatorService>();

await app.RunAsync();
