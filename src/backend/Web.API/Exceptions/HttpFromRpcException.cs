using System.Net;
using Grpc.Core;

namespace Web.API.Exceptions;

public class HttpFromRpcException
{
    public required string ErrorMessage { get; set; }
    public required string StatusCode { get; set; }
}