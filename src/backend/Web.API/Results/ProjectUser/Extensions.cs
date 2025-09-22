namespace Web.API.Results.ProjectUser;

public static class Extensions
{
    public static AddManyUsersToProjectResult ToHttp(this ProjectUserService.Contracts.AddManyUsersToProjectResponse response)
    {
        return new AddManyUsersToProjectResult
        {
            Users = response.Users.ToList()
        };
    }
}