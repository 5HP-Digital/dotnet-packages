namespace Digital5HP.HealthChecks.SystemInfo;

using System;
using System.Globalization;
using System.IO;
using System.Linq;

public class LinuxSystemInfo : ISystemInfo
{
    public MemoryUsage GetMemoryUsage()
    {
        string[] output;
        try
        {
            output = File.ReadAllLines("/proc/meminfo");
        }
        catch (Exception ex)
        {
            throw new HealthCheckException("Failed to retrieve memory usage data.", ex);
        }

        var total = GetBytesFromLine(output, "MemTotal:");
        var free = GetBytesFromLine(output, "MemAvailable:");
        var used = total - free;
        var metrics = new MemoryUsage(total, used, free);

        return metrics;

        ulong GetBytesFromLine(string[] memInfo, string token)
        {
            const string KB_TOKEN = "kB";

            var memLine = memInfo.FirstOrDefault(
                line => line.StartsWith(token, StringComparison.Ordinal)
                        && line.EndsWith(KB_TOKEN, StringComparison.Ordinal));

            if (memLine == null)
                return 0;

            var mem = memLine.Replace(token, string.Empty, StringComparison.Ordinal)
                             .Replace(KB_TOKEN, string.Empty, StringComparison.Ordinal)
                             .Trim();

            if (ulong.TryParse(mem, NumberStyles.Number, CultureInfo.InvariantCulture, out var memKb))
                return memKb * 1024;

            return 0;
        }
    }
}
