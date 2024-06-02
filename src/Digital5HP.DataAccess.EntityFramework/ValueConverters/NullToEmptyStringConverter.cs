namespace Digital5HP.DataAccess.EntityFramework.ValueConverters;

public class NullToEmptyStringConverter : NullableValueConverter<string, string>
{
    public NullToEmptyStringConverter()
        : base(x => x ?? string.Empty,
               x => string.IsNullOrWhiteSpace(x) ? null: x)
    {
    }
}
