namespace Digital5HP.AspNetCore.Versioning;

using System;
using System.Globalization;

using Asp.Versioning;

public static class ApiVersionConverter
{
    public static ApiVersion Convert(string version)
    {
        ArgumentNullException.ThrowIfNull(version);

        var versionParts = version.Split('.');

        int? minor = null;

        if (versionParts.Length == 2)
        {
            if (int.TryParse(versionParts[1], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var m))
                minor = m;
            else
                throw new InvalidApiVersionSyntaxException("Invalid API version syntax.");
        }

        if (versionParts.Length is 0 or > 2
            || !int.TryParse(versionParts[0], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var major))
        {
            throw new InvalidApiVersionSyntaxException("Invalid API version syntax.");
        }

        return new ApiVersion(major, minor);
    }
}
