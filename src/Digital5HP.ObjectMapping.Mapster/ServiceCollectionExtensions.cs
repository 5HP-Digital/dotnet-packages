namespace Digital5HP.ObjectMapping.Mapster;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using global::Mapster;

using MapsterMapper;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Digital5HP.Logging;

public static class ServiceCollectionExtensions
{
    private static readonly ICollection<Action<TypeAdapterConfig>> OptionsActions = [];

    /// <summary>
    /// Registers all mapping object interfaces using Mapster (without loading any mapping profiles).
    /// </summary>
    public static IServiceCollection AddObjectMappingCore(this IServiceCollection services, Action<TypeAdapterConfig> options = null)
    {
        services.TryAddTransient<IMapperProvider, MapperProvider>();

        if (options != null) OptionsActions.Add(options);

        services.TryAddSingleton(
            sp =>
            {
                var logger = sp.GetRequiredService<ILogger>();

                logger.LogDebug("Loading Mapster mapping profiles...");
                var config = new TypeAdapterConfig
                             {
                                 RequireExplicitMapping = true,
                                 RequireDestinationMemberSource = true
                             };
                config.Default.Settings.AvoidInlineMapping = true;

                // apply options
                foreach (var optionsAction in OptionsActions)
                {
                    optionsAction.Invoke(config);
                }

                // register profiles
                ConfigureProfiles(config);

                logger.LogDebug("Registered Mapster mapping profiles.");

                return config;
            });
        services.TryAddScoped<IMapper, ServiceMapper>();

        return services;
    }

    /// <summary>
    /// Registers all mapping object interfaces using Mapster and loads all mapping profiles in the <see cref="Assembly"/> provided.
    /// </summary>
    /// <remarks>
    /// If no <see cref="Assembly"/> provided, the calling assembly of this method is used.
    /// </remarks>
    public static IServiceCollection AddObjectMapping(this IServiceCollection services, Assembly assembly = null, Action<TypeAdapterConfig> options = null)
    {
        services.AddObjectMappingCore(options);

        TypeInfo[] types;
        try
        {
            assembly ??= Assembly.GetCallingAssembly();
            types = assembly.DefinedTypes.ToArray();
        }
        catch (ReflectionTypeLoadException ex)
        {
            types = ex.Types.Where(t => t != null)
                      .Select(t => t.GetTypeInfo())
                      .ToArray();
        }

        var profiles = GetProfiles(types);

        foreach (var (srcType, profileType) in profiles)
        {
            GlobalRegistration.AddProfile(srcType, profileType);

            services.AddTransient(
                typeof(IMapper<>).MakeGenericType(srcType),
                typeof(Mapper<>).MakeGenericType(srcType));
        }

        return services;
    }

    private static Dictionary<Type, Type> GetProfiles(IEnumerable<TypeInfo> assemblyTypes)
    {
        var profiles = new Dictionary<Type, Type>();

        var mappingProfileType = typeof(MappingProfile<>);

        foreach (var type in assemblyTypes.Where(
            t => !t.IsAbstract
                 && !t.IsGenericTypeDefinition
                 && t.GetConstructor(Type.EmptyTypes) != null
                 && t.IsSubclassOfGenericType(mappingProfileType)))
        {
            var typeArguments = type.GetGenericArgumentsOfSubclass(mappingProfileType);
            if (typeArguments == null || typeArguments.Length != 1)
            {
                throw new MappingConfigurationException(
                    $"Mapping profile must have a single generic type argument ({type.Name}).");
            }

            var srcType = typeArguments[0];

            if (!profiles.TryAdd(srcType, type))
            {
                throw new MappingConfigurationException(
                    $"Mapping profile for type '{srcType.Name}' already exists.");
            }
        }

        return profiles;
    }

    private static void ConfigureProfiles(TypeAdapterConfig config)
    {
        var mappingConfigurationType = typeof(MappingConfiguration<>);
        foreach (var (srcType, profileType) in GlobalRegistration.Profiles)
        {
            var configType = mappingConfigurationType.MakeGenericType(srcType);
            var mappingConfig = Activator.CreateInstance(configType, config);

            var configureInternalMethodInfo = profileType.GetMethod(
                nameof(MappingProfile<object>.ConfigureInternal),
                BindingFlags.Instance | BindingFlags.NonPublic)
                ?? throw new MappingConfigurationException($"'{profileType.Name}' does not contain an internal method '{nameof(MappingProfile<object>.ConfigureInternal)}'.");

            var profile = Activator.CreateInstance(profileType);

            configureInternalMethodInfo.Invoke(profile, [mappingConfig]);
        }
    }
}
