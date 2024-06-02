namespace Digital5HP;

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

public static class ObjectExtensions
{
    /// <summary>
    /// Converts the object provided to type <paramref name="toType"/> using the culture info provided.
    /// </summary>
    public static object ChangeType(this object value, Type toType, CultureInfo cultureInfo)
    {
        while (true)
        {
            ArgumentNullException.ThrowIfNull(toType);

            if (value == null) return toType.GetDefault();

            // extract underlying type if T is Nullable<>
            if (toType.IsGenericType && toType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                toType = Nullable.GetUnderlyingType(toType);
            }

            Debug.Assert(toType != null, nameof(toType) + " != null");

            // if value inherits from T, return value.
            if (toType.IsInstanceOfType(value)) return value;

            if (value is string str)
            {
                // if string is empty return default
                if (string.IsNullOrEmpty(str) && toType != typeof(string))
                {
                    return toType.GetDefault();
                }

                // convert string to Guid
                if (toType == typeof(Guid))
                {
                    value = new Guid(str);
                    continue;
                }

                // parse string to enum
                if (toType.IsEnum)
                {
                    return Enum.Parse(toType, str);
                }

                // boolean special case for converting a numeric string
                if (toType == typeof(bool)
                    && double.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands, cultureInfo, out var d))
                {
                    value = d;
                    continue;
                }
            }
            // toType is a string, use Convert.ToString
            else if (toType == typeof(string))
            {
                value = Convert.ToString(value, cultureInfo);
                continue;
            }

            // check for coercion operators
            if(value.TryConvert(toType, out var result)) return result;

            // convert to Enum
            if(toType.IsEnum) return Enum.ToObject(toType, value);

            // use IConvertible
            if(typeof(IConvertible).IsAssignableFrom(toType)) return Convert.ChangeType(value, toType, cultureInfo);

            return value;
        }
    }

    /// <summary>
    /// Converts the object provided to type <typeparamref name="T"/> using the culture info provided.
    /// </summary>
    public static T ChangeType<T>(this object value, CultureInfo cultureInfo)
    {
        var toType = typeof(T);

        return (T)ChangeType(value, toType, cultureInfo);
    }

    /// <summary>
    /// Converts the object provided to type <typeparamref name="T"/> using the <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    public static T ChangeType<T>(this object value)
    {
        return ChangeType<T>(value, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts the object provided to type <paramref name="toType"/> using the <see cref="CultureInfo.CurrentCulture"/>.
    /// </summary>
    public static object ChangeType(this object value, Type toType)
    {
        return ChangeType(value, toType, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Attempts to convert the object provided to type <typeparamref name="T"/> using the <paramref name="cultureInfo"/> provided and sets parameter <paramref name="dest"/>. If succeeds, returns <see langword="true" />, otherwise returns <see langword="false" />.
    /// </summary>
    public static bool TryChangeType<T>(this object value, CultureInfo cultureInfo, out T dest)
    {
        try
        {
            dest = ChangeType<T>(value, cultureInfo);
            return true;
        }
        catch (Exception)
        {
            dest = default;
            return false;
        }
    }

    /// <summary>
    /// Attempts to convert the object provided to type <typeparamref name="T"/> and sets parameter <paramref name="dest"/>. If succeeds, returns <see langword="true" />, otherwise returns <see langword="false" />.
    /// </summary>
    public static bool TryChangeType<T>(this object value, out T dest)
    {
        return TryChangeType(value, CultureInfo.InvariantCulture, out dest);
    }

    /// <summary>
    /// Returns the value or default if there is no value
    /// </summary>
    public static TValue? GetValueOrDefault<T, TValue>(this T value,
                                                       Func<T, TValue> valueSelector,
                                                       TValue? defaultValue = default)
        where TValue : struct
    {
        ArgumentNullException.ThrowIfNull(valueSelector);

        return value != null ? valueSelector(value) : defaultValue;
    }

    /// <summary>
    /// Returns Default value for <paramref name="type"/>
    /// </summary>
    public static object GetDefault(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }

    /// <summary>
    /// Returns null if passed value matches any of params.
    /// </summary>
    public static T? NullIf<T>(this T? value, params T[] values)
        where T : struct
    {
        ArgumentNullException.ThrowIfNull(values);

        if (value == null) return null;
        if (values.Length == 0) return value;

        return !values.Contains(value.Value) ? value : null;
    }

    /// <summary>
    /// Returns null if passed value matches any of params.
    /// </summary>
    public static T? NullIf<T>(this T value, params T[] values)
        where T : struct
    {
        return NullIf((T?)value, values);
    }

    /// <summary>
    /// Tries to convert object to type <typeparamref name="T"/> using coercion operators.
    /// </summary>
    /// <param name="from">Value to convert.</param>
    /// <param name="result">Converted value.</param>
    /// <typeparam name="T">Type of convert to.</typeparam>
    /// <returns><see langword="true"/> if succeeds, otherwise <see langword="false"/>.</returns>
    public static bool TryConvert<T>(this object from, out T result)
    {
        if (!TryConvert(from, typeof(T), out var tmp))
        {
            result = default;
            return false;
        }

        result = (T)tmp;
        return true;
    }

    /// <summary>
    /// Tries to convert object to type <paramref name="toType"/> using coercion operators.
    /// </summary>
    /// <param name="from">Value to convert.</param>
    /// <param name="toType">Type of convert to.</param>
    /// <param name="result">Converted value.</param>
    /// <returns><see langword="true"/> if succeeds, otherwise <see langword="false"/>.</returns>
    public static bool TryConvert(this object from, Type toType, out object result)
    {
        try
        {
            // Throws an exception if there is no conversion from fromType to toType
            var convert = Expression.Convert(Expression.Constant(from), toType);

            result = convert.Method!.Invoke(
                null,
                new[]
                {
                    from,
                });

            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}
