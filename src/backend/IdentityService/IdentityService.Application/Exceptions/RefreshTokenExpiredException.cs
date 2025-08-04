using Grpc.Core;

namespace IdentityService.Application.Exceptions;

public class RefreshTokenExpiredException() : RpcException(new Status(StatusCode.Unauthenticated, "Refresh token has expired."))
{
    
}