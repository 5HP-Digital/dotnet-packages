namespace Digital5HP;

using System;

public static class UriExtensions
{
    /// <summary>
    /// Combines the current <see cref="Uri"/> with the provided relative path.
    /// </summary>
    /// <remarks>
    /// New <see cref="Uri"/> object is created.
    /// </remarks>
    /// <param name="uri"></param>
    /// <param name="path">Relative path to combine</param>
    /// <returns>Combined <see cref="Uri"/>.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="uri"/> is null.</exception>
    public static Uri Combine(this Uri uri, string path)
    {
        ArgumentNullException.ThrowIfNull(uri);

        return string.IsNullOrEmpty(path)
            ? uri
            : new Uri(
                uri.ToString()
                   .UrlCombine(path.TrimStart('/')));
    }
}