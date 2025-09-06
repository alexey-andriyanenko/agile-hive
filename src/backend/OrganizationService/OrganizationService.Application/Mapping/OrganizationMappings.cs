
using OrganizationService.Contracts;

namespace OrganizationService.Application.Mapping;

public static class OrganizationMappings
{
    public static OrganizationDto ToDto(this Domain.Entities.Organization organization, Domain.Entities.OrganizationMember organizationMember) =>
        new()
        {
            Id = organization.Id.ToString(),
            Name = organization.Name,
            Slug = organization.Slug,
            MyRole = (OrganizationMemberRole)organizationMember.Role,
        };
}