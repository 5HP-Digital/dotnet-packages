namespace Digital5HP;

using System.Collections.Generic;

public static class DictionaryExtensions
{
    /// <summary>
    /// Merges dictionary <paramref name="right"/> into dictionary <paramref name="left"/>.
    /// </summary>
    /// <remarks>
    /// If key exists in both dictionaries, value from <paramref name="right"/> will override <paramref name="left"/>.
    /// </remarks>
    public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> left,
                                                                IDictionary<TKey, TValue> right)
    {
        if (right != null && left != null)
        {
            foreach (var (key, value) in right)
                left[key] = value;
        }

        return left;
    }

    /// <summary>
    /// Converts a <see cref="IReadOnlyDictionary{TKey,TValue}"/> to a <see cref="IDictionary{TKey,TValue}"/>
    /// </summary>
    public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary)
    {
        return dictionary as Dictionary<TKey, TValue> ?? new Dictionary<TKey, TValue>(dictionary);
    }

    /// <summary>
    /// Converts a <see cref="IDictionary{TKey,TValue}"/> to a <see cref="IReadOnlyDictionary{TKey,TValue}"/>
    /// </summary>
    public static IReadOnlyDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
    {
        return dictionary as Dictionary<TKey, TValue> ?? new Dictionary<TKey, TValue>(dictionary);
    }
}
