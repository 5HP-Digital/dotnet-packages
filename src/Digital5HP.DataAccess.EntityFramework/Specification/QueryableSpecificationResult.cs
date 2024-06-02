namespace Digital5HP.DataAccess.EntityFramework.Specification;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Digital5HP.DataAccess.Specifications;
using Digital5HP.ObjectMapping;

/// <summary>
/// Specification result class contains common functionality for filtering result.
/// </summary>
/// <typeparam name="TEntity">Domain entity.</typeparam>
/// <remarks>
/// Constructor.
/// </remarks>
public class QueryableSpecificationResult<TEntity>(IQueryable<TEntity> queryable, IMapperProvider mapperProvider) : ISpecificationResult<TEntity>
{
    private readonly IMapperProvider mapperProvider = mapperProvider;

    private IQueryable<TEntity> queryable = queryable;

    public IQueryable<TEntity> AsQueryable()
    {
        return this.queryable;
    }

    /// <summary>
    /// Takes given count of the records represented by the specification.
    /// </summary>
    public ISpecificationResult<TEntity> Take(int count)
    {
        this.queryable = this.queryable.Take(count);
        return this;
    }

    /// <summary>
    /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
    /// </summary>
    public ISpecificationResult<TEntity> Skip(int count)
    {
        this.queryable = this.queryable.Skip(count);
        return this;
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending order according to a key.
    /// </summary>
    public ISpecificationResult<TEntity> OrderByAscending<TKey>(Expression<Func<TEntity, TKey>> keySelector)
    {
        this.queryable = this.queryable.OrderBy(keySelector);
        return this;
    }

    /// <summary>
    /// Sorts the elements of a sequence in descending order according to a key.
    /// </summary>
    public ISpecificationResult<TEntity> OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> keySelector)
    {
        this.queryable = this.queryable.OrderByDescending(keySelector);
        return this;
    }

    public ISpecificationResult<TEntity> Distinct()
    {
        this.queryable = this.queryable.Distinct();
        return this;
    }

    /// <summary>
    /// Executes the specification and query behind it and returns list of records
    /// that matches criteria.
    /// </summary>
    public IList<TEntity> ToList()
    {
        return this.queryable.ToList();
    }

    /// <summary>
    /// Executes the specification and query behind it and returns list of records
    /// that matches criteria.
    /// </summary>
    public async Task<IList<TEntity>> ToListAsync()
    {
        return await this.queryable.ToListAsync();
    }

    /// <inheritdoc />
    public ISpecificationResult<TResult> ProjectTo<TResult>(Expression<Func<TEntity, TResult>> selector)
    {
        return new QueryableSpecificationResult<TResult>(this.queryable.Select(selector), this.mapperProvider);
    }

    /// <inheritdoc />
    public IList<TResult> ToList<TResult>()
    {
        var result = this.queryable.ToList();

        return this.MapInternal<TResult>(result)
                   .ToList();
    }

    /// <inheritdoc />
    public async Task<IList<TResult>> ToListAsync<TResult>()
    {
        var result = await this.queryable.ToListAsync();

        return this.MapInternal<TResult>(result)
                   .ToList();
    }

#pragma warning disable CA1720
    public TEntity Single()
#pragma warning restore CA1720
    {
        return this.queryable.Single();
    }

#pragma warning disable CA1720
    public TResult Single<TResult>()
#pragma warning restore CA1720
    {
        var result = this.queryable.Single();

        return this.MapInternal<TResult>(result);
    }

    public Task<TEntity> SingleAsync()
    {
        return this.queryable.SingleAsync();
    }

    public async Task<TResult> SingleAsync<TResult>()
    {
        var result = await this.queryable.SingleAsync();

        return this.MapInternal<TResult>(result);
    }

    public TEntity SingleOrDefault()
    {
        return this.queryable.SingleOrDefault();
    }

