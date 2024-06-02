namespace Digital5HP.DataAccess;

using Digital5HP.DataAccess.Specifications;

public interface IWritableKeyedRepository<TEntity, TPrimaryKey>
    : IReadableKeyedRepository<TEntity, TPrimaryKey>, IWritableRepository<TEntity>
    where TEntity : class, IKeyedEntity<TPrimaryKey>
{
}

public interface IWritableKeyedRepository<TEntity, TPrimaryKey, out TSpecification>
    : IReadableKeyedRepository<TEntity, TPrimaryKey, TSpecification>, IWritableKeyedRepository<TEntity, TPrimaryKey>
    where TEntity : class, IKeyedEntity<TPrimaryKey>
    where TSpecification : IKeyedSpecification<TSpecification, TEntity, TPrimaryKey>
{
}
