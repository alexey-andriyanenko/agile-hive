using System.Text;
using BoardService.Contracts;
using BoardService.Infrastructure.DelegatingHandlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TaskAggregatorService.Infrastructure.Interceptors;

namespace TaskAggregatorService.Infrastructure.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
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
        services.AddScoped<TokenProvider>();

        services.AddGrpcClient<ProjectUserService.Contracts.ProjectUserService.ProjectUserServiceClient>(options =>
            {
                options.Address = new Uri(configuration["ServiceAddresses:UserService"]!);
            })
            .ConfigureChannel(options => { options.UnsafeUseInsecureChannelCallCredentials = true; })
            .AddJwtCallCredentials();

        services.AddGrpcClient<BoardColumnService.BoardColumnServiceClient>(options =>
            {
                options.Address = new Uri(configuration["ServiceAddresses:BoardService"]!);
            })
            .ConfigureChannel(options => { options.UnsafeUseInsecureChannelCallCredentials = true; })
            .AddJwtCallCredentials();

        services.AddGrpcClient<TaskService.Contracts.TaskService.TaskServiceClient>(options =>
            {
                options.Address = new Uri(configuration["ServiceAddresses:TaskService"]!);
            })
            .ConfigureChannel(options => { options.UnsafeUseInsecureChannelCallCredentials = true; })
            .AddJwtCallCredentials();

        services.AddGrpc(options =>
        {
            options.Interceptors.Add<AuthInterceptor>();
            options.Interceptors.Add<ValidationInterceptor>();
        });

        return services;
    }
}