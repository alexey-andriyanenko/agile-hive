using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using OrganizationService.Domain.Interfaces;

namespace OrganizationService.Infrastructure;

public class UserContext : IUserContext
{
    public Guid UserId { get; }
    
    public UserContext(ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new ArgumentException("User ID claim is missing or invalid.");
        }
        
        UserId = userId;
    }
}