namespace Digital5HP.AspNetCore.Swagger;

using System;

using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

public class ModelSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(schema);

        ArgumentNullException.ThrowIfNull(context);

        // To replace the full name with namespace with the class name only
        schema.Title = context.Type.Name;
    }
}
