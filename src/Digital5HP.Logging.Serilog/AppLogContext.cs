namespace Digital5HP.Logging.Serilog;

using System;

using global::Serilog;

using Digital5HP.Logging.Serilog.Internals;

public sealed class AppLogContext : IDisposable
{
    public AppLogContext(string applicationName, string environment, LoggingOptions options, Type executingType)
    {
        Log.Logger = LoggerFactory.CreateLogger(applicationName, environment, options);

        var logger = Log.Logger.ForContext(executingType);

        this.Logger = new StartupLogger(logger);
    }

    public Logging.ILogger Logger { get; }

    public void Dispose()
    {
        Log.CloseAndFlush();
    }
}
