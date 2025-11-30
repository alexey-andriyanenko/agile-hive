using Microsoft.EntityFrameworkCore;
using OrganizationService.API.DI;
using OrganizationService.Application.DI;
using OrganizationService.Application.Services;
using OrganizationService.Infrastructure.Data;
using OrganizationService.Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddApiServices(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbInitializer.Database.MigrateAsync();
}

app.MapGrpcService<OrganizationService.Application.Services.OrganizationService>();
app.MapGrpcService<OrganizationMemberService>();

app.Run();