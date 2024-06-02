namespace Digital5HP.Logging;

using System;

public interface ILogger
{
    [StructuredMessageTemplateMethod("message")]
    void LogDebug(string message, params object[] args);

    [StructuredMessageTemplateMethod("message")]
    void LogDebug(Exception exception, string message, params object[] args);

    [StructuredMessageTemplateMethod("message")]
    void LogTrace(string message, params object[] args);

    [StructuredMessageTemplateMethod("message")]
    void LogTrace(Exception exception, string message, params object[] args);

    [StructuredMessageTemplateMethod("message")]
    void LogInformation(string message, params object[] args);

    [StructuredMessageTemplateMethod("message")]
    void LogInformation(Exception exception, string message, params object[] args);

    [StructuredMessageTemplateMethod("message")]
    void LogWarning(string message, params object[] args);

    [StructuredMessageTemplateMethod("message")]
    void LogWarning(Exception exception, string message, params object[] args);

    [StructuredMessageTemplateMethod("message")]
    void LogError(string message, params object[] args);

    [StructuredMessageTemplateMethod("message")]
    void LogError(Exception exception, string message, params object[] args);

    [StructuredMessageTemplateMethod("message")]
    void LogCritical(string message, params object[] args);

    [StructuredMessageTemplateMethod("message")]
    void LogCritical(Exception exception, string message, params object[] args);
}
