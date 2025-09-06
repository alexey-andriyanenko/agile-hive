using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace OrganizationService.Infrastructure.Interceptors;

public class ValidationInterceptor(IServiceProvider serviceProvider) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var validator = serviceProvider.GetService<IValidator<TRequest>>();
        if (validator == null)
        {
            return await continuation(request, context);
        }

        var result = await validator.ValidateAsync(request);
        if (result.IsValid)
        {
            return await continuation(request, context);
        }

        var metadata = new Metadata();
        foreach (var error in result.Errors)
        {
            metadata.Add(error.PropertyName, error.ErrorMessage);
        }

        throw new RpcException(
            new Status(StatusCode.InvalidArgument, "Validation failed"), 
            metadata
        );
    }
}