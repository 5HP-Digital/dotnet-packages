namespace Digital5HP.CronJobs;

using System;

using NCrontab;

public sealed record CronRegistryEntry(Type Type, Func<IServiceProvider, CrontabSchedule> ScheduleProvider)
{
    private CrontabSchedule schedule;

    public CrontabSchedule GetSchedule(IServiceProvider serviceProvider)
    {
        return this.schedule ??= this.ScheduleProvider.Invoke(serviceProvider);
    }
}
