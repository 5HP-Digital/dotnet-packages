namespace Digital5HP.DataAccess;

using System;
using System.Threading;
using System.Threading.Tasks;

public interface IUnitOfWork : IDisposable
{
    // TODO: Implement transaction support.

    /// <summary>
    /// Persists all changes to the backing data stores.
    /// </summary>
    /// <returns>True if any changes made.</returns>
    Task<bool> SaveAsync(CancellationToken cancellationToken = default);
}