    public TResult SingleOrDefault<TResult>()
    {
        var result = this.queryable.SingleOrDefault();

        return this.MapInternal<TResult>(result);
    }

    public Task<TEntity> SingleOrDefaultAsync()
    {
        return this.queryable.SingleOrDefaultAsync();
    }

    public async Task<TResult> SingleOrDefaultAsync<TResult>()
    {
        var result = await this.queryable.SingleOrDefaultAsync();

        return this.MapInternal<TResult>(result);
    }

    public TEntity First()
    {
        return this.queryable.First();
    }

    public TResult First<TResult>()
    {
        var result = this.queryable.First();

        return this.MapInternal<TResult>(result);
    }

    public Task<TEntity> FirstAsync()
    {
        return this.queryable.FirstAsync();
    }

    public async Task<TResult> FirstAsync<TResult>()
    {
        var result = await this.queryable.FirstAsync();

        return this.MapInternal<TResult>(result);
    }

    public TEntity FirstOrDefault()
    {
        return this.queryable.FirstOrDefault();
    }

    public TResult FirstOrDefault<TResult>()
    {
        var result = this.queryable.FirstOrDefault();

        return this.MapInternal<TResult>(result);
    }

    public Task<TEntity> FirstOrDefaultAsync()
    {
        return this.queryable.FirstOrDefaultAsync();
    }

    public async Task<TResult> FirstOrDefaultAsync<TResult>()
    {
        var result = await this.queryable.FirstOrDefaultAsync();

        return this.MapInternal<TResult>(result);
    }

    /// <summary>
    /// Returns the number of elements the specification represents.
    /// </summary>
    /// <returns></returns>
    public int Count()
    {
        return this.queryable.Count();
    }

    /// <summary>
    /// Returns the number of elements the specification represents.
    /// </summary>
    /// <returns></returns>
    public Task<int> CountAsync()
    {
        return this.queryable.CountAsync();
    }

    public double Average(Expression<Func<TEntity, int>> selector)
    {
        return this.queryable.Average(selector);
    }

    public double? Average(Expression<Func<TEntity, int?>> selector)
    {
        return this.queryable.Average(selector);
    }

    public double Average(Expression<Func<TEntity, long>> selector)
    {
        return this.queryable.Average(selector);
    }

    public double? Average(Expression<Func<TEntity, long?>> selector)
    {
        return this.queryable.Average(selector);
    }

    public double Average(Expression<Func<TEntity, double>> selector)
    {
        return this.queryable.Average(selector);
    }

    public double? Average(Expression<Func<TEntity, double?>> selector)
    {
        return this.queryable.Average(selector);
    }

    public decimal Average(Expression<Func<TEntity, decimal>> selector)
    {
        return this.queryable.Average(selector);
    }

    public decimal? Average(Expression<Func<TEntity, decimal?>> selector)
    {
        return this.queryable.Average(selector);
    }

    public float Average(Expression<Func<TEntity, float>> selector)
    {
        return this.queryable.Average(selector);
    }

    public float? Average(Expression<Func<TEntity, float?>> selector)
    {
        return this.queryable.Average(selector);
    }

    public Task<int> MaxAsync(Expression<Func<TEntity, int>> selector)
    {
        return this.queryable.MaxAsync(selector);
    }

    public Task<int?> MaxAsync(Expression<Func<TEntity, int?>> selector)
    {
        return this.queryable.MaxAsync(selector);
    }

    public Task<int> MaxAsync(Expression<Func<TEntity, int?>> selector, int defaultValue)
    {
        return this.queryable.Select(selector)
                   .MaxAsync(o => o ?? defaultValue);
    }

    public Task<long> MaxAsync(Expression<Func<TEntity, long>> selector)
    {
        return this.queryable.MaxAsync(selector);
    }

    public Task<long?> MaxAsync(Expression<Func<TEntity, long?>> selector)
    {
        return this.queryable.MaxAsync(selector);
    }

