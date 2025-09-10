namespace Web.API.Parameters.Project;

public static class Extensions
{
    public static ProjectService.Contracts.CreateProjectRequest ToGrpc(this CreateProjectRequest request)
    {
        return new ProjectService.Contracts.CreateProjectRequest
        {
            Name = request.Name,
            Description = request.Description,
            OrganizationId = request.OrganizationId.ToString(),
            Visibility = request.Visibility,
            Members =
            { request.Users.Select(u => new ProjectService.Contracts.CreateProjectWithUserItem
                {
                    UserId = u.UserId.ToString(),
                    Role = u.Role
                })
            }
        };
    }
}