using System.Text;
using IdentityService.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ProjectService.Infrastructure.Data;
using ProjectService.Infrastructure.DelegatingHandlers;
using ProjectService.Infrastructure.Interceptors;

namespace ProjectService.Infrastructure.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var tenantContext = sp.GetRequiredService<TenantContext>();

            var defaultConnection = configuration.GetConnectionString("ProjectDb");

            var connectionString = string.IsNullOrEmpty(tenantContext.DbConnectionString)
                ? defaultConnection
                : tenantContext.DbConnectionString;

            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });
        });


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
        services.AddHttpContextAccessor();
        services.AddScoped<TenantContext>();

        services.AddMemoryCache();
        
        services.AddTransient<AuthHeaderHandler>();
        services.AddScoped<TokenProvider>();

        services.AddGrpcClient<UserService.UserServiceClient>(options =>
            {
                options.Address = new Uri(configuration["ServiceAddresses:IdentityService"]!);
            })
            .ConfigureChannel(options =>
            {
                options.UnsafeUseInsecureChannelCallCredentials = true;
            })
            .AddJwtCallCredentials();
        
        services
            .AddGrpcClient<TenantContextService.Contracts.TenantContextService.TenantContextServiceClient>(options =>
            {
                options.Address = new Uri(configuration["ServiceAddresses:TenantContextService"]!);
            })
            .ConfigureChannel(options => { options.UnsafeUseInsecureChannelCallCredentials = true; });
        
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<AuthInterceptor>();
            options.Interceptors.Add<ValidationInterceptor>();
        });
        
        return services;
    }
}