    public Task<long> MaxAsync(Expression<Func<TEntity, long?>> selector, long defaultValue)
    {
        return this.queryable.Select(selector)
                   .MaxAsync(o => o ?? defaultValue);
    }

    public Task<double> MaxAsync(Expression<Func<TEntity, double>> selector)
    {
        return this.queryable.MaxAsync(selector);
    }

    public Task<double?> MaxAsync(Expression<Func<TEntity, double?>> selector)
    {
        return this.queryable.MaxAsync(selector);
    }

    public Task<double> MaxAsync(Expression<Func<TEntity, double?>> selector, double defaultValue)
    {
        return this.queryable.Select(selector)
                   .MaxAsync(o => o ?? defaultValue);
    }

    public Task<decimal> MaxAsync(Expression<Func<TEntity, decimal>> selector)
    {
        return this.queryable.MaxAsync(selector);
    }

    public Task<decimal?> MaxAsync(Expression<Func<TEntity, decimal?>> selector)
    {
        return this.queryable.MaxAsync(selector);
    }

    public Task<decimal> MaxAsync(Expression<Func<TEntity, decimal?>> selector, decimal defaultValue)
    {
        return this.queryable.Select(selector)
                   .MaxAsync(o => o ?? defaultValue);
    }

    public Task<float> MaxAsync(Expression<Func<TEntity, float>> selector)
    {
        return this.queryable.MaxAsync(selector);
    }

    public Task<float?> MaxAsync(Expression<Func<TEntity, float?>> selector)
    {
        return this.queryable.MaxAsync(selector);
    }

    public Task<float> MaxAsync(Expression<Func<TEntity, float?>> selector, float defaultValue)
    {
        return this.queryable.Select(selector)
                   .MaxAsync(o => o ?? defaultValue);
    }

