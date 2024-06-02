namespace Digital5HP.HealthChecks.SystemInfo;

using System;
using System.Runtime.InteropServices;

public class OsxSystemInfo : ISystemInfo
{
    [DllImport("libc", CharSet = CharSet.Unicode)]
#pragma warning disable CA5392
    private static extern int sysctlbyname(string name, out nint oldp, ref nint oldlenp, nint newp, nint newlen);
#pragma warning restore CA5392

    public MemoryUsage GetMemoryUsage()
    {
        var sizeOfLineSize = (nint)nint.Size;

        ulong total = 0;
        if (sysctlbyname("hw.memsize", out var lineSize, ref sizeOfLineSize, nint.Zero, nint.Zero) == 0)
        {
            total = (ulong)lineSize.ToInt64();
        }

        // TODO: retrieve RAM used/available on OSX (low priority)
        return new MemoryUsage(total, 0, 0);
    }
}
