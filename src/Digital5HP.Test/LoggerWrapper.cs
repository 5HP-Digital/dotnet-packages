#pragma warning disable DHPLOG0004
namespace Digital5HP.Test
{
    using System;

    using Digital5HP.Logging;

    /// <summary>
    /// Wrapper (composite) for <see cref="ILogger"/>.
    /// </summary>
    public class LoggerWrapper(ILogger logger) : ILogger
    {
        private readonly ILogger loggerImplementation = logger;

        public void LogDebug(string message, params object[] args)
        {
            this.loggerImplementation.LogDebug(message, args);
        }

        public void LogDebug(Exception exception, string message, params object[] args)
        {
            this.loggerImplementation.LogDebug(exception, message, args);
        }

        public void LogTrace(string message, params object[] args)
        {
            this.loggerImplementation.LogTrace(message, args);
        }

        public void LogTrace(Exception exception, string message, params object[] args)
        {
            this.loggerImplementation.LogTrace(exception, message, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            this.loggerImplementation.LogInformation(message, args);
        }

        public void LogInformation(Exception exception, string message, params object[] args)
        {
            this.loggerImplementation.LogInformation(exception, message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            this.loggerImplementation.LogWarning(message, args);
        }

        public void LogWarning(Exception exception, string message, params object[] args)
        {
            this.loggerImplementation.LogWarning(exception, message, args);
        }

        public void LogError(string message, params object[] args)
        {
            this.loggerImplementation.LogError(message, args);
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            this.loggerImplementation.LogError(exception, message, args);
        }

        public void LogCritical(string message, params object[] args)
        {
            this.loggerImplementation.LogCritical(message, args);
        }

        public void LogCritical(Exception exception, string message, params object[] args)
        {
            this.loggerImplementation.LogCritical(exception, message, args);
        }
    }
}
