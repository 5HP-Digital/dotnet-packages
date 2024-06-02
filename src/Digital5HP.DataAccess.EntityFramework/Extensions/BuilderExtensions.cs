namespace Digital5HP.DataAccess.EntityFramework.Extensions;

using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Digital5HP.DataAccess.EntityFramework.ValueConverters;

public static class BuilderExtensions
{
    public static PropertyBuilder HasLegacyDateTime(this PropertyBuilder builder,
                                                    bool isRequired = true,
                                                    bool isMax = false,
                                                    string columnType = "datetime")
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasColumnType(columnType)
               .HasConversion(new LegacyDateTimeValueConverter(isMax));
        if (isRequired)
        {
            builder.IsRequired();
        }
        return builder;
    }

    public static PropertyBuilder HasLegacyDateTime(this PropertyBuilder builder,
                                                    DateTime defaultValue,
                                                    bool isRequired = true,
                                                    string columnType = "datetime")
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasColumnType(columnType)
               .HasConversion(new LegacyDateTimeValueConverter(defaultValue));
        if (isRequired)
            builder.IsRequired();
        return builder;
    }

    public static PropertyBuilder<string> ValueStoredAsNotNull(this PropertyBuilder<string> propertyBuilder)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        propertyBuilder.HasConversion(new NullToEmptyStringConverter());

        return propertyBuilder;
    }

    public static PropertyBuilder<TIn?> ValueStoredAsNotNull<TIn, TOut>(this PropertyBuilder<TIn?> propertyBuilder,
                                                                        TOut defaultValue)
        where TIn : struct
        where TOut : struct
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        propertyBuilder.HasConversion(new NullableToDefaultConverter<TIn, TOut>(defaultValue));
        return propertyBuilder;
    }

    public static PropertyBuilder<bool?> ValueStoredAsInt32(this PropertyBuilder<bool?> propertyBuilder)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        propertyBuilder.HasConversion(new TriStateBooleanConverter());
        return propertyBuilder;
    }

    public static PropertyBuilder<bool> ValueStoredAsInt32(this PropertyBuilder<bool> propertyBuilder)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        propertyBuilder.HasConversion(new BooleanToIntConverter());
        return propertyBuilder;
    }

    public static PropertyBuilder<decimal> ValueStoredAsPercentage(this PropertyBuilder<decimal> propertyBuilder)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);
        propertyBuilder.HasConversion(new DecimalToPercentageConverter());
        return propertyBuilder;
    }

    public static PropertyBuilder<int?> ValueStoredAsString(this PropertyBuilder<int?> propertyBuilder,
                                                            bool isRequired = false)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        propertyBuilder.HasConversion(new Int32ToStringConverter(isRequired));
        return propertyBuilder;
    }

    public static PropertyBuilder<string> ValueStoredAsInt64(this PropertyBuilder<string> propertyBuilder,
                                                             bool isRequired = false)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        propertyBuilder.HasConversion(new StringToInt64Converter(isRequired));
        return propertyBuilder;
    }

    public static PropertyBuilder<TEnum?> ValueStoredAsString<TEnum>(
        this PropertyBuilder<TEnum?> propertyBuilder,
        string defaultValue = null)
        where TEnum : struct, Enum
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        propertyBuilder.HasConversion(new EnumToStringConverter<TEnum>(defaultValue));
        return propertyBuilder;
    }
    
    
    public static PropertyBuilder<IDictionary<string, string>> ValueStoredAsJson(
        this PropertyBuilder<IDictionary<string, string>> propertyBuilder)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        propertyBuilder.HasConversion(new DictionaryToJsonConverter());

        return propertyBuilder;
    }
}
