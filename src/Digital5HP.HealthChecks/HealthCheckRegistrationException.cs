namespace Digital5HP.HealthChecks;

using global::System;

public class HealthCheckRegistrationException : ExceptionBase
{
    public HealthCheckRegistrationException(string message)
        : base(message)
    {
    }

    public HealthCheckRegistrationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
