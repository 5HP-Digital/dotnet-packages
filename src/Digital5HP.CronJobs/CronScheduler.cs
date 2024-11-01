namespace Digital5HP.CronJobs;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

public sealed class CronScheduler(
    IServiceProvider serviceProvider,
    IReadOnlyCollection<CronRegistryEntry> cronJobs) : BackgroundService
{
    private readonly IServiceProvider serviceProvider = serviceProvider;
    private readonly IReadOnlyCollection<CronRegistryEntry> cronJobs = cronJobs;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Create a timer that has a resolution less than 60 seconds
        // Because cron has a resolution of a minute
        // So everything under will work
        using var tickTimer = new PeriodicTimer(TimeSpan.FromSeconds(30));

        // Create a map of the next upcoming entries
        var runMap = new Dictionary<DateTime, List<Type>>();
        while (await tickTimer.WaitForNextTickAsync(stoppingToken))
        {
            // Get UTC Now with minute resolution (remove microseconds and seconds)
            var now = Digital5HP.TimeProvider.Current.Now.Truncate(TimeSpan.FromMinutes(1));

            // Run jobs that are in the map
            this.RunActiveJobs(runMap, now, stoppingToken);

            // Get the next run for the upcoming tick
            runMap = this.GetJobRuns(now);
        }
    }

    private void RunActiveJobs(Dictionary<DateTime, List<Type>> runMap, DateTime now, CancellationToken stoppingToken)
    {
        if (!runMap.TryGetValue(now, out var currentRuns))
        {
            return;
        }

        foreach (var run in currentRuns)
        {
            // We are sure (thanks to our extension method)
            // that the service is of type ICronJob
            var job = (ICronJob)this.serviceProvider.GetRequiredService(run);

            // We don't want to await jobs explicitly because that
            // could interfere with other job runs
            _ = job.RunAsync(stoppingToken);
        }
    }

    private Dictionary<DateTime, List<Type>> GetJobRuns(DateTime now)
    {
        var runMap = new Dictionary<DateTime, List<Type>>();
        foreach (var cron in this.cronJobs)
        {
            var runDates = cron.GetSchedule(this.serviceProvider).GetNextOccurrences(now, now.AddMinutes(1));
            if (runDates is not null)
            {
                AddJobRuns(runMap, runDates, cron);
            }
        }

        return runMap;
    }

    private static void AddJobRuns(Dictionary<DateTime, List<Type>> runMap, IEnumerable<DateTime> runDates, CronRegistryEntry cron)
    {
        ArgumentNullException.ThrowIfNull(runMap);

        foreach (var runDate in runDates)
        {
            if (runMap.TryGetValue(runDate, out var value))
            {
                value.Add(cron.Type);
            }
            else
            {
                runMap[runDate] = [cron.Type];
            }
        }
    }
}
