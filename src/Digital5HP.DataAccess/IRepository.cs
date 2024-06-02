namespace Digital5HP.DataAccess;

using System;

/// <summary>
/// Repository interface
/// </summary>
public interface IRepository : IDisposable
{
    /// <summary>
    /// The underlying context.
    /// </summary>
    IContext Context { get; }
}
