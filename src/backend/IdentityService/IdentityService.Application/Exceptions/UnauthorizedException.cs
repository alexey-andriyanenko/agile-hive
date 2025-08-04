using Grpc.Core;

namespace IdentityService.Application.Exceptions;

public class UnauthorizedException() : RpcException(new Status(StatusCode.Unauthenticated, "Provided credentials are invalid"))
{
    
}