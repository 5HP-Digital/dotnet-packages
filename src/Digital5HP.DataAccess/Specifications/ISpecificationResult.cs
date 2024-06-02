namespace Digital5HP.DataAccess.Specifications;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

/// <summary>
/// Generic domain specification result interface that
/// contains methods common for filtering the result.
/// </summary>
/// <remarks>
/// If the specification model uses lazy loading using <see cref="System.Linq.IQueryable{T}"/>,
/// this is the place where the query is translated and executed by calling for example
/// <see cref="ToList"/> or <see cref="Single"/> methods.
///
/// The interface is very similar to IQueryable interface. It is basically
/// the wrapper over the IQueryable if used under the hood. The main goal is to
/// provide set of common methods used by all specifications. Other methods that
/// normally IQueryable supports should be implemented in <see cref="ISpecification{TEntity}"/>
/// interface using strong domain names of the method (e.g. joins, grouping, aggregate functions).
///
/// Note it is perfectly fine if from domain perspective the specification already
/// uses the functionality of this specification result. The specification should represents
/// one query that can be customized using fluent interface but in general it should be
/// strongly domain focused.
/// </remarks>
public interface ISpecificationResult<TEntity>
{
    IQueryable<TEntity> AsQueryable();

    /// <summary>
    /// Takes given count of domain objects.
    /// </summary>
    ISpecificationResult<TEntity> Take(int count);

    /// <summary>
    /// Bypasses a specified number of elements in a sequence and then returns
    /// the remaining elements.
    /// </summary>
    /// <param name="count">The number of elements to skip before returning
    /// the remaining elements.</param>
    ISpecificationResult<TEntity> Skip(int count);

    /// <summary>
    /// Sorts the elements of a sequence in ascending order according to a key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key returned by the function that is
    /// represented by keySelector.</typeparam>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    ISpecificationResult<TEntity> OrderByAscending<TKey>(Expression<Func<TEntity, TKey>> keySelector);

    /// <summary>
    /// Sorts the elements of a sequence in descending order according to a key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key returned by the function that is
    /// represented by keySelector.</typeparam>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    ISpecificationResult<TEntity> OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> keySelector);

    ISpecificationResult<TEntity> Distinct();

    /// <summary>
    /// Gets the list of domain objects the specification represents.
    /// </summary>
    IList<TEntity> ToList();

    /// <summary>
    /// Gets the list of domain objects the specification represents.
    /// </summary>
    Task<IList<TEntity>> ToListAsync();

    /// <summary>
    /// Projects the entities of type <typeparamref name="TEntity"/> to <typeparamref name="TResult"/> using the expression provided.
    /// </summary>
    ISpecificationResult<TResult> ProjectTo<TResult>(Expression<Func<TEntity, TResult>> selector);

    /// <summary>
    /// Gets the list of entities and projects each to <typeparamref name="TResult"/> using the current mapping provider.
    /// </summary>
    /// <remarks>
    /// See Digital5HP.ObjectMapping for more details.
    /// </remarks>
    IList<TResult> ToList<TResult>();

    /// <summary>
    /// Gets the list of entities and projects each to <typeparamref name="TResult"/> using the current mapping provider.
    /// </summary>
    /// <remarks>
    /// See Digital5HP.ObjectMapping for more details.
    /// </remarks>
    Task<IList<TResult>> ToListAsync<TResult>();

    /// <summary>
    /// Gets a single entity the specification represents.
    /// </summary>
    /// <remarks>
    /// This method throws an exception if there is more than one element in the sequence.
    /// </remarks>
#pragma warning disable CA1716, CA1720
    TEntity Single();
#pragma warning restore CA1720, CA1716

    /// <summary>
    /// Gets a single entity the specification represents and projects it to <typeparamref name="TResult"/> using the current mapping provider.
    /// </summary>
    /// <remarks>
    /// This method throws an exception if there is more than one element in the sequence.
    /// </remarks>
#pragma warning disable CA1716, CA1720
    TResult Single<TResult>();
#pragma warning restore CA1716, CA1720

    /// <summary>
    /// Gets a single entity the specification represents.
    /// </summary>
    /// <remarks>
    /// This method throws an exception if there is more than one element in the sequence.
    /// </remarks>
    Task<TEntity> SingleAsync();

    /// <summary>
    /// Gets a single entity the specification represents and projects it to <typeparamref name="TResult"/> using the current mapping provider.
    /// </summary>
    /// <remarks>
    /// This method throws an exception if there is more than one element in the sequence.
    /// </remarks>
    Task<TResult> SingleAsync<TResult>();

