using OrganizationService.Domain.Base;
using OrganizationService.Domain.ValueObjects;

namespace OrganizationService.Domain.Entities;

public sealed class Organization : Entity<Guid>
{
    public OrganizationName Name { get; private set; }
    public Guid OwnerUserId { get; private set; }

    private Organization(Guid id, OrganizationName name, Guid ownerUserId)
    {
        Id = id;
        Name = name;
        OwnerUserId = ownerUserId;
    }
    
    public static Organization Create(OrganizationName name, Guid ownerUserId)
    {
        if (ownerUserId == Guid.Empty)
            throw new ArgumentException("Owner user id must be provided.", nameof(ownerUserId));

        return new Organization(Guid.NewGuid(), name, ownerUserId);
    }
    
    public void Rename(OrganizationName newName)
    {
        if (newName is null)
            throw new ArgumentNullException(nameof(newName));

        if (newName.Equals(Name))
            return;

        Name = newName;
    }
    
    public void TransferOwnership(Guid newOwnerUserId)
    {
        if (newOwnerUserId == Guid.Empty)
            throw new ArgumentException("New owner user id must be provided.", nameof(newOwnerUserId));

        if (newOwnerUserId == OwnerUserId)
            return;

        OwnerUserId = newOwnerUserId;
    }
}