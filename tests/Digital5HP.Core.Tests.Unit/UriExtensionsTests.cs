namespace Digital5HP.Core.Tests.Unit
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using FluentAssertions;

    using Xunit;

    [Trait("Category", "Unit")]
    public class UriExtensionsTests
    {
        [Theory]
        [InlineData("http://localhost", "path", "http://localhost/path")]
        [InlineData("http://localhost:8080", "path", "http://localhost:8080/path")]
        [InlineData("http://www.domain.com/dir", "relative", "http://www.domain.com/dir/relative")]
        [InlineData("http://www.domain.com/", "relative", "http://www.domain.com/relative")]
        [InlineData("http://www.domain.com/", "/relative", "http://www.domain.com/relative")]
        [InlineData("http://www.domain.com", "/relative", "http://www.domain.com/relative")]
        [InlineData("http://www.domain.com", null, "http://www.domain.com")]
        [InlineData("http://www.domain.com", "", "http://www.domain.com")]
        public void Combine_ValidValues_Success(string url, string path, string expected)
        {
            // Arrange
            var uri = new Uri(url);

            // Act
            var result = uri.Combine(path);

            // Assert
            result.Should()
                  .Be(expected);
        }

        [Fact]
        [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
        public void Combine_InvalidValues_Success()
        {
            // Arrange
            Uri uri = null;
            const string PATH = "somePath";

            // Act
            var result = Assert.Throws<ArgumentNullException>(() => uri.Combine(PATH));

            // Assert
            result.Should()
                  .NotBeNull();
        }
    }
}