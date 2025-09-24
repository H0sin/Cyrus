namespace Cyrus.Core.Domain.ValueObjects;

/// <summary>
/// Base class for all Value Objects.
/// Value Objects are immutable types defined by their structural equality rather than identity.
/// See: https://martinfowler.com/bliki/ValueObject.html
/// </summary>
/// <typeparam name="TValueObject">Concrete value object type inheriting from this base class.</typeparam>
public abstract class BaseValueObject<TValueObject> : IEquatable<TValueObject>
    where TValueObject : BaseValueObject<TValueObject>
{
    /// <summary>
    /// Strongly-typed equality check based on equality components.
    /// </summary>
    public bool Equals(TValueObject? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <inheritdoc />
    public sealed override bool Equals(object? obj)
        => obj is TValueObject other && Equals(other);

    /// <inheritdoc />
    public sealed override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var component in GetEqualityComponents())
        {
            hash.Add(component);
        }
        return hash.ToHashCode();
    }

    /// <summary>
    /// Returns the sequence of values that participate in equality.
    /// </summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public static bool operator ==(BaseValueObject<TValueObject>? left, BaseValueObject<TValueObject>? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals((TValueObject)right);
    }

    public static bool operator !=(BaseValueObject<TValueObject>? left, BaseValueObject<TValueObject>? right)
        => !(left == right);
}