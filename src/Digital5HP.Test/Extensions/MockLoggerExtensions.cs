#pragma warning disable DHPLOG0004
namespace Digital5HP.Test
{
    using System;

    using Moq;

    using Digital5HP.Logging;

    /// <summary>
    /// Extension methods for verifying a mock <see cref="ILogger{T}"/> was used to create logs.
    /// </summary>
    public static class MockLoggerExtensions
    {
        /// <summary>
        /// Default value for each <c>times</c> parameter.
        /// </summary>
        private static readonly Times DefaultTimes = Times.AtLeastOnce();

        /// <summary>
        /// Verifies that the method <see cref="ILogger.LogDebug(string,object[])"/> was called.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mockLogger"></param>
        /// <param name="times">The number of calls that should have been verified.</param>
        public static void VerifyLogDebug<T>(this Mock<ILogger<T>> mockLogger, Times? times = null)
        {
            ArgumentNullException.ThrowIfNull(mockLogger);

            mockLogger.Verify(
                logger => logger.LogDebug(It.IsAny<string>(), It.IsAny<object[]>()),
                times ?? DefaultTimes);
        }

        /// <summary>
        /// Verifies that the method <see cref="ILogger.LogDebug(Exception,string,object[])"/> was called with <typeparamref name="TException"/> exception type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="mockLogger"></param>
        /// <param name="times">The number of calls that should have been verified.</param>
        public static void VerifyLogDebug<T, TException>(this Mock<ILogger<T>> mockLogger, Times? times = null)
            where TException : Exception
        {
            ArgumentNullException.ThrowIfNull(mockLogger);

            mockLogger.Verify(
                logger => logger.LogDebug(It.IsAny<TException>(), It.IsAny<string>(), It.IsAny<object[]>()),
                times ?? DefaultTimes);
        }

        /// <summary>
        /// Verifies that the method <see cref="ILogger.LogInformation(string,object[])"/> was called.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mockLogger"></param>
        /// <param name="times">The number of calls that should have been verified.</param>
        public static void VerifyLogInformation<T>(this Mock<ILogger<T>> mockLogger, Times? times = null)
        {
            ArgumentNullException.ThrowIfNull(mockLogger);

            mockLogger.Verify(
                logger => logger.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
                times ?? DefaultTimes);
        }

        /// <summary>
        /// Verifies that the method <see cref="ILogger.LogInformation(Exception,string,object[])"/> was called with <typeparamref name="TException"/> exception type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="mockLogger"></param>
        /// <param name="times">The number of calls that should have been verified.</param>
        public static void VerifyLogInformation<T, TException>(this Mock<ILogger<T>> mockLogger, Times? times = null)
            where TException : Exception
        {
            ArgumentNullException.ThrowIfNull(mockLogger);

            mockLogger.Verify(
                logger => logger.LogInformation(It.IsAny<TException>(), It.IsAny<string>(), It.IsAny<object[]>()),
                times ?? DefaultTimes);
        }

        /// <summary>
        /// Verifies that the method <see cref="ILogger.LogWarning(string,object[])"/> was called.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mockLogger"></param>
        /// <param name="times">The number of calls that should have been verified.</param>
        public static void VerifyLogWarning<T>(this Mock<ILogger<T>> mockLogger, Times? times = null)
        {
            ArgumentNullException.ThrowIfNull(mockLogger);

            mockLogger.Verify(
                logger => logger.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()),
                times ?? DefaultTimes);
        }

        /// <summary>
        /// Verifies that the method <see cref="ILogger.LogWarning(Exception,string,object[])"/> was called with <typeparamref name="TException"/> exception type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="mockLogger"></param>
        /// <param name="times">The number of calls that should have been verified.</param>
        public static void VerifyLogWarning<T, TException>(this Mock<ILogger<T>> mockLogger, Times? times = null)
            where TException : Exception
        {
            ArgumentNullException.ThrowIfNull(mockLogger);

            mockLogger.Verify(
                logger => logger.LogWarning(It.IsAny<TException>(), It.IsAny<string>(), It.IsAny<object[]>()),
                times ?? DefaultTimes);
        }

