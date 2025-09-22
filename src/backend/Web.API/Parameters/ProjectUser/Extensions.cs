namespace Web.API.Parameters.ProjectUser;

public static class Extensions
{
    public static ProjectUserService.Contracts.AddManyUsersToProjectRequest ToGrpc(this AddManyUsersToProjectRequest request)
    {
        return new ProjectUserService.Contracts.AddManyUsersToProjectRequest
        {
            ProjectId = request.ProjectId.ToString(),
            Users =
            {
                request.Users.Select(u => new ProjectUserService.Contracts.AddUserToProjectItem
                {
                    UserId = u.UserId.ToString(),
                    Role = u.Role
                })
            }
        };
    }
}