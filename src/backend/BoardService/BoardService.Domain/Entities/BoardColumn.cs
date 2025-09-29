namespace BoardService.Domain.Entities;

public class BoardColumn
{
    public Guid Id { get; set; }
    
    public Guid BoardId { get; set; }
    
    public Board? Board { get; set; }
    
    public int Order { get; set; }
    
    public required string Name { get; set; }
    
    public Guid CreatedByUserId { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}
