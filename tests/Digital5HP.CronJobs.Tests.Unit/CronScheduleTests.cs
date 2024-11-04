namespace Digital5HP.CronJobs.Tests.Unit;

using System;
using System.Collections.Generic;

using Digital5HP.Test;

using FluentAssertions;

using NCrontab;

using Xunit;

[Trait("Category", "Unit")]
public class CronScheduleTests : FixtureBase
{
    [Theory]
    [ClassData(typeof(TestData))]
    public void Test_GetNextOccurrences(string expression, DateTime from, DateTime to, List<DateTime> expectedResult)
    {
        // Arrange

        // Act
        var result = CrontabSchedule.TryParse(expression).GetNextOccurrences(from, to);

        // Assert
        result.Should()
            .BeEquivalentTo(expectedResult);
    }

    private class TestData : TheoryData<string, DateTime, DateTime, List<DateTime>>
    {
        public TestData()
        {
            // every minute
            this.Add(
                "* * * * *",
                new DateTime(2020, 1, 1, 0, 0, 0),
                new DateTime(2020, 1, 1, 0, 5, 0),
                [
                    new DateTime(2020, 1, 1, 0, 1, 0),
                    new DateTime(2020, 1, 1, 0, 2, 0),
                    new DateTime(2020, 1, 1, 0, 3, 0),
                    new DateTime(2020, 1, 1, 0, 4, 0),
                ]);
            // every minute
            this.Add(
                "* * * * *",
                new DateTime(2020, 1, 1, 0, 0, 0),
                new DateTime(2020, 1, 1, 0, 1, 0),
                []);
            // every hour
            this.Add(
                "0 * * * *",
                new DateTime(2020, 1, 1, 0, 0, 0),
                new DateTime(2020, 1, 1, 3, 20, 10),
                [
                    new DateTime(2020, 1, 1, 1, 0, 0),
                    new DateTime(2020, 1, 1, 2, 0, 0),
                    new DateTime(2020, 1, 1, 3, 0, 0),
                ]);
            // every 3 hours at round hour
            this.Add(
                "0 */3 * * *",
                new DateTime(2020, 1, 1, 0, 0, 0),
                new DateTime(2020, 1, 1, 23, 59, 59),
                [
                    new DateTime(2020, 1, 1, 3, 0, 0),
                    new DateTime(2020, 1, 1, 6, 0, 0),
                    new DateTime(2020, 1, 1, 9, 0, 0),
                    new DateTime(2020, 1, 1, 12, 0, 0),
                    new DateTime(2020, 1, 1, 15, 0, 0),
                    new DateTime(2020, 1, 1, 18, 0, 0),
                    new DateTime(2020, 1, 1, 21, 0, 0),
                ]);
        }
    }
}
