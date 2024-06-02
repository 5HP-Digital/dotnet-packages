// ReSharper disable TemplateIsNotCompileTimeConstantProblem
namespace Digital5HP.Logging.Serilog.Internals;

using System;

using Serilog = global::Serilog;

public class StartupLogger(Serilog.ILogger logger) : ILogger
{
    private readonly Serilog.ILogger logger = logger;

    public void LogDebug(string message, params object[] args)
    {
        this.logger.Debug(message, args);
    }

    public void LogDebug(Exception exception, string message, params object[] args)
    {
        this.logger.Debug(exception, message, args);
    }

    public void LogTrace(string message, params object[] args)
    {
        this.logger.Verbose(message, args);
    }

    public void LogTrace(Exception exception, string message, params object[] args)
    {
        this.logger.Verbose(exception, message, args);
    }

    public void LogInformation(string message, params object[] args)
    {
        this.logger.Information(message, args);
    }

    public void LogInformation(Exception exception, string message, params object[] args)
    {
        this.logger.Information(exception, message, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        this.logger.Warning(message, args);
    }

    public void LogWarning(Exception exception, string message, params object[] args)
    {
        this.logger.Warning(exception, message, args);
    }

    public void LogError(string message, params object[] args)
    {
        this.logger.Error(message, args);
    }

    public void LogError(Exception exception, string message, params object[] args)
    {
        this.logger.Error(exception, message, args);
    }

    public void LogCritical(string message, params object[] args)
    {
        this.logger.Fatal(message, args);
    }

    public void LogCritical(Exception exception, string message, params object[] args)
    {
        this.logger.Fatal(exception, message, args);
    }
}
