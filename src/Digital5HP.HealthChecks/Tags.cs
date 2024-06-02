namespace Digital5HP.HealthChecks;

public static class Tags
{
    /// <summary>
    /// Probe for application startup completed
    /// </summary>
    public const string STARTUP = "startup";

    /// <summary>
    /// Probe for application live
    /// </summary>
    public const string LIVENESS = "liveness";

    /// <summary>
    /// Probe for application readiness for requests or processing
    /// </summary>
    public const string READINESS = "readiness";
}
