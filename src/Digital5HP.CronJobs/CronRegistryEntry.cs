namespace Digital5HP.CronJobs;

using System;

using NCrontab;

public sealed record CronRegistryEntry(Type Type, CrontabSchedule Schedule);
