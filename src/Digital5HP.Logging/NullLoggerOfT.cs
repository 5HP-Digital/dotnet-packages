namespace Digital5HP.Logging;

using System;

public class NullLogger<T> : ILogger<T>
{
    public void LogDebug(string message, params object[] args)
    {
    }

    public void LogDebug(Exception exception, string message, params object[] args)
    {
    }

    public void LogTrace(string message, params object[] args)
    {
    }

    public void LogTrace(Exception exception, string message, params object[] args)
    {
    }

    public void LogInformation(string message, params object[] args)
    {
    }

    public void LogInformation(Exception exception, string message, params object[] args)
    {
    }

    public void LogWarning(string message, params object[] args)
    {
    }

    public void LogWarning(Exception exception, string message, params object[] args)
    {
    }

    public void LogError(string message, params object[] args)
    {
    }

    public void LogError(Exception exception, string message, params object[] args)
    {
    }

    public void LogCritical(string message, params object[] args)
    {
    }

    public void LogCritical(Exception exception, string message, params object[] args)
    {
    }
}