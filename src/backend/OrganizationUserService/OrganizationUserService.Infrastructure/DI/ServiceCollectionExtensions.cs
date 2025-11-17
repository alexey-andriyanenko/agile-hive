using System.Text;
using IdentityService.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OrganizationService.Contracts;
using OrganizationUserService.Infrastructure.DelegatingHandlers;
using OrganizationUserService.Infrastructure.Interceptors;

namespace OrganizationUserService.Infrastructure.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");

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

        services.AddTransient<AuthHeaderHandler>();
        services.AddTransient<TenantMessageHandler>();
        services.AddScoped<UserContextProvider>();
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

        services.AddGrpcClient<OrganizationMemberService.OrganizationMemberServiceClient>(options =>
            {
                options.Address = new Uri(configuration["ServiceAddresses:OrganizationService"]!);
            })
            .ConfigureChannel(options =>
            {
                options.UnsafeUseInsecureChannelCallCredentials = true;
            })
            .AddJwtCallCredentials()
            .AddHttpMessageHandler<TenantMessageHandler>();
        
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<AuthInterceptor>();
            options.Interceptors.Add<ValidationInterceptor>();
        });
        
        return services;
    }
}