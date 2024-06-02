namespace Digital5HP.DataAccess.EntityFramework.Specification;

using System.Linq;

using Digital5HP.DataAccess.Specifications;
using Digital5HP.ObjectMapping;

public abstract class KeyedAbstractQueryableSpecification<TSpec, TEntity, TPrimaryKey>(IMapperProvider mapperProvider, ISpecificationFactory specificationFactory)
    : KeyedQueryableSpecification<TSpec, TEntity, TPrimaryKey>(mapperProvider, specificationFactory), IAbstractSpecification<TEntity>
    where TSpec : class, IKeyedSpecification<TSpec, TEntity, TPrimaryKey>, IAbstractSpecification<TEntity>
    where TEntity : class, IKeyedEntity<TPrimaryKey>
{
    public TSpec1 As<TSpec1, TSpecEntity>()
        where TSpec1 : ISpecification<TSpecEntity>
        where TSpecEntity : class, TEntity
    {
        return this.SpecificationFactory.Create<TSpec1, TSpecEntity>(this.Queryable.OfType<TSpecEntity>());
    }
}
