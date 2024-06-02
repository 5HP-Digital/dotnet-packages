namespace Digital5HP.DataAccess.EntityFramework.Specification;

using Digital5HP.DataAccess.Specifications;
using Digital5HP.ObjectMapping;

public abstract class KeyedResultableQueryableSpecification<TSpec, TEntity, TPrimaryKey>(IMapperProvider mapperProvider, ISpecificationFactory specificationFactory)
    : KeyedQueryableSpecification<TSpec, TEntity, TPrimaryKey>(mapperProvider, specificationFactory),
      IResultableSpecification<TEntity>
    where TSpec : class, IKeyedSpecification<TSpec, TEntity, TPrimaryKey>, IResultableSpecification<TEntity>
    where TEntity : class, IKeyedEntity<TPrimaryKey>, new()
{

    /// <inheritdoc />
    /// <summary>
    /// Returns queryable specification result.
    /// </summary>
    public ISpecificationResult<TEntity> Result => new QueryableSpecificationResult<TEntity>(this.Queryable, this.MapperProvider);
}
