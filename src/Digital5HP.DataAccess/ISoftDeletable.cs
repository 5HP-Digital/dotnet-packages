namespace Digital5HP.DataAccess;

/// <summary>
/// To be used by entity models with a IsDeleted boolean field.
/// Use this interface on models to inform the repository to use
/// this flag instead of actually deleting the entity.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Gets or sets a value indicating whether or not a record has been flagged as deleted.
    /// </summary>
    bool IsDeleted { get; set; }
}