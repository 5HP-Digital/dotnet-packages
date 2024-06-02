namespace Digital5HP.AspNetCore.Versioning;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Asp.Versioning;

internal class ApiVersionContainer
{
    private readonly ApiVersion currentVersion;
    private readonly Lazy<IReadOnlyList<ApiVersion>> allVersionsLazy;

    private static ImmutableList<ApiVersion> GetAllVersions(ApiVersion start, ApiVersion current) => ApiVersionCollection
                                                         .Instance.Where(
                                                              v => v >= start && v <= current)
                                                         .ToImmutableList();

    internal ApiVersionContainer(ApiVersion startApiVersion, ApiVersion currentApiVersion)
    {
        if (currentApiVersion < startApiVersion)
            throw new ArgumentException(
                $"{nameof(currentApiVersion)} must be >= {nameof(startApiVersion)}",
                nameof(currentApiVersion));

        this.currentVersion = currentApiVersion;

        this.allVersionsLazy =
            new Lazy<IReadOnlyList<ApiVersion>>(() => GetAllVersions(startApiVersion, currentApiVersion));
    }

    internal ApiVersion[] GetSupportedVersions(ApiVersion introducedIn, ApiVersion removedAsOf = null)
    {
        if (introducedIn > this.currentVersion)
            return Array.Empty<ApiVersion>();

        if (removedAsOf is null)
            return
                   [
                       this.currentVersion,
                   ];

        if (introducedIn > removedAsOf)
            throw new InvalidOperationException(
                $"Cannot remove an API version ({removedAsOf}) before it has been introduced ({introducedIn}).");

        if (introducedIn == removedAsOf)
            throw new InvalidOperationException(
                $"Cannot remove an API version ({removedAsOf}) in the same version it has been introduced ({introducedIn}).");

        if (removedAsOf > this.currentVersion)
            return
                   [
                       this.currentVersion,
                   ];

        return Array.Empty<ApiVersion>();
    }

    internal ApiVersion[] GetDeprecatedVersions(ApiVersion introducedIn, ApiVersion removedAsOf = null)
    {
        ArgumentNullException.ThrowIfNull(introducedIn);

        removedAsOf ??= this.currentVersion;

        return this.allVersionsLazy.Value.Where(v => v >= introducedIn && v < removedAsOf)
                   .ToArray();
    }
}
