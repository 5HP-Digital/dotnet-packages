namespace Digital5HP.Logging.Serilog;

using System;

using global::Serilog;
using global::Serilog.Configuration;

using Digital5HP.Logging.Serilog.Enrichers;

public static class LoggerEnrichmentConfigurationExtensions
{
    public static LoggerConfiguration WithSensitiveDataSanitized(
        this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        ArgumentNullException.ThrowIfNull(enrichmentConfiguration);

        return enrichmentConfiguration.With<SensitiveDataEnricher>();
    }
}
