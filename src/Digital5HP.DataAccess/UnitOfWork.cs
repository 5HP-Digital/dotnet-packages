namespace Digital5HP.DataAccess;

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Unit of Work.
/// <para>
/// This base class manages access to its repositories and coordinates persistence to their underlying contexts.
/// </para>
/// </summary>
/// <remarks>
/// All repositories and their underlying contexts are injected in a scoped dependency resolution. Once disposed, all scoped services are automatically disposed.
/// </remarks>
public abstract class UnitOfWork(IServiceProvider serviceProvider) : IUnitOfWork
{
    private readonly IServiceScope serviceScope = serviceProvider?.CreateScope()
                            ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly SemaphoreSlim mutex = new(1, 1);

    private readonly ConcurrentDictionary<Type, IRepository> repositoriesContainer = new();
    private readonly ConcurrentDictionary<Type, IContext> contextContainer = new();

    /// <summary>
    /// Field indicating whether <see cref="Dispose"/> was called.
    /// </summary>
    protected bool IsDisposed { get; private set; }

    public async Task<bool> SaveAsync(CancellationToken cancellationToken = default)
    {
        bool result;
        await this.mutex.WaitAsync(cancellationToken);

        try
        {
            var results = await Task.WhenAll(
                this.contextContainer.Values
                    .Select(ctx => ctx.CommitAsync(cancellationToken)));

            result = results.Any(r => r > 0);
        }
        finally
        {
            this.mutex.Release();
        }

        return result;
    }

    /// <summary>Public implementation of Dispose pattern callable by consumers.</summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Protected implementation of Dispose pattern.</summary>
    protected virtual void Dispose(bool disposing)
    {
        if (this.IsDisposed)
        {
            return;
        }

        if (disposing)
        {
            this.mutex.Wait();
            try
            {
                this.contextContainer.Clear();
                this.repositoriesContainer.Clear();

                // technically we don't need to dispose all repos and contexts above since they are all
                // scoped and will get picked up by IoC container once the scope is disposed.
                this.serviceScope.Dispose();
            }
            finally
            {
                this.mutex.Release();
            }

            this.mutex.Dispose();
        }

        this.IsDisposed = true;
    }

    /// <summary>
    /// Retrieves a repository.
    /// </summary>
    /// <remarks>
    /// A repository is instantiated using the current scoped IoC container on initial call.
    /// </remarks>
    protected TRepository GetRepository<TRepository>()
        where TRepository : IRepository
    {
        var type = typeof(TRepository);

        this.mutex.Wait();
        try
        {
            return (TRepository)this.repositoriesContainer.GetOrAdd(
                type,
                _ => this.CreateRepository<TRepository>());
        }
        finally
        {
            this.mutex.Release();
        }
    }

    private TRepository CreateRepository<TRepository>()
        where TRepository : IRepository
    {
        var repo = this.serviceScope.ServiceProvider.GetRequiredService<TRepository>();

        if (repo != null)
        {
            // register context in container
            var contextType = repo.Context.GetType();
            this.contextContainer.TryAdd(contextType, repo.Context);
        }

        return repo;
    }
}
