using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TenantContextService.Infrastructure;

public class UserContext
{
    public Guid UserId { get; }
    
    public UserContext(ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new ArgumentException("User ID claim is missing or invalid.");
        }
        
        UserId = userId;
    }
}