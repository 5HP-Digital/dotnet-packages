namespace Digital5HP.HealthChecks;

using global::System;
using global::System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers health check.
    /// </summary>
    /// <remarks><paramref name="name"/> must be unique. If multiple registration of same health check is possible, use TryAddHealthCheck.</remarks>
    /// <param name="services">Service collection</param>
    /// <param name="name">The name of health check.</param>
    /// <param name="failureStatus">The <see cref="HealthStatus"/> that should be reported when the health check fails. Defaults to <see cref="HealthStatus.Unhealthy"/>.</param>
    /// <param name="tags">A list of tags that can be used to filter health checks.</param>
    /// <param name="timeout">An optional timeout of the check.</param>
    public static IServiceCollection AddHealthCheck<TCheck>(this IServiceCollection services,
                                                            string name,
                                                            HealthStatus? failureStatus = null,
                                                            IEnumerable<string> tags = null,
                                                            TimeSpan? timeout = null)
        where TCheck : class, IHealthCheck =>
        services.AddHealthCheck(
            name,
            ActivatorUtilities.GetServiceOrCreateInstance<TCheck>,
            failureStatus,
            tags,
            timeout);

    /// <summary>
    /// Registers health check using a factory.
    /// </summary>
    /// <remarks><paramref name="name"/> must be unique. If multiple registration of same health check is possible, use TryAddHealthCheck.</remarks>
    /// <param name="services">Service collection</param>
    /// <param name="name">The name of health check.</param>
    /// <param name="factory">Factory for <typeparamref name="TCheck"/> using service provider.</param>
    /// <param name="failureStatus">The <see cref="HealthStatus"/> that should be reported when the health check fails. Defaults to <see cref="HealthStatus.Unhealthy"/>.</param>
    /// <param name="tags">A list of tags that can be used to filter health checks.</param>
    /// <param name="timeout">An optional timeout of the check.</param>
    public static IServiceCollection AddHealthCheck<TCheck>(this IServiceCollection services,
                                                            string name,
                                                            Func<IServiceProvider, TCheck> factory,
                                                            HealthStatus? failureStatus = null,
                                                            IEnumerable<string> tags = null,
                                                            TimeSpan? timeout = null)
        where TCheck : class, IHealthCheck
    {
        ArgumentNullException.ThrowIfNull(name);

        var checkOptions = new CheckOptions
                           {
                               Name = name,
                               FailureStatus = failureStatus,
                               Tags = tags,
                               Timeout = timeout
                           };

        if(!CheckRegistrator.TryAddCheck(factory, checkOptions))
            throw new AppCoreException($"Health check with name '{name}' already exists.");

        return services;
    }

    /// <summary>
    /// Registers health check if the health check hasn't already been registered.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="name">The name of health check.</param>
    /// <param name="failureStatus">The <see cref="HealthStatus"/> that should be reported when the health check fails. Defaults to <see cref="HealthStatus.Unhealthy"/>.</param>
    /// <param name="tags">A list of tags that can be used to filter health checks.</param>
    /// <param name="timeout">An optional timeout of the check.</param>
    public static IServiceCollection TryAddHealthCheck<TCheck>(this IServiceCollection services,
                                                               string name,
                                                               HealthStatus? failureStatus = null,
                                                               IEnumerable<string> tags = null,
                                                               TimeSpan? timeout = null)
        where TCheck : class, IHealthCheck =>
        services.TryAddHealthCheck(
            name,
            ActivatorUtilities.GetServiceOrCreateInstance<TCheck>,
            failureStatus,
            tags,
            timeout);

    /// <summary>
    /// Registers health check using a factory if the health check hasn't already been registered.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="name">The name of health check.</param>
    /// <param name="factory">Factory for <typeparamref name="TCheck"/> using service provider.</param>
    /// <param name="failureStatus">The <see cref="HealthStatus"/> that should be reported when the health check fails. Defaults to <see cref="HealthStatus.Unhealthy"/>.</param>
    /// <param name="tags">A list of tags that can be used to filter health checks.</param>
    /// <param name="timeout">An optional timeout of the check.</param>
    public static IServiceCollection TryAddHealthCheck<TCheck>(this IServiceCollection services,
                                                            string name,
                                                            Func<IServiceProvider, TCheck> factory,
                                                            HealthStatus? failureStatus = null,
                                                            IEnumerable<string> tags = null,
                                                            TimeSpan? timeout = null)
        where TCheck : class, IHealthCheck
    {
        ArgumentNullException.ThrowIfNull(name);

        var checkOptions = new CheckOptions
                           {
                               Name = name,
                               FailureStatus = failureStatus,
                               Tags = tags,
                               Timeout = timeout
                           };

        CheckRegistrator.TryAddCheck(factory, checkOptions);

        return services;
    }
}
