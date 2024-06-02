namespace Digital5HP.DataAccess.EntityFramework.Specification;

using System.Linq;

using Digital5HP.DataAccess.Specifications;
using Digital5HP.ObjectMapping;

public class AbstractSpecificationResult<TEntity>(IQueryable<TEntity> queryable, IMapperProvider mapperProvider) : IAbstractSpecificationResult<TEntity>
    where TEntity : class, IEntity
{
    private readonly IQueryable<TEntity> queryable = queryable;
    private readonly IMapperProvider mapperProvider = mapperProvider;

    public ISpecificationResult<TEntity> Result => new QueryableSpecificationResult<TEntity>(this.queryable, this.mapperProvider);
}
