namespace Digital5HP.DataAccess.EntityFramework.Specification;

using System.Linq;

using Digital5HP.DataAccess.Specifications;
using Digital5HP.ObjectMapping;

public abstract class AbstractQueryableSpecification<TEntity>(IMapperProvider mapperProvider, ISpecificationFactory specificationFactory)
    : QueryableSpecification<TEntity>(mapperProvider, specificationFactory),
      IAbstractSpecification<TEntity>
    where TEntity : class, IEntity
{
    public TSpec As<TSpec, TSpecEntity>()
        where TSpec : ISpecification<TSpecEntity>
        where TSpecEntity : class, TEntity
    {
        return this.SpecificationFactory.Create<TSpec, TSpecEntity>(this.Queryable.OfType<TSpecEntity>());
    }
}
