namespace Digital5HP.Core.Tests.Unit
{
    using System;

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
