namespace Digital5HP;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

public static class StringExtensions
{
    private static readonly char[] INITIALS_SEPARATORS =
                         [
                             ' ', ','
                         ];

    /// <summary>
    /// Combines 2 parts of URL into one, separated with '/' character.
    /// </summary>
#pragma warning disable CA1055, CA1054
    public static string UrlCombine(this string url, string path)
#pragma warning restore CA1055, CA1054
    {
        ArgumentNullException.ThrowIfNull(url);
        ArgumentNullException.ThrowIfNull(path);

        return $"{url.TrimEnd('/')}/{path.TrimStart('/')}";
    }

    /// <summary>
    /// Attaches the query arguments dictionary to the existing URL.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
#pragma warning disable CA1055, CA1054
    public static string AddUrlQueryArguments(this string url, IEnumerable<(string key, string value)> queryArguments)
#pragma warning restore CA1054, CA1055
    {
        ArgumentNullException.ThrowIfNull(url);
        ArgumentNullException.ThrowIfNull(queryArguments);

        foreach (var (key, value) in queryArguments)
        {
            var queryString = string.Empty;
            // Is this the first parameter we're adding?
            if (!url.Contains('?', StringComparison.Ordinal))
            {
                queryString = "?";
            }
            // If query argument exist and URL doesn't end with "&", add one...
            else if (!url.EndsWith('&'))
            {
                queryString = "&";
            }

            queryString += $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(value)}";

            url += queryString;
        }

        return url;
    }

    /// <summary>
    /// Returns <see langword="null"/> if <paramref name="str"/> is null or whitespace. Otherwise, returns the provided string.
    /// </summary>
    public static string NullIfWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str) ? null : str;
    
    /// <summary>
    /// Returns whether the <see cref="string"/> provided is null, empty or whitespace characters only.
    /// </summary>
    public static bool IsNullOrWhitespace(this string str) => string.IsNullOrWhiteSpace(str);


    /// <summary>
    /// Gets concatenated Initial from names provided
    /// </summary>
    /// <param name="names">names</param>
    /// <returns>Concatenated Initials</returns>
    public static string Initial(this string[] names)
    {
        static string InitialName(string name) =>
            !string.IsNullOrWhiteSpace(name)
                ? string.Concat(
                    name.Split(INITIALS_SEPARATORS, StringSplitOptions.RemoveEmptyEntries)
                        .Select(
                             n => n[..1]
                                .ToUpper(CultureInfo.InvariantCulture)))
                : string.Empty;

        return string.Concat(names.Select(InitialName));
    }

    /// <summary>
    /// Gets byte array of hex formatted (00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00) string
    /// </summary>
    /// <returns>Byte Array</returns>
    public static byte[] GetBytesFromHex(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return Array.Empty<byte>();
        }

        var splitBytes = str.Split('-');

        if (splitBytes.Any(x => x.Length != 2))
        {
            throw new ArgumentException("String is not in hex format.", nameof(str));
        }

        return splitBytes
              .Select(b => Convert.ToByte(b, 16))
              .ToArray();
    }
}
