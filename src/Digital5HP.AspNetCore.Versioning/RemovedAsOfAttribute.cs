namespace Digital5HP.AspNetCore.Versioning;

using System;

using Asp.Versioning;

/// <summary>
/// Marks a controller or action with which API version it was removed.
/// </summary>
/// <remarks>
/// The <see cref="IntroducedInAttribute"/> must be applied to the controller, before this attribute can be applied.
/// </remarks>
/// <param name="version">Version API introduced</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RemovedAsOfAttribute(ApiVersion version) : Attribute
{
    public ApiVersion Version { get; } = version;

    /// <summary>
    /// Marks a controller or action with which API version it was removed.
    /// </summary>
    /// <remarks>
    /// The <see cref="IntroducedInAttribute"/> must be applied to the controller, before this attribute can be applied.
    /// </remarks>
    /// <param name="version">Version API introduced</param>
    public RemovedAsOfAttribute(string version)
        : this(ApiVersionConverter.Convert(version))
    {
    }
}
