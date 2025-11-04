using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using PermissionService.Domain.Interfaces;

namespace PermissionService.Infrastructure;

public class UserContext : IUserContext
{
    public Guid UserId { get; }
    
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