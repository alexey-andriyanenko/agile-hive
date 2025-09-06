using System.Text;
using IdentityService.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OrganizationService.Infrastructure.Data;
using OrganizationService.Infrastructure.Interceptors;

namespace OrganizationService.Infrastructure.DI;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("OrganizationDb")));

        services.AddHttpContextAccessor();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],

                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),

                    ValidateLifetime = true,
                };
            });
        services.AddAuthorization();
        
        services.AddGrpcClient<UserService.UserServiceClient>(options =>
        {
            options.Address = new Uri(configuration["ServiceAddresses:IdentityService"]!);
        });

        services.AddGrpc(options =>
        {
            options.Interceptors.Add<AuthInterceptor>();
            options.Interceptors.Add<ValidationInterceptor>();
        });
        
        
        return services;
    }
}