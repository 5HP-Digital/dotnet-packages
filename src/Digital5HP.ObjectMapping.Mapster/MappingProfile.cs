namespace Digital5HP.ObjectMapping.Mapster;

public abstract class MappingProfile<TSrc>
{
    protected abstract void Configure(MappingConfiguration<TSrc> config);

    internal void ConfigureInternal(MappingConfiguration<TSrc> configuration)
    {
        this.Configure(configuration);
    }
}
