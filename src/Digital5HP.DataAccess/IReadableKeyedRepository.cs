namespace Digital5HP.DataAccess;

using System.Threading.Tasks;

using Digital5HP.DataAccess.Specifications;

/// <summary>
/// Interface for a read only keyed ID data store.
/// </summary>
/// <typeparam name="TEntity">Type of entity managed by the repository.</typeparam>
/// <typeparam name="TPrimaryKey">Type of primary key of the entity managed by the repository.</typeparam>
public interface IReadableKeyedRepository<TEntity, in TPrimaryKey> : IReadableRepository<TEntity>
    where TEntity : class, IKeyedEntity<TPrimaryKey>
{
    Task<TEntity> GetByKeyAsync(TPrimaryKey key);
}

/// <summary>
/// Interface for a read only keyed ID data store.
/// </summary>
/// <typeparam name="TEntity">Type of entity managed by the repository.</typeparam>
/// <typeparam name="TPrimaryKey">Type of primary key of the entity managed by the repository.</typeparam>
/// <typeparam name="TSpecification"></typeparam>
public interface IReadableKeyedRepository<TEntity, in TPrimaryKey, out TSpecification>
    : IReadableKeyedRepository<TEntity, TPrimaryKey>, IReadableRepository<TEntity, TSpecification>
    where TEntity : class, IKeyedEntity<TPrimaryKey>
    where TSpecification : ISpecification<TEntity>
{
}
