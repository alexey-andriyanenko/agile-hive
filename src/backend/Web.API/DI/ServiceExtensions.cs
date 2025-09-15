using System.Text;
using IdentityService.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OrganizationService.Contracts;
using ProjectService.Contracts;
using Web.API.DelegatingHandlers;
using Web.API.Exceptions;

namespace Web.API.DI;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
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

        services.AddIdentityServices(configuration);
        services.AddOrganizationServices(configuration);
        services.AddProjectServices(configuration);
        services.AddOrganizationUserServices(configuration);
        
        return services;
    }

    private static IServiceCollection AddIdentityServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var identityServiceAddress = new Uri(configuration["ServiceAddresses:IdentityService"]!);

        services.AddGrpcClient<AuthService.AuthServiceClient>(options =>
            {
                options.Address = identityServiceAddress;
            })
            .ConfigureChannel(options =>
            {
                options.UnsafeUseInsecureChannelCallCredentials = true;
            });
        
        services.AddGrpcClient<TokenService.TokenServiceClient>(options =>
            {
                options.Address = identityServiceAddress;
            })
            .ConfigureChannel(options =>
            {
                options.UnsafeUseInsecureChannelCallCredentials = true;
            });


        services.AddGrpcClient<UserService.UserServiceClient>(options =>
            {
                options.Address = identityServiceAddress;
            })
            .ConfigureChannel(options =>
            {
                options.UnsafeUseInsecureChannelCallCredentials = true;
            })
            .AddJwtCallCredentials();
        
        return services;
    }

    private static IServiceCollection AddOrganizationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var organizationServiceAddress = new Uri(configuration["ServiceAddresses:OrganizationService"]!);
        
        services.AddGrpcClient<OrganizationService.Contracts.OrganizationService.OrganizationServiceClient>(options =>
            {
                options.Address = organizationServiceAddress;
            })
            .AddHttpMessageHandler<AuthHeaderHandler>();

        return services;
    }

    private static IServiceCollection AddProjectServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var projectServiceAddress = new Uri(configuration["ServiceAddresses:ProjectService"]!);

        services.AddGrpcClient<ProjectService.Contracts.ProjectService.ProjectServiceClient>(options =>
            {
                options.Address = projectServiceAddress;
            })
            .AddHttpMessageHandler<AuthHeaderHandler>();

        return services;
    }


    private static IServiceCollection AddOrganizationUserServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var organizationServiceAddress = new Uri(configuration["ServiceAddresses:OrganizationUserService"]!);
        
        services.AddGrpcClient<OrganizationUserService.Contracts.OrganizationUserService.OrganizationUserServiceClient>(options =>
            {
                options.Address = organizationServiceAddress;
            })
            .AddHttpMessageHandler<AuthHeaderHandler>();

        return services;
    }
}