namespace Digital5HP;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public static class EnumerableExtensions
{
    /// <summary>
    /// Returns <see langword="true"/> if collection is null or has no members, otherwise returns <see langword="false"/>.
    /// </summary>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
    {
        return collection == null || !collection.Any();
    }

    public static int Count(this IEnumerable source)
    {
        switch (source)
        {
            case null:
                throw new ArgumentNullException(nameof(source));
            case ICollection col:
                return col.Count;
        }

        var c = 0;
        foreach (var _ in source)
        {
            c++;
        }

        return c;
    }

    /// <summary>
    /// Return string representation of the collection of objects provided.
    /// </summary>
    /// <remarks>
    /// For example:
    /// <para>
    ///     [A, B, C]
    /// </para>
    /// Or
    /// <para>
    ///     [12, 89, 992]
    /// </para>
    /// </remarks>
    public static string ToAuditString<T>(this IEnumerable<T> collection, string separator = ", ")
    {
        return $"[{string.Join(separator, collection)}]";
    }

    /// <summary>
    /// Gets the index of element <paramref name="value"/> within collection <paramref name="source"/> using default equality comparer.
    /// </summary>
    /// <param name="source">Collection</param>
    /// <param name="value">Element</param>
    /// <returns>Index of element in collection</returns>
    public static int IndexOf<T>(this IEnumerable<T> source, T value)
    {
        return source.IndexOf(value, comparer: null);
    }

    /// <summary>
    /// Gets the index of element <paramref name="value"/> within collection <paramref name="source"/> using equality comparer provided.
    /// </summary>
    /// <param name="source">Collection</param>
    /// <param name="value">Element</param>
    /// <param name="comparer">Equality comparer to use</param>
    /// <returns>Index of element in collection</returns>
    public static int IndexOf<T>(this IEnumerable<T> source, T value, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(source);

        comparer ??= EqualityComparer<T>.Default;

        var index = 0;
        foreach (var item in source)
        {
            if (comparer.Equals(item, value)) return index;
            index++;
        }
        return -1;
    }

    /// <summary>
    /// Determines whether sequences compared are equal by comparing the elements using a specified <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <remarks>This method safely compares and handles <c>null</c> as input sequence.</remarks>
    /// <param name="first">An <see cref="IEnumerable{T}"/> to compare to <paramref name="second"/>.</param>
    /// <param name="second">An <see cref="IEnumerable{T}"/> to compare to <paramref name="first"/>.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to use to compare elements.</param>
    /// <typeparam name="TSource">The type of the elements of input sequences.</typeparam>
    /// <returns><c>true</c> if the two source sequences are of equal length and their corresponding elements compare equal according to comparer; otherwise, <c>false</c>.</returns>
    public static bool SequenceEqualSafe<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer = null)
    {
        comparer ??= EqualityComparer<TSource>.Default;

        if (ReferenceEquals(first, second)) return true;

        return first != null
               && second != null
               && first.SequenceEqual(second, comparer);
    }

    /// <summary>
    ///     Returns the results of the query in batches of a specified size.
    ///     This will result in a number of queries equalling (TotalRecords / BatchSize + 1)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="size">Number of records per batch.</param>
    /// <returns></returns>
    public static BatchEnumerable<T> InBatches<T>(this IEnumerable<T> collection, int size)
    {
        return new BatchEnumerable<T>(collection, size);
    }

    /// <summary>
    /// Executes the process delegate provided for each element in the collection, with max degree of concurrency provided.
    /// </summary>
    /// <param name="collection">Items</param>
    /// <param name="process">Asynchronous delegate</param>
    /// <param name="maxConcurrency">Max degree of concurrency</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <typeparam name="T">Item type</typeparam>
    /// <exception cref="ArgumentNullException"></exception>
    public static async Task ProcessConcurrentlyAsync<T>(this IEnumerable<T> collection,
                                                         Func<T, CancellationToken, Task> process,
                                                         int maxConcurrency,
                                                         CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(collection);

        ArgumentNullException.ThrowIfNull(process);

        using var semaphore = new SemaphoreSlim(maxConcurrency);

        var tasks = new List<Task>();

        foreach (var item in collection)
        {
            await semaphore.WaitAsync(cancellationToken);

            tasks.Add(
                Task.Run(
                    async () =>
                    {
                        try
                        {
                            await process(item, cancellationToken);
                        }
                        finally
                        {
                            // ReSharper disable once AccessToDisposedClosure
                            semaphore.Release();
                        }
                    },
                    cancellationToken));
        }

        await Task.WhenAll(tasks);
    }


    /// <summary>
    /// Transforms each element in the collection using the transform delegate provided, with max degree of concurrency provided.
    /// </summary>
    /// <param name="collection">Items</param>
    /// <param name="transform">Asynchronous delegate</param>
    /// <param name="maxConcurrency">Max degree of concurrency</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <typeparam name="T">Item type</typeparam>
    /// <typeparam name="TResult">Result type</typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static async Task<TResult[]> TransformConcurrentlyAsync<T, TResult>(this IEnumerable<T> collection,
        Func<T, CancellationToken, Task<TResult>> transform,
        int maxConcurrency,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(collection);

        ArgumentNullException.ThrowIfNull(transform);

        using var semaphore = new SemaphoreSlim(maxConcurrency);

        var tasks = new List<Task<TResult>>();

        foreach (var item in collection)
        {
            await semaphore.WaitAsync(cancellationToken);

            tasks.Add(
                Task.Run(
                    async () =>
                    {
                        try
                        {
                            return await transform(item, cancellationToken);
                        }
                        finally
                        {
                            // ReSharper disable once AccessToDisposedClosure
                            semaphore.Release();
                        }
                    },
                    cancellationToken));
        }

        return await Task.WhenAll(tasks);
    }
}
