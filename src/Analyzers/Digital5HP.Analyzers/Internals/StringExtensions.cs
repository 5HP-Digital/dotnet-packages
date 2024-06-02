namespace Digital5HP.Analyzers.Internals;

using System;
using System.Runtime.InteropServices;

internal static class StringExtensions
{
    private static readonly Func<char, char> ToLower = char.ToLower;
    private static readonly Func<char, char> ToUpper = char.ToUpper;

#if NETSTANDARD2_0
    public static bool Contains(this string str, string value, StringComparison stringComparison)
    {
        return str.IndexOf(value, stringComparison) >= 0;
    }

    public static bool Contains(this string str, char value, StringComparison stringComparison)
    {
        return str.IndexOf(value, stringComparison) >= 0;
    }

    public static int IndexOf(this string str, char value, StringComparison stringComparison)
    {
        if (stringComparison == StringComparison.Ordinal)
            return str.IndexOf(value);

        return str.IndexOf(value.ToString(), stringComparison);
    }
#endif

    public static string ReplaceOrdinal(this string str, string oldValue, string newValue)
    {
#if NET5_0_OR_GREATER
        return str.Replace(oldValue, newValue, StringComparison.Ordinal);
#else
        return str.Replace(oldValue, newValue);
#endif
    }
    public static LineSplitEnumerator SplitLines(this string str) => new(str.AsSpan());


    public static string? ToPascalCase(
        this string? shortName,
        bool trimLeadingTypePrefix = true)
    {
        return ConvertCase(shortName, trimLeadingTypePrefix, ToUpper);
    }

    public static string? ToCamelCase(
        this string? shortName,
        bool trimLeadingTypePrefix = true)
    {
        return ConvertCase(shortName, trimLeadingTypePrefix, ToLower);
    }

    public static bool LooksLikeInterfaceName(this string name)
    {
        return name.Length >= 3 && name[0] == 'I' && char.IsUpper(name[1]) && char.IsLower(name[2]);
    }

    public static bool LooksLikeTypeParameterName(this string name)
    {
        return name.Length >= 3 && name[0] == 'T' && char.IsUpper(name[1]) && char.IsLower(name[2]);
    }

    private static string? ConvertCase(
        this string? shortName,
        bool trimLeadingTypePrefix,
        Func<char, char> convert)
    {
        // Special case the common .NET pattern of "IGoo" as a type name.  In this case we
        // want to generate "goo" as the parameter name.
        if (!string.IsNullOrEmpty(shortName))
        {
            if (trimLeadingTypePrefix && (shortName.LooksLikeInterfaceName() || shortName.LooksLikeTypeParameterName()))
            {
                return convert(shortName[1]) + shortName.Substring(2);
            }

            if (convert(shortName[0]) != shortName[0])
            {
                return convert(shortName[0]) + shortName.Substring(1);
            }
        }

        return shortName;
    }

    public static string? NullWhenWhitespace(this string? source)
    {
        return string.IsNullOrWhiteSpace(source) ? null : source;
    }

    [StructLayout(LayoutKind.Auto)]
    public ref struct LineSplitEnumerator(ReadOnlySpan<char> str)
    {
        private ReadOnlySpan<char> _str = str;

        public LineSplitEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            if (this._str.Length == 0)
                return false;

            var span = this._str;
            var index = span.IndexOfAny('\r', '\n');
            if (index == -1)
            {
                this._str = ReadOnlySpan<char>.Empty;
                this.Current = new LineSplitEntry(span, ReadOnlySpan<char>.Empty);
                return true;
            }

            if (index < span.Length - 1 && span[index] == '\r')
            {
                var next = span[index + 1];
                if (next == '\n')
                {
                    this.Current = new LineSplitEntry(span.Slice(0, index), span.Slice(index, 2));
                    this._str = span.Slice(index + 2);
                    return true;
                }
            }

            this.Current = new LineSplitEntry(span.Slice(0, index), span.Slice(index, 1));
            this._str = span.Slice(index + 1);
            return true;
        }

        public LineSplitEntry Current { get; private set; } = default;
    }

    [StructLayout(LayoutKind.Auto)]
    public readonly ref struct LineSplitEntry(ReadOnlySpan<char> line, ReadOnlySpan<char> separator)
    {
        public ReadOnlySpan<char> Line { get; } = line;
        public ReadOnlySpan<char> Separator { get; } = separator;

        public void Deconstruct(out ReadOnlySpan<char> line, out ReadOnlySpan<char> separator)
        {
            line = this.Line;
            separator = this.Separator;
        }

        public static implicit operator ReadOnlySpan<char>(LineSplitEntry entry) => entry.Line;
    }
}
