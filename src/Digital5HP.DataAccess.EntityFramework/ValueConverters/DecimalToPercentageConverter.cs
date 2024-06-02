namespace Digital5HP.DataAccess.EntityFramework.ValueConverters;

using System;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class DecimalToPercentageConverter : ValueConverter<decimal, int>
{
    public DecimalToPercentageConverter()
        : base(
            dec => (int)Math.Round(dec * 100, 0, MidpointRounding.AwayFromZero),
            i => i / 100m)
    {
    }
}
