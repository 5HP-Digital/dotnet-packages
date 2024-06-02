namespace Digital5HP.DataAccess;

using Digital5HP.DataAccess.Operations;
using Digital5HP.DataAccess.Specifications;

/// <summary>
/// Generic repository interface (DDD) for reading and writing domain entities to a storage.
/// </summary>
/// <typeparam name="TEntity">Domain entity.</typeparam>
public interface IWritableRepository<TEntity> : IReadableRepository<TEntity>
    where TEntity : class, IEntity
{
    /// <summary>
    /// Inserts entity to the storage.
    /// </summary>
    void Add<T>(T entity)
        where T : TEntity;

    /// <summary>
    /// Updates entity in the storage.
    /// </summary>
    void Update<T>(T entity)
        where T : TEntity;

    /// <summary>
    /// Deletes entity from the storage.
    /// </summary>
    void Delete<T>(T entity)
        where T : TEntity;

    /// <summary>
    /// Upserts entity to the storage
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    public void Upsert<T>(T entity)
        where T : TEntity;

    /// <summary>
    /// Instantiates an <see cref="Operations.IOperationDefinitionBuilder{TEntity}"/> used for creating a data manipulation operation.
    /// </summary>
    /// <returns></returns>
    IOperationDefinitionBuilder<TEntity> DefineOperation();
}

/// <summary>
/// Generic repository interface (DDD) for reading and writing domain entities to a storage.
/// </summary>
/// <typeparam name="TEntity">Domain entity.</typeparam>
/// <typeparam name="TSpecification"></typeparam>
public interface IWritableRepository<TEntity, out TSpecification>
    : IWritableRepository<TEntity>, IReadableRepository<TEntity, TSpecification>
    where TEntity : class, IEntity
    where TSpecification : ISpecification<TEntity>
{
}
