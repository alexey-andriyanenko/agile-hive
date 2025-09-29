namespace Web.API.Dtos.Board;

public class BoardTypeDto
{
    public Guid Id { get; set; }
    
    public required string Name { get; set; }
    
    public bool IsEssential { get; set; }
}