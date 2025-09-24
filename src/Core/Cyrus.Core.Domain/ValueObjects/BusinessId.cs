using Cyrus.Core.Domain.Exceptions;

namespace Cyrus.Core.Domain.ValueObjects;

/// <summary>
/// Represents the business identity of an entity using a Guid value.
/// Prefer using this identity for relationships and equality across the domain.
/// </summary>
public sealed class BusinessId : BaseValueObject<BusinessId>
{
    /// <summary>
    /// Creates a BusinessId from a string representation of a GUID.
    /// </summary>
    public static BusinessId FromString(string value) => new(value);

    /// <summary>
    /// Creates a BusinessId from a Guid value.
    /// </summary>
    public static BusinessId FromGuid(Guid value) => new() { Value = value };

    /// <summary>
    /// Initializes a BusinessId from a string. The value must be a valid GUID.
    /// </summary>
    public BusinessId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidValueObjectStateException("ValidationErrorIsRequire", nameof(BusinessId));
        }
        if (Guid.TryParse(value, out Guid tempValue))
        {
            Value = tempValue;
        }
        else
        {
            throw new InvalidValueObjectStateException("ValidationErrorInvalidValue", nameof(BusinessId));
        }
    }

    private BusinessId() { }

    /// <summary>
    /// Underlying Guid value of the business identity.
    /// </summary>
    public Guid Value { get; private set; }

    /// <inheritdoc />
    public override string ToString() => Value.ToString();

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public static explicit operator string(BusinessId id) => id.Value.ToString();
    public static implicit operator BusinessId(string value) => new(value);

    public static explicit operator Guid(BusinessId id) => id.Value;
    public static implicit operator BusinessId(Guid value) => new() { Value = value };

}