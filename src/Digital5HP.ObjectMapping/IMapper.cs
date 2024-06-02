
namespace Digital5HP.ObjectMapping;

using System.Collections.Generic;

public interface IMapper<in TSrc>
{
    /// <summary>
    /// Maps the object provided to the destination type.
    /// </summary>
    /// <exception cref="MappingException">Mapping exception occurred.</exception>
    TDest Map<TDest>(TSrc source);

    /// <summary>
    /// Maps all members of the object collection provided to the destination type.
    /// </summary>
    /// <exception cref="MappingException">Mapping exception occurred.</exception>
    IEnumerable<TDest> Map<TDest>(IEnumerable<TSrc> source);
}
