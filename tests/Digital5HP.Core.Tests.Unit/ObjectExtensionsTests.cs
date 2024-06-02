namespace Digital5HP.Core.Tests.Unit
{
    using System;
    using System.Diagnostics;

    using FluentAssertions;

    using Xunit;

    [Trait("Category", "Unit")]
    public class ObjectExtensionsTests
    {
        [Theory]
        [InlineData("1", typeof(int), 1)]
        [InlineData(null, typeof(int), 0)]
        [InlineData("", typeof(int), 0)]
        [InlineData("true", typeof(bool), true)]
        [InlineData("False", typeof(bool), false)]
        [InlineData("1", typeof(bool), true)]
        [InlineData("0", typeof(bool), false)]
        [InlineData("", typeof(bool), false)]
        [InlineData(null, typeof(bool), false)]
        [InlineData("1", typeof(DayOfWeek), DayOfWeek.Monday)]
        [InlineData("Monday", typeof(DayOfWeek), DayOfWeek.Monday)]
        [InlineData("", typeof(DayOfWeek), DayOfWeek.Sunday)]
        [InlineData(1, typeof(DayOfWeek), DayOfWeek.Monday)]
        [InlineData(null, typeof(DayOfWeek), DayOfWeek.Sunday)]
        [InlineData(DayOfWeek.Monday, typeof(int), 1)]
        [InlineData("Text", typeof(string), "Text")]
        [InlineData(null, typeof(string), null)]
        [InlineData(1.23, typeof(string), "1.23")]
        [InlineData(0, typeof(string), "0")]
        [InlineData(false, typeof(string), "False")]
        [InlineData(DayOfWeek.Monday, typeof(string), "Monday")]
        public void ChangeType_Succeed(object value, Type type, object expected)
        {
            // Arrange
            var methodInfo = typeof(ObjectExtensions).GetMethod(
                nameof(ObjectExtensions.ChangeType),
                new[] {typeof(object)});

            Debug.Assert(methodInfo != null, nameof(methodInfo) + " != null");

            var genericMethodInfo = methodInfo.MakeGenericMethod(type);

            // Act
            var result = genericMethodInfo.Invoke(null, new[] {value});

            // Assert
            result.Should()
                  .Be(expected);
        }
    }
}
