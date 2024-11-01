namespace Digital5HP;

using System;
using System.Globalization;

public static class DateTimeExtensions
{
    private const decimal AVERAGE_DAYS_IN_MONTH = 30.436875m;

    /// <summary>
    /// Returns a new <see cref="DateTime"/> with the <see cref="DateTime.Kind"/> property set to <see cref="DateTimeKind.Utc"/>.
    /// </summary>
    public static DateTime AsUtc(this DateTime dateTime)
    {
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }

    /// <summary>
    /// Converts the given DateTime to an ISO 8601 compatible string
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/> to convert</param>
    /// <param name="includeMilliseconds">Whether to include milliseconds</param>
    /// <returns>ISO 8601 string ("yyyy-MM-ddTHH:mm:ssZ" or  "yyyy-MM-ddTHH:mm:ss.fffZ")</returns>
    public static string ToZuluTime(this DateTime dateTime, bool includeMilliseconds = false)
    {
        const string FORMAT = "yyyy-MM-ddTHH:mm:ssZ";
        const string FORMAT_WITH_MS = "yyyy-MM-ddTHH:mm:ss.fffZ";

        return dateTime.ToString(includeMilliseconds ? FORMAT_WITH_MS : FORMAT, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Returns the number of months between two dates
    /// </summary>
    /// <param name="startDate">Start Date</param>
    /// <param name="endDate">End Date</param>
    /// <returns>The difference in months as a decimal.
    /// Returns a negative number for an end date that is earlier than the start date,
    /// and uses an average number of days a month to determine the fractional month value.</returns>
    public static decimal DifferenceInMonths(this DateTime startDate, DateTime endDate)
    {
        decimal multiplier;
        if(endDate < startDate)
        {
            (startDate, endDate) = (endDate, startDate);
            multiplier = -1;
        }
        else
        {
            multiplier = 1;
        }

        decimal totalMonths = (endDate.Year - startDate.Year) * 12;
        totalMonths += endDate.Month - startDate.Month;

        //Remove the days we've included in the starting month (not in actual period):
        totalMonths -= startDate.Day / AVERAGE_DAYS_IN_MONTH;

        //Add the days in the finish month (weren't yet included, since had "1st to 1st"):
        totalMonths += endDate.Day / AVERAGE_DAYS_IN_MONTH;

        return totalMonths * multiplier;
    }

    public static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
    {
        if (timeSpan == TimeSpan.Zero)
            return dateTime;

        // Some comments suggest removing the following line.  I think the check
        // for MaxValue makes sense - it's often used to represent an indefinite expiry date.
        // (The check for DateTime.MinValue has no effect, because DateTime.MinValue % timeSpan
        // is equal to DateTime.MinValue for any non-zero value of timeSpan.  But I think
        // leaving the check in place makes the intent clearer).
        // YMMV and the fact that different people have different expectations is probably
        // part of the reason such a method doesn't exist in the Framework.
        if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue)
            return dateTime; // do not modify "guard" values

        return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
    }
}
