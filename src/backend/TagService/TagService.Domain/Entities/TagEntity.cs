namespace TagService.Domain.Entities;

public class TagEntity
{
    public Guid Id { get; set; }
    
    public Guid TenantId { get; set; }
    
    public Guid? ProjectId { get; set; }
    
    public required string Name { get; set; }
    
    public required string Color { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}
