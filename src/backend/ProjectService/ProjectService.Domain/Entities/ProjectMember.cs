namespace ProjectService.Domain.Enums;

public class ProjectMember
{
    public Guid UserId { get; set; }
    
    public Guid ProjectId { get; set; }
    
    public ProjectMemberRole ProjectRole { get; set; }
}
