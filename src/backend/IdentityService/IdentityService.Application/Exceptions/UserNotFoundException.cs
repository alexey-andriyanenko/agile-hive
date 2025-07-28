namespace IdentityService.Application.Exceptions;

public class UserNotFoundException(Guid userId) : Exception($"User with userId {userId} not found")
{
    
}

public class UserNotFoundByEmailException(string email) : Exception($"User with email {email} not found")
{
}

public class UserNotFoundByUserNameException(string username) : Exception($"User with username {username} not found")
{
    
}
