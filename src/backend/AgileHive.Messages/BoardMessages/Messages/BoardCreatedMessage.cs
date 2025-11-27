namespace BoardMessages.Messages;

public class BoardCreatedMessage
{
    public Guid OrganizationId { get; set; }
    
    public Guid ProjectId { get; set; }
    
    public Guid BoardId { get; set; }
    
    public string BoardName { get; set; } = string.Empty;
    
    public IReadOnlyList<Guid> BoardColumnIds { get; set; } = [];
    
    public Guid CreatedByUserId { get; set; }
}