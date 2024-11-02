namespace Digital5HP.CronJobs;

using System.Threading.Tasks;
using System.Threading;

public interface ICronJob
{
    Task RunAsync(CancellationToken token = default);
}
