namespace Digital5HP.DataAccess.EntityFramework.Specification;

using System.Linq;

using Digital5HP.DataAccess.Specifications;
using Digital5HP.ObjectMapping;

public abstract class KeyedQueryableSpecification<TSpec, TEntity, TPrimaryKey>(IMapperProvider mapperProvider, ISpecificationFactory specificationFactory)
    : QueryableSpecification<TEntity>(mapperProvider, specificationFactory),
      IKeyedSpecification<TSpec, TEntity, TPrimaryKey>
    where TEntity : class, IKeyedEntity<TPrimaryKey>
    where TSpec : class, IKeyedSpecification<TSpec, TEntity, TPrimaryKey>
{
    public TSpec ById(params TPrimaryKey[] ids)
    {
        this.Queryable = this.Queryable.Where(e => ids.Contains(e.Id));
        return this as TSpec;
    }
}
