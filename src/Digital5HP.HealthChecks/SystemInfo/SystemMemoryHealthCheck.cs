namespace Digital5HP.HealthChecks.SystemInfo;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// Memory usage health check.
/// </summary>
/// <remarks>
/// Health check doesn't respect <see cref="HealthStatus"/> set in <see cref="CheckOptions"/> and uses the following policies:
/// <list type="bullet">
/// <item>
/// <term><see cref="HealthStatus.Healthy"/></term>
/// <description> &lt;= 80%</description>
/// </item>
/// <item>
/// <term><see cref="HealthStatus.Degraded"/></term>
/// <description> &gt; 80% and  &lt;= 90%</description>
/// </item>
/// <item>
/// <term><see cref="HealthStatus.Healthy"/></term>
/// <description> &gt; 90%</description>
/// </item>
/// </list>
/// </remarks>
public class SystemMemoryHealthCheck : IHealthCheck
{
    private const int HEALTHY_THRESHOLD = 80;
    private const int DEGRADED_THRESHOLD = 90;

    private static readonly string[] UnitsOfData =
    [
        "KiB", "MiB", "GiB", "TiB",
    ];

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
                                                    CancellationToken cancellationToken = default)
    {
        try
        {
            // retrieve metrics
            var systemInfo = SystemInfoFactory.Create();
            var (total, used, free) = systemInfo.GetMemoryUsage();

            // determine health status
            var status = HealthStatus.Healthy;

            if (total != 0)
            {
                // calculate percent used
                var percentUsed = 100 * (double)used / total;

                if (percentUsed > HEALTHY_THRESHOLD)
                {
                    status = HealthStatus.Degraded;
                }

                if (percentUsed > DEGRADED_THRESHOLD)
                {
                    status = HealthStatus.Unhealthy;
                }
            }

            // create data dictionary
            var data = new Dictionary<string, object>(StringComparer.Ordinal)
                       {
                           {
                               "Total", total
                           },
                           {
                               "Used", used
                           },
                           {
                               "Free", free
                           }
                       };

            return Task.FromResult(
                new HealthCheckResult(
                    status,
                    $"Total / Used / Free: {ToUnitOfData(total)} / {ToUnitOfData(used)} / {ToUnitOfData(free)}",
                    data: data));
        }
        catch (Exception ex)
        {
            return Task.FromResult(
                new HealthCheckResult(
                    HealthStatus.Unhealthy,
                    "Failed to check system memory usage.",
                    ex));
        }
    }

    private static string ToUnitOfData(ulong bytes)
    {
        var val = (double)bytes;
        var current = "B"; // start with bytes

        foreach (var unit in UnitsOfData)
        {
            if (val < 1024)
                break;

            current = unit;
            val /= 1024;
        }

        return string.Join(" ", val.ToString("##,0.##", CultureInfo.InvariantCulture), current);
    }
}
