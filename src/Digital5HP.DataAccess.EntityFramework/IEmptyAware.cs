namespace Digital5HP.DataAccess.EntityFramework;

/// <summary>
/// Use this interface on models to require a IsEmpty, which 
/// evaluates all relevant properties to determine whether the
/// object lacks any usable values, so any mapped object can
/// set its corresponding property to null rather than instantiate
/// an empty object.
/// </summary>
public interface IEmptyAware
{
    /// <summary>
    /// Gets a value indicating whether or not all of an object's relevant values are unset/default.
    /// </summary>
    bool IsEmpty { get; }
}
