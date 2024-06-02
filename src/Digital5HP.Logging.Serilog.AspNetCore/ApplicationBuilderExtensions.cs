namespace Digital5HP.Logging.Serilog.AspNetCore;

using global::Serilog;

using Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds Serilog request logging to ASP.NET Core pipeline.
    /// </summary>
    /// <param name="applicationBuilder"><see cref="IApplicationBuilder"/> from Configure method in Startup.cs</param>
    public static IApplicationBuilder UseSerilog(this IApplicationBuilder applicationBuilder)
    {
        return applicationBuilder.UseSerilogRequestLogging();
    }
}
