using ProjectUserService.Contracts;

namespace Web.API.Results.ProjectUser;

public class AddManyUsersToProjectResult
{
    public List<ProjectUserDto> Users { get; set; } = [];
}