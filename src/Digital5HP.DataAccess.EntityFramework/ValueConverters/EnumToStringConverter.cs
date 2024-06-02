namespace Digital5HP.DataAccess.EntityFramework.ValueConverters;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class EnumToStringConverter<TEnum>(string defaultValue = null) : ValueConverter<TEnum?, string>(
        region => GetString(region, defaultValue),
        str => GetEnum(str))
    where TEnum : struct, Enum
{
    private static readonly IReadOnlyCollection<(string Text, TEnum Value)> EnumLookup = InitializeLookup();

    private static List<(string, TEnum)> InitializeLookup()
    {
        return typeof(TEnum).GetFields()
                            .Select(
                                 x => new
                                      {
                                          Attribute = x.GetCustomAttribute<EnumMemberAttribute>(),
                                          Field = x
                                      })
                            .Where(x => x.Attribute != null)
                            .Select(
                                 x => (x.Attribute.Value, x.Field.GetValue(null)
                                                                 .ChangeType<TEnum>(CultureInfo.InvariantCulture)))
                            .ToList();
    }

    private static TEnum? GetEnum(string str)
    {
        var hasEnum = EnumLookup.Any(x => x.Text == str);

        if (!hasEnum)
            return null;

        return EnumLookup.Single(x => x.Text == str)
                         .Value;
    }

    private static string GetString(TEnum? enumValue, string defaultValue)
    {
        if (enumValue == null)
            return defaultValue;

        var hasEnum = EnumLookup.Any(x => x.Value.Equals(enumValue.Value));

        return !hasEnum
            ? defaultValue
            : EnumLookup.Single(x => x.Value.Equals(enumValue.Value))
                        .Text;
    }
}
