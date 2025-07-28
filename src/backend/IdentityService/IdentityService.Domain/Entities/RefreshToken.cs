﻿namespace IdentityService.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    
    public string Token { get; set; }
    
    public DateTime ExpiresAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? RevokedAt { get; set; }
    
    public string? ReplacedByToken { get; set; }

    public Guid UserId { get; set; }
    
    public User User { get; set; }
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    
    public bool IsRevoked => RevokedAt != null;
}
