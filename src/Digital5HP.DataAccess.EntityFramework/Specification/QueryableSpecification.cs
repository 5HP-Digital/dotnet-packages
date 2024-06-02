namespace Digital5HP.DataAccess.EntityFramework.Specification;

using System;
using System.Linq;

using Digital5HP.DataAccess.Specifications;
using Digital5HP.ObjectMapping;

/// <inheritdoc />
/// <summary>
/// Base class for <see cref="System.Linq.IQueryable" /> based specifications.
/// </summary>
public abstract class QueryableSpecification<TEntity>(IMapperProvider mapperProvider, ISpecificationFactory specificationFactory) : Specification<TEntity>(mapperProvider)
    where TEntity : class, IEntity
{
    protected ISpecificationFactory SpecificationFactory { get; } = specificationFactory;

    /// <summary>
    /// Gets or sets the queryable instance.
    /// </summary>
    protected IQueryable<TEntity> Queryable { get; set; }

    /// <summary>
    /// Sets <see cref="Queryable"/> and applies.
    /// </summary>
    /// <param name="queryable"><see cref="IQueryable"/> for the specification to work with.</param>
    internal void SetQueryable(IQueryable<TEntity> queryable)
    {
        this.Queryable = this.ApplyInclude(queryable ?? throw new ArgumentNullException(nameof(queryable)));
    }

    protected virtual IQueryable<TEntity> ApplyInclude(IQueryable<TEntity> queryable)
    {
        return queryable;
    }
}
