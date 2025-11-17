using OrganizationService.Contracts;

namespace Web.API.Dtos.Organization;

public static class Mappings
{
    public static OrganizationDto ToDto(this TenantContextService.Contracts.TenantResponse dto)=>
        new()
        {
            Id = Guid.Parse(dto.Id),
            Name = dto.Name,
            Slug = dto.Slug,
            IsActive = dto.IsActive,
            MyRole = (OrganizationMemberRole)dto.MyRole
        }; 
}