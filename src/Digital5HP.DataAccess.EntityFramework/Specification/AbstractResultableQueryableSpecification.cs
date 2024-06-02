namespace Digital5HP.DataAccess.EntityFramework.Specification;

using Digital5HP.DataAccess.Specifications;
using Digital5HP.ObjectMapping;

public abstract class AbstractResultableQueryableSpecification<TEntity>(IMapperProvider mapperProvider, ISpecificationFactory specificationFactory)
    : AbstractQueryableSpecification<TEntity>(mapperProvider, specificationFactory),
      IAbstractResultableSpecification<TEntity>
    where TEntity : class, IEntity
{
    public abstract IAbstractSpecificationResult<TEntity> AsAny();
}
