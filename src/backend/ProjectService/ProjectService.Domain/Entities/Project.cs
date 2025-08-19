using ProjectService.Domain.Enums;

namespace ProjectService.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    
    public required string Name { get; set; }
    
    public required string Description { get; set; }
    
    public required string Slug { get; set; }
    
    public Guid CreateByUserId { get; set; }
    
    public ProjectVisibility Visibility { get; set; }
    
    public Guid OrganizationId { get; set; }
    
    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}
