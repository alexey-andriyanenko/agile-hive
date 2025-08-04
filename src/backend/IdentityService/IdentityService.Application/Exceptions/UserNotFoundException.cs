using Grpc.Core;

namespace IdentityService.Application.Exceptions;

public class UserNotFoundException(Guid userId) : RpcException(new Status(StatusCode.NotFound, $"User with userId {userId} not found"))
{
    
}

public class UserNotFoundByEmailException(string email) : RpcException(new Status(StatusCode.NotFound, $"User with email {email} not found"))
{
}

public class UserNotFoundByUserNameException(string username) : RpcException(new Status(StatusCode.NotFound, $"User with username {username} not found"))
{
    
}
