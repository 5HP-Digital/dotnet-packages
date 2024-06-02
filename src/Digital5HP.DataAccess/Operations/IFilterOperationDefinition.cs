namespace Digital5HP.DataAccess.Operations;

public interface IFilterOperationDefinition<TEntity> : IOperationDefinition<TEntity>
    where TEntity : class, IEntity
{

}