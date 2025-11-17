using TenantContextService.Domain.Enum;

namespace TenantContextService.Domain.Entities;

public class TenantMemberReadEntity
{
    public Guid TenantId { get; set; }
    
    public Guid UserId { get; set; }
    
    public TenantMemberRole Role { get; set; }
}