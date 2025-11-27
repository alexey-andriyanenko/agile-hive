namespace OrganizationMessages.Messages;

public class OrganizationCreationSucceededMessage
{
    public Guid OrganizationId { get; set; }
    
    public Guid CreatedByUserId { get; set; }
    
    public string OrganizationName { get; set; } = string.Empty;
}