    /// <summary>
    /// Returns the only element of a sequence, or a default value if the sequence is empty.
    /// </summary>
    /// <remarks>
    /// This method throws an exception if there is more than one element in the sequence.
    /// </remarks>
    TEntity SingleOrDefault();

    /// <summary>
    /// Returns the only element of a sequence, or a default value if the sequence is empty, and projects it to <typeparamref name="TResult"/> using the current mapping provider.
    /// </summary>
    /// <remarks>
    /// This method throws an exception if there is more than one element in the sequence.
    /// </remarks>
    TResult SingleOrDefault<TResult>();

    /// <summary>
    /// Returns the only element of a sequence, or a default value if the sequence is empty.
    /// </summary>
    /// <remarks>
    /// This method throws an exception if there is more than one element in the sequence.
    /// </remarks>
    Task<TEntity> SingleOrDefaultAsync();

    /// <summary>
    /// Returns the only element of a sequence, or a default value if the sequence is empty, and projects it to <typeparamref name="TResult"/> using the current mapping provider.
    /// </summary>
    /// <remarks>
    /// This method throws an exception if there is more than one element in the sequence.
    /// </remarks>
    Task<TResult> SingleOrDefaultAsync<TResult>();

    /// <summary>
    /// Gets the first entity the specification represents.
    /// </summary>
    TEntity First();

    /// <summary>
    /// Gets the first entity the specification represents, and projects it to <typeparamref name="TResult"/> using the current mapping provider.
    /// </summary>
    TResult First<TResult>();

    /// <summary>
    /// Gets the first entity the specification represents.
    /// </summary>
    Task<TEntity> FirstAsync();

    /// <summary>
    /// Gets the first entity the specification represents, and projects it to <typeparamref name="TResult"/> using the current mapping provider.
    /// </summary>
    Task<TResult> FirstAsync<TResult>();

    /// <summary>
    /// Gets the first entity the specification represents. If none, it returns the default value.
    /// </summary>
    TEntity FirstOrDefault();

    /// <summary>
    /// Gets the first entity the specification represents, and projects it to <typeparamref name="TResult"/> using the current mapping provider. If none, it returns the default value.
    /// </summary>
    TResult FirstOrDefault<TResult>();

    /// <summary>
    /// Gets the first entity the specification represents. If none, it returns the default value.
    /// </summary>
    Task<TEntity> FirstOrDefaultAsync();

    /// <summary>
    /// Gets the first entity the specification represents, and projects it to <typeparamref name="TResult"/> using the current mapping provider. If none, it returns the default value.
    /// </summary>
    Task<TResult> FirstOrDefaultAsync<TResult>();

    /// <summary>
    /// Returns the number of elements the specification represents.
    /// </summary>
    int Count();

    /// <summary>
    /// Returns the number of elements the specification represents.
    /// </summary>
    Task<int> CountAsync();

    double Average(Expression<Func<TEntity, int>> selector);

    double? Average(Expression<Func<TEntity, int?>> selector);

    double Average(Expression<Func<TEntity, long>> selector);

    double? Average(Expression<Func<TEntity, long?>> selector);

    double Average(Expression<Func<TEntity, double>> selector);

    double? Average(Expression<Func<TEntity, double?>> selector);

    decimal Average(Expression<Func<TEntity, decimal>> selector);

    decimal? Average(Expression<Func<TEntity, decimal?>> selector);

    float Average(Expression<Func<TEntity, float>> selector);

    float? Average(Expression<Func<TEntity, float?>> selector);

    Task<int> MaxAsync(Expression<Func<TEntity, int>> selector);

    Task<int?> MaxAsync(Expression<Func<TEntity, int?>> selector);

    /// <summary>
    /// Returns the maximum value according to the property selected, all nulls are treated as <paramref name="defaultValue"/>.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    Task<int> MaxAsync(Expression<Func<TEntity, int?>> selector, int defaultValue);

    Task<long> MaxAsync(Expression<Func<TEntity, long>> selector);

    Task<long?> MaxAsync(Expression<Func<TEntity, long?>> selector);

    /// <summary>
    /// Returns the maximum value according to the property selected, all nulls are treated as <paramref name="defaultValue"/>.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    Task<long> MaxAsync(Expression<Func<TEntity, long?>> selector, long defaultValue);

    Task<double> MaxAsync(Expression<Func<TEntity, double>> selector);

    Task<double?> MaxAsync(Expression<Func<TEntity, double?>> selector);

    /// <summary>
    /// Returns the maximum value according to the property selected, all nulls are treated as <paramref name="defaultValue"/>.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    Task<double> MaxAsync(Expression<Func<TEntity, double?>> selector, double defaultValue);

