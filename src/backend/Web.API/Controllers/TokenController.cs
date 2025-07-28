using IdentityService.Contracts.Protos;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers;

[ApiController]
[Route("api/v1/token")]
public class TokenController(Token.TokenClient tokenClient)
{
    [HttpPost("refresh")]
    public async Task<RefreshTokenResponse> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
    {
        return await tokenClient.RefreshTokenAsync(request).ResponseAsync;
    }
}
