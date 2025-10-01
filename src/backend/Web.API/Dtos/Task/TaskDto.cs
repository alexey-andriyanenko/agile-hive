namespace Web.API.Dtos.Task;

public class TaskDto
{
    public Guid Id { get; set; }
    
    public required string Title { get; set; }
    
    public required string description { get; set; }
    
    public Guid TenantId { get; set; }
    
    public Guid ProjectId { get; set; }
    
    public Guid BoardId { get; set; }
    
    public required TaskBoardColumnDto BoardColumn { get; set; }
    
    public required TaskUserDto CreatedBy { get; set; }
    
    public TaskUserDto? AssignedTo { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}

public class TaskUserDto
{
    public Guid Id { get; set; }
    
    public required string FullName { get; set; }
    
    public required string Email { get; set; }
}

public class TaskBoardColumnDto
{
    public Guid Id { get; set; }
    
    public required string Name { get; set; }
}
