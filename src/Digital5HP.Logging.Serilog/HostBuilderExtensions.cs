namespace Digital5HP.Logging.Serilog;

using System;

using global::Serilog;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public static class HostBuilderExtensions
{
    /// <summary>Sets Serilog as the logging provider.</summary>
    /// <param name="builder">The web host builder to configure.</param>
    /// <returns>The web host builder.</returns>
    public static IHostBuilder UseSerilogForLogging(this IHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // register logging using Serilog hosting extension
        builder.UseSerilog()
               .ConfigureServices(
                    (_, collection) =>
                    {
                        // add Shared.Logging
                        collection.AddLoggingCore();

                        // add Serilog as logger provider in .NET Core logging extension
                        collection.AddLogging(
                            loggingBuilder =>
                                loggingBuilder.AddSerilog(dispose: true));
                    });

        return builder;
    }
}
