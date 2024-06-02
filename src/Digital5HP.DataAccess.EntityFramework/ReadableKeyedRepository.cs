namespace Digital5HP.DataAccess.EntityFramework;

using System.Threading.Tasks;

using Digital5HP.DataAccess.EntityFramework.Specification;
using Digital5HP.DataAccess.Specifications;
using Digital5HP.Logging;

public abstract class ReadableKeyedRepository<TEntity, TPrimaryKey, TSpecification>(ILogger logger,
                                  IDbContext context,
                                  ISpecificationFactory specificationFactory)
    : ReadableRepository<TEntity, TSpecification>(logger, context, specificationFactory),
      IReadableKeyedRepository<TEntity, TPrimaryKey, TSpecification>
    where TEntity : class, IKeyedEntity<TPrimaryKey>
    where TSpecification : IKeyedSpecification<TSpecification, TEntity, TPrimaryKey>
{
    public Task<TEntity> GetByKeyAsync(TPrimaryKey key)
    {
        return this.Context.Set<TEntity>()
                   .FindAsync(key)
                   .AsTask();
    }
}
