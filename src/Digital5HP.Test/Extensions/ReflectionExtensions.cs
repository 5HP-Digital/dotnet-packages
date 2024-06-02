namespace Digital5HP.Test
{
    using System;
    using System.Reflection;

    public static class ReflectionExtensions
    {
        /// <summary>
        /// Sets the value of a non-public (private/protected/internal) field on class <typeparamref name="T"/>.
        /// </summary>
        public static void SetNonPublicField<T>(this T obj, string fieldName, object value)
        {
            var fieldInfo = typeof(T).GetNonPublicFieldInfo(fieldName) ?? throw new ArgumentException($"Field '{fieldName}' cannot be found in type '{typeof(T).Name}'.", nameof(fieldName));

            if (value == null)
            {
                if (!fieldInfo.FieldType.CanBeNull())
                {
                    throw new ArgumentNullException(nameof(value), "Cannot set non-nullable field to null.");
                }

                fieldInfo.SetValue(obj, null);
                return;
            }

            if (!fieldInfo.FieldType.IsInstanceOfType(value))
            {
                throw new ArgumentException(
                    $"Cannot set field of type '{typeof(T).Name}' with value of type '{value.GetType().Name}'.",
                    nameof(value));
            }

            fieldInfo.SetValue(obj, value);
        }

        /// <summary>
        /// Sets the value of a non-public (private/protected/internal) property on class <typeparamref name="T"/>.
        /// </summary>
        public static void SetNonPublicProperty<T>(this T obj, string propertyName, object value)
        {
            var propertyInfo = typeof(T).GetNonPublicPropertyInfo(propertyName) ?? throw new ArgumentException($"Property '{propertyName}' cannot be found in type '{typeof(T).Name}'.", nameof(propertyName));

            if (value == null)
            {
                if (!propertyInfo.PropertyType.CanBeNull())
                {
                    throw new ArgumentNullException(nameof(value), "Cannot set non-nullable property to null.");
                }

                propertyInfo.SetValue(obj, null);
                return;
            }

            if (!propertyInfo.PropertyType.IsInstanceOfType(value))
            {
                throw new ArgumentException(
                    $"Cannot set property of type '{typeof(T).Name}' with value of type '{value.GetType().Name}'.",
                    nameof(value));
            }

            propertyInfo.SetValue(obj, value);
        }

        /// <summary>
        /// Sets the value of a public property on class <typeparamref name="T"/>. Usually used if setter is not accessible.
        /// </summary>
        public static void SetPublicProperty<T>(this T obj, string propertyName, object value)
        {
            var propertyInfo = typeof(T).GetPublicPropertyInfo(propertyName) ?? throw new ArgumentException($"Property '{propertyName}' cannot be found in type '{typeof(T).Name}'.", nameof(propertyName));

            if (value == null && !propertyInfo.PropertyType.CanBeNull())
            {
                throw new ArgumentNullException(nameof(value), "Cannot set non-nullable property to null.");
            }

            if (value != null && !propertyInfo.PropertyType.IsInstanceOfType(value))
            {
                throw new ArgumentException(
                    $"Cannot set property of type '{typeof(T).Name}' with value of type '{value.GetType().Name}'.",
                    nameof(value));
            }

            propertyInfo.SetValue(obj, value);
        }

        /// <summary>
        /// Gets value from a non-public (private/protected/internal) field on class <typeparamref name="T"/>.
        /// </summary>
        public static object GetNonPublicField<T>(this T obj, string fieldName)
        {
            var fieldInfo = typeof(T).GetNonPublicFieldInfo(fieldName) ?? throw new ArgumentException($"Field '{fieldName}' cannot be found in type '{typeof(T).Name}'.", nameof(fieldName));

            return fieldInfo.GetValue(obj);
        }

        /// <summary>
        /// Gets value from a non-public (private/protected/internal) property on class <typeparamref name="T"/>.
        /// </summary>
        public static object GetNonPublicProperty<T>(this T obj, string propertyName)
        {
            var propertyInfo = typeof(T).GetNonPublicPropertyInfo(propertyName) ?? throw new ArgumentException($"Property '{propertyName}' cannot be found in type '{typeof(T).Name}'.", nameof(propertyName));

            return propertyInfo.GetValue(obj);
        }

        /// <summary>
        /// Invokes a non-public (private/protected/internal) method on class <typeparamref name="T"/> with the provided arguments.
        /// </summary>
        public static void InvokeNonPublicMethod<T>(T obj, string methodName, params object[] args)
        {
            var methodInfo = typeof(T).GetNonPublicMethodInfo(methodName) ?? throw new ArgumentException($"Method '{methodName}' cannot be found in type '{typeof(T).Name}'.", nameof(methodName));

            try
            {
                methodInfo.Invoke(obj, args);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        /// <summary>
        /// Invokes a non-public (private/protected/internal) method on <paramref name="obj"/> with the provided arguments and returns the result of type <typeparamref name="T"/>.
        /// </summary>
        public static T InvokeNonPublicMethod<T>(this object obj, string methodName, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(obj);

            var type = obj.GetType();
            var methodInfo = type.GetNonPublicMethodInfo(methodName) ?? throw new ArgumentException($"Method '{methodName}' cannot be found in type '{type.Name}'.", nameof(methodName));

            try
            {
                var result = methodInfo.Invoke(obj, args);

                return (T)result;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        /// <summary>
        /// Invokes a static non-public (private/protected/internal) method on <paramref name="type"></paramref> with the provided arguments and returns the result of type <typeparamref name="T"/>.
        /// </summary>
        public static T InvokeStaticNonPublicMethod<T>(this Type type, string methodName, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(type);

            var methodInfo = type.GetNonPublicMethodInfo(methodName, true) ?? throw new ArgumentException($"Method '{methodName}' cannot be found in type '{type.Name}'.", nameof(methodName));

            try
            {
                var result = methodInfo.Invoke(null, args);

                return (T)result;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }
    }
}
