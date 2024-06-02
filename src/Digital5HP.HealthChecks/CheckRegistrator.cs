namespace Digital5HP.HealthChecks;

using global::System;
using global::System.Collections.Concurrent;
using global::System.Collections.Generic;
using global::System.Linq;

using Microsoft.Extensions.Diagnostics.HealthChecks;

public static class CheckRegistrator
{
    private static readonly
        ConcurrentDictionary<string, (Func<IServiceProvider, IHealthCheck> Factory, CheckOptions Options)> Checks =
            new(StringComparer.Ordinal);

    internal static bool TryAddCheck(Func<IServiceProvider, IHealthCheck> factory, CheckOptions options)
    {
        return Checks.TryAdd(options.Name, (factory, options));
    }

    public static IEnumerable<HealthCheckRegistration> Registrations =>
        Checks.Values.Select(
            tuple => new HealthCheckRegistration(
                tuple.Options.Name,
                tuple.Factory,
                tuple.Options.FailureStatus,
                tuple.Options.Tags,
                tuple.Options.Timeout));
}
