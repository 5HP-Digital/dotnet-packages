namespace Digital5HP.Logging.Analyzers.Internals;

using System;
using System.Collections.Generic;
using System.Linq;

internal static class PropertyBindingHelper
{
    public static IEnumerable<MessageTemplateDiagnostic> AnalyzeProperties(List<PropertyToken> propertyTokens,
                                                                           List<SourceArgument> arguments)
    {
        if (propertyTokens.Count > 0)
        {
            var allPositional = true;
            var anyPositional = false;
            foreach (var propertyToken in propertyTokens)
            {
                if (propertyToken.IsPositional)
                    anyPositional = true;
                else
                    allPositional = false;
            }

            if (allPositional)
            {
                return AnalyzePositionalProperties(propertyTokens, arguments);
            }

            var diagnostics = new List<MessageTemplateDiagnostic>();
            if (anyPositional)
            {
                diagnostics.AddRange(
                    propertyTokens.Where(pt => pt.IsPositional)
                                  .Select(
                                       pt => new MessageTemplateDiagnostic(
                                           pt.StartIndex,
                                           pt.Length,
                                           "Positional properties are not allowed, when named properties are being used")));
            }

            AnalyzeNamedProperties(diagnostics, propertyTokens, arguments);

            return diagnostics;
        }

        if (arguments.Count > 0)
        {
            return arguments.Select(
                                 argument => new MessageTemplateDiagnostic(
                                     argument.StartIndex,
                                     argument.Length,
                                     "There is no property that corresponds to this argument",
                                     false))
                            .ToList();
        }

        return Array.Empty<MessageTemplateDiagnostic>();
    }

    private static IEnumerable<MessageTemplateDiagnostic> AnalyzePositionalProperties(List<PropertyToken> positionalProperties,
                                                    IReadOnlyList<SourceArgument> arguments)
    {
        var diagnostics = new List<MessageTemplateDiagnostic>();

        var mapped = new List<KeyValuePair<int, PropertyToken>>();
        foreach (var property in positionalProperties)
        {
            if (property.TryGetPositionalValue(out var position))
            {
                if (position < 0)
                {
                    diagnostics.Add(
                        new MessageTemplateDiagnostic(
                            property.StartIndex,
                            property.Length,
                            "Positional index cannot be negative"));
                }

                if (position >= arguments.Count)
                {
                    diagnostics.Add(
                        new MessageTemplateDiagnostic(
                            property.StartIndex,
                            property.Length,
                            $"There is no argument that corresponds to the positional property {position}"));
                }

                mapped.Add(new KeyValuePair<int, PropertyToken>(position, property));
            }
            else
            {
                diagnostics.Add(
                    new MessageTemplateDiagnostic(
                        property.StartIndex,
                        property.Length,
                        "Couldn't get the position of this property while analyzing"));
            }
        }

        for (var i = 0; i < arguments.Count; ++i)
        {
            var indexMatched = mapped.Any(t => t.Key == i);

            if (!indexMatched)
            {
                diagnostics.Add(
                    new MessageTemplateDiagnostic(
                        arguments[i]
                           .StartIndex,
                        arguments[i]
                           .Length,
                        "There is no positional property that corresponds to this argument",
                        false));
            }
        }

        return diagnostics;
    }

    private static void AnalyzeNamedProperties(ICollection<MessageTemplateDiagnostic> diagnostics,
                                               IReadOnlyList<PropertyToken> namedProperties,
                                               IReadOnlyList<SourceArgument> arguments)
    {
        var matchedRun = Math.Min(namedProperties.Count, arguments.Count);

        // could still possibly work when it hits a name of a contextual property but it's better practice to be explicit at the callsite
        for (var i = matchedRun; i < namedProperties.Count; i++)
        {
            var namedProperty = namedProperties[i];
            diagnostics.Add(
                new MessageTemplateDiagnostic(
                    namedProperty.StartIndex,
                    namedProperty.Length,
                    $"There is no argument that corresponds to the named property '{namedProperty.PropertyName}'"));
        }

        for (var i = matchedRun; i < arguments.Count; i++)
        {
            var argument = arguments[i];
            diagnostics.Add(
                new MessageTemplateDiagnostic(
                    argument.StartIndex,
                    argument.Length,
                    "There is no named property that corresponds to this argument",
                    false));
        }
    }
}
