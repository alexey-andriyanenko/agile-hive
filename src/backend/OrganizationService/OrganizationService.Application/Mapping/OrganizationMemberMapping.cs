
using OrganizationService.Contracts;

namespace OrganizationService.Application.Mapping;

public static class OrganizationMemberMapping
{
    public static OrganizationMemberDto ToDto(this Domain.Entities.OrganizationMember organizationMember) =>
        new()
        {
            OrganizationId = organizationMember.OrganizationId.ToString(),
            UserId = organizationMember.UserId.ToString(),
            Role = (OrganizationMemberRole)organizationMember.Role,
        };
}