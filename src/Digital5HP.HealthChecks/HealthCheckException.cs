namespace Digital5HP.HealthChecks;

using global::System;

public class HealthCheckException : ExceptionBase
{
    public HealthCheckException(string message)
        : base(message)
    {
    }

    public HealthCheckException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
