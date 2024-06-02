namespace Digital5HP.DataAccess.Specifications;

/// <summary>
/// Generic base interface for the domain specifications (domain oriented queries) that can provide a result.
/// </summary>
/// <remarks>
/// If your specification have different requirements, you can implement other public
/// method than <see cref="Result"/>. For example, it would be possible to create specification that
/// specifies several input data and has Execute method that writes something into
/// underlying unit of work.
/// </remarks>
public interface IResultableSpecification<TEntity> : ISpecification<TEntity>
    where TEntity : class, IEntity, new()
{
    /// <summary>
    /// Gets specification result wrapped into <see cref="ISpecificationResult{TEntity}"/> interface.
    /// </summary>
    /// <remarks>
    /// <see cref="ISpecificationResult{TEntity}"/> interface wraps common functionality that is shared
    /// across all specifications.
    /// </remarks>
    ISpecificationResult<TEntity> Result { get; }
}