namespace Cyrus.Utilities.Guards.GuardClauses;

/// <summary>
/// Common guard clauses to validate preconditions and throw meaningful exceptions early.
/// Inspired by popular guard patterns; kept dependency-free.
/// </summary>
public static class Against
{
    public static T Null<T>(T? input, string? parameterName = null, string? message = null)
    {
        if (input is null)
            throw new ArgumentNullException(parameterName ?? nameof(input), message ?? "Value cannot be null.");
        return input;
    }

    public static string NullOrEmpty(string? input, string? parameterName = null, string? message = null)
    {
        if (string.IsNullOrEmpty(input))
            throw new ArgumentException(message ?? "Value cannot be null or empty.", parameterName ?? nameof(input));
        return input;
    }

    public static string NullOrWhiteSpace(string? input, string? parameterName = null, string? message = null)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException(message ?? "Value cannot be null, empty, or whitespace.", parameterName ?? nameof(input));
        return input;
    }

    public static T Default<T>(T input, string? parameterName = null, string? message = null) where T : struct
    {
        if (EqualityComparer<T>.Default.Equals(input, default))
            throw new ArgumentException(message ?? "Value cannot be the default value.", parameterName ?? nameof(input));
        return input;
    }

    public static int Negative(int input, string? parameterName = null, string? message = null)
    {
        if (input < 0)
            throw new ArgumentOutOfRangeException(parameterName ?? nameof(input), input, message ?? "Value cannot be negative.");
        return input;
    }

    public static long Negative(long input, string? parameterName = null, string? message = null)
    {
        if (input < 0)
            throw new ArgumentOutOfRangeException(parameterName ?? nameof(input), input, message ?? "Value cannot be negative.");
        return input;
    }

    public static int Zero(int input, string? parameterName = null, string? message = null)
    {
        if (input == 0)
            throw new ArgumentOutOfRangeException(parameterName ?? nameof(input), input, message ?? "Value cannot be zero.");
        return input;
    }

    public static long Zero(long input, string? parameterName = null, string? message = null)
    {
        if (input == 0)
            throw new ArgumentOutOfRangeException(parameterName ?? nameof(input), input, message ?? "Value cannot be zero.");
        return input;
    }

    public static T OutOfRange<T>(T input, T min, T max, string? parameterName = null, string? message = null) where T : IComparable<T>
    {
        if (input.CompareTo(min) < 0 || input.CompareTo(max) > 0)
            throw new ArgumentOutOfRangeException(parameterName ?? nameof(input), input, message ?? $"Value must be in range [{min}, {max}].");
        return input;
    }

    public static IEnumerable<T> NullOrEmpty<T>(IEnumerable<T>? input, string? parameterName = null, string? message = null)
    {
        if (input is null)
            throw new ArgumentException(message ?? "Collection cannot be null or empty.", parameterName ?? nameof(input));

        if (input is ICollection<T> coll)
        {
            if (coll.Count == 0)
                throw new ArgumentException(message ?? "Collection cannot be null or empty.", parameterName ?? nameof(input));
            return coll;
        }

        var list = input.ToList();
        if (list.Count == 0)
            throw new ArgumentException(message ?? "Collection cannot be null or empty.", parameterName ?? nameof(input));
        return list;
    }

    public static IDictionary<TKey, TValue> NullOrEmpty<TKey, TValue>(IDictionary<TKey, TValue>? input, string? parameterName = null, string? message = null)
        where TKey : notnull
    {
        if (input is null || input.Count == 0)
            throw new ArgumentException(message ?? "Dictionary cannot be null or empty.", parameterName ?? nameof(input));
        return input;
    }

    public static void True(bool condition, string? parameterName = null, string? message = null)
    {
        if (condition)
            throw new ArgumentException(message ?? "Condition must be false.", parameterName ?? "condition");
    }

    public static void False(bool condition, string? parameterName = null, string? message = null)
    {
        if (!condition)
            throw new ArgumentException(message ?? "Condition must be true.", parameterName ?? "condition");
    }
}
