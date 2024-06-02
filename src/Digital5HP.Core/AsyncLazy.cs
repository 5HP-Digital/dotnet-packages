namespace Digital5HP;

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

public class AsyncLazy<T> : Lazy<Task<T>>
{
    public AsyncLazy(Func<T> valueFactory,
                     TaskCreationOptions taskCreationOptions = TaskCreationOptions.None,
                     CancellationToken cancellationToken = default)
        :
        base(
            () => Task.Factory.StartNew(
                valueFactory,
                cancellationToken,
                taskCreationOptions,
                TaskScheduler.Default))
    {
    }

    public AsyncLazy(Func<Task<T>> taskFactory,
                     TaskCreationOptions taskCreationOptions = TaskCreationOptions.None,
                     CancellationToken cancellationToken = default)
        :
        base(
            () => Task.Factory.StartNew(taskFactory, cancellationToken, taskCreationOptions, TaskScheduler.Default)
                      .Unwrap())
    {
    }

    public TaskAwaiter<T> GetAwaiter() { return this.Value.GetAwaiter(); }
}
