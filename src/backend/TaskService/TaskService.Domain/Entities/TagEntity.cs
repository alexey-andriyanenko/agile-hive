namespace TaskService.Domain.Entities;

public class TagEntity
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }
    
    public Guid TenantId { get; set; }
    
    public required string Name { get; set; }

    public string? Color { get; set; }

    public ICollection<TaskTagEntity> TaskTags { get; set; } = [];

}