    Task<decimal> MaxAsync(Expression<Func<TEntity, decimal>> selector);

    Task<decimal?> MaxAsync(Expression<Func<TEntity, decimal?>> selector);

    /// <summary>
    /// Returns the maximum value according to the property selected, all nulls are treated as <paramref name="defaultValue"/>.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    Task<decimal> MaxAsync(Expression<Func<TEntity, decimal?>> selector, decimal defaultValue);

    Task<float> MaxAsync(Expression<Func<TEntity, float>> selector);

    Task<float?> MaxAsync(Expression<Func<TEntity, float?>> selector);

    /// <summary>
    /// Returns the maximum value according to the property selected, all nulls are treated as <paramref name="defaultValue"/>.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    Task<float> MaxAsync(Expression<Func<TEntity, float?>> selector, float defaultValue);

    Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> selector);

    Task<int> MinAsync(Expression<Func<TEntity, int>> selector);

    Task<int?> MinAsync(Expression<Func<TEntity, int?>> selector);

    /// <summary>
    /// Returns the minimum value according to the property selected, all nulls are treated as <paramref name="defaultValue"/>.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    Task<int> MinAsync(Expression<Func<TEntity, int?>> selector, int defaultValue);

    Task<long> MinAsync(Expression<Func<TEntity, long>> selector);

    Task<long?> MinAsync(Expression<Func<TEntity, long?>> selector);

    /// <summary>
    /// Returns the minimum value according to the property selected, all nulls are treated as <paramref name="defaultValue"/>.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    Task<long> MinAsync(Expression<Func<TEntity, long?>> selector, long defaultValue);

    Task<double> MinAsync(Expression<Func<TEntity, double>> selector);

    Task<double?> MinAsync(Expression<Func<TEntity, double?>> selector);

    /// <summary>
    /// Returns the minimum value according to the property selected, all nulls are treated as <paramref name="defaultValue"/>.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    Task<double> MinAsync(Expression<Func<TEntity, double?>> selector, double defaultValue);

    Task<decimal> MinAsync(Expression<Func<TEntity, decimal>> selector);

    Task<decimal?> MinAsync(Expression<Func<TEntity, decimal?>> selector);

    /// <summary>
    /// Returns the minimum value according to the property selected, all nulls are treated as <paramref name="defaultValue"/>.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    Task<decimal> MinAsync(Expression<Func<TEntity, decimal?>> selector, decimal defaultValue);

    Task<float> MinAsync(Expression<Func<TEntity, float>> selector);

    Task<float?> MinAsync(Expression<Func<TEntity, float?>> selector);

    /// <summary>
    /// Returns the minimum value according to the property selected, all nulls are treated as <paramref name="defaultValue"/>.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    Task<float> MinAsync(Expression<Func<TEntity, float?>> selector, float defaultValue);

    Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> selector);

    int Sum(Expression<Func<TEntity, int>> selector);

    int? Sum(Expression<Func<TEntity, int?>> selector);

    long Sum(Expression<Func<TEntity, long>> selector);

    long? Sum(Expression<Func<TEntity, long?>> selector);

    double Sum(Expression<Func<TEntity, double>> selector);

    double? Sum(Expression<Func<TEntity, double?>> selector);

    decimal Sum(Expression<Func<TEntity, decimal>> selector);

    decimal? Sum(Expression<Func<TEntity, decimal?>> selector);

    float Sum(Expression<Func<TEntity, float>> selector);

    float? Sum(Expression<Func<TEntity, float?>> selector);

    ISpecificationResult<TResult> GroupBy<TElement, TKey, TResult>(Expression<Func<TEntity, TKey>> keySelector,
                                                   Expression<Func<TEntity, TElement>> elementSelector,
                                                   Expression<Func<TKey, IEnumerable<TElement>, TResult>>
                                                       resultSelector);

    /// <summary>
    /// Returns whether the specification provided contains any results.
    /// </summary>
    /// <returns></returns>
    bool Any();

    /// <summary>
    /// Returns whether the specification provided contains any results.
    /// </summary>
    /// <returns></returns>
    Task<bool> AnyAsync();

    /// <summary>
    /// Returns a bitwise OR (operator |) of the values the specification represents.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="selector"></param>
    /// <returns></returns>
    TResult CombineFlag<TResult>(Expression<Func<TEntity, TResult>> selector)
        where TResult : struct, Enum;

    /// <summary>
    /// Combines all values that the specification represents in a bitwise OR (operator |) and returns whether they satisfy the flag provided.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="selector"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    bool HasFlagCombined<TResult>(Expression<Func<TEntity, TResult>> selector, TResult flag)
        where TResult : struct, Enum;
}
