namespace Digital5HP.ObjectMapping.Mapster;

using System;
using System.Collections.Generic;

using global::Mapster;

using MapsterMapper;

using Digital5HP.Logging;

public class Mapper<TSrc>(IMapper mapper, ILogger<Mapper<TSrc>> logger) : IMapper<TSrc>
{
    private readonly IMapper mapper = mapper;
    private readonly ILogger<Mapper<TSrc>> logger = logger;

    public TDest Map<TDest>(TSrc source)
    {
        try
        {
            using var scope = MapContextScope.RequiresNew();
            return this.mapper.Map<TSrc, TDest>(source);
        }
        catch (Exception ex)
        {
            this.logger.LogError(
                ex,
                "Failed to map object {SourceType} to {DestinationType}.",
                typeof(TSrc).FullName,
                typeof(TDest).FullName);

            throw new MappingException("Failed to map object.", ex);
        }
    }

    public IEnumerable<TDest> Map<TDest>(IEnumerable<TSrc> source)
    {
        try
        {
            using var scope = MapContextScope.RequiresNew();
            return this.mapper.Map<IEnumerable<TSrc>, IEnumerable<TDest>>(source);
        }
        catch (Exception ex)
        {
            this.logger.LogError(
                ex,
                "Failed to map object collection {SourceType} to {DestinationType}.",
                typeof(TSrc).FullName,
                typeof(TDest).FullName);

            throw new MappingException("Failed to map object collection.", ex);
        }
    }
}
