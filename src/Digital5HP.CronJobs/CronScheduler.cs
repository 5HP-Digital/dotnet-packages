namespace Digital5HP.CronJobs;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

public sealed class CronScheduler(
    IServiceProvider serviceProvider,
    IEnumerable<CronRegistryEntry> cronJobs) : BackgroundService
{
    private readonly IServiceProvider serviceProvider = serviceProvider;
    private readonly IEnumerable<CronRegistryEntry> cronJobs = cronJobs;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Create a timer that has a resolution less than 60 seconds
        // Because cron has a resolution of a minute so everything under will work
        using var tickTimer = new PeriodicTimer(TimeSpan.FromSeconds(45));


        // Create a map of the next upcoming entries
        var runMap = new Dictionary<DateTime, HashSet<Type>>();
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

    private void RunActiveJobs(Dictionary<DateTime, HashSet<Type>> runMap, DateTime now, CancellationToken stoppingToken)
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

        runMap.Remove(now);
    }

    private Dictionary<DateTime, HashSet<Type>> GetJobRuns(DateTime now)
    {
        var runMap = new Dictionary<DateTime, HashSet<Type>>();
        foreach (var cron in this.cronJobs)
        {
            var runDates = cron.GetSchedule(this.serviceProvider)
                .GetNextOccurrences(now, now.AddSeconds(61)); // since both baseTime and endTime are exclusive, we need to add a second for the end minute to be included.
            if (runDates is not null)
            {
                AddJobRuns(runMap, runDates, cron);
            }
        }

        return runMap;
    }

    private static void AddJobRuns(Dictionary<DateTime, HashSet<Type>> runMap, IEnumerable<DateTime> runDates, CronRegistryEntry cron)
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
