namespace Digital5HP;

using System;
using System.Collections.Generic;
using System.Linq;

using Digital5HP.Lookups;

public static class LookupObjectExtensions
{
    /// <summary>Returns the only element of a sequence that matched the specified <paramref name="code"/> or a
    /// default value if no such element exists; query is case-insensitive; this method throws an exception if
    /// more than one element satisfies the condition.</summary>
    /// <returns>The single element of the input sequence that satisfies the condition, or default if no such element is found.</returns>
    /// <param name="source">An <see cref="System.Collections.Generic.IEnumerable{T}" /> to return a single element from.</param>
    /// <param name="code">The code to match.</param>
    /// <typeparam name="TLookupObject">The Lookup object type of the elements of <paramref name="source" />.</typeparam>
    /// <exception cref="System.ArgumentNullException"> <paramref name="source" /> is null.</exception>
    public static TLookupObject ByCode<TLookupObject>(this IEnumerable<TLookupObject> source, string code)
        where TLookupObject : ILookupObject
    {
        return source.SingleOrDefault(o => o.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
    }

    public static TLookupObject ByEnumValue<TLookupObject, TEnum>(this IEnumerable<TLookupObject> source, TEnum value)
        where TEnum : Enum
        where TLookupObject : ILookupObject<TEnum>
    {
        return source.SingleOrDefault(o => o.EnumValue.Equals(value));
    }

    public static IEnumerable<TLookupObject> ByEnumValues<TLookupObject, TEnum>(
        this IEnumerable<TLookupObject> source,
        params TEnum[] values)
        where TEnum : Enum
        where TLookupObject : ILookupObject<TEnum>
    {
        return source.Where(o => values.Contains(o.EnumValue));
    }
}
