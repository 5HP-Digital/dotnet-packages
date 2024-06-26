namespace Digital5HP.DataAccess.EntityFramework;

using System;

using Digital5HP.Logging;

public abstract class RepositoryBase(ILogger logger, IDbContext context) : IRepository
{
#pragma warning disable CA2213 // Disposable fields should be disposed
    private readonly IDbContext context = context ?? throw new EntityFrameworkException("Data access context was not initialized.");
#pragma warning restore CA2213 // Disposable fields should be disposed

    /// <summary>
    /// Indicates whether the current repository is read-only (no change tracking).
    /// </summary>
    protected abstract bool IsReadOnly { get; }

    protected ILogger Logger { get; } = logger;

    /// <summary>
    /// The Context.
    /// </summary>
    protected internal IDbContext Context => this.context;

    IContext IRepository.Context => this.Context;

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // dispose here
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
