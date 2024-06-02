#pragma warning disable RS0030
namespace Digital5HP;

using System;

public static class TimeProvider
{
    private static readonly ITimeProvider Default = new DefaultTimeProvider();

    private static ITimeProvider current = Default;

    /// <summary>
    /// The current <see cref="ITimeProvider"/> (normally this is a pass-through to <see cref="DateTime.UtcNow"/>, but it can be overridden for testing or debugging).
    /// </summary>
    public static ITimeProvider Current
    {
        get => current;
        set => current = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Sets <see cref="Current"/> to the default <see cref="ITimeProvider"/> (which is a pass-through to <see cref="DateTime.UtcNow"/>).
    /// </summary>
    public static void ResetProvider()
    {
        Current = Default;
    }

    private class DefaultTimeProvider : ITimeProvider
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
