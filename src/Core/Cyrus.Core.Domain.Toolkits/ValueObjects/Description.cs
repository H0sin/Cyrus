using Cyrus.Core.Domain.Exceptions;
using Cyrus.Core.Domain.ValueObjects;

namespace Cyrus.Core.Domain.Toolkits.ValueObjects;

public class Description : BaseValueObject<Description>
{
    public string Value { get; private set; }
    
    public static Description FromString(string value) => new(value);

    private Description(string value)
    {
        if (!string.IsNullOrWhiteSpace(value) && value.Length > 500)
        {
            throw new InvalidValueObjectStateException("ValidationErrorIsRequire", nameof(Description), "0", "500");
        }

        Value = value;
    }

    private Description()
    {
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static explicit operator string(Description description) => description.Value;

    public static implicit operator Description(string value) => new(value);
}