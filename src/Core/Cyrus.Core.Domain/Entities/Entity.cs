using Cyrus.Core.Domain.ValueObjects;

namespace Cyrus.Core.Domain.Entities;

/// <summary>
/// Base class for all entities in the domain model.
/// </summary>
public abstract class Entity<TId> : IAuditableEntity, IEquatable<Entity<TId>>
    where TId : struct,
    IComparable,
    IComparable<TId>,
    IConvertible,
    IEquatable<TId>,
    IFormattable
{
    /// <summary>
    /// Database numeric identifier. Used primarily for persistence concerns.
    /// </summary>
    public TId Id { get; protected set; }

    /// <summary>
    /// Business identity of the entity. This is the primary identity used across the system for relations.
    /// </summary>
    public BusinessId BusinessId { get; protected set; } = BusinessId.FromGuid(Guid.NewGuid());

    /// <summary>
    /// Protected default constructor to support ORM materialization.
    /// </summary>
    protected Entity() { }

    #region Equality Check
    /// <summary>
    /// Entities are considered equal if their <see cref="BusinessId"/> values are equal.
    /// </summary>
    public bool Equals(Entity<TId>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return BusinessId.Equals(other.BusinessId);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is Entity<TId> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => BusinessId.GetHashCode();

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !(left == right);
    #endregion
}

public abstract class Entity : Entity<long>
{
}
