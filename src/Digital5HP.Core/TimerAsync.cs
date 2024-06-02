namespace Digital5HP;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Async friendly Timer implementation.
/// Provides a mechanism for executing an async method on
/// a thread pool thread at specified intervals.
///
/// This class cannot be inherited.
/// </summary>
public sealed class TimerAsync : IDisposable
{
    private readonly Func<CancellationToken, Task> scheduledAction;
    private readonly TimeSpan dueTime;
    private readonly TimeSpan period;
    private readonly SemaphoreSlim semaphore;
    private readonly bool canStartNextActionBeforePreviousIsCompleted;

    private CancellationTokenSource cancellationTokenSource;
    private Task scheduledTask;
    private bool disposing;

    /// <summary>
    /// Occurs when an error is raised in the scheduled action
    /// </summary>
    public event EventHandler<TimerErrorEventArgs> OnError;

    /// <summary>
    /// Gets the running status of the TimerAsync instance.
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Initializes a new instance of the TimerAsync.
    /// </summary>
    /// <param name="scheduledAction">A delegate representing a method to be executed.</param>
    /// <param name="dueTime">The amount of time to delay before scheduledAction is invoked for the first time.</param>
    /// <param name="period">The time interval between invocations of the scheduledAction.</param>
    /// <param name="canStartNextActionBeforePreviousIsCompleted">
    ///   Whether or not the interval starts at the end of the previous scheduled action or at precise points in time.
    /// </param>
    public TimerAsync(Func<CancellationToken, Task> scheduledAction, TimeSpan dueTime, TimeSpan period, bool canStartNextActionBeforePreviousIsCompleted = false)
    {
        this.scheduledAction = scheduledAction ?? throw new ArgumentNullException(nameof(scheduledAction));

        if (dueTime < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(dueTime), "due time must be equal or greater than zero");

        this.dueTime = dueTime;

        if (period < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(period), "period must be equal or greater than zero");

        this.period = period;

        this.canStartNextActionBeforePreviousIsCompleted = canStartNextActionBeforePreviousIsCompleted;

        this.semaphore = new SemaphoreSlim(1);
    }

    /// <summary>
    /// Starts the TimerAsync.
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(this.disposing, nameof(TimerAsync));

        if (cancellationToken.IsCancellationRequested)
            return;

        await this.semaphore.WaitAsync(cancellationToken)
                  .ConfigureAwait(false);
        try
        {

            if (this.IsRunning || cancellationToken.IsCancellationRequested)
                return;

            this.cancellationTokenSource = new CancellationTokenSource();
            this.scheduledTask = this.RunScheduledActionAsync(this.cancellationTokenSource.Token);
            this.IsRunning = true;
        }
        catch (OperationCanceledException) { }
        finally
        {
            this.semaphore.Release();
        }
    }

    /// <summary>
    /// Stops the TimerAsync.
    /// </summary>
    /// <returns>A task that completes when the timer is stopped.</returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(this.disposing, typeof(TimerAsync));

        if (cancellationToken.IsCancellationRequested)
            return;

        await this.semaphore.WaitAsync(cancellationToken)
                  .ConfigureAwait(false);
        try
        {
            if (!this.IsRunning || cancellationToken.IsCancellationRequested)
                return;

            await this.cancellationTokenSource?.CancelAsync();

            if (this.scheduledTask != null)
#pragma warning disable VSTHRD003
                await this.scheduledTask.ConfigureAwait(false);
#pragma warning restore VSTHRD003
        }
        catch (OperationCanceledException) { }
        finally
        {
            this.IsRunning = false;

            this.StopAndClean();

            this.semaphore.Release();
        }
    }

    private async Task RunScheduledActionAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(this.dueTime, cancellationToken)
                      .ConfigureAwait(false);

            while (true)
            {
                try
                {
                    if (this.canStartNextActionBeforePreviousIsCompleted)
#pragma warning disable 4014
                        this.scheduledAction(cancellationToken);
#pragma warning restore 4014
                    else
                        await this.scheduledAction(cancellationToken)
                                  .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    try
                    {
                        this.OnError?.Invoke(this, new TimerErrorEventArgs(ex));
                    }
                    catch
                    {
                        // ignored
                    }
                }
                finally
                {
                    await Task.Delay(this.period, cancellationToken)
                              .ConfigureAwait(false);
                }
            }
        }
        catch (OperationCanceledException) { }
        catch (ObjectDisposedException) { }
        finally
        {
            await this.semaphore.WaitAsync(cancellationToken)
                      .ConfigureAwait(false);

            try
            {
                this.IsRunning = false;

                this.StopAndClean();
            }
            catch
            {
                this.semaphore.Release();
            }
        }
    }

    /// <summary>
    /// Releases all resources used by the current instance of TimerAsync.
    /// </summary>
    public void Dispose()
    {
        this.disposing = true;

        this.StopAndClean();

        this.semaphore.Dispose();
    }

    private void StopAndClean()
    {
        this.cancellationTokenSource?.Dispose();
        this.cancellationTokenSource = null;
        this.scheduledTask = null;
    }
}
