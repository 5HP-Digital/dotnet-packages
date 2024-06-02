namespace Digital5HP.DataAccess;

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using Digital5HP.Logging;

public class ContextHealthCheck<TContext>(TContext context, ILogger<ContextHealthCheck<TContext>> logger) : IHealthCheck
    where TContext : IContext
{
    private readonly TContext context = context;
    private readonly ILogger<ContextHealthCheck<TContext>> logger = logger;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            if (await this.context.CanConnectAsync(cancellationToken)
                          .ConfigureAwait(false))
            {
                return HealthCheckResult.Healthy("Connection established.");
            }

            return new HealthCheckResult(context.Registration.FailureStatus, "Connection failed.");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Data context health check failed to execute.");
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}
