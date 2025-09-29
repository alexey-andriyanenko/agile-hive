namespace BoardService.Domain.Entities;

public class BoardType
{
    public Guid Id { get; set; }
    
    public required string Name { get; set; }
    
    // Indicates if the board type is essential and cannot be deleted
    public bool IsEssential { get; set; }
    
    public ICollection<Board> Boards { get; set; } = [];
}
