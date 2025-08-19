using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ProjectService.Domain.Interfaces;

namespace ProjectService.Infrastructure;

public class UserContext : IUserContext
{
    public Guid UserId { get; }
    
    public Guid OrganizationId { get; }
    
    public UserContext(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new ArgumentException("User ID claim is missing or invalid.");
        }
        
        var organizationIdClaim = user.FindFirst("organization_id");
        if (organizationIdClaim == null || !Guid.TryParse(organizationIdClaim.Value, out var organizationId))
        {
            throw new ArgumentException("Organization ID claim is missing or invalid.");
        }
        
        UserId = userId;
        OrganizationId = organizationId;
    }
}