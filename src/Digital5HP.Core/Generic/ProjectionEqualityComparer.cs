namespace Digital5HP.Generic;

using System;
using System.Collections.Generic;

/// <summary>
/// Helper class for <see cref="ProjectionEqualityComparer{TSource,TKey}"/>.
/// </summary>
public static class ProjectionEqualityComparer<TSource>
{
    /// <summary>
    ///  Instantiates a <see cref="ProjectionEqualityComparer{TSource,TKey}"/> using the projection provided.
    /// </summary>
#pragma warning disable CA1000, MA0018
    public static ProjectionEqualityComparer<TSource, TKey> Create<TKey>(Func<TSource, TKey> projection, IEqualityComparer<TKey> comparer = null)
#pragma warning restore CA1000, MA0018
    {
        return new ProjectionEqualityComparer<TSource, TKey>(projection, comparer);
    }
}

/// <summary>
/// Defines an <see cref="IEqualityComparer{T}"/> using projection to a child property of <typeparamref name="TSource"/>.
/// </summary>
public class ProjectionEqualityComparer<TSource, TKey>(Func<TSource, TKey> projection, IEqualityComparer<TKey> comparer = null) : IEqualityComparer<TSource>
{
    private readonly Func<TSource, TKey> projection = projection ?? throw new ArgumentNullException(nameof(projection));
    private readonly IEqualityComparer<TKey> comparer = comparer ?? EqualityComparer<TKey>.Default;

    public bool Equals(TSource x, TSource y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        return this.comparer.Equals(this.projection(x), this.projection(y));
    }

    public int GetHashCode(TSource obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        return this.comparer.GetHashCode(this.projection(obj));
    }
}
