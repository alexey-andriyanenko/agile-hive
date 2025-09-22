using ProjectUserService.Application.DI;
using ProjectUserService.Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<ProjectUserService.Application.Services.ProjectUserService>();

await app.RunAsync();