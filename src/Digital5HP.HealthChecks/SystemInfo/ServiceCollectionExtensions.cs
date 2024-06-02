namespace Digital5HP.HealthChecks.SystemInfo;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMemoryHealthCheck(this IServiceCollection services)
    {
        services.AddHealthCheck<SystemMemoryHealthCheck>(
            "Memory Usage",
            tags: new[]
                  {
                      "system", "ram", Tags.STARTUP, Tags.LIVENESS, Tags.READINESS,
                  });

        return services;
    }
}
