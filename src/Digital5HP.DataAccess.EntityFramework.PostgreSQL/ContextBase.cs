namespace Digital5HP.DataAccess.EntityFramework.PostgreSQL;

using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Digital5HP.DataAccess.EntityFramework;

public abstract class ContextBase(IOptions<ContextOptions> contextOptions, ILoggerFactory loggerFactory)
    : EntityFramework.ContextBase(contextOptions, loggerFactory)
{
    protected override void ConfigureDatabase(DbContextOptionsBuilder builder, ContextOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        builder.UseNpgsql(
           options.ConnectionString,
           o =>
           {
               if (options.EnableSplitQuery)
               {
                   o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
               }
           });
    }
}
