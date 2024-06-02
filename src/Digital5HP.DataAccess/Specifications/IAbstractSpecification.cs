namespace Digital5HP.DataAccess.Specifications;

public interface IAbstractSpecification<TEntity> : ISpecification<TEntity>
    where TEntity : class, IEntity
{
#pragma warning disable CA1716
    TSpec As<TSpec, TSpecEntity>()
#pragma warning restore CA1716
        where TSpec : ISpecification<TSpecEntity>
        where TSpecEntity : class, TEntity;
}
