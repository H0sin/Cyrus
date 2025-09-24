using Cyrus.Core.Domain.Exceptions;
using Cyrus.Core.Domain.ValueObjects;
using Cyrus.Utilities.Extensions;

namespace Cyrus.Core.Domain.Toolkits.ValueObjects;

public class LegalNationalId : BaseValueObject<LegalNationalId>
{
    public string Value { get; private set; }
    
    public static LegalNationalId FromString(string value) => new(value);
    private LegalNationalId(string value)
    {
        if (!value.IsLegalNationalIdValid())
        {
            throw new InvalidValueObjectStateException("ValidationErrorStringFormat", nameof(LegalNationalId));
        }

        Value = value;
    }

    private LegalNationalId()
    {
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static explicit operator string(LegalNationalId title) => title.Value;
    public static implicit operator LegalNationalId(string value) => new(value);

    public override string ToString() => Value;
}