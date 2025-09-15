using IdentityService.Contracts;

namespace IdentityService.Application.Mappings;

public static class UserMappings
{
    public static UserDto ToDto(this Domain.Entities.User user)
    {
        return new UserDto
        {
            Id = user.Id.ToString(),
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            UserName = user.UserName,
        };
    }
}