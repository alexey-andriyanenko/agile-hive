using IdentityService.gRPC;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers;

[ApiController]
[Route("api/v1/identity")]
public class IdentityController(Auth.AuthClient authClient)
{
    [HttpPost("login")]
    public async Task<LoginResponse> LoginAsync([FromBody] LoginRequest request)
    {
        return await authClient.LoginAsync(request).ResponseAsync;
    }

    [HttpPost("register")]
    public async Task<RegisterResponse> RegisterAsync([FromBody] RegisterRequest request)
    {
        return await authClient.RegisterAsync(request).ResponseAsync;
    }
}
