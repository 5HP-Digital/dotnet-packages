namespace Digital5HP.ObjectMapping.Mapster;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

internal static class GlobalRegistration
{
    private static readonly ConcurrentDictionary<Type, Type> ProfileDictionary = new();

    public static IEnumerable<KeyValuePair<Type, Type>> Profiles => ProfileDictionary.ToArray();

    internal static void AddProfile(Type destType, Type profileType)
    {
        ProfileDictionary.TryAdd(destType, profileType);
    }
}
