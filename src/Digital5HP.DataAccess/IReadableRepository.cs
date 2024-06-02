namespace Digital5HP.DataAccess;

using System;

using Digital5HP.DataAccess.Specifications;

// ReSharper disable once UnusedTypeParameter
public interface IReadableRepository<TEntity> : IRepository
    where TEntity : class, IEntity
{
}

public interface IReadableRepository<TEntity, out TSpecification> : IReadableRepository<TEntity>
    where TEntity : class, IEntity
    where TSpecification : ISpecification<TEntity>
{
    /// <summary>
    /// Gets specification interface for complex searching of entities.
    /// </summary>
    TSpecification Specify();

    /// <summary>
    /// Gets specification interface for complex searching of entities with <see cref="QueryOptions"/> provided.
    /// </summary>
    TSpecification Specify(Action<QueryOptions> options);
}
