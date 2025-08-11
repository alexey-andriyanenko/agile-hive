using OrganizationService.Domain.Base;

namespace OrganizationService.Domain.Entities;

public class OrganizationUser(Guid id) : Entity<Guid>(id)
{
    public required Guid OrganizationId { get; set; }

    public required Guid UserId { get; set; }
    
    // in far future
    // public Guid UserRoleId { get; set; }
    // public string UserStatus { get; set; } = string.Empty;
}
