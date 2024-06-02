﻿namespace Digital5HP.Logging.Analyzers.Internals;

using System;
using System.Globalization;
using System.Text;

using Microsoft.CodeAnalysis.CSharp;

#pragma warning disable RS0010
/// <summary>
/// Displays a value in the C# style.
/// </summary>
/// <remarks>
/// Separate from <see cref="T:Microsoft.CodeAnalysis.CSharp.SymbolDisplay"/> because we want to link this functionality into
/// the Formatter project and we don't want it to be public there.
/// </remarks>
/// <seealso cref="T:Microsoft.CodeAnalysis.VisualBasic.Symbols.ObjectDisplay"/>
#pragma warning restore RS0010
internal class ObjectDisplay
{
    /// <summary>
    /// Returns true if the character should be replaced and sets
    /// <paramref name="replaceWith"/> to the replacement text.
    /// </summary>
    private static bool TryReplaceChar(char c, out string? replaceWith)
    {
        replaceWith = null;
        switch (c)
        {
            case '\\':
                replaceWith = "\\\\";
                break;
            case '\0':
                replaceWith = "\\0";
                break;
            case '\a':
                replaceWith = "\\a";
                break;
            case '\b':
                replaceWith = "\\b";
                break;
            case '\f':
                replaceWith = "\\f";
                break;
            case '\n':
                replaceWith = "\\n";
                break;
            case '\r':
                replaceWith = "\\r";
                break;
            case '\t':
                replaceWith = "\\t";
                break;
            case '\v':
                replaceWith = "\\v";
                break;
        }

        if (replaceWith != null)
        {
            return true;
        }

        if (!NeedsEscaping(CharUnicodeInfo.GetUnicodeCategory(c))) return false;

        replaceWith = "\\u" + ((int)c).ToString("x4", CultureInfo.InvariantCulture);
        return true;

    }

    private static bool NeedsEscaping(UnicodeCategory category)
    {
        switch (category)
        {
            case UnicodeCategory.Control:
            case UnicodeCategory.OtherNotAssigned:
            case UnicodeCategory.ParagraphSeparator:
            case UnicodeCategory.LineSeparator:
            case UnicodeCategory.Surrogate:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Returns a C# string literal with the given value.
    /// </summary>
    /// <param name="value">The value that the resulting string literal should have.</param>
    /// <param name="useQuotes"></param>
    /// <param name="escapeNonPrintable"></param>
    /// <returns>A string literal with the given value.</returns>
    /// <remarks>
    /// Optionally escapes non-printable characters.
    /// </remarks>
    public static string FormatLiteral(string value, bool useQuotes, bool escapeNonPrintable)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        const char QUOTE = '"';

        var builder = new StringBuilder();
        var isVerbatim = useQuotes && !escapeNonPrintable && ContainsNewLine(value);

        if (useQuotes)
        {
            if (isVerbatim)
            {
                builder.Append('@');
            }
            builder.Append(QUOTE);
        }

        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (escapeNonPrintable && CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.Surrogate)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(value, i);
                if (category == UnicodeCategory.Surrogate)
                {
                    // an unpaired surrogate
                    builder.Append("\\u" + ((int)c).ToString("x4", CultureInfo.InvariantCulture));
                }
                else if (NeedsEscaping(category))
                {
                    // a surrogate pair that needs to be escaped
                    var unicode = char.ConvertToUtf32(value, i);
                    builder.Append("\\U" + unicode.ToString("x8", CultureInfo.InvariantCulture));
                    i++; // skip the already-encoded second surrogate of the pair
                }
                else
                {
                    // copy a printable surrogate pair directly
                    builder.Append(c);
                    builder.Append(value[++i]);
                }
            }
            else if (escapeNonPrintable && TryReplaceChar(c, out var replaceWith))
            {
                builder.Append(replaceWith);
            }
            else if (useQuotes && c == QUOTE)
            {
                if (isVerbatim)
                {
                    builder.Append(QUOTE);
                    builder.Append(QUOTE);
                }
                else
                {
                    builder.Append('\\');
                    builder.Append(QUOTE);
                }
            }
            else
            {
                builder.Append(c);
            }
        }

        if (useQuotes)
        {
            builder.Append(QUOTE);
        }

        return builder.ToString();
    }

    private static bool ContainsNewLine(string s)
    {
        foreach (var c in s)
        {
            if (SyntaxFacts.IsNewLine(c))
            {
                return true;
            }
        }

        return false;
    }
}
