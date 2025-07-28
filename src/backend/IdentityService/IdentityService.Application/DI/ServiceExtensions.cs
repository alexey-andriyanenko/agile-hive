using FluentValidation;
using IdentityService.Application.Services;
using IdentityService.Application.Validations;
using IdentityService.Contracts.Protos;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Application.DI;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<TokenService>();

        services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
        services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();

        return services;
    }
}