namespace Cyrus.Core.RequestResponse.Queries;

/// <summary>
/// Supported filter operators for building structured filter conditions.
/// </summary>
public enum FilterOperator
{
    Equals,
    NotEquals,
    Contains,
    StartsWith,
    EndsWith,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    In,
    Between,
    IsNull,
    IsNotNull
}

/// <summary>
/// Represents a single filter condition for a field/property.
/// </summary>
/// <param name="Property">The name of the property/column to filter.</param>
/// <param name="Operator">The operator to apply.</param>
/// <param name="Value">The comparison value (or the first value in a range).</param>
/// <param name="Value2">Optional second value for range/between operations.</param>
/// <param name="CaseInsensitive">When true, string comparisons should be case-insensitive.</param>
public sealed record FilterCondition(
    string Property,
    FilterOperator Operator,
    object? Value = null,
    object? Value2 = null,
    bool CaseInsensitive = false
)
{
    /// <summary>Indicates whether this condition represents a range (Between) operation.</summary>
    public bool IsRange() => Operator == FilterOperator.Between;

    /// <summary>Indicates whether this condition checks for nullability.</summary>
    public bool IsNullCheck() => Operator is FilterOperator.IsNull or FilterOperator.IsNotNull;

    /// <summary>Indicates whether a second value is provided (for range operations).</summary>
    public bool HasSecondValue() => Value2 is not null;

    /// <summary>Indicates that the comparison is case-insensitive (for string-based operators).</summary>
    public bool IsCaseInsensitive() => CaseInsensitive;

    /// <inheritdoc />
    public override string ToString()
        => $"{Property} {Operator} {Value}{(HasSecondValue() ? $", {Value2}" : string.Empty)}";
}
