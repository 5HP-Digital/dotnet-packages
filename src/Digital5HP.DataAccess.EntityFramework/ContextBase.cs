namespace Digital5HP.DataAccess.EntityFramework;

using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public abstract class ContextBase(IOptions<ContextOptions> contextOptions, ILoggerFactory loggerFactory) : DbContext, IDbContext
{
    private readonly IOptions<ContextOptions> contextOptions = contextOptions;
#pragma warning disable CA2213 // Disposable fields should be disposed
    private readonly ILoggerFactory loggerFactory = loggerFactory;
#pragma warning restore CA2213 // Disposable fields should be disposed

    public bool IsDetached(object entity)
    {
        return this.Entry(entity)
                   .State
               == EntityState.Detached;
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        var writes = await this.SaveChangesAsync(cancellationToken);
        return writes;
    }

    public Task<bool> CanConnectAsync(CancellationToken cancellationToken = default) =>
        base.Database.CanConnectAsync(cancellationToken);

    public async Task<int> ExecuteCommandAsync(FormattableString sql)
    {
        var executed = await this.Database.ExecuteSqlInterpolatedAsync(sql);
        return executed;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);

        var options = this.contextOptions.Value;

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            throw new DataAccessException($"{nameof(options.ConnectionString)} must be configured.");
        }

        this.ConfigureDatabase(optionsBuilder, options);

        if (options.EnableLogging)
        {
            optionsBuilder.UseLoggerFactory(this.loggerFactory)
                          .EnableDetailedErrors();

            // Enable logging of parameters
            optionsBuilder.EnableSensitiveDataLogging();
        }

        if (options.DisableChangeTracking)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        if (options.TreatWarningsAsErrors)
        {
            optionsBuilder.ConfigureWarnings(
                warnings =>
                {
                    warnings.Default(WarningBehavior.Throw);

                    if (options.EnableLogging)
                    {
                        warnings.Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning);
                    }
                });

        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        this.ConfigureSharedTypeEntities(modelBuilder);

        var assemblies = this.GetAssembliesContainingConfiguration();

        foreach (var assembly in assemblies) modelBuilder.ApplyConfigurationsFromAssembly(assembly);

        this.ApplyAdditionalConfigurations(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // If many-to-one navigation property is required but navigation property is ISoftDeletable, set IsRequired to false (prevent EF warning).
            // This means that a required navigation property can be null if its entity was deleted.
            foreach (var navigation in entityType.GetNavigations()
                                                 .Where(
                                                      navigation => !navigation.IsCollection
                                                                    && typeof(ISoftDeletable).IsAssignableFrom(
                                                                        navigation.TargetEntityType.ClrType)
                                                                    && navigation.ForeignKey.IsRequired)
                                                 .ToList())
            {
                navigation.ForeignKey.IsRequired = false;
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    protected abstract void ConfigureDatabase(DbContextOptionsBuilder builder, ContextOptions options);

    /// <summary>
    /// Gets an array of assemblies containing all <see cref="EntityMapping{T}"/> classes required for this context.
    /// </summary>
    protected abstract Assembly[] GetAssembliesContainingConfiguration();

    /// <summary>
    /// Allows configuration of shared-type entities prior to auto-loading of entity mappings from assemblies.
    /// </summary>
    protected virtual void ConfigureSharedTypeEntities(ModelBuilder modelBuilder)
    {
    }

    /// <summary>
    /// Applies additional configurations required for this context. (For example, type conversions, serialization, etc.)
    /// </summary>
    protected virtual void ApplyAdditionalConfigurations(ModelBuilder modelBuilder)
    {
    }
}
