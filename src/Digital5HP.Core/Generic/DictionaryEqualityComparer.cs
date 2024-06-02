namespace Digital5HP.Generic;

using System;
using System.Collections.Generic;
using System.Linq;

public class DictionaryEqualityComparer<TKey, TValue>(IEqualityComparer<TValue> valueComparer = null) : IEqualityComparer<IDictionary<TKey, TValue>>
{
#pragma warning disable MA0018
    public static readonly DictionaryEqualityComparer<TKey, TValue> Default = new();
#pragma warning restore MA0018

    private readonly IEqualityComparer<TValue> valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;

    public bool Equals(IDictionary<TKey, TValue> x, IDictionary<TKey, TValue> y)
    {
        if (ReferenceEquals(x, y)) return true;

        return x != null
               && y != null
               && x.Count == y.Count
               && !x.Keys.Except(y.Keys)
                    .Any()
               && !y.Keys.Except(x.Keys)
                    .Any()
               && x.All(pair => this.valueComparer.Equals(pair.Value, y[pair.Key]));
    }

    public int GetHashCode(IDictionary<TKey, TValue> obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var hashCode = new HashCode();
        foreach (var key in obj.Keys)
        {
            hashCode.Add(key);
        }

        foreach (var value in obj.Values)
        {
            hashCode.Add(value);
        }

        return hashCode.ToHashCode();
    }

#pragma warning disable CA1000, MA0018
    public static DictionaryEqualityComparer<TKey, TValue> Create(IEqualityComparer<TValue> valueComparer) =>
        new(valueComparer);
#pragma warning restore CA1000, MA0018
}
