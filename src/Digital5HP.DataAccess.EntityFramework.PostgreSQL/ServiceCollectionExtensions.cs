namespace Digital5HP.DataAccess.EntityFramework.PostgreSQL;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    private static readonly string[] Tags =
            [
                "postgresql", "entity-framework"
            ];

    public static IServiceCollection AddEntityFrameworkPostgreSQL<TContext>(this IServiceCollection services)
        where TContext : ContextBase, IDbContext
    {
        services.AddEntityFrameworkCore();

        services.AddDbContext<IDbContext, TContext>();
        services.AddContextHealthCheck<IDbContext, TContext>(
            typeof(TContext).Name,
            Tags);
        return services;
    }
}
