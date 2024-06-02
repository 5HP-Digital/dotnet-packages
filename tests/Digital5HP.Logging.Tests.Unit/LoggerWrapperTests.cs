namespace Digital5HP.Logging.Test.Unit
{
    using System;
    using System.Linq;
    using System.Text;

    using AutoFixture.Xunit2;

    using Microsoft.Extensions.Logging;

    using Moq;

    using Digital5HP.Test;

    using Xunit;

    using ILogger = Digital5HP.Logging.ILogger;

    public class LoggerWrapperTests : UnitFixtureFor<ILogger>
    {
        private readonly Mock<Microsoft.Extensions.Logging.ILogger> mockLogger;

        public LoggerWrapperTests()
        {
            this.mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger>();
        }

        protected override ILogger CreateSut()
        {
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            mockLoggerFactory.Setup(factory => factory.CreateLogger(It.IsAny<string>()))
                             .Returns(this.mockLogger.Object);

            var loggerType = typeof(Digital5HP.Logging.Internals.LoggerWrapper);
            return (ILogger) Activator.CreateInstance(loggerType, mockLoggerFactory.Object);
        }

        private void VerifyLogger(LogLevel logLevel, Exception exception, string message, params object[] args)
        {
            this.mockLogger.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(level => level == logLevel),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>(
                        (v, t) =>
                            args.Length != 0
                                ? args.ToList()
                                      .TrueForAll(
                                           obj => v.ToString()
                                                   .Contains(obj.ToString(), StringComparison.Ordinal))
                                : v.ToString()
                                   .Contains(message, StringComparison.Ordinal)),
                    It.Is<Exception>(e => e == exception),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        #region LogDebug

        [Theory]
        [AutoData]
        public void LogDebug_NoArgsNoException_LogsMessage(string message)
        {
            // Arrange

            // Act
            this.Sut.LogDebug(message);

            // Assert
            this.VerifyLogger(LogLevel.Debug, null, message);
        }

        [Theory]
        [AutoData]
        public void LogDebug_NoException_LogsArgs(params object[] args)
        {
            // Arrange
            var messageBuilder = new StringBuilder();
            for (var index = 0; index < args.Length; index++)
            {
                messageBuilder.Append($"{{{index}}}");
            }

            // Act
            this.Sut.LogDebug(messageBuilder.ToString(), args);

            // Assert
            this.VerifyLogger(LogLevel.Debug, null, null, args);
        }

        [Theory]
        [AutoData]
        public void LogDebug_WithException_LogsException(Exception exception, string message)
        {
            // Arrange

            // Act
            this.Sut.LogDebug(exception, message);

            // Assert
            this.VerifyLogger(LogLevel.Debug, exception, message);
        }

        #endregion

        #region LogTrace

        [Theory]
        [AutoData]
        public void LogTrace_NoArgsNoException_LogsMessage(string message)
        {
            // Arrange

            // Act
            this.Sut.LogTrace(message);

            // Assert
            this.VerifyLogger(LogLevel.Trace, null, message);
        }

        [Theory]
        [AutoData]
        public void LogTrace_NoException_LogsArgs(params object[] args)
        {
            // Arrange
            var messageBuilder = new StringBuilder();
            for (var index = 0; index < args.Length; index++)
            {
                messageBuilder.Append($"{{{index}}}");
            }

            // Act
            this.Sut.LogTrace(messageBuilder.ToString(), args);

            // Assert
            this.VerifyLogger(LogLevel.Trace, null, null, args);
        }

        [Theory]
        [AutoData]
        public void LogTrace_WithException_LogsException(Exception exception, string message)
        {
            // Arrange

            // Act
            this.Sut.LogTrace(exception, message);

            // Assert
            this.VerifyLogger(LogLevel.Trace, exception, message);
        }

        #endregion

        #region LogInformation

        [Theory]
        [AutoData]
        public void LogInformation_NoArgsNoException_LogsMessage(string message)
        {
            // Arrange

            // Act
            this.Sut.LogInformation(message);

            // Assert
            this.VerifyLogger(LogLevel.Information, null, message);
        }

        [Theory]
        [AutoData]
        public void LogInformation_NoException_LogsArgs(params object[] args)
        {
            // Arrange
            var messageBuilder = new StringBuilder();
            for (var index = 0; index < args.Length; index++)
            {
                messageBuilder.Append($"{{{index}}}");
            }

            // Act
            this.Sut.LogInformation(messageBuilder.ToString(), args);

            // Assert
            this.VerifyLogger(LogLevel.Information, null, null, args);
        }

        [Theory]
        [AutoData]
        public void LogInformation_WithException_LogsException(Exception exception, string message)
        {
            // Arrange

            // Act
            this.Sut.LogInformation(exception, message);

            // Assert
            this.VerifyLogger(LogLevel.Information, exception, message);
        }

        #endregion

        #region LogWarning

        [Theory]
        [AutoData]
        public void LogWarning_NoArgsNoException_LogsMessage(string message)
        {
            // Arrange

            // Act
            this.Sut.LogWarning(message);

            // Assert
            this.VerifyLogger(LogLevel.Warning, null, message);
        }

        [Theory]
        [AutoData]
        public void LogWarning_NoException_LogsArgs(params object[] args)
        {
            // Arrange
            var messageBuilder = new StringBuilder();
            for (var index = 0; index < args.Length; index++)
            {
                messageBuilder.Append($"{{{index}}}");
            }

            // Act
            this.Sut.LogWarning(messageBuilder.ToString(), args);

            // Assert
            this.VerifyLogger(LogLevel.Warning, null, null, args);
        }

        [Theory]
        [AutoData]
        public void LogWarning_WithException_LogsException(Exception exception, string message)
        {
            // Arrange

            // Act
            this.Sut.LogWarning(exception, message);

            // Assert
            this.VerifyLogger(LogLevel.Warning, exception, message);
        }

        #endregion

        #region LogError

        [Theory]
        [AutoData]
        public void LogError_NoArgsNoException_LogsMessage(string message)
        {
            // Arrange

            // Act
            this.Sut.LogError(message);

            // Assert
            this.VerifyLogger(LogLevel.Error, null, message);
        }

        [Theory]
        [AutoData]
        public void LogError_NoException_LogsArgs(params object[] args)
        {
            // Arrange
            var messageBuilder = new StringBuilder();
            for (var index = 0; index < args.Length; index++)
            {
                messageBuilder.Append($"{{{index}}}");
            }

            // Act
            this.Sut.LogError(messageBuilder.ToString(), args);

            // Assert
            this.VerifyLogger(LogLevel.Error, null, null, args);
        }

        [Theory]
        [AutoData]
        public void LogError_WithException_LogsException(Exception exception, string message)
        {
            // Arrange

            // Act
            this.Sut.LogError(exception, message);

            // Assert
            this.VerifyLogger(LogLevel.Error, exception, message);
        }

        #endregion

        #region LogCritical

        [Theory]
        [AutoData]
        public void LogCritical_NoArgsNoException_LogsMessage(string message)
        {
            // Arrange

            // Act
            this.Sut.LogCritical(message);

            // Assert
            this.VerifyLogger(LogLevel.Critical, null, message);
        }

        [Theory]
        [AutoData]
        public void LogCritical_NoException_LogsArgs(params object[] args)
        {
            // Arrange
            var messageBuilder = new StringBuilder();
            for (var index = 0; index < args.Length; index++)
            {
                messageBuilder.Append($"{{{index}}}");
            }

            // Act
            this.Sut.LogCritical(messageBuilder.ToString(), args);

            // Assert
            this.VerifyLogger(LogLevel.Critical, null, null, args);
        }

        [Theory]
        [AutoData]
        public void LogCritical_WithException_LogsException(Exception exception, string message)
        {
            // Arrange

            // Act
            this.Sut.LogCritical(exception, message);

            // Assert
            this.VerifyLogger(LogLevel.Critical, exception, message);
        }

        #endregion
    }
}
