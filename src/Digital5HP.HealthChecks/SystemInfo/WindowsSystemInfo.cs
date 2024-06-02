namespace Digital5HP.HealthChecks.SystemInfo;

using System;
using System.Diagnostics;
using System.Globalization;

public class WindowsSystemInfo : ISystemInfo
{
    public MemoryUsage GetMemoryUsage()
    {
        var info = new ProcessStartInfo
        {
            FileName = "wmic",
            Arguments = "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value",
            RedirectStandardOutput = true
        };

        using var process = Process.Start(info);

        if (process == null)
            throw new HealthCheckException("Failed to retrieve memory usage data from wmic process.");

        var output = process.StandardOutput.ReadToEnd();

        var lines = output.Trim()
                          .Split("\n");
        var freeMemoryParts = lines[0]
           .Split("=", StringSplitOptions.RemoveEmptyEntries);
        var totalMemoryParts = lines[1]
           .Split("=", StringSplitOptions.RemoveEmptyEntries);

        var total = ulong.Parse(totalMemoryParts[1], CultureInfo.InvariantCulture) * 1024;
        var free = ulong.Parse(freeMemoryParts[1], CultureInfo.InvariantCulture) * 1024;
        var used = total - free;

        return new MemoryUsage(total, used, free);
    }
}
