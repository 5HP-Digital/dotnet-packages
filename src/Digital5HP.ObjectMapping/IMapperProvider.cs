namespace Digital5HP.ObjectMapping;

public interface IMapperProvider
{
    /// <summary>
    /// Retrieves mapper for the source type provided.
    /// </summary>
    /// <typeparam name="TSrc">Source type.</typeparam>
    /// <exception cref="MappingException">When <typeparamref name="TSrc"/> doesn't have a corresponding mapper registered.</exception>
#pragma warning disable CA1716
    IMapper<TSrc> Get<TSrc>();
#pragma warning restore CA1716
}
