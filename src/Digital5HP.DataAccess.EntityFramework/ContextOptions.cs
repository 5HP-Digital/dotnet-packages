namespace Digital5HP.DataAccess.EntityFramework;

public class ContextOptions
{
    /// <summary>
    /// Enables logging of queries.
    /// </summary>
    public bool EnableLogging { get; set; }

    /// <summary>
    /// Connection string to data store.
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Globally disables Change Tracking using proxies.
    /// </summary>
    public bool DisableChangeTracking { get; set; }

    /// <summary>
    /// Globally enables split queries (performs query for each included collection, instead of JOINs).
    /// </summary>
    public bool EnableSplitQuery { get; set; }

    /// <summary>
    /// Throws exception when EF  model validation identifies warnings.
    /// </summary>
    public bool TreatWarningsAsErrors { get; set; }
}
