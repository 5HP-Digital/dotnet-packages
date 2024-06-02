namespace Digital5HP.Logging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Digital5HP.Logging.Internals;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLoggingCore(this IServiceCollection services)
    {
        services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger), typeof(LoggerWrapper)));
        services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(LoggerWrapper<>)));
        services.TryAddSingleton<ILoggerFactory, LoggerFactoryWrapper>();

        return services;
    }
}
