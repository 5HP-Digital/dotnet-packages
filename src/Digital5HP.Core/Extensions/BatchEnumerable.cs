namespace Digital5HP;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class BatchEnumerable<T>(IEnumerable<T> collection, int size)
{
    private readonly IEnumerable<T> collection = collection;

    private readonly int size = size;

    public void ForEach(Action<IEnumerable<T>, CancellationToken> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        var total = this.collection.Count();

        for (var start = 0; start < total; start += this.size)
        {
            action(
                this.collection.Skip(start)
                    .Take(this.size),
                cancellationToken);
        }
    }

    public void ForEach(Action<int, IEnumerable<T>, CancellationToken> action,
                        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        var total = this.collection.Count();
        var batch = 1;

        for (var start = 0; start < total; start += this.size)
        {
            action(
                batch++,
                this.collection.Skip(start)
                    .Take(this.size),
                cancellationToken);
        }
    }

    public async Task ForEachAsync(Func<IEnumerable<T>, CancellationToken, Task> action,
                                   CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        var total = this.collection.Count();

        for (var start = 0; start < total; start += this.size)
        {
            await action(
                this.collection.Skip(start)
                    .Take(this.size),
                cancellationToken);
        }
    }

    public async Task ForEachAsync(Func<int, IEnumerable<T>, CancellationToken, Task> action,
                                   CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        var total = this.collection.Count();
        var batch = 1;

        for (var start = 0; start < total; start += this.size)
        {
            await action(
                batch++,
                this.collection.Skip(start)
                    .Take(this.size),
                cancellationToken);
        }
    }
}
