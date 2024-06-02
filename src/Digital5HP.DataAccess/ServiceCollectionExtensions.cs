namespace Digital5HP.DataAccess;

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using HealthChecks;
using ObjectMapping.Mapster;

public static class ServiceCollectionExtensions
{
    private static readonly IEnumerable<string> DefaultTags = new[]
                                                              {
                                                                  "db",
                                                                  Tags.STARTUP,
                                                                  Tags.READINESS
                                                              };

    public static IServiceCollection AddDataAccessCore(this IServiceCollection services)
    {
        services.TryAddTransient<IUnitOfWorkFactory, UnitOfWorkFactory>();

        services.AddObjectMappingCore();

        // Add persistence resiliency configuration
        services.ConfigureOptions<PersistenceResiliencyConfigureOptions>();

        return services;
    }

    public static IServiceCollection AddContextHealthCheck<TContext, TContextImplementation>(
        this IServiceCollection services,
        string name,
        IEnumerable<string> tags = null)
        where TContext : IContext
        where TContextImplementation : class, TContext
    {
        services.AddHealthCheck<ContextHealthCheck<TContext>>(
            name ?? typeof(TContextImplementation).Name,
            HealthStatus.Unhealthy,
            tags != null ? DefaultTags.Union(tags, StringComparer.Ordinal) : DefaultTags);

        return services;
    }
}
