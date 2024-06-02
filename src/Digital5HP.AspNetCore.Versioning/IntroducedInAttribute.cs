namespace Digital5HP.AspNetCore.Versioning;

using System;

using Asp.Versioning;

/// <summary>
/// Marks a controller or action with which API version it was introduced.
/// </summary>
/// <remarks>
/// This attribute must be applied to a controller, before it can be applied to an action.
/// </remarks>
/// <param name="version">Version API introduced</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class IntroducedInAttribute(ApiVersion version) : Attribute
{
    public ApiVersion Version { get; } = version;

    /// <summary>
    /// Marks a controller or action with which API version it was introduced.
    /// </summary>
    /// <remarks>
    /// This attribute must be applied to a controller, before it can be applied to an action.
    /// </remarks>
    /// <param name="version">Version API introduced</param>
    public IntroducedInAttribute(string version)
        : this(ApiVersionConverter.Convert(version))
    {
    }
}
