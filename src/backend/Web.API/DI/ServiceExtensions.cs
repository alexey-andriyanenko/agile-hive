using System.Text;
using BoardService.Contracts;
using IdentityService.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Tag.Contracts;
using Web.API.DelegatingHandlers;
using Web.API.Exceptions;
using Web.API.Policies.Tenant;

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

        services.AddAuthorization(options =>
        {
            options.AddPolicy("TenantAccess", policy =>
                policy.RequireAuthenticatedUser()
                    .AddRequirements(new TenantRequirement()));
        });
        
        services.AddTransient<TenantMessageHandler>();
        
        services.AddIdentityServices(configuration);
        services.AddOrganizationServices(configuration);
        services.AddProjectServices(configuration);
        services.AddOrganizationUserServices(configuration);
        services.AddProjectUserServices(configuration);
        services.AddBoardServices(configuration);
        services.AddTaskAggregatorServices(configuration);
        services.AddTagServices(configuration);
        services.AddTenantContextServices(configuration);
        
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
            .ConfigureChannel(options =>
            {
                options.UnsafeUseInsecureChannelCallCredentials = true;
            })
            .AddJwtCallCredentials()
            .AddHttpMessageHandler<TenantMessageHandler>();

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
            .ConfigureChannel(options =>
            {
                options.UnsafeUseInsecureChannelCallCredentials = true;
            })
            .AddJwtCallCredentials()
            .AddHttpMessageHandler<TenantMessageHandler>();

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
            .ConfigureChannel(options =>
            {
                options.UnsafeUseInsecureChannelCallCredentials = true;
            })
            .AddJwtCallCredentials()
            .AddHttpMessageHandler<TenantMessageHandler>();

        return services;
    }
    
    private static IServiceCollection AddProjectUserServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var projectUserServiceAddress = new Uri(configuration["ServiceAddresses:ProjectUserService"]!);
        
        services.AddGrpcClient<ProjectUserService.Contracts.ProjectUserService.ProjectUserServiceClient>(options =>
            {
                options.Address = projectUserServiceAddress;
            })
            .ConfigureChannel(options =>
            {
                options.UnsafeUseInsecureChannelCallCredentials = true;
            })
            .AddJwtCallCredentials()
            .AddHttpMessageHandler<TenantMessageHandler>();

        return services;
    }

    private static IServiceCollection AddBoardServices(this IServiceCollection services, IConfiguration configuration)
    {
        var boardServiceAddress = new Uri(configuration["ServiceAddresses:BoardService"]!);
        
        services.AddGrpcClient<BoardService.Contracts.BoardService.BoardServiceClient>(options =>
        {
            options.Address = boardServiceAddress;
        })
        .ConfigureChannel(options =>
        {
            options.UnsafeUseInsecureChannelCallCredentials = true;
        })
        .AddJwtCallCredentials()
        .AddHttpMessageHandler<TenantMessageHandler>();


        services.AddGrpcClient<BoardColumnService.BoardColumnServiceClient>(options =>
        {
            options.Address = boardServiceAddress;
        })
        .ConfigureChannel(options =>
        {
            options.UnsafeUseInsecureChannelCallCredentials = true;
        })
        .AddJwtCallCredentials()
        .AddHttpMessageHandler<TenantMessageHandler>();
        
        return services;
    }

    private static IServiceCollection AddTaskAggregatorServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var taskAggregatorServiceAddress = new Uri(configuration["ServiceAddresses:TaskAggregatorService"]!);
        
        services.AddGrpcClient<TaskAggregatorService.Contracts.TaskAggregateService.TaskAggregateServiceClient>(options =>
        {
            options.Address = taskAggregatorServiceAddress;
        })
        .ConfigureChannel(options =>
        {
            options.UnsafeUseInsecureChannelCallCredentials = true;
        })
        .AddJwtCallCredentials()
        .AddHttpMessageHandler<TenantMessageHandler>();
        
        return services;
    }

    private static IServiceCollection AddTagServices(this IServiceCollection services, IConfiguration configuration)
    {
        var tagServiceAddress = new Uri(configuration["ServiceAddresses:TagService"]!);
        
        services.AddGrpcClient<TagService.TagServiceClient>(options =>
        {
            options.Address = tagServiceAddress;
        })
        .ConfigureChannel(options =>
        {
            options.UnsafeUseInsecureChannelCallCredentials = true;
        })
        .AddJwtCallCredentials()
        .AddHttpMessageHandler<TenantMessageHandler>();
        
        return services;
    }

    public static IServiceCollection AddTenantContextServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var tenantServiceAddress = new Uri(configuration["ServiceAddresses:TenantContextService"]!);
        
        services.AddGrpcClient<TenantContextService.Contracts.TenantContextService.TenantContextServiceClient>(options =>
            {
                options.Address = tenantServiceAddress;
            })
            .ConfigureChannel(options =>
            {
                options.UnsafeUseInsecureChannelCallCredentials = true;
            })
            .AddJwtCallCredentials()
            .AddHttpMessageHandler<TenantMessageHandler>();
        
        return services;
    }
}