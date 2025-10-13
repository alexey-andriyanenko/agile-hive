using Microsoft.AspNetCore.Authorization;
using Web.API.DelegatingHandlers;
using Web.API.DI;
using Web.API.Middlewares;
using Web.API.Policies.Tenant;
using Web.API.Services;

namespace Web.API;

public class Program
{
    public static void Main(string[] args)
    {
        var allowedOriginsPolicy = "AllowedOrigins";
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: allowedOriginsPolicy,
                policy  =>
                {
                    policy.WithOrigins("http://localhost:5175")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<AuthHeaderHandler>();
        builder.Services.AddScoped<TokenProvider>();
        builder.Services.AddScoped<IAuthorizationHandler, TenantAuthorizationHandler>();
        
        builder.Services.AddOpenApi();

        builder.Services.AddServices(builder.Configuration);
        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {  
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors(allowedOriginsPolicy);
        
        app.UseMiddleware<RpcExceptionHandlingMiddleware>();
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();
        
        
        app.Run();
    }
}