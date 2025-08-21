using ProjectService.Domain.Enums;

namespace ProjectService.Domain.Entities;

public class ProjectMember
{
    public Guid UserId { get; set; }
    
    public Guid ProjectId { get; set; }
    
    public ProjectMemberRole Role { get; set; }
}
