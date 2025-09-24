using System.Collections;

namespace Cyrus.Utilities.Guards.GuardClauses;

public static class EmptyGuardClause
{
    public static void Empty<T>(this Guard guard, T value, string message)
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentException("Message cannot be null or empty.", nameof(message));

        if (value == null)
            return;

        if (value is ICollection collectionValue && collectionValue.Count == 0)
            return;

        if (value is IEnumerable enumerableValue)
        {
            var enumerator = enumerableValue.GetEnumerator();
            try
            {
                if (!enumerator.MoveNext())
                    return;
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }
        }

        if (value is string strValue && string.IsNullOrWhiteSpace(strValue))
            return;

        if (!Equals(value, default(T)))
            throw new InvalidOperationException(message);
    }

    public static void Empty<T>(this Guard guard, T value, IEqualityComparer<T> equalityComparer, string message)
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentException("Message cannot be null or empty.", nameof(message));

        if (!equalityComparer.Equals(value, default!))
            throw new InvalidOperationException(message);
    }
}