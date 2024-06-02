namespace Digital5HP.DataAccess;

public class QueryOptions
{
    /// <summary>
    /// If set to <see langword="true"/>, enables Change Tracking for query results. If set to <see langword="false"/>, disables Change Tracking.
    /// Otherwise, inherits from data context. Defaults to <see langword="false"/> if repository is readable.
    /// </summary>
    /// <remarks>
    /// Supported by EntityFramework.
    /// </remarks>
    public bool? ChangeTracking { get; set; }

    /// <summary>
    /// If set to <see langword="true"/>, splits into multiple SQL queries. If set to <see langword="false"/>, executes as single query.
    /// Otherwise, inherits from data context.
    /// </summary>
    /// <remarks>
    /// Supported by EntityFramework.
    /// </remarks>
    public bool? SplitQuery { get; set; }
}