        /// <summary>
        /// Verifies that the method <see cref="ILogger.LogError(string,object[])"/> was called.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mockLogger"></param>
        /// <param name="times">The number of calls that should have been verified.</param>
        public static void VerifyLogError<T>(this Mock<ILogger<T>> mockLogger, Times? times = null)
        {
            ArgumentNullException.ThrowIfNull(mockLogger);

            mockLogger.Verify(
                logger => logger.LogError(It.IsAny<string>(), It.IsAny<object[]>()),
                times ?? DefaultTimes);
        }

        /// <summary>
        /// Verifies that the method <see cref="ILogger.LogError(Exception,string,object[])"/> was called with <typeparamref name="TException"/> exception type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="mockLogger"></param>
        /// <param name="times">The number of calls that should have been verified.</param>
        public static void VerifyLogError<T, TException>(this Mock<ILogger<T>> mockLogger,
                                                         Times? times = null)
            where TException : Exception
        {
            ArgumentNullException.ThrowIfNull(mockLogger);

            mockLogger.Verify(
                logger => logger.LogError(It.IsAny<TException>(), It.IsAny<string>(), It.IsAny<object[]>()),
                times ?? DefaultTimes);
        }

        /// <summary>
        /// Verifies that the method <see cref="ILogger.LogCritical(string,object[])"/> was called.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mockLogger"></param>
        /// <param name="times">The number of calls that should have been verified.</param>
        public static void VerifyLogCritical<T>(this Mock<ILogger<T>> mockLogger, Times? times = null)
        {
            ArgumentNullException.ThrowIfNull(mockLogger);

            mockLogger.Verify(
                logger => logger.LogCritical(It.IsAny<string>(), It.IsAny<object[]>()),
                times ?? DefaultTimes);
        }

        /// <summary>
        /// Verifies that the method <see cref="ILogger.LogCritical(Exception,string,object[])"/> was called with <typeparamref name="TException"/> exception type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="mockLogger"></param>
        /// <param name="times">The number of calls that should have been verified.</param>
        public static void VerifyLogCritical<T, TException>(this Mock<ILogger<T>> mockLogger,
                                                            Times? times = null)
            where TException : Exception
        {
            ArgumentNullException.ThrowIfNull(mockLogger);

            mockLogger.Verify(
                logger => logger.LogCritical(It.IsAny<TException>(), It.IsAny<string>(), It.IsAny<object[]>()),
                times ?? DefaultTimes);
        }

        /// <summary>
        /// Verifies that the method <see cref="ILogger.LogDebug(string,object[])"/> was called.
        /// </summary>
        /// <param name="mockLogger"></param>
        /// <param name="times">The number of calls that should have been verified.</param>
        public static void VerifyLogDebug(this Mock<ILogger> mockLogger, Times? times = null)
        {
            ArgumentNullException.ThrowIfNull(mockLogger);

            mockLogger.Verify(
                logger => logger.LogDebug(It.IsAny<string>(), It.IsAny<object[]>()),
                times ?? DefaultTimes);
        }

        /// <summary>
        /// Verifies that the method <see cref="ILogger.LogDebug(Exception,string,object[])"/> was called with <typeparamref name="TException"/> exception type.
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="mockLogger"></param>
        /// <param name="times">The number of calls that should have been verified.</param>
        public static void VerifyLogDebug<TException>(this Mock<ILogger> mockLogger, Times? times = null)
            where TException : Exception
        {
            ArgumentNullException.ThrowIfNull(mockLogger);

            mockLogger.Verify(
                logger => logger.LogDebug(It.IsAny<TException>(), It.IsAny<string>(), It.IsAny<object[]>()),
                times ?? DefaultTimes);
        }

        /// <summary>
        /// Verifies that the method <see cref="ILogger.LogInformation(string,object[])"/> was called.
        /// </summary>
        /// <param name="mockLogger"></param>
        /// <param name="times">The number of calls that should have been verified.</param>
        public static void VerifyLogInformation(this Mock<ILogger> mockLogger, Times? times = null)
        {
            ArgumentNullException.ThrowIfNull(mockLogger);

            mockLogger.Verify(
                logger => logger.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
                times ?? DefaultTimes);
        }

