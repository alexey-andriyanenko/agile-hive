﻿namespace IdentityService.Domain.Entities;

public class Tenant
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public ICollection<User>  Users { get; set; }
}