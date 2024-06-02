namespace Digital5HP.DataAccess.Specifications;

public interface IKeyedSpecification<out TSpecification, TEntity, in TPrimaryKey> : ISpecification<TEntity>
    where TSpecification : IKeyedSpecification<TSpecification, TEntity, TPrimaryKey>
    where TEntity : IKeyedEntity<TPrimaryKey>
{
    TSpecification ById(params TPrimaryKey[] ids);
}
