namespace Digital5HP;

using System.Linq;

/// <summary>
/// Helper providing methods for manipulating strings
/// </summary>
public static class StringHelper
{
    /// <summary>
    /// Concatenates the specified strings that are not null or whitespace, using the specified separator between each string.
    /// </summary>
    public static string Join(string separator, params string[] parts) => string.Join(
        separator,
        parts.Where(s => !string.IsNullOrWhiteSpace(s)));
}
