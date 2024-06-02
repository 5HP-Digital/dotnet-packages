namespace Digital5HP.Generic;

using System;
using System.Collections.Generic;

/// <summary>
/// Defines an <see cref="IEqualityComparer{T}"/> using the provided comparer delegate.
/// </summary>
public class DelegateEqualityComparer<T>(Func<T, T, bool> comparer) : IEqualityComparer<T>
{
    private readonly Func<T, T, bool> comparer = comparer;

    public bool Equals(T x, T y)
    {
        return this.comparer(x, y);
    }

    public int GetHashCode(T obj)
    {
        return obj.GetHashCode();
    }
}
