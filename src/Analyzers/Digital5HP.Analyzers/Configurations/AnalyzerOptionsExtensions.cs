﻿namespace Digital5HP.Analyzers.Configurations;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

public static class AnalyzerOptionsExtensions
{
    public static string GetConfigurationValue(this AnalyzerOptions options, SyntaxTree syntaxTree, string key, string defaultValue)
    {
        var configuration = options.AnalyzerConfigOptionsProvider.GetOptions(syntaxTree);
        if (configuration.TryGetValue(key, out var value))
            return value;

        return defaultValue;
    }

    public static bool GetConfigurationValue(this AnalyzerOptions options, SyntaxTree syntaxTree, string key, bool defaultValue)
    {
        var configuration = options.AnalyzerConfigOptionsProvider.GetOptions(syntaxTree);
        if (configuration.TryGetValue(key, out var value))
            return ChangeType(value, defaultValue);

        return defaultValue;
    }

    public static bool? GetConfigurationValue(this AnalyzerOptions options, SyntaxTree syntaxTree, string key, bool? defaultValue)
    {
        var configuration = options.AnalyzerConfigOptionsProvider.GetOptions(syntaxTree);
        if (configuration.TryGetValue(key, out var value))
            return ChangeType(value, defaultValue);

        return defaultValue;
    }

    public static int GetConfigurationValue(this AnalyzerOptions options, SyntaxTree syntaxTree, string key, int defaultValue)
    {
        var configuration = options.AnalyzerConfigOptionsProvider.GetOptions(syntaxTree);
        if (configuration.TryGetValue(key, out var value))
            return ChangeType(value, defaultValue);

        return defaultValue;
    }

    public static ReportDiagnostic? GetConfigurationValue(this AnalyzerOptions options, SyntaxTree syntaxTree, string key, ReportDiagnostic? defaultValue)
    {
        var configuration = options.AnalyzerConfigOptionsProvider.GetOptions(syntaxTree);
        if (configuration.TryGetValue(key, out var value))
        {
            if (Enum.TryParse<ReportDiagnostic>(value, ignoreCase: true, out var result))
                return result;
        }

        return defaultValue;
    }

    public static bool GetConfigurationValue(this AnalyzerOptions options, ISymbol symbol, string key, bool defaultValue)
    {
        foreach (var location in symbol.Locations)
        {
            var syntaxTree = location.SourceTree;
            if (syntaxTree != null && options.TryGetConfigurationValue(syntaxTree, key, out var str))
                return ChangeType(str, defaultValue);
        }

        return defaultValue;
    }

    public static bool TryGetConfigurationValue(this AnalyzerOptions options, SyntaxTree syntaxTree, string key, [NotNullWhen(true)] out string? value)
    {
        var configuration = options.AnalyzerConfigOptionsProvider.GetOptions(syntaxTree);
        return configuration.TryGetValue(key, out value);
    }

    public static bool TryGetConfigurationValue(this AnalyzerOptions options, IOperation operation, string key, [NotNullWhen(true)] out string? value)
    {
        return TryGetConfigurationValue(options, operation.Syntax.SyntaxTree, key, out value);
    }

    private static bool ChangeType(string value, bool defaultValue)
    {
        if (bool.TryParse(value, out var result))
            return result;

        return defaultValue;
    }

    private static bool? ChangeType(string value, bool? defaultValue)
    {
        if (bool.TryParse(value, out var result))
            return result;

        return defaultValue;
    }

    private static int ChangeType(string value, int defaultValue)
    {
        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
            return result;

        return defaultValue;
    }
}
