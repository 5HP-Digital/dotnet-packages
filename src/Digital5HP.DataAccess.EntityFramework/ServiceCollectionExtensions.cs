namespace Digital5HP.DataAccess.EntityFramework;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Digital5HP.DataAccess.EntityFramework.Specification;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkCore(this IServiceCollection services)
    {
        services.AddDataAccessCore();

        services.TryAddScoped<ISpecificationFactory, SpecificationFactory>();

        return services;
    }
}
