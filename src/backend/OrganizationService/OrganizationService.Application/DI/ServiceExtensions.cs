using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OrganizationService.Application.Validators;
using OrganizationService.Contracts;

namespace OrganizationService.Application.DI;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateOrganizationRequest>, CreateOrganizationRequestValidator>();
        
        services.AddScoped<Services.OrganizationMemberService>();
        
        return services;
    }
}