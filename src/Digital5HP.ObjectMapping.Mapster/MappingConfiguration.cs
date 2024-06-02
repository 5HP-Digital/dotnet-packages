namespace Digital5HP.ObjectMapping.Mapster;

using global::Mapster;
using global::Mapster.Models;

public class MappingConfiguration<TSrc>(TypeAdapterConfig typeAdapterConfig)
{
    private readonly TypeAdapterConfig typeAdapterConfig = typeAdapterConfig;

    /// <summary>
    /// Creates map from source type <typeparamref name="TSrc"/> to <typeparamref name="TDest"/>.
    /// </summary>
    public TypeAdapterSetter<TSrc, TDest> MapTo<TDest>()
    {
        this.ThrowIfMappingExists<TSrc, TDest>();

        return this.typeAdapterConfig.NewConfig<TSrc, TDest>();
    }

    /// <summary>
    /// Creates map from dependent type <typeparamref name="TDepend"/> to <typeparamref name="TDest"/>.
    /// </summary>
    public TypeAdapterSetter<TDepend, TDest> MapDependentTo<TDepend, TDest>()
    {
        this.ThrowIfMappingExists<TDepend, TDest>();

        return this.typeAdapterConfig.NewConfig<TDepend, TDest>();
    }

    private void ThrowIfMappingExists<TSource, TDest>()
    {
        if (this.typeAdapterConfig.RuleMap.ContainsKey(new TypeTuple(typeof(TSource), typeof(TDest))))
        {
            throw new MappingConfigurationException(
                $"Mapping between {typeof(TSource).FullName} and {typeof(TDest).FullName} already exists.");
        }
    }
}
