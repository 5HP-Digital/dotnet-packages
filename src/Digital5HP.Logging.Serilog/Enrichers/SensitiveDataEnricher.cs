namespace Digital5HP.Logging.Serilog.Enrichers;

using System;
using System.Linq;
using System.Text.RegularExpressions;

using global::Serilog.Core;
using global::Serilog.Events;

public class SensitiveDataEnricher : ILogEventEnricher
{
    // TODO: improve sensitive data property search
    private static readonly Regex SensitivePropertyNameRegex =
        new(
            "^.*(password)|(token)|(secretkey).*$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture,
            TimeSpan.FromSeconds(1));

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        ArgumentNullException.ThrowIfNull(logEvent);

        logEvent.Properties.Keys.Where(k => SensitivePropertyNameRegex.IsMatch(k))
                .ToList()
                .ForEach(
                     k =>
                     {
                         var val = logEvent.Properties[k]
                                           .ToString();
                         var sanitize = Sanitize(val);
                         logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(k, sanitize));
                     });
    }

    private static string Sanitize(string val)
    {
        return val == null
            ? null
            : new string('*', 10);
    }
}
