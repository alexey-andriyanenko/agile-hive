using OrganizationUserService.Application.DI;
using OrganizationUserService.Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthentication();

app.MapGrpcService<OrganizationUserService.Application.Services.OrganizationUserService>();

await app.RunAsync();