using Cyrus.Core.Domain.Exceptions;
using Cyrus.Core.Domain.ValueObjects;

namespace Cyrus.Core.Domain.Toolkits.ValueObjects;

public class Title : BaseValueObject<Title>
{
    public string Value { get; private set; }

    public static Title FromString(string value) => new Title(value);

    private Title(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidValueObjectStateException("ValidationErrorIsRequire {0}", nameof(Title));
        }

        if (value.Length < 2 || value.Length > 250)
        {
            throw new InvalidValueObjectStateException("ValidationErrorStringLength {0} {1} {2}", nameof(Title), "2",
                "250");
        }

        Value = value;
    }

    private Title()
    {
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static explicit operator string(Title title) => title.Value;
    public static implicit operator Title(string value) => new(value);
    public override string ToString() => Value;
}