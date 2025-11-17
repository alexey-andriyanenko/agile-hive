using TenantContextService.Domain.Enum;

namespace TenantContextService.Domain.Entities;

public class TenantEntity
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;
}