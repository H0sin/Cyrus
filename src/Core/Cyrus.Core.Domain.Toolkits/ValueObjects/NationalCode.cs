using Cyrus.Core.Domain.Exceptions;
using Cyrus.Core.Domain.ValueObjects;
using Cyrus.Utilities.Extensions;

namespace Cyrus.Core.Domain.Toolkits.ValueObjects;

public class NationalCode : BaseValueObject<NationalCode>
{
    public string Value { get; private set; }

    public static NationalCode FromString(string value) => new(value);

    public NationalCode(string value)
    {
        if (!value.IsNationalCode())
        {
            throw new InvalidValueObjectStateException("ValidationErrorStringFormat", nameof(NationalCode));
        }

        Value = value;
    }

    private NationalCode()
    {
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static explicit operator string(NationalCode title) => title.Value;
    public static implicit operator NationalCode(string value) => new(value);
    public override string ToString() => Value;
}