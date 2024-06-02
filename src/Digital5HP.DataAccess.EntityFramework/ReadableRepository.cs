namespace Digital5HP.DataAccess.EntityFramework;

using System;

using Microsoft.EntityFrameworkCore;

using Digital5HP.DataAccess.EntityFramework.Specification;
using Digital5HP.DataAccess.Specifications;
using Digital5HP.Logging;

public abstract class ReadableRepository<TEntity, TSpecification>(ILogger logger, IDbContext context, ISpecificationFactory specificationFactory)
    : RepositoryBase(logger, context),
      IReadableRepository<TEntity, TSpecification>
    where TEntity : class, IEntity
    where TSpecification : ISpecification<TEntity>
{
    private readonly ISpecificationFactory specificationFactory = specificationFactory;

    /// <inheritdoc />
    protected override bool IsReadOnly => true;

    /// <inheritdoc />
    public TSpecification Specify()
    {
        return this.Specify(null);
    }

    /// <inheritdoc />
    public TSpecification Specify(Action<QueryOptions> options)
    {
        var queryOptions = new QueryOptions();

        if (this.IsReadOnly) queryOptions.ChangeTracking = false;

        options?.Invoke(queryOptions);

        var queryable = this.Context.Set<TEntity>()
                            .AsQueryable();

        queryable = queryOptions.ChangeTracking switch
                    {
                        true  => queryable.AsTracking(),
                        false => queryable.AsNoTracking(),
                        _     => queryable
                    };

        queryable = queryOptions.SplitQuery switch
                    {
                        true  => queryable.AsSplitQuery(),
                        false => queryable.AsSingleQuery(),
                        _     => queryable
                    };

        return this.specificationFactory.Create<TSpecification, TEntity>(queryable);
    }
}