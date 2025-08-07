namespace OrganizationService.Domain.Base;

public abstract class Entity<TId>(TId id)
    where TId : notnull
{
    public TId Id { get; protected init; } = id;

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        return Id.Equals(other.Id);
    }

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
}
