using FluentValidation;
using IdentityService.Application.Validations;
using IdentityService.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Application.DI;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<Services.AuthService>();
        services.AddScoped<Services.TokenService>();
        services.AddScoped<Services.UserService>();
        
        services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
        services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();

        return services;
    }
}