namespace Web.API.Services;

public class TokenProvider(IHttpContextAccessor httpContextAccessor)
{
    public async Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        var token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new UnauthorizedAccessException("Authorization token is missing.");
        }
        
        return await Task.FromResult(token);
    }
}