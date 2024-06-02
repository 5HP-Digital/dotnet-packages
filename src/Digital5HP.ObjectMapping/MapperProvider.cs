namespace Digital5HP.ObjectMapping;

using System;

using Microsoft.Extensions.DependencyInjection;

public class MapperProvider(IServiceProvider serviceProvider) : IMapperProvider
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

    public IMapper<TSrc> Get<TSrc>()
    {
        try
        {
            return this.serviceProvider.GetRequiredService<IMapper<TSrc>>();
        }
        catch (InvalidOperationException ex)
        {
            throw new MappingException(
                $"Failed to retrieve mapper for {typeof(TSrc).FullName}. Make sure type has a corresponding mapping profile.",
                ex);
        }
    }
}
