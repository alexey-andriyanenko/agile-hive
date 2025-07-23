using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
    
    public Guid? TenantId { get; set; }
    
    public Tenant? Tenant { get; set; }
    
    public Guid RoleId { get; set; }
    
    public Role Role { get; set; }
}
