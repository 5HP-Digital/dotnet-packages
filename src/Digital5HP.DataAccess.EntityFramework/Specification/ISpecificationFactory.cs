namespace Digital5HP.DataAccess.EntityFramework.Specification;

using System.Linq;

using Digital5HP.DataAccess.Specifications;

public interface ISpecificationFactory
{
    T Create<T, TEntity>(IQueryable<TEntity> queryable)
        where T : ISpecification<TEntity>
        where TEntity : class, IEntity;
}