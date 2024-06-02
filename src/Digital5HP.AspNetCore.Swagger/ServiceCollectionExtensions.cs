namespace Digital5HP.AspNetCore.Swagger;

using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Asp.Versioning.ApiExplorer;

using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up Swagger documentation services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds services for generating Swagger documentation to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which to add services for generating Swagger documentation.</param>
    /// <param name="configuration"></param>
    /// <returns>A reference to the updated <paramref name="services"/> after the operation completes.</returns>
    public static IServiceCollection AddSwagger(this IServiceCollection services,
                                                IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (services.All(sd => sd.ServiceType != typeof(IApiVersionDescriptionProvider)))
            throw new AppCoreException($"{nameof(IApiVersionDescriptionProvider)} is not registered. Make sure API Versioning is enabled when using Swagger.");

        services.Configure<ServiceConfiguration>(configuration.GetSection("Swagger"));
        services.ConfigureOptions<ConfigureSwaggerOptions>();
        services.AddSwaggerGen(
            options =>
            {
                // Add a custom operation filter which sets default values
                options.OperationFilter<SwaggerDefaultValues>();

                // Integrate XML comments if they exist
                var xmlCommentsFilePath = GetXmlCommentsFilePath();

                if (File.Exists(xmlCommentsFilePath))
                {
                    options.IncludeXmlComments(xmlCommentsFilePath, true);
                }

                // Filter endpoints by version and whether or not they are publicly accessible
                options.DocInclusionPredicate(IncludeApiInDoc);

                options.CustomSchemaIds(t => t.FullName);
                options.SchemaFilter<ModelSchemaFilter>();
            });

        return services;
    }

    /// <summary>
    /// Gets the file path of the XML documentation file for the assembly in which the specified <see cref="Type"/> is defined.
    /// </summary>
    /// <returns>The file path of the XML documentation file for the assembly in which the specified <see cref="Type"/> is defined.</returns>
    private static string GetXmlCommentsFilePath()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly == null)
        {
            throw new AppCoreException("Could not resolve entry assembly.");
        }

        var fileName = $"{entryAssembly.GetName().Name}.xml";

        return Path.Combine(AppContext.BaseDirectory, fileName);
    }

    /// <summary>
    /// Returns a flag indicating whether the specified <see cref="ApiDescription"/> should be included in the specified Swagger doc.
    /// </summary>
    /// <param name="docName">The name of the Swagger doc for which to determine whether the specified <paramref name="apiDescription"/> should be included.</param>
    /// <param name="apiDescription">The <see cref="ApiDescription"/> to determine whether it belongs in the specified Swagger doc.</param>
    /// <returns>
    /// <para>
    /// If the Swagger doc is internal, returns <see langword="true"/> if <paramref name="docName"/> begins with the
    /// <see cref="ApiDescription.GroupName"/> of the specified <paramref name="apiDescription"/>; otherwise, <see langword="false"/>.
    /// </para>
    /// <para>
    /// If the Swagger doc is public, returns <see langword="true"/> if <paramref name="docName"/> matches the
    /// <see cref="ApiDescription.GroupName"/> of the specified <paramref name="apiDescription"/> and the specified
    /// <paramref name="apiDescription"/> is public (see remarks); otherwise, <see langword="false"/>.
    /// </para>
    /// </returns>
    /// <remarks>An <see cref="ApiDescription"/> is public if the <see cref="PublicEndpointAttribute"/> has been applied to it.</remarks>
    private static bool IncludeApiInDoc(string docName, ApiDescription apiDescription)
    {
        // If the doc is internal, only ensure the API version matches
        if (docName.EndsWith(ConfigureSwaggerOptions.INTERNAL_DOC_NAME_SUFFIX, StringComparison.Ordinal))
        {
            var docVersion =  docName[..^ConfigureSwaggerOptions.INTERNAL_DOC_NAME_SUFFIX.Length];
            return docVersion == apiDescription.GroupName;
        }

        // If the doc is public, ensure the API version matches and the API has the PublicEndpointAttribute
        return docName == apiDescription.GroupName && IsPublicEndpoint(apiDescription);
    }

    /// <summary>
    /// Returns a flag indicating whether the <see cref="PublicEndpointAttribute"/> has been applied to the specified <see cref="ApiDescription"/>.
    /// </summary>
    /// <param name="apiDescription">The <see cref="ApiDescription"/> to inspect and determine whether the <see cref="PublicEndpointAttribute"/> has been applied.</param>
    /// <returns><see langword="true"/> if the <see cref="PublicEndpointAttribute"/> has been applied to the specified <paramref name="apiDescription"/>; otherwise, <see langword="false"/>.</returns>
    private static bool IsPublicEndpoint(ApiDescription apiDescription)
    {
        return apiDescription.ActionDescriptor.EndpointMetadata.OfType<PublicEndpointAttribute>()
                             .Any();
    }
}
