namespace Digital5HP.ObjectMapping.Mapster;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using global::Mapster;

using Digital5HP.Lookups;

public static class EnumMapping
{
    public static string ConvertToCode<TEnum, TService, TDomain>(TEnum? type,
                                                                  Func<TService, Task<IEnumerable<TDomain>>>
                                                                      domainTypesProvider)
        where TEnum : struct, Enum
        where TDomain : ILookupObject<TEnum>
    {
        return type == null ? null : ConvertToCode(type.Value, domainTypesProvider);
    }

    public static string ConvertToCode<TEnum, TService, TDomain>(TEnum type,
                                                                  Func<TService, Task<IEnumerable<TDomain>>>
                                                                      domainTypesProvider)
        where TEnum : struct, Enum
        where TDomain : ILookupObject<TEnum>
    {
        return ConvertToObject(type, domainTypesProvider)
          ?.Code;
    }

    public static TDomain ConvertToObject<TEnum, TService, TDomain>(TEnum type,
                                                                 Func<TService, Task<IEnumerable<TDomain>>>
                                                                     domainTypesProvider)
        where TEnum : struct, Enum
        where TDomain : ILookupObject<TEnum>
    {
        ArgumentNullException.ThrowIfNull(domainTypesProvider);

        var types = ConvertToObjectInternal(domainTypesProvider);

        return types != null ? types.FirstOrDefault(t => t.EnumValue.Equals(type)) : default;
    }

    public static IEnumerable<TDomain> ConvertToObject<TEnum, TService, TDomain>(IEnumerable<TEnum> enums,
                                                                 Func<TService, Task<IEnumerable<TDomain>>>
                                                                     domainTypesProvider)
        where TEnum : struct, Enum
        where TDomain : ILookupObject<TEnum>
    {
        ArgumentNullException.ThrowIfNull(domainTypesProvider);

        var types = ConvertToObjectInternal(domainTypesProvider);

        return enums.Select(e => types.FirstOrDefault(t => t.EnumValue.Equals(e)))
                    .ToList();
    }

    public static TDomain ConvertToObject<TEnum, TService, TDomain>(TEnum? type,
                                                                 Func<TService, Task<IEnumerable<TDomain>>>
                                                                     domainTypesProvider)
        where TEnum : struct, Enum
        where TDomain : ILookupObject<TEnum>
    {
        return type == null ? default : ConvertToObject(type.Value, domainTypesProvider);
    }

    private static IEnumerable<TDomain> ConvertToObjectInternal<TService, TDomain>(
        Func<TService, Task<IEnumerable<TDomain>>> domainTypesProvider)
    {
        var service = MapContext.Current.GetService<TService>();

        return AsyncHelper.RunSync(() => domainTypesProvider(service) );
    }
}
