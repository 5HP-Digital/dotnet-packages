namespace Digital5HP.Core.Tests.Unit
{
    using System;

    using AutoFixture.Xunit2;

    using FluentAssertions;

    using Xunit;

    [Trait("Category", "Unit")]
    public class DateTimeExtensionsTests
    {
        [Theory]
        [ClassData(typeof(TestData))]
        public void DifferenceInMonths_Succeed(DateTime startDate, DateTime endDate, decimal expected)
        {
            var result = startDate.DifferenceInMonths(endDate);

            // Assert
            result.Should()
                  .Be(expected);
        }

        [Theory]
        [AutoData]
        public void Truncate_Hour_Succeed(DateTime date)
        {
            // Arrange

            // Act
            var result = date.Truncate(TimeSpan.FromHours(1));

            // Assert
            result.Year.Should()
                .Be(date.Year);
            result.Month.Should()
                .Be(date.Month);
            result.Day.Should()
                .Be(date.Day);
            result.Hour.Should()
                .Be(date.Hour);
            result.Minute.Should()
                .Be(0);
            result.Second.Should()
                .Be(0);
            result.Millisecond.Should()
                .Be(0);
        }

        [Theory]
        [AutoData]
        public void Truncate_Minute_Succeed(DateTime date)
        {
            // Arrange

            // Act
            var result = date.Truncate(TimeSpan.FromMinutes(1));

            // Assert
            result.Year.Should()
                .Be(date.Year);
            result.Month.Should()
                .Be(date.Month);
            result.Day.Should()
                .Be(date.Day);
            result.Hour.Should()
                .Be(date.Hour);
            result.Minute.Should()
                .Be(date.Minute);
            result.Second.Should()
                .Be(0);
            result.Millisecond.Should()
                .Be(0);
        }

        private class TestData : TheoryData<DateTime, DateTime, decimal>
        {
            public TestData()
            {
                this.Add(
                    new DateTime(2019, 3, 20),
                    new DateTime(2021, 1, 13),
                    21.770015811412965358631594078M);
                this.Add(
                    new DateTime(2020, 1, 15),
                    new DateTime(2021, 7, 30),
                    18.492823261257931374360869833M);
                this.Add(
                    new DateTime(2020, 1, 20),
                    new DateTime(2020, 1, 25),
                    0.1642744204193104581202899444M);
            }
        }
    }
}
