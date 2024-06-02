namespace Digital5HP.DataAccess.Specifications;

using Digital5HP.ObjectMapping;

public abstract class Specification<TEntity>(IMapperProvider mapperProvider) : ISpecification<TEntity>
    where TEntity : IEntity
{
    protected IMapperProvider MapperProvider { get; } = mapperProvider;
}
