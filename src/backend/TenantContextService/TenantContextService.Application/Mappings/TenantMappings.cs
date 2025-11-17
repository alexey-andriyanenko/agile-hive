using TenantContextService.Contracts;
using TenantContextService.Domain.Entities;

namespace TenantContextService.Application.Mappings;

public static class TenantMappings
{
    public static TenantResponse ToDto(this Domain.Entities.TenantEntity tenant, Domain.Entities.TenantMemberReadEntity tenantMember, IReadOnlyList<TenantDbEntity> tenantDbs) =>
        new()
        {
            Id = tenant.Id.ToString(),
            Name = tenant.Name,
            Slug = tenant.Slug,
            MyRole = (Contracts.TenantMemberRole)tenantMember.Role,
            IsActive = tenantDbs.All(x => x.Active)
        };
}