namespace Digital5HP.AspNetCore.Swagger;

using System;

using Asp.Versioning.ApiExplorer;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

/// <summary>
/// Configures the Swagger generation options.
/// </summary>
/// <remarks>This allows API versioning to define a Swagger document per API version after the
/// <see cref="IApiVersionDescriptionProvider"/> service has been resolved from the service container.</remarks>
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    /// <summary>
    /// The string that is appended to the name of a Swagger document to indicate it is for internal use only.
    /// </summary>
    public const string INTERNAL_DOC_NAME_SUFFIX = "-internal";

    private readonly IApiVersionDescriptionProvider provider;
    private readonly ServiceConfiguration serviceConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
    /// </summary>
    /// <param name="serviceConfigurationOptions">The configuration to use to generate Swagger documents.</param>
    /// <param name="provider">The <see cref="IApiVersionDescriptionProvider"/> with the APIs for which to generate Swagger documents.</param>
    public ConfigureSwaggerOptions(IOptions<ServiceConfiguration> serviceConfigurationOptions,
                                   IApiVersionDescriptionProvider provider)
    {
        ArgumentNullException.ThrowIfNull(serviceConfigurationOptions);

        this.serviceConfiguration = serviceConfigurationOptions.Value;
        this.provider = provider;
    }

    /// <summary>
    /// Configures the specified <see cref="SwaggerGenOptions"/> such that a public document and an internal
    /// document are generated for each API version. Also sets the title of each document to the value specified
    /// in the <see cref="IConfiguration"/>.
    /// </summary>
    /// <param name="options">The <see cref="SwaggerGenOptions"/> to configure.</param>
    public void Configure(SwaggerGenOptions options)
    {
        // Enable Swagger attributes
        options.EnableAnnotations(enableAnnotationsForInheritance: true, enableAnnotationsForPolymorphism: true);

        // Add an internal and a public Swagger document for each discovered API version
        var title = this.serviceConfiguration.Title;
        foreach (var description in this.provider.ApiVersionDescriptions)
        {
            var info = this.CreateInfoForApiVersion(title, description);
            options.SwaggerDoc($"{description.GroupName}{INTERNAL_DOC_NAME_SUFFIX}", info);
            options.SwaggerDoc(description.GroupName, info);
        }
    }

    /// <summary>
    /// Creates an <see cref="OpenApiInfo"/> object containing metadata about the specified <see cref="ApiVersionDescription"/>.
    /// </summary>
    /// <param name="title">The title of the API.</param>
    /// <param name="description">The description of the API version for which to create an <see cref="OpenApiInfo"/> object.</param>
    /// <returns>An <see cref="OpenApiInfo"/> object containing metadata about the specified <paramref name="description"/>.</returns>
    private OpenApiInfo CreateInfoForApiVersion(string title, ApiVersionDescription description)
    {
        var info = new OpenApiInfo
        {
            Title = title,
            Version = description.ApiVersion.ToString(),
            Description = "",
        };

        if (this.serviceConfiguration.OpenApi != null)
        {
            if (this.serviceConfiguration.OpenApi.ContactEmail != null || this.serviceConfiguration.OpenApi.ContactName != null || this.serviceConfiguration.OpenApi.ContactUrl != null)
            {

                info.Contact = new OpenApiContact
                {
                    Name = this.serviceConfiguration.OpenApi.ContactName,
                    Email = this.serviceConfiguration.OpenApi.ContactEmail,
                    Url = this.serviceConfiguration.OpenApi.ContactUrl,
                };
            }

            if (this.serviceConfiguration.OpenApi.LicenseName != null || this.serviceConfiguration.OpenApi.LicenceUrl != null)
            {
                info.License = new OpenApiLicense
                {
                    Name = this.serviceConfiguration.OpenApi.LicenseName,
                    Url = this.serviceConfiguration.OpenApi.LicenceUrl,
                };
            }

            if (this.serviceConfiguration.OpenApi.TermsOfServiceUrl != null)
            {
                info.TermsOfService = this.serviceConfiguration.OpenApi.TermsOfServiceUrl;
            }
        }

        if (description.IsDeprecated)
        {
            info.Description = string.Join(" ", "[Deprecated] ", info.Description);
        }

        return info;
    }
}
