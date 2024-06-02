namespace Digital5HP;

using System;
using System.Collections.Generic;
using System.Linq;

public static class ListExtensions
{
    public static IList<T> AddIfNotNull<T>(this IList<T> list, params T[] items)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(items);

        foreach (var item in items)
        {
            list.AddIfNotNull(item);
        }

        return list;
    }

    public static IList<T> AddIfNotNull<T>(this IList<T> list, T item)
    {
        ArgumentNullException.ThrowIfNull(list);

        if (item != null)
        {
            list.Add(item);
        }

        return list;
    }

    public static IList<T> AddIfNotNull<T>(this IList<T> list, T? item)
        where T: struct
    {
        ArgumentNullException.ThrowIfNull(list);

        if (item != null)
        {
            list.Add((T)item);
        }

        return list;
    }

    public static IList<T> AddIfTrue<T>(this IList<T> list, bool condition, T item)
    {
        ArgumentNullException.ThrowIfNull(list);

        if (condition)
        {
            list.AddIfNotNull(item);
        }

        return list;
    }

    /// <summary>
    ///  Compares 2 collections, element to element, using the comparer provided. Order of elements is considered.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public static bool AreEqual<TSource, TDest>(this IReadOnlyCollection<TSource> source, IReadOnlyCollection<TDest> other, Func<TSource, TDest, bool> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        if (source == null && other == null) return true;

        if(source == null || other == null) return false;

        if (source.Count != other.Count) return false;

        for (var i = 0; i < source.Count; i++)
        {
            var s = source.ElementAt(i);
            var d = other.ElementAt(i);

            if(s == null && d == null) continue;
            if(s == null || d == null) return false;

            if (!comparer(s, d))
                return false;
        }

        return true;
    }
}
