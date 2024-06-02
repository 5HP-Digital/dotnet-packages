namespace Digital5HP.Logging.Serilog;

using System;
using System.IO;

using global::Serilog;
using global::Serilog.Events;
using global::Serilog.Exceptions;

using Microsoft.Extensions.Hosting;

// TODO: refactor to more modular design, possible builder pattern
public static class LoggerFactory
{
    /// <summary>
    /// Creates <see cref="ILogger"/>.
    /// </summary>
    /// <param name="applicationName">Application name</param>
    /// <param name="environment">Environment name</param>
    /// <param name="options">Logging options</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static ILogger CreateLogger(string applicationName, string environment, LoggingOptions options)
    {
        ArgumentNullException.ThrowIfNull(applicationName);
        ArgumentNullException.ThrowIfNull(options);

        // create and configure logger
        var loggerConfiguration = new LoggerConfiguration();

        loggerConfiguration
           .Enrich.FromLogContext()
           .Enrich.WithMachineName()
           .Enrich.WithProperty("Application", applicationName)
           .Enrich.WithExceptionDetails();

        if (environment == Environments.Development)
        {
            const string TEMPLATE =
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {MachineName} ({SourceContext}) {Message:lj}{NewLine}{Exception}";
            const int FILE_SIZE_LIMIT_BYTES = 26214400;
            const int RETAINED_FILE_COUNT_LIMIT = 90;

#pragma warning disable CA1305
            loggerConfiguration.MinimumLevel.Debug()
                               .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                               .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                               .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Debug)
                               .WriteTo.File(
                                    $"/logs/{SanitizeFolderName(applicationName)}/app.log",
                                    outputTemplate: TEMPLATE,
                                    rollingInterval: RollingInterval.Day,
                                    retainedFileCountLimit: RETAINED_FILE_COUNT_LIMIT,
                                    rollOnFileSizeLimit: true,
                                    fileSizeLimitBytes: FILE_SIZE_LIMIT_BYTES,
                                    shared: true)
                               .WriteTo.Console(outputTemplate: TEMPLATE);
#pragma warning restore CA1305
        }
        else
        {
            loggerConfiguration
               .Enrich.WithSensitiveDataSanitized()
               .MinimumLevel.Information()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
               .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
               .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information);
        }

        if (options.LogServerUrl != null)
        {
            //loggerConfiguration.WriteTo.Seq(options.LogServerUrl.ToString());
        }

        return loggerConfiguration.CreateLogger();
    }

    private static string SanitizeFolderName(string name)
    {
        var invalidChars = Path.GetInvalidPathChars();

        return string.Join("_", name.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries))
                     .TrimEnd('.');
    }
}

