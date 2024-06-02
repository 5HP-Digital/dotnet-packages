namespace Digital5HP.DataAccess.EntityFramework.ValueConverters;

using System.Globalization;

public class StringToInt64Converter(bool isRequired) : NullableValueConverter<string, long?>(
        str => ParseInt64(str, isRequired),
        num => num.HasValue ? num.Value.ToString(CultureInfo.InvariantCulture) : null)
{
    private static long? ParseInt64(string str, bool isRequired)
    {
        return long.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out var val)
            ? val
            : isRequired
                ? 0
                : null;
    }
}
