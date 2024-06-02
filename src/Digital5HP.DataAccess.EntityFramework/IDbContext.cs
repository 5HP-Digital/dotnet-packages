namespace Digital5HP.DataAccess.EntityFramework;

using System.Threading.Tasks;
using System;

using Microsoft.EntityFrameworkCore;

public interface IDbContext : IContext
{
#pragma warning disable CA1716, VSTHRD200
    DbSet<TEntity> Set<TEntity>()
#pragma warning restore CA1716, VSTHRD200
        where TEntity : class;

    bool IsDetached(object entity);

    Task<int> ExecuteCommandAsync(FormattableString sql);
}
