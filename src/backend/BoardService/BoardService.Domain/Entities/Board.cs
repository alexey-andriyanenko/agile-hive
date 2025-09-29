namespace BoardService.Domain.Entities;

public class Board
{
    public Guid Id { get; set; }
    
    public required string Name { get; set; }
    
    public Guid ProjectId { get; set; }
    
    public Guid CreatedByUserId { get; set; }
    
    public Guid BoardTypeId { get; set; }
    
    public BoardType? BoardType { get; set; }

    public ICollection<BoardColumn> Columns { get; set; } = [];
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}