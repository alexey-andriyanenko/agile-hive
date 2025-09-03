using IdentityService.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers;

[ApiController]
[Route("api/v1/token")]
public class TokenController(TokenService.TokenServiceClient tokenClient)
{
    [HttpPost("refresh")]
    public async Task<RefreshTokenResponse> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
    {
        return await tokenClient.RefreshTokenAsync(request).ResponseAsync;
    }
}
