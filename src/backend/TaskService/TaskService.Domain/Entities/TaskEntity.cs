namespace TaskService.Domain.Entities;

public class TaskEntity
{
    public Guid Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid BoardId { get; set; }
    public Guid BoardColumnId { get; set; }

    public Guid CreatedByUserId { get; set; }
    public Guid? AssigneeUserId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<TaskTagEntity> TaskTags { get; set; } = [];
}
