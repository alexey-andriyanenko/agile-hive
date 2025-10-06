namespace Web.API.Parameters.Task;

public class CreateTaskParameters
{
    public Guid TenantId { get; set; }
    
    public Guid ProjectId { get; set; }
    
    public Guid BoardId { get; set; }
    
    public required string Title { get; set; }
    
    public required string Description { get; set; }
    
    public Guid BoardColumnId { get; set; }
    
    public Guid? AssigneeUserId { get; set; }
    
    public List<Guid> TagIds { get; set; } = [];
}