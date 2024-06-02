namespace Digital5HP.HealthChecks.SystemInfo;

using System;

public static class SystemInfoFactory
{
    public static ISystemInfo Create()
    {
        if (OperatingSystem.IsLinux())
        {
            return new LinuxSystemInfo();
        }

        if (OperatingSystem.IsMacOS())
        {
            return new OsxSystemInfo();
        }

        if (OperatingSystem.IsWindows())
        {
            return new WindowsSystemInfo();
        }

        throw new NotSupportedException("OS does not support system metrics.");
    }
}
