namespace Digital5HP.CronJobs;

using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using NCrontab;

public static class ServiceCollectionExtentions
{
    public static IServiceCollection AddCronJob<T>(this IServiceCollection services, string cronExpression) where T : class, ICronJob
    {
        return services.AddCronJob<T>(_ => cronExpression);
    }

    public static IServiceCollection AddCronJob<T>(this IServiceCollection services, Func<IServiceProvider, string> configurationProvider) where T : class, ICronJob
    {
        ArgumentNullException.ThrowIfNull(configurationProvider);

        var entry = new CronRegistryEntry(typeof(T), servicerProvider =>
        {
            var expression = configurationProvider(servicerProvider);

            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new ArgumentException($"Cron expression cannot be null or whitespace.", nameof(configurationProvider));
            }

            var cron = CrontabSchedule.TryParse(expression) ?? throw new ArgumentException("Invalid cron expression", nameof(configurationProvider));

            return cron;
        });

        services.AddHostedService<CronScheduler>();
        services.TryAddSingleton<T>();
        services.AddSingleton(entry);

        return services;
    }
}
