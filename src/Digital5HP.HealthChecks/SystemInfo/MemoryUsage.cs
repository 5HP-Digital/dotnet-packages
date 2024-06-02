namespace Digital5HP.HealthChecks.SystemInfo;

/// <summary>
/// Memory usage metrics
/// </summary>
/// <param name="Total">Total physical memory (in bytes)</param>
/// <param name="Used">Total used physical memory (in bytes)</param>
/// <param name="Free">Total available physical memory (in bytes)</param>
public record MemoryUsage(ulong Total, ulong Used, ulong Free);
