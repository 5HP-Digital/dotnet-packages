namespace Digital5HP.AspNetCore.Versioning;

public class VersioningOptions
{
    /// <summary>
    /// First API version the web app supports.
    /// </summary>
    /// <remarks>
    /// Any version lower than this will not be available. Defaults to 1.0.
    /// </remarks>
    public string FirstSupportedVersion { get; set; } = "1.0";

    /// <summary>
    /// Current API version for web app.
    /// </summary>
    /// <remarks>
    /// Any version higher than this will not be available.
    /// Defaults to 1.0.
    /// Must be higher or equals to <see cref="FirstSupportedVersion"/>.
    /// </remarks>
    public string CurrentVersion { get; set; } = "1.0";
}
