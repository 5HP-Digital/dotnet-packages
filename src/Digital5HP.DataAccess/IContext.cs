namespace Digital5HP.DataAccess;

using System;
using System.Threading;
using System.Threading.Tasks;

public interface IContext : IDisposable
{
    /// <summary>
    /// Persists all changes to the backing data store.
    /// </summary>
    /// <returns>Returns the number of changes committed.</returns>
    Task<int> CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether or not the database is available and can be connected to.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns> <see langword="true" /> if the database is available; <see langword="false" /> otherwise. </returns>
    Task<bool> CanConnectAsync(CancellationToken cancellationToken = default);
}
