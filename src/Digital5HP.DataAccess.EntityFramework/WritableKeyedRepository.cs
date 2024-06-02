namespace Digital5HP.DataAccess.EntityFramework;

using System.Threading.Tasks;

using Digital5HP.DataAccess.EntityFramework.Specification;
using Digital5HP.DataAccess.Specifications;
using Digital5HP.Logging;

public abstract class WritableKeyedRepository<TEntity, TPrimaryKey, TSpec>(ILogger logger,
                                  IDbContext context,
                                  ISpecificationFactory specificationFactory)
    : WritableRepository<TEntity, TSpec>(logger, context, specificationFactory),
      IWritableKeyedRepository<TEntity, TPrimaryKey, TSpec>
    where TEntity : class, IKeyedEntity<TPrimaryKey>
    where TSpec : IKeyedSpecification<TSpec, TEntity, TPrimaryKey>
{
    public Task<TEntity> GetByKeyAsync(TPrimaryKey key)
    {
        return this.Context.Set<TEntity>()
                   .FindAsync(key)
                   .AsTask();
    }
}
