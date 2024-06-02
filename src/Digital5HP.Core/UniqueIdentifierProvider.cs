namespace Digital5HP;

using System;
using System.Globalization;

public static class UniqueIdentifierProvider
{
    private static readonly IUniqueIdentifierProvider Default = new DefaultUniqueIdentifierProvider();

    private static IUniqueIdentifierProvider current = Default;

    /// <summary>
    /// The current <see cref="IUniqueIdentifierProvider"/>.
    /// </summary>
    public static IUniqueIdentifierProvider Current
    {
        get => current;
        set => current = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Sets <see cref="Current"/> to the default <see cref="IUniqueIdentifierProvider"/> (<see cref="DefaultUniqueIdentifierProvider"/>).
    /// </summary>
    public static void ResetProvider()
    {
        Current = Default;
    }

    private class DefaultUniqueIdentifierProvider : IUniqueIdentifierProvider
    {
        public T Generate<T>()
            where T : struct
        {
            var type = typeof(T);

            if (type == typeof(Guid))
            {
                return Guid.NewGuid()
                           .ChangeType<T>(CultureInfo.InvariantCulture);
            }

            throw new NotSupportedException($"Generating identifier for type '{type.Name}' is not supported.");
        }
    }
}
