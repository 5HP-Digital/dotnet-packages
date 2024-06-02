namespace Digital5HP;

public interface IUniqueIdentifierProvider
{
    /// <summary>
    /// Generates value based on type provided.
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <returns>Random value</returns>
    public T Generate<T>() where T : struct;
}
