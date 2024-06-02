namespace Digital5HP.DataAccess.Operations;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

public interface IOperationDefinition<TEntity>
    where TEntity : class, IEntity
{

    /// <summary>
    /// Specifies a single value field to perform the operation on
    /// </summary>
    /// <typeparam name="TField"></typeparam>
    /// <param name="fieldSelector">Field selector</param>
    IValueFieldOperationDefinition<TEntity, TField> OnField<TField>(
        Expression<Func<TEntity, TField>> fieldSelector);

    /// <summary>
    /// Specifies an array field to perform the operation on
    /// </summary>
    /// <typeparam name="TField"></typeparam>
    /// <param name="fieldSelector">Field selector</param>
    IArrayFieldOperationDefinition<TEntity, TField> OnField<TField>(
        Expression<Func<TEntity, IEnumerable<TField>>> fieldSelector);

    /// <summary>
    /// Adds a delete operation to the bulk operation queue.
    /// </summary>
    void Delete();
}