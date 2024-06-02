namespace Digital5HP.DataAccess.EntityFramework.ValueConverters;

using System.Globalization;

public class NullableToDefaultConverter<TIn, TOut>(TOut defaultValue) : NullableValueConverter<TIn?, TOut>(x => x.HasValue ? x.Value.ChangeType<TOut>(CultureInfo.InvariantCulture) : defaultValue,
           x => x.Equals(defaultValue) ? null: x.ChangeType<TIn>(CultureInfo.InvariantCulture))
where TIn: struct
where TOut: struct
{
}
