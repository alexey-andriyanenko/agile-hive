using OrganizationService.Domain.Base;
using OrganizationService.Domain.ValueObjects;

namespace OrganizationService.Domain.Entities;

public sealed class Organization : Entity<Guid>
{
    public OrganizationName Name { get; private set; }
    
    
    public ICollection<OrganizationMember> Members { get; private set; } = new List<OrganizationMember>();
    
    private Organization(Guid id, OrganizationName name) : base(id)
    {
        Name = name;
    }
    
    public static Organization Create(OrganizationName name)
    {
        return new Organization(Guid.NewGuid(), name);
    }
    
    public void Rename(OrganizationName newName)
    {
        if (newName is null)
            throw new ArgumentNullException(nameof(newName));

        if (newName.Equals(Name))
            return;

        Name = newName;
    }
}