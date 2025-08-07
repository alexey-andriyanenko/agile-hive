using OrganizationService.Domain.Base;

namespace OrganizationService.Domain.Entities;

public class OrganizationUser(Guid id, Guid organizationId, Guid userId) : Entity<Guid>(id)
{
    public Guid OrganizationId { get; set; } = organizationId;

    public Guid UserId { get; set; } = userId;
    
    // in far future
    // public Guid UserRoleId { get; set; }
    // public string UserStatus { get; set; } = string.Empty;
}
