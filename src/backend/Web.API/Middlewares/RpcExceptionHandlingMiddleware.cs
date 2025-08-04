using Grpc.Core;
using Web.API.Exceptions;

namespace Web.API.Middlewares;

public class RpcExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (RpcException ex)
        {
            context.Response.StatusCode = ex.Status.StatusCode switch
            {
                StatusCode.PermissionDenied => StatusCodes.Status403Forbidden,
                StatusCode.NotFound => StatusCodes.Status404NotFound,
                StatusCode.InvalidArgument => StatusCodes.Status400BadRequest,
                StatusCode.Unauthenticated => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };
            context.Response.ContentType = "application/json";
            
            var response = new HttpFromRpcException()
            {
                ErrorMessage = ex.Status.Detail,
                StatusCode = ex.Status.StatusCode.ToString(),
            };
            
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}