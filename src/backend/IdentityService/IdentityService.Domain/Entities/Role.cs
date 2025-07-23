using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain.Entities;

public class Role : IdentityRole<Guid>
{
    public ICollection<User> Users { get; set; }
}