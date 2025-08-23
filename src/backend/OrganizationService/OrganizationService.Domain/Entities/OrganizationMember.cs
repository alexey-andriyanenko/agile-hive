using OrganizationService.Domain.Base;
using OrganizationService.Domain.Enums;

namespace OrganizationService.Domain.Entities;

public class OrganizationMember()
{
    public required Guid OrganizationId { get; set; }

    public required Guid UserId { get; set; }
    
    public OrganizationMemberRole Role { get; set; }
}