        /// <summary>
        /// Verifies that the method <see cref="ILogger.LogInformation(Exception,string,object[])"/> was called with <typeparamref name="TException"/> exception type.
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="mockLogger"></param>
        /// <param name="times">The number of calls that should have been verified.</param>
        public static void VerifyLogInformation<TException>(this Mock<ILogger> mockLogger, Times? times = null)
            where TException : Exception
        {
            ArgumentNullException.ThrowIfNull(mockLogger);

            mockLogger.Verify(
                logger => logger.LogInformation(It.IsAny<TException>(), It.IsAny<string>(), It.IsAny<object[]>()),
                times ?? DefaultTimes);
        }

        /// <summary>
        /// Verifies that the method <see cref="ILogger.LogWarning(string,object[])"/> was called.
        /// </summary>
        /// <param name="mockLogger"></param>
        /// <param name="times">The number of calls that should have been verified.</param>
        public static void VerifyLogWarning(this Mock<ILogger> mockLogger, Times? times = null)
        {
            ArgumentNullException.ThrowIfNull(mockLogger);

            mockLogger.Verify(
                logger => logger.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()),
                times ?? DefaultTimes);
        }

        /// <summary>
        /// Verifies that the method <see cref="ILogger.LogWarning(Exception,string,object[])"/> was called with <typeparamref name="TException"/> exception type.
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="mockLogger"></param>
        /// <param name="times">The number of calls that should have been verified.</param>
        public static void VerifyLogWarning<TException>(this Mock<ILogger> mockLogger, Times? times = null)
            where TException : Exception
        {
            ArgumentNullException.ThrowIfNull(mockLogger);

            mockLogger.Verify(
                logger => logger.LogWarning(It.IsAny<TException>(), It.IsAny<string>(), It.IsAny<object[]>()),
                times ?? DefaultTimes);
        }

        /// <summary>
        /// Verifies that the method <see cref="ILogger.LogError(string,object[])"/> was called.
        /// </summary>
        /// <param name="mockLogger"></param>
        /// <param name="times">The number of calls that should have been verified.</param>
        public static void VerifyLogError(this Mock<ILogger> mockLogger, Times? times = null)
        {
            ArgumentNullException.ThrowIfNull(mockLogger);

            mockLogger.Verify(
                logger => logger.LogError(It.IsAny<string>(), It.IsAny<object[]>()),
                times ?? DefaultTimes);
        }

        /// <summary>
        /// Verifies that the method <see cref="ILogger.LogError(Exception,string,object[])"/> was called with <typeparamref name="TException"/> exception type.
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="mockLogger"></param>
        /// <param name="times">The number of calls that should have been verified.</param>
        public static void VerifyLogError<TException>(this Mock<ILogger> mockLogger,
                                                         Times? times = null)
            where TException : Exception
        {
            ArgumentNullException.ThrowIfNull(mockLogger);

            mockLogger.Verify(
                logger => logger.LogError(It.IsAny<TException>(), It.IsAny<string>(), It.IsAny<object[]>()),
                times ?? DefaultTimes);
        }

        /// <summary>
        /// Verifies that the method <see cref="ILogger.LogCritical(string,object[])"/> was called.
        /// </summary>
        /// <param name="mockLogger"></param>
        /// <param name="times">The number of calls that should have been verified.</param>
        public static void VerifyLogCritical(this Mock<ILogger> mockLogger, Times? times = null)
        {
            ArgumentNullException.ThrowIfNull(mockLogger);

            mockLogger.Verify(
                logger => logger.LogCritical(It.IsAny<string>(), It.IsAny<object[]>()),
                times ?? DefaultTimes);
        }

        /// <summary>
        /// Verifies that the method <see cref="ILogger.LogCritical(Exception,string,object[])"/> was called with <typeparamref name="TException"/> exception type.
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="mockLogger"></param>
        /// <param name="times">The number of calls that should have been verified.</param>
        public static void VerifyLogCritical<TException>(this Mock<ILogger> mockLogger,
                                                            Times? times = null)
            where TException : Exception
        {
            ArgumentNullException.ThrowIfNull(mockLogger);

            mockLogger.Verify(
                logger => logger.LogCritical(It.IsAny<TException>(), It.IsAny<string>(), It.IsAny<object[]>()),
                times ?? DefaultTimes);
        }
    }
}
