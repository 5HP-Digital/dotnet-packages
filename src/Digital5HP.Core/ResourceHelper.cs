namespace Digital5HP;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

public static class ResourceHelper
{
    /// <summary>
    /// Gets a stream for an embedded resource from the provided assembly.
    /// If <paramref name="assembly"/> is not provided, currently executing assembly is used.
    /// </summary>
    public static Stream GetManifestResource(string resourceName, Assembly assembly = null)
    {
        assembly ??= Assembly.GetExecutingAssembly();
        var resource = assembly.GetManifestResourceNames()
                               .SingleOrDefault(
                                    name => name.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase));

        if (resource == null) throw new AppCoreException($"Resource '{resourceName}' not found.");

        return assembly.GetManifestResourceStream(resource);
    }

    /// <summary>
    /// Reads the content of an embedded resource as a string from the provided assembly.
    /// If <paramref name="assembly"/> is not provided, currently executing assembly is used.
    /// </summary>
    public static string GetManifestResourceAsString(string resourceName, Assembly assembly = null)
    {
        using var stream = GetManifestResource(resourceName, assembly);

        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }

    /// <summary>
    /// Reads the content of an embedded resource as a string from the provided assembly.
    /// If <paramref name="assembly"/> is not provided, currently executing assembly is used.
    /// </summary>
    public static async Task<string> GetManifestResourceAsStringAsync(string resourceName, Assembly assembly = null)
    {
        await using var stream = GetManifestResource(resourceName, assembly);

        using var reader = new StreamReader(stream);

        return await reader.ReadToEndAsync();
    }
}
