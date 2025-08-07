using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
    
    // public ICollection<Guid> OrganizationIds { get; set; } = new List<Guid>();
    
    public Guid RoleId { get; set; }
    
    public Role Role { get; set; } = new();
    
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
