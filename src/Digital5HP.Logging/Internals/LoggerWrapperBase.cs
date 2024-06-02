#pragma warning disable CA1848, CA2254
// ReSharper disable TemplateIsNotCompileTimeConstantProblem
namespace Digital5HP.Logging.Internals;

using System;

using Microsoft.Extensions.Logging;

using ILogger = ILogger;

internal abstract class LoggerWrapperBase(Microsoft.Extensions.Logging.ILogger innerLogger) : ILogger
{
    private readonly Microsoft.Extensions.Logging.ILogger innerLogger = innerLogger;

    public void LogDebug(string message, params object[] args)
    {
        this.innerLogger.LogDebug(message, args);
    }

    public void LogDebug(Exception exception, string message, params object[] args)
    {
        this.innerLogger.LogDebug(exception, message, args);
    }

    public void LogTrace(string message, params object[] args)
    {
        this.innerLogger.LogTrace(message, args);
    }

    public void LogTrace(Exception exception, string message, params object[] args)
    {
        this.innerLogger.LogTrace(exception, message, args);
    }

    public void LogInformation(string message, params object[] args)
    {
        this.innerLogger.LogInformation(message, args);
    }

    public void LogInformation(Exception exception, string message, params object[] args)
    {
        this.innerLogger.LogInformation(exception, message, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        this.innerLogger.LogWarning(message, args);
    }

    public void LogWarning(Exception exception, string message, params object[] args)
    {
        this.innerLogger.LogWarning(exception, message, args);
    }

    public void LogError(string message, params object[] args)
    {
        this.innerLogger.LogError(message, args);
    }

    public void LogError(Exception exception, string message, params object[] args)
    {
        this.innerLogger.LogError(exception, message, args);
    }

    public void LogCritical(string message, params object[] args)
    {
        this.innerLogger.LogCritical(message, args);
    }

    public void LogCritical(Exception exception, string message, params object[] args)
    {
        this.innerLogger.LogCritical(exception, message, args);
    }
}
