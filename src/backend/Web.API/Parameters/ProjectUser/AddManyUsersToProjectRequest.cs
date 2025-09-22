namespace Web.API.Parameters.ProjectUser;

public class AddManyUsersToProjectRequest
{
    public Guid ProjectId { get; set; }
    
    public List<AddUserToProjectItem> Users { get; set; } = [];
}

public class AddUserToProjectItem
{
    public Guid UserId { get; set; }
    
    public ProjectUserService.Contracts.ProjectUserRole Role { get; set; }
}
