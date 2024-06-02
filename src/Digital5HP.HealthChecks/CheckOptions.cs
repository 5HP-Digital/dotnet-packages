namespace Digital5HP.HealthChecks;

using global::System;
using global::System.Collections.Generic;

using Microsoft.Extensions.Diagnostics.HealthChecks;

public class CheckOptions
{
    /// <summary>
    /// The name of health check.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The <see cref="HealthStatus"/> that should be reported when the health check fails. If provided value is <see langword="null"/>, <see cref="HealthStatus.Unhealthy"/> will be reported.
    /// </summary>
    public HealthStatus? FailureStatus { get; set; }

    /// <summary>
    ///  A list of tags that can be used to filter health checks.
    /// </summary>
    public IEnumerable<string> Tags { get; set; }

    /// <summary>
    /// An optional timeout of the check.
    /// </summary>
    public TimeSpan? Timeout { get; set; }
}
