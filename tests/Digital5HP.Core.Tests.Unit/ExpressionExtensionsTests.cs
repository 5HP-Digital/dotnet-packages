namespace Digital5HP.Core.Tests.Unit
{
    using System;
    using System.Linq.Expressions;

    using FluentAssertions;

    using Xunit;

    [Trait("Category", "Unit")]
    public class ExpressionExtensionsTests
    {
        [Theory]
        [ClassData(typeof(TestData))]
        public void GetMemberName(Expression<Func<object>> expression, string expectedResult)
        {
            // Arrange

            // Act
            var result = expression.GetMemberName();

            // Assert
            result.Should()
                  .Be(expectedResult);
        }

        // ReSharper disable UnusedAutoPropertyAccessor.Global
        internal class TestClass
        {
            public ChildTestClass Child { get; set; }
        }

        internal class ChildTestClass
        {
            public int Integer { get; set; }
            public object[] Array { get; set; }
            public DateTime Date { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Global

        private class TestData : TheoryData<Expression<Func<object>>, string>
        {
            public TestData()
            {
                var localVar = 1;
                const int LOCAL_CONST = 1;
                var obj = new TestClass();
                this.Add(
                    () => obj.Child,
                    "Child");
                this.Add(
                    () => obj.Child.Integer,
                    "Child.Integer");
                this.Add(
                    () => obj.Child.Array,
                    "Child.Array");
                this.Add(
                    () => obj.Child.Array.Length,
                    "Child.Array.Length");
                this.Add(
                    () => obj.Child.Date,
                    "Child.Date");
                this.Add(
                    () => obj.Child.Date.Millisecond,
                    "Child.Date.Millisecond");
                this.Add(
                    () => obj.Child.ToString(),
                    "Child.ToString()");
                this.Add(
                    () => obj.Child.Date.AddMonths(1).Date,
                    "Child.Date.AddMonths().Date");
                this.Add(
                    () => obj.Child.Date.AddDays(DateTime.UtcNow.Day).Date,
                    "Child.Date.AddDays().Date");
                this.Add(
                    () => obj.Child.Array[localVar],
                    "Child.Array[]");
                this.Add(
                    () => obj.Child.Array[LOCAL_CONST].ToString(),
                    "Child.Array[].ToString()");
            }
        }
    }
}
