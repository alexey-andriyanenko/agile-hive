namespace ProjectMessages.Messages;

public class ProjectCreationSucceededMessage
{
    public Guid OrganizationId { get; set; }
    
    public Guid ProjectId { get; set; }
    
    public Guid CreatedByUserId { get; set; }
}