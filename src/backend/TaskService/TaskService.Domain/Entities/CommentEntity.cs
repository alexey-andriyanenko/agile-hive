namespace TaskService.Domain.Entities;

public class CommentEntity
{
    public Guid Id { get; set; }
    
    public Guid TaskId { get; set; }
    
    public TaskEntity? Task { get; set; }
    
    public required string Content { get; set; }
    
    public Guid CreatedByUserId { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}