namespace Digital5HP.AspNetCore.Swagger;

using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures the application to use Swagger and Swagger UI and creates endpoints for all configured Swagger documents.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> to tell to use Swagger and Swagger UI.</param>
    /// <returns>A reference to the updated <paramref name="app"/> after the operation completes.</returns>
    public static IApplicationBuilder UseSwaggerWithUi(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(
            options =>
            {
                var genOptions =
                    app.ApplicationServices.GetRequiredService<IOptions<SwaggerGenOptions>>();

                // Build a Swagger endpoint for each Swagger document
                foreach (var docName in genOptions.Value.SwaggerGeneratorOptions.SwaggerDocs.Keys)
                {
                    options.SwaggerEndpoint($"/swagger/{docName}/swagger.json", docName);
                }

                options.SupportedSubmitMethods(Array.Empty<SubmitMethod>());
            });

        return app;
    }
}
