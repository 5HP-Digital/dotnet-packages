namespace Digital5HP.DataAccess.Operations;

using System.Collections.Generic;

// ReSharper disable once UnusedTypeParameter
public interface IArrayFieldOperationDefinition<TEntity, in TField>
    where TEntity : class, IEntity
{
    /// <summary>
    /// Adds a value to an array unless the value is already present, in which case it does nothing to that array.
    /// </summary>
    /// <param name="values"></param>
    void AddToSet(params TField[] values);

    /// <summary>
    /// Appends the specified <see cref="values"/> to an array.
    /// </summary>
    /// <param name="values"></param>
    void Push(params TField[] values);

    /// <summary>
    /// Removes from an existing array all instances of the specified <see cref="values"/>.
    /// </summary>
    /// <param name="values"></param>
    void Pull(params TField[] values);

    /// <summary>
    /// Removes the first element of an array.
    /// </summary>
    void PopFirst();

    /// <summary>
    /// Removes the last element of an array.
    /// </summary>
    void PopLast();

    /// <summary>
    /// Removes all elements of an array.
    /// </summary>
    void Empty();

    /// <summary>
    /// Sets the field to the specified <paramref name="values"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://docs.mongodb.com/manual/reference/operator/update/set/"/> for more info.
    /// </remarks>
    /// <param name="values"></param>
#pragma warning disable CA1716
    void Set(IEnumerable<TField> values);
#pragma warning restore CA1716

    /// <summary>
    /// Deletes the specified field.
    /// </summary>
    /// <remarks>
    /// If the field doesn't exist, it does nothing.
    /// <para>
    /// <see href="https://docs.mongodb.com/manual/reference/operator/update/unset/"/> for more info.
    /// </para>
    /// </remarks>
    void Remove();
}