    public Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> selector)
    {
        return this.queryable.MaxAsync(selector);
    }

    public Task<int> MinAsync(Expression<Func<TEntity, int>> selector)
    {
        return this.queryable.MinAsync(selector);
    }

    public Task<int?> MinAsync(Expression<Func<TEntity, int?>> selector)
    {
        return this.queryable.MinAsync(selector);
    }

    public Task<int> MinAsync(Expression<Func<TEntity, int?>> selector, int defaultValue)
    {
        return this.queryable.Select(selector)
                   .MinAsync(o => o ?? defaultValue);
    }

    public Task<long> MinAsync(Expression<Func<TEntity, long>> selector)
    {
        return this.queryable.MinAsync(selector);
    }

    public Task<long?> MinAsync(Expression<Func<TEntity, long?>> selector)
    {
        return this.queryable.MinAsync(selector);
    }

    public Task<long> MinAsync(Expression<Func<TEntity, long?>> selector, long defaultValue)
    {
        return this.queryable.Select(selector)
                   .MinAsync(o => o ?? defaultValue);
    }

    public Task<double> MinAsync(Expression<Func<TEntity, double>> selector)
    {
        return this.queryable.MinAsync(selector);
    }

    public Task<double?> MinAsync(Expression<Func<TEntity, double?>> selector)
    {
        return this.queryable.MinAsync(selector);
    }

    public Task<double> MinAsync(Expression<Func<TEntity, double?>> selector, double defaultValue)
    {
        return this.queryable.Select(selector)
                   .MinAsync(o => o ?? defaultValue);
    }

    public Task<decimal> MinAsync(Expression<Func<TEntity, decimal>> selector)
    {
        return this.queryable.MinAsync(selector);
    }

    public Task<decimal?> MinAsync(Expression<Func<TEntity, decimal?>> selector)
    {
        return this.queryable.MinAsync(selector);
    }

    public Task<decimal> MinAsync(Expression<Func<TEntity, decimal?>> selector, decimal defaultValue)
    {
        return this.queryable.Select(selector)
                   .MinAsync(o => o ?? defaultValue);
    }

    public Task<float> MinAsync(Expression<Func<TEntity, float>> selector)
    {
        return this.queryable.MinAsync(selector);
    }

    public Task<float?> MinAsync(Expression<Func<TEntity, float?>> selector)
    {
        return this.queryable.MinAsync(selector);
    }

    public Task<float> MinAsync(Expression<Func<TEntity, float?>> selector, float defaultValue)
    {
        return this.queryable.Select(selector)
                   .MinAsync(o => o ?? defaultValue);
    }

    public Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> selector)
    {
        return this.queryable.MinAsync(selector);
    }

    public int Sum(Expression<Func<TEntity, int>> selector)
    {
        return this.queryable.Sum(selector);
    }

    public int? Sum(Expression<Func<TEntity, int?>> selector)
    {
        return this.queryable.Sum(selector);
    }

    public long Sum(Expression<Func<TEntity, long>> selector)
    {
        return this.queryable.Sum(selector);
    }

    public long? Sum(Expression<Func<TEntity, long?>> selector)
    {
        return this.queryable.Sum(selector);
    }

    public double Sum(Expression<Func<TEntity, double>> selector)
    {
        return this.queryable.Sum(selector);
    }

    public double? Sum(Expression<Func<TEntity, double?>> selector)
    {
        return this.queryable.Sum(selector);
    }

    public decimal Sum(Expression<Func<TEntity, decimal>> selector)
    {
        return this.queryable.Sum(selector);
    }

    public decimal? Sum(Expression<Func<TEntity, decimal?>> selector)
    {
        return this.queryable.Sum(selector);
    }

    public float Sum(Expression<Func<TEntity, float>> selector)
    {
        return this.queryable.Sum(selector);
    }

    public float? Sum(Expression<Func<TEntity, float?>> selector)
    {
        return this.queryable.Sum(selector);
    }

    public ISpecificationResult<TResult> GroupBy<TElement, TKey, TResult>(
        Expression<Func<TEntity, TKey>> keySelector,
        Expression<Func<TEntity, TElement>> elementSelector,
        Expression<Func<TKey, IEnumerable<TElement>, TResult>> resultSelector)
    {
        return new QueryableSpecificationResult<TResult>(
            this.queryable.GroupBy(keySelector, elementSelector, resultSelector),
            this.mapperProvider);
    }

    public bool Any()
    {
        return this.queryable.Any();
    }

    public Task<bool> AnyAsync()
    {
        return this.queryable.AnyAsync();
    }

    public TResult CombineFlag<TResult>(Expression<Func<TEntity, TResult>> selector)
        where TResult : struct, Enum
    {
        return
            this.queryable.Select(selector)
                .AsEnumerable()
                .Select(v => Convert.ToInt64(v, CultureInfo.InvariantCulture))
                .Aggregate((long)0, (v1, v2) => v1 | v2)
                .ChangeType<TResult>(CultureInfo.InvariantCulture);
    }

    public bool HasFlagCombined<TResult>(Expression<Func<TEntity, TResult>> selector, TResult flag)
        where TResult : struct, Enum
    {
        var iFlag = Convert.ToInt64(flag, CultureInfo.InvariantCulture);
        return this.queryable.Select(selector)
                   .AsEnumerable()
                   .Select(v => Convert.ToInt64(v, CultureInfo.InvariantCulture))
                   .Aggregate((long) 0, (v1, v2) => v1 | v2, v => (v & iFlag) == iFlag);
    }

    private TResult MapInternal<TResult>(TEntity entity)
    {
        var mapper = this.mapperProvider.Get<TEntity>();
        return mapper.Map<TResult>(entity);
    }

    private IEnumerable<TResult> MapInternal<TResult>(IEnumerable<TEntity> entities)
    {
        var mapper = this.mapperProvider.Get<TEntity>();
        return mapper.Map<TResult>(entities);
    }
}
