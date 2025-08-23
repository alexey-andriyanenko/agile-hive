using OrganizationService.gRPC;

namespace OrganizationService.Application.Mapping;

public static class OrganizationMappings
{
    public static OrganizationDto ToDto(this Domain.Entities.Organization organization) =>
        new()
        {
            Id = organization.Id.ToString(),
            Name = organization.Name.Value,
        };
}