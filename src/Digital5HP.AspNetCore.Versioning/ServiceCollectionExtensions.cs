namespace Digital5HP.AspNetCore.Versioning;

using System;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiVersioning(this IServiceCollection services, Action<VersioningOptions> configure = null)
    {
        var options = new VersioningOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);

        services.AddApiVersioning(opts =>
        {
            opts.ReportApiVersions = true;
            opts.ApiVersionReader = new HeaderApiVersionReader("X-Api-Header");
            opts.AssumeDefaultVersionWhenUnspecified = true;
            opts.DefaultApiVersion = ApiVersionConverter.Convert(options.CurrentVersion);
        })
        .AddMvc(
             opts =>
             {
                 opts.Conventions = new IntroducedApiVersionConventionBuilder(
                     options.FirstSupportedVersion,
                     options.CurrentVersion);
             })
        .AddApiExplorer(
             opts =>
             {
                 opts.GroupNameFormat = "'v'VVV";

                 opts.SubstituteApiVersionInUrl = true;
             });

        services.TryAddEnumerable(
            ServiceDescriptor.Transient<IApplicationModelProvider, ApplicationModelApiVersionDiscoveryProvider>());

        return services;
    }
}
