using OrganizationService.Contracts;

namespace Web.API.Dtos.Organization;

public class OrganizationDto
{
    public Guid Id { get; set; }
    
    public required string Name { get; set; }
    
    public required string Slug { get; set; }
    
    public bool IsActive { get; set; }
    
    public OrganizationMemberRole MyRole { get; set; }
}