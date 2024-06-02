namespace Digital5HP.Core.Tests.Unit
{
    using System;

    using FluentAssertions;

    using Xunit;

    [Trait("Category", "Unit")]
    public class TypeExtensionsTests
    {
        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(TimeSpan))]
        [InlineData(typeof(double))]
        [InlineData(typeof(char))]
        [InlineData(typeof(Base64FormattingOptions))]
        public void CanBeNull_ForValueType_Success(Type type)
        {
            // Arrange

            // Act
            var result = type.CanBeNull();

            // Assert
            result.Should()
                  .BeFalse();
        }

        [Theory]
        [InlineData(typeof(Type))]
        [InlineData(typeof(object))]
        public void CanBeNull_ForReferenceType_Success(Type type)
        {
            // Arrange

            // Act
            var result = type.CanBeNull();

            // Assert
            result.Should()
                  .BeTrue();
        }

        [Theory]
        [InlineData(typeof(int?))]
        [InlineData(typeof(DateTime?))]
        [InlineData(typeof(TimeSpan?))]
        [InlineData(typeof(double?))]
        [InlineData(typeof(char?))]
        [InlineData(typeof(Base64FormattingOptions?))]
        public void CanBeNull_ForNullable_Success(Type type)
        {
            // Arrange

            // Act
            var result = type.CanBeNull();

            // Assert
            result.Should()
                  .BeTrue();
        }
    }
}