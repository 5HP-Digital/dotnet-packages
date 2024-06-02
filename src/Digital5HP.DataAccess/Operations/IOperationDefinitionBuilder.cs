namespace Digital5HP.DataAccess.Operations;

using System;
using System.Linq.Expressions;

public interface IOperationDefinitionBuilder<TEntity>
    where TEntity : class, IEntity
{
    /// <summary>
    /// Defines an operation for the entity.
    /// </summary>
#pragma warning disable CA1716
    IEntityOperationDefinition<TEntity> For(TEntity entity);
#pragma warning restore CA1716

    /// <summary>
    /// Defines an operation on a set of entities (bulk).
    /// </summary>
#pragma warning disable CA1716
    IFilterOperationDefinition<TEntity> For(Expression<Func<TEntity, bool>> filterExpression);
#pragma warning restore CA1716
}
