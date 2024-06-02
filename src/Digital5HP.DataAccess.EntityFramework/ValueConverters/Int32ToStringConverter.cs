namespace Digital5HP.DataAccess.EntityFramework.ValueConverters;

using System.Globalization;

public class Int32ToStringConverter(bool isRequired) : NullableValueConverter<int?, string>(
        num => num.HasValue
                ? num.Value.ToString(CultureInfo.InvariantCulture)
                : isRequired
                    ? string.Empty
                    : null,
        str => ParseInt(str))
{
    private static int? ParseInt(string str)
    {
        return int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out var val) ? val : null;
    }
}
