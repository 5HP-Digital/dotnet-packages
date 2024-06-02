namespace Digital5HP.DataAccess.EntityFramework.Extensions;

using System;

public static class DateTimeExtensions
{
    private static readonly DateTime MinDate = new DateTime(1900, 1, 1).AsUtc();
    private static readonly DateTime MaxDate = new DateTime(2040, 1, 1).AsUtc();

    /// <summary>
    /// Converts a <see cref="DateTime"/> originating from the database into a null where necessary
    /// <remarks>The purpose being to work with the design of the data, where <see cref="DateTime"/>
    /// objects with a value less than 1/1/1900, and greater than 1/1/2040 are supposed to be null.</remarks>
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/> to convert</param>
    /// <returns>null value if dateTime is less than or equal to 1/1/1900, or greater than 1/1/2040, otherwise returns the original dateTime</returns>
    public static DateTime? FromLegacyDateTime(this DateTime dateTime)
    {
        if (dateTime.AsUtc().Date >= MaxDate
            || dateTime.AsUtc().Date <= MinDate)
        {
            return null;
        }

        return dateTime;
    }
}
