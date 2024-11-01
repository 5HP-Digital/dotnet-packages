namespace Digital5HP.CronJobs;

using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using NCrontab;

public static class ServiceCollectionExtentions
{
    public static IServiceCollection AddCronJob<T>(this IServiceCollection services, string cronExpression) where T : class, ICronJob
    {
        if (string.IsNullOrWhiteSpace(cronExpression))
        {
            throw new ArgumentException($"'{nameof(cronExpression)}' cannot be null or whitespace.", nameof(cronExpression));
        }

        var cron = CrontabSchedule.TryParse(cronExpression)
            ?? throw new ArgumentException("Invalid cron expression", nameof(cronExpression));

        var entry = new CronRegistryEntry(typeof(T), cron);

        services.AddHostedService<CronScheduler>();
        services.TryAddSingleton<T>();
        services.AddSingleton(entry);

        return services;
    }
}
