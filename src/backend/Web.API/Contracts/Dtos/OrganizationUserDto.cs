using OrganizationService.Contracts;

namespace Web.API.Contracts.Dtos;

public class OrganizationUserDto
{
    public Guid Id { get; set; }
    
    public Guid OrganizationId { get; set; }
    
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public required string Email { get; set; }
    
    public required string UserName { get; set; }
    
    public string FullName => $"{FirstName} {LastName}";
    
    public OrganizationMemberRole Role { get; set; }
}