using IdentityService.Domain.Entities;

namespace IdentityService.Domain.Constants;

public static class AppRoles
{
    public static readonly Role Admin = new()
    {
        Id = Guid.Parse("252856b5-fbd5-417e-81a1-fcc932f6266d"),
        Name = "Admin",
    };

    public static readonly Role Manager = new()
    {
        Id = Guid.Parse("996ab5b1-24bd-4ad0-8381-f0075562ac6f"),
        Name = "Manager",
    };
    
    public static readonly Role Employee = new()
    {
        Id = Guid.Parse("b51d5649-1357-4645-a2c1-fec2503007f8"),
        Name = "Employee",
    };
}