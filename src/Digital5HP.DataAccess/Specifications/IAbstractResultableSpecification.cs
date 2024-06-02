namespace Digital5HP.DataAccess.Specifications;

public interface IAbstractResultableSpecification<TEntity> : IAbstractSpecification<TEntity>
    where TEntity : class, IEntity
{
    /// <summary>
    /// Specifies a derived entity type to include in query result.
    /// </summary>
    IAbstractSpecificationResult<TEntity> AsAny();
}