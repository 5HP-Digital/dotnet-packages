namespace Digital5HP.Test
{
    using System;
    using System.Linq;

    using Digital5HP.Logging;

    using Xunit.Abstractions;

    /// <summary>
    /// Logger that implements <see cref="ILogger{T}"/> and pass through all logs to the inner <see cref="ITestOutputHelper"/>.
    /// </summary>
    public class TestLogger<T>(ITestOutputHelper testOutputHelper) : TestLogger(testOutputHelper), ILogger<T>
    {
    }

    /// <summary>
    /// Logger that implements <see cref="ILogger"/> and pass through all logs to the inner <see cref="ITestOutputHelper"/>.
    /// </summary>
    public class TestLogger(ITestOutputHelper testOutputHelper) : ILogger
    {
        private readonly ITestOutputHelper testOutputHelper = testOutputHelper;

        private void WriteLog(string level, Exception ex, string message, params object[] args)
        {
            var argsStr = args.Length != 0 ? "(" + string.Join("|", args.Select(a => a.ToString())) + ")" : null;
            var exStr = ex != null ? ">> Exception: " + ex.Message : null;
            this.testOutputHelper.WriteLine($"{Digital5HP.TimeProvider.Current.Now:HH:mm:ss.fff} [{level}] {message} {argsStr} {exStr}");
        }

        public void LogDebug(string message, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);

            this.WriteLog("DBG", null, message, args);
        }

        public void LogDebug(Exception exception, string message, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);

            this.WriteLog("DBG", exception, message, args);
        }

        public void LogTrace(string message, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);

            this.WriteLog("TRC", null, message, args);
        }

        public void LogTrace(Exception exception, string message, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);

            this.WriteLog("TRC", exception, message, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);

            this.WriteLog("INF", null, message, args);
        }

        public void LogInformation(Exception exception, string message, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);

            this.WriteLog("INF", exception, message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);

            this.WriteLog("WRN", null, message, args);
        }

        public void LogWarning(Exception exception, string message, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);

            this.WriteLog("WRN", exception, message, args);
        }

        public void LogError(string message, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);

            this.WriteLog("ERR", null, message, args);
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);

            this.WriteLog("ERR", exception, message, args);
        }

        public void LogCritical(string message, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);

            this.WriteLog("CRT", null, message, args);
        }

        public void LogCritical(Exception exception, string message, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);

            this.WriteLog("CRT", exception, message, args);
        }
    }
}
