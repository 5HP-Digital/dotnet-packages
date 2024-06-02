namespace Digital5HP.Core.Tests.Unit
{
    using System;

    using FluentAssertions;
    using FluentAssertions.Extensions;

    using Moq;

    using Xunit;

    [Trait("Category", "Unit")]
    public class TimeProviderTests
    {
        [Fact]
        public void Now_DefaultProvider_ReturnsCurrentDateTime()
        {
            // Arrange
            const int ACCURACY_MILLISECONDS = 500;

            // Act
            var result = Digital5HP.TimeProvider.Current.Now;

            // Assert
            result.Should()
                  .BeCloseTo(DateTime.UtcNow, ACCURACY_MILLISECONDS.Milliseconds());
        }

        [Fact]
        public void ResetProvider_CurrentProviderNotDefault_SetsCurrentToDefault()
        {
            // Arrange
            const int ACCURACY_MILLISECONDS = 500;
            var expectedDateTime = new DateTime(2020, 1, 1);

            var timeProviderMock = new Mock<ITimeProvider>(MockBehavior.Strict);
            timeProviderMock.Setup(provider => provider.Now)
                            .Returns(expectedDateTime);
            Digital5HP.TimeProvider.Current = timeProviderMock.Object;

            // Act
            Digital5HP.TimeProvider.ResetProvider();

            // Assert
            Digital5HP.TimeProvider.Current.Now.Should()
                        .BeCloseTo(DateTime.UtcNow, ACCURACY_MILLISECONDS.Milliseconds());

            timeProviderMock.Verify(provider => provider.Now, Times.Never);
        }

        [Fact]
        public void SetCurrent_Succeed()
        {
            // Arrange
            var expectedDateTime = new DateTime(2020, 1, 1);

            var timeProviderMock = new Mock<ITimeProvider>(MockBehavior.Strict);
            timeProviderMock.Setup(provider => provider.Now)
                            .Returns(expectedDateTime);

            // Act
            Digital5HP.TimeProvider.Current = timeProviderMock.Object;

            // Assert
            var result = Digital5HP.TimeProvider.Current.Now;
            Digital5HP.TimeProvider.ResetProvider();
            result.Should()
                  .Be(expectedDateTime);

            timeProviderMock.Verify(provider => provider.Now, Times.Once);
        }
    }
}

