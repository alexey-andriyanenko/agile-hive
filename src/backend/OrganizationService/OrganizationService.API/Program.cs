using Microsoft.EntityFrameworkCore;
using OrganizationService.API.DI;
using OrganizationService.Application.DI;
using OrganizationService.Application.Services;
using OrganizationService.Infrastructure;
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

app.UseMiddleware<TenantMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.MapGrpcService<OrganizationMemberService>();

app.Run();