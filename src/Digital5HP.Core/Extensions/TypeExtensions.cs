namespace Digital5HP;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class TypeExtensions
{
    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="type"/> can be set to <see langword="null"/>. Otherwise, returns <see langword="false"/>
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> cannot be null.</exception>
    public static bool CanBeNull(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
    }

    /// <summary>
    /// Determines if <paramref name="type"/> derives from <paramref name="genericType"/>.
    /// </summary>
    /// <remarks>
    /// This method does not check for implemented or inherited interfaces. For checking interfaces see <see cref="IsAssignableToGenericType"/>.
    /// </remarks>
    /// <param name="type">Type to check.</param>
    /// <param name="genericType">Generic type (for example, <see cref="List{T}"/>).</param>
    /// <returns></returns>
    public static bool IsSubclassOfGenericType(this Type type, Type genericType)
    {
        while (type != null && type != typeof(object))
        {
            var current = type.IsGenericType ? type.GetGenericTypeDefinition() : type;

            if (genericType == current)
                return true;

            type = type.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Determines if <paramref name="givenType"/> derives from <paramref name="genericType"/>.
    /// </summary>
    /// <remarks>
    /// This method checks for implemented or inherited interfaces.
    /// </remarks>
    /// <param name="givenType">Type to check.</param>
    /// <param name="genericType">Generic type, including interface (for example, <see cref="ICollection{T}"/>).</param>
    /// <returns></returns>
    public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
    {
        ArgumentNullException.ThrowIfNull(givenType);
        ArgumentNullException.ThrowIfNull(genericType);

        var interfaceTypes = givenType.GetInterfaces();
        if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
        {
            return true;
        }

        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            return true;

        var baseType = givenType.BaseType;
        return baseType != null && IsAssignableToGenericType(baseType, genericType);
    }

    /// <summary>
    /// Retrieves the generic arguments of <paramref name="genericType"/> if derived by <paramref name="type"/>. Returns <see langword="null"/> if not derived.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <param name="genericType">Generic type (for example, <see cref="List{T}"/>).</param>
    /// <returns></returns>
    public static Type[] GetGenericArgumentsOfSubclass(this Type type, Type genericType)
    {
        while (type != null && type != typeof(object))
        {
            var current = type.IsGenericType ? type.GetGenericTypeDefinition() : type;

            if (genericType == current)
                return type.GetGenericArguments();

            type = type.BaseType;
        }

        return null;
    }

    /// <summary>
    /// Determines whether the current type inherits from the generic interface provided.
    /// </summary>
    /// <param name="type">Current type.</param>
    /// <param name="interfaceType">Generic interface.</param>
    public static bool ImplementsGenericInterface(this Type type, Type interfaceType)
        => type.IsGenericType(interfaceType)
           || type.GetTypeInfo()
                  .ImplementedInterfaces.Any(i => i.IsGenericType(interfaceType));

    /// <summary>
    /// Determines if the current type inherits from the generic type provided.
    /// </summary>
    /// <param name="type">Current type.</param>
    /// <param name="genericType">The generic type.</param>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> cannot be null.</exception>
    public static bool IsGenericType(this Type type, Type genericType)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.GetTypeInfo()
                   .IsGenericType
               && type.GetGenericTypeDefinition() == genericType;
    }

    /// <summary>
    /// Retrieves a non-public method info by its name. This method will search all base classes.
    /// </summary>
    /// <remarks>
    /// Non-public are private, protected and internal.
    /// </remarks>
    public static MethodInfo GetNonPublicMethodInfo(this Type type, string name, bool isStatic = false)
    {
        ArgumentNullException.ThrowIfNull(type);

        var bindingAttr = BindingFlags.NonPublic | (isStatic ? BindingFlags.Static : BindingFlags.Instance);

        var methodInfo = type.GetMethod(name, bindingAttr);

        if (methodInfo == null && type.BaseType != null)
            return type.BaseType.GetNonPublicMethodInfo(name, isStatic);

        return methodInfo;
    }

    /// <summary>
    /// Retrieves a non-public field info by its name. This method will search all base classes.
    /// </summary>
    /// <remarks>
    /// Non-public are private, protected and internal.
    /// </remarks>
    public static FieldInfo GetNonPublicFieldInfo(this Type type, string name, bool isStatic = false)
    {
        ArgumentNullException.ThrowIfNull(type);

        var bindingAttr = BindingFlags.NonPublic | (isStatic ? BindingFlags.Static : BindingFlags.Instance);

        var fieldInfo = type.GetField(name, bindingAttr);

        if (fieldInfo == null && type.BaseType != null)
            return type.BaseType.GetNonPublicFieldInfo(name, isStatic);

        return fieldInfo;
    }

    /// <summary>
    /// Retrieves a non-public property info by its name. This method will search all base classes.
    /// </summary>
    /// <remarks>
    /// Non-public are private, protected and internal.
    /// </remarks>
    public static PropertyInfo GetNonPublicPropertyInfo(this Type type, string name, bool isStatic = false)
    {
        ArgumentNullException.ThrowIfNull(type);

        var bindingAttr = BindingFlags.NonPublic | (isStatic ? BindingFlags.Static : BindingFlags.Instance);

        var propInfo = type.GetProperty(name, bindingAttr);

        if (propInfo == null && type.BaseType != null)
            return type.BaseType.GetNonPublicPropertyInfo(name, isStatic);

        return propInfo;
    }

    /// <summary>
    /// Retrieves a public method info by its name. This method will search all base classes.
    /// </summary>
    /// <remarks>
    /// Non-public are private, protected and internal.
    /// </remarks>
    public static MethodInfo GetPublicMethodInfo(this Type type, string name, bool isStatic = false)
    {
        ArgumentNullException.ThrowIfNull(type);

        var bindingAttr = BindingFlags.Public | (isStatic ? BindingFlags.Static : BindingFlags.Instance);

        var methodInfo = type.GetMethod(name, bindingAttr);

        if (methodInfo == null && type.BaseType != null)
            return type.BaseType.GetPublicMethodInfo(name, isStatic);

        return methodInfo;
    }

    /// <summary>
    /// Retrieves a public field info by its name. This method will search all base classes.
    /// </summary>
    public static FieldInfo GetPublicFieldInfo(this Type type, string name, bool isStatic = false)
    {
        ArgumentNullException.ThrowIfNull(type);

        var bindingAttr = BindingFlags.Public | (isStatic ? BindingFlags.Static : BindingFlags.Instance);

        var fieldInfo = type.GetField(name, bindingAttr);

        if (fieldInfo == null && type.BaseType != null)
            return type.BaseType.GetPublicFieldInfo(name, isStatic);

        return fieldInfo;
    }

    /// <summary>
    /// Retrieves a public property info by its name. This method will search all base classes.
    /// </summary>
    public static PropertyInfo GetPublicPropertyInfo(this Type type, string name, bool isStatic = false)
    {
        ArgumentNullException.ThrowIfNull(type);

        var bindingAttr = BindingFlags.Public | (isStatic ? BindingFlags.Static : BindingFlags.Instance);

        var propInfo = type.GetProperty(name, bindingAttr);

        if (propInfo == null && type.BaseType != null)
            return type.BaseType.GetPublicPropertyInfo(name, isStatic);

        return propInfo;
    }
}
