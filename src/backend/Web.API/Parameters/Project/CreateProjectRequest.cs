using ProjectService.Contracts;

namespace Web.API.Parameters.Project;

public class CreateProjectRequest
{
    public required string Name { get; set; }
    
    public string Description { get; set; } = string.Empty;
    
    public Guid OrganizationId { get; set; }
    
    public ProjectVisibility Visibility { get; set; }

    public List<CreateProjectWithUserItemParameters> Users { get; set; } = [];


}

public class CreateProjectWithUserItemParameters
{
    public Guid UserId { get; set; }
    
    public ProjectMemberRole Role { get; set; }
}
