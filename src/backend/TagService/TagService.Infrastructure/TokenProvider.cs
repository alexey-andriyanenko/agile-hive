using Grpc.Core;

namespace TagService.Infrastructure;

public class TokenProvider
{
    public async Task<string> GetTokenAsync(Metadata metadata, CancellationToken cancellationToken = default)
    {
        var authHeader = metadata.FirstOrDefault(h => h.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase))?.Value;

        if (string.IsNullOrWhiteSpace(authHeader))
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Authorization token is missing."));
        }

        var token = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? authHeader.Substring("Bearer ".Length).Trim()
            : authHeader.Trim();

        return await Task.FromResult(token);
    }
}