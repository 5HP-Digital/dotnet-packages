namespace Digital5HP.Test
{
    using System;

    using Moq;

    public static class DateTimeExtensions
    {
        /// <summary>
        /// Sets <see cref="TimeProvider.Current"/> to a mock <see cref="ITimeProvider"/> setup to return the specified <see cref="DateTime"/>.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> to return.</param>
        /// <returns>The mock <see cref="ITimeProvider"/> that was created.</returns>
        public static Mock<ITimeProvider> Freeze(this DateTime dateTime)
        {
            var timeProviderMock = new Mock<ITimeProvider>(MockBehavior.Strict);
            timeProviderMock.SetupGet(provider => provider.Now)
                            .Returns(dateTime);
            Digital5HP.TimeProvider.Current = timeProviderMock.Object;

            return timeProviderMock;
        }
    }
}
