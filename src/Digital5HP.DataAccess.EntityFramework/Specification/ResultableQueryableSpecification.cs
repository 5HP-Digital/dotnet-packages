namespace Digital5HP.DataAccess.EntityFramework.Specification;

using Digital5HP.DataAccess.Specifications;
using Digital5HP.ObjectMapping;

public abstract class ResultableQueryableSpecification<TEntity>(IMapperProvider mapperProvider, ISpecificationFactory specificationFactory)
    : QueryableSpecification<TEntity>(mapperProvider, specificationFactory),
      IResultableSpecification<TEntity>
    where TEntity : class, IEntity, new()
{

    /// <inheritdoc />
    /// <summary>
    /// Returns specification result.
    /// </summary>
    public ISpecificationResult<TEntity> Result => new QueryableSpecificationResult<TEntity>(this.Queryable, this.MapperProvider);
}
