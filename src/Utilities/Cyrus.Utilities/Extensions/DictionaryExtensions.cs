namespace Cyrus.Utilities.Extensions;

/// <summary>
/// Helpful extensions for dictionaries to simplify common patterns.
/// </summary>
public static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> valueFactory)
        where TKey : notnull
    {
        if (!dict.TryGetValue(key, out var value))
        {
            value = valueFactory();
            dict[key] = value;
        }
        return value;
    }

    public static TValue? GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue? defaultValue = default)
        where TKey : notnull
        => dict.TryGetValue(key, out var value) ? value : defaultValue;

    public static bool TryGetAs<TKey, TValue, TOut>(this IDictionary<TKey, TValue> dict, TKey key, out TOut? value)
        where TKey : notnull
    {
        value = default;
        if (dict.TryGetValue(key, out var boxed) && boxed is TOut casted)
        {
            value = casted;
            return true;
        }
        return false;
    }

    public static void MergeWith<TKey, TValue>(this IDictionary<TKey, TValue> target, IEnumerable<KeyValuePair<TKey, TValue>> source, bool overwriteExisting = true)
        where TKey : notnull
    {
        foreach (var kv in source)
        {
            if (overwriteExisting || !target.ContainsKey(kv.Key))
            {
                target[kv.Key] = kv.Value;
            }
        }
    }

    public static int RemoveWhere<TKey, TValue>(this IDictionary<TKey, TValue> dict, Func<KeyValuePair<TKey, TValue>, bool> predicate)
        where TKey : notnull
    {
        var keys = dict.Where(predicate).Select(kv => kv.Key).ToList();
        foreach (var k in keys) dict.Remove(k);
        return keys.Count;
    }
}

