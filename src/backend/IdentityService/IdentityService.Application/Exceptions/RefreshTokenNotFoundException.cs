using Grpc.Core;

namespace IdentityService.Application.Exceptions;

public class RefreshTokenNotFoundException() : RpcException(new Status(StatusCode.NotFound, "Refresh token not found."))
{
    
}