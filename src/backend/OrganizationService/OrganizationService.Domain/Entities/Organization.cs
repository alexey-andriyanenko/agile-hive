using OrganizationService.Domain.Base;

namespace OrganizationService.Domain.Entities;

public sealed class Organization(Guid id) : Entity<Guid>(id)
{
    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;
    
    public ICollection<OrganizationMember> Members { get; private set; } = new List<OrganizationMember>();
}
