namespace Digital5HP.DataAccess.EntityFramework.Specification;

using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Digital5HP.DataAccess.Specifications;

public class SpecificationFactory(IServiceProvider serviceProvider) : ISpecificationFactory
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

    public T Create<T, TEntity>(IQueryable<TEntity> queryable)
        where T : ISpecification<TEntity>
        where TEntity : class, IEntity
    {
        var spec = this.serviceProvider.GetRequiredService<T>();

        if (!(spec is QueryableSpecification<TEntity> qSpec))
            throw new DataAccessException(
                $"Resolved specification for entity '{typeof(TEntity).Name}' must inherit from '{typeof(QueryableSpecification<>).Name}'");

        qSpec.SetQueryable(queryable);

        return spec;
    }
}