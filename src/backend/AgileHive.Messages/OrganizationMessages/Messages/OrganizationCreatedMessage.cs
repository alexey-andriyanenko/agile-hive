namespace OrganizationMessages.Messages;

public class OrganizationCreatedMessage
{
    public Guid OrganizationId { get; set; }
    
    public string OrganizationName { get; set; } = string.Empty;
    
    public Guid OwnerUserId { get; set; }
}