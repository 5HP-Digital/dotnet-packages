namespace Digital5HP.DataAccess.EntityFramework.ValueConverters;

using System;

using Digital5HP.DataAccess.EntityFramework.Extensions;

public class LegacyDateTimeValueConverter : NullableValueConverter<DateTime?, DateTime>
{
    private static readonly DateTime MinDate = new DateTime(1800, 1, 1).AsUtc();
    private static readonly DateTime MaxDate = new DateTime(2040, 1, 1).AsUtc();

    public LegacyDateTimeValueConverter(bool isMax)
        : base(x => ConvertDate(x, isMax),
               x => x.FromLegacyDateTime())
    {
    }

    public LegacyDateTimeValueConverter(DateTime defaultValue)
        : base(x => x.GetValueOrDefault(defaultValue),
               x => x == defaultValue ? null : x)
    {
    }

    private static DateTime ConvertDate(DateTime? dateToConvert, bool isMax)
    {
        if (dateToConvert == null)
        {
            return isMax ? MaxDate : MinDate;
        }

        return dateToConvert.Value;
    }
}
