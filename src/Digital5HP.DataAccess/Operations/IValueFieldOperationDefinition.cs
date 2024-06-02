namespace Digital5HP.DataAccess.Operations;

// ReSharper disable once UnusedTypeParameter
public interface IValueFieldOperationDefinition<TEntity, in TField>
    where TEntity : class, IEntity
{
    /// <summary>
    /// Increases the field by the <see cref="value"/> provided. If doesn't exists, field is set to <see cref="value"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://docs.mongodb.com/manual/reference/operator/update/inc/"/> for more info.
    /// </remarks>
    /// <param name="value"></param>
    void Increment(TField value);

    /// <summary>
    /// Multiplies the field by the <see cref="value"/> provided. If doesn't exists, field is set to zero.
    /// </summary>
    /// <remarks>
    /// <see href="https://docs.mongodb.com/manual/reference/operator/update/mul/"/> for more info.
    /// </remarks>
    /// <param name="value"></param>
    void Multiply(TField value);

    /// <summary>
    /// Updates the value of the field to a specified <see cref="value"/> if the specified value is less than the current value of the field.
    /// </summary>
    /// <remarks>
    /// If the field does not exists, sets the field to the specified value.
    /// <para>
    /// <see href="https://docs.mongodb.com/manual/reference/operator/update/min/"/> for more info.
    /// </para>
    /// </remarks>
    /// <param name="value"></param>
    void Min(TField value);

    /// <summary>
    /// Updates the value of the field to a specified <see cref="value"/> if the specified value is greater than the current value of the field.
    /// </summary>
    /// <remarks>
    /// If the field does not exists, sets the field to the specified value.
    /// <para>
    /// <see href="https://docs.mongodb.com/manual/reference/operator/update/max/"/> for more info.
    /// </para>
    /// </remarks>
    /// <param name="value"></param>
    void Max(TField value);

    /// <summary>
    /// Sets the field to the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://docs.mongodb.com/manual/reference/operator/update/set/"/> for more info.
    /// </remarks>
    /// <param name="value"></param>
#pragma warning disable CA1716
    void Set(TField value);
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

    /// <summary>
    /// Performs a bitwise AND on the field to the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://docs.mongodb.com/manual/reference/operator/update/bit/"/> for more info.
    /// </remarks>
    /// <param name="value"></param>
    void BitwiseAnd(TField value);

    /// <summary>
    /// Performs a bitwise OR on the field to the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://docs.mongodb.com/manual/reference/operator/update/bit/"/> for more info.
    /// </remarks>
    /// <param name="value"></param>
    void BitwiseOr(TField value);

    /// <summary>
    /// Performs a bitwise XOR on the field to the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://docs.mongodb.com/manual/reference/operator/update/bit/"/> for more info.
    /// </remarks>
    /// <param name="value"></param>
    void BitwiseXor(TField value);
}
