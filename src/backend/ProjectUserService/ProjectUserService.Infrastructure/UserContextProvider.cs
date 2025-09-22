using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ProjectUserService.Domain.Interfaces;

namespace ProjectUserService.Infrastructure;

public class UserContextProvider(IHttpContextAccessor httpContextAccessor)
{
    public IUserContext GetUserContext() => (UserContext)httpContextAccessor.HttpContext!.Items["UserContext"]!;
}

public class UserContext : IUserContext
{
    public Guid UserId { get; set; }

    public UserContext(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub);
        
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new ArgumentException("User ID claim is missing or invalid.");
        }
        
        UserId = userId;
    }
}