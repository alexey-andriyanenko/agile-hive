using Grpc.Core;

namespace IdentityService.Application.Exceptions;

public class RefreshTokenAlreadyRevokedException() : RpcException(new Status(StatusCode.PermissionDenied, "Refresh token has already been revoked."))
{
    
}