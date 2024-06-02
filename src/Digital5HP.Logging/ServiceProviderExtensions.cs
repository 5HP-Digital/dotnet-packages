namespace Digital5HP.Logging;

using System;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceProviderExtensions
{
    /// <summary>
    /// Retrieves <see cref="ILogger{TCategoryName}"/> for the provided type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static ILogger<T> GetLoggerFor<T>(this IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<ILogger<T>>();
    }

    /// <summary>
    /// Retrieves <see cref="ILogger"/> with its source context set for the provided <paramref name="contextType"/>.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="contextType"></param>
    /// <returns></returns>
    public static ILogger GetLoggerFor(this IServiceProvider serviceProvider, Type contextType)
    {
        var loggerType = typeof(ILogger<>).MakeGenericType(contextType);

        return (ILogger) serviceProvider.GetRequiredService(loggerType);
    }
}