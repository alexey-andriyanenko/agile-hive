using IdentityService.Contracts.Protos;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers;

[ApiController]
[Route("api/v1/identity")]
public class IdentityController(Auth.AuthClient authClient)
{
    [HttpPost("login")]
    public async Task<LoginResponse> Login([FromBody] LoginRequest request)
    {
        return await authClient.LoginAsync(request).ResponseAsync;
    }

    [HttpPost("register")]
    public async Task<RegisterResponse> Register([FromBody] RegisterRequest request)
    {
        return await authClient.RegisterAsync(request).ResponseAsync;
    }
}
