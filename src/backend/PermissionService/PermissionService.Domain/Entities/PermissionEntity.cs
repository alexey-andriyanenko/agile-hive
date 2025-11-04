using PermissionService.Domain.Enums;

namespace PermissionService.Domain.Entities;

public class PermissionEntity
{
    public Guid Id { get; set; }
    
    public Guid TenantId { get; set; }
 
    public ScopeType Scope { get; set; }
    
    public string Role { get; set; } = string.Empty;
    
    public ResourceType Resource { get; set; }
    
    public OperationType Operation { get; set; }
    
    public bool Allowed { get; set; }
}
