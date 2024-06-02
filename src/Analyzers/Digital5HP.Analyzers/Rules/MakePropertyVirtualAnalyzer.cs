namespace Digital5HP.Analyzers.Rules;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using Digital5HP.Analyzers.Configurations;
using Digital5HP.Analyzers.Internals;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MakePropertyVirtualAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        RuleIdentifiers.MAKE_PROPERTY_VIRTUAL,
        title: "Make property virtual",
        messageFormat: "Make property virtual",
        RuleCategories.DESIGN,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: false,
        description: "",
        helpLinkUri: RuleIdentifiers.GetHelpUri(RuleIdentifiers.MAKE_PROPERTY_VIRTUAL));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze);

        context.RegisterCompilationStartAction(
            compilationContext =>
            {
                var analyzerContext = new AnalyzerContext();

                compilationContext.RegisterSyntaxNodeAction(
                    analyzerContext.AnalyzeProperty,
                    SyntaxKind.PropertyDeclaration);
                compilationContext.RegisterSymbolAction(
                    analyzerContext.AnalyzeNamedTypeSymbol,
                    SymbolKind.NamedType);
                compilationContext.RegisterCompilationEndAction(analyzerContext.CompilationEnd);
            });
    }

    private sealed class AnalyzerContext
    {
        private readonly List<ITypeSymbol> potentialClasses = new List<ITypeSymbol>();

        [SuppressMessage(
            "MicrosoftCodeAnalysisCorrectness",
            "RS1024:Compare symbols correctly",
            Justification = "False positive")]
        private readonly HashSet<ISymbol> potentialSymbols = new HashSet<ISymbol>(SymbolEqualityComparer.Default);

        public void CompilationEnd(CompilationAnalysisContext context)
        {
            lock (this.potentialSymbols)
            {
                foreach (var symbol in this.potentialSymbols)
                {
                    if (!this.potentialClasses.Contains(symbol.ContainingType))
                        continue;

                    if (symbol is IPropertySymbol property)
                    {
                        if (!TryGetSyntaxTreeForOption(property, out var tree))
                            continue;

                        var referencingPropertiesOnly = context.Options.GetConfigurationValue(
                            tree,
                            $"dotnet_code_quality.{Rule.Id}.referencing_properties_only",
                            false);

                        if (!referencingPropertiesOnly
                            || this.potentialClasses.Contains(GetTypeOrUnderlyingType(property.Type)))
                            context.ReportDiagnostic(Rule, symbol);
                    }
                    else
                    {
                        throw new InvalidOperationException("Symbol is not supported: " + symbol);
                    }
                }
            }
        }

        private static ITypeSymbol GetTypeOrUnderlyingType(ITypeSymbol propertyType)
        {
            var underlyingType = propertyType.GetAllInterfacesIncludingThis()
                                             .FirstOrDefault(
                                                  nts => nts.IsGenericType
                                                         && nts.Name.Contains(
                                                             nameof(IEnumerable),
                                                             StringComparison.Ordinal))
                                            ?.TypeParameters.First();

            return underlyingType ?? propertyType;
        }

        public void AnalyzeNamedTypeSymbol(SymbolAnalysisContext context)
        {
            var symbol = (INamedTypeSymbol)context.Symbol;
            switch (symbol.TypeKind)
            {
                case TypeKind.Class:
                    if (IsPotentialClass(context.Options, symbol))
                    {
                        lock (this.potentialClasses)
                        {
                            this.potentialClasses.Add(symbol);
                        }
                    }
                    break;
            }
        }

        private static bool IsPotentialClass(AnalyzerOptions options, INamedTypeSymbol? symbol)
        {
            if (symbol == null || !TryGetSyntaxTreeForOption(symbol, out var tree))
                return false;

            if (!options.TryGetConfigurationValue(
                tree,
                $"dotnet_code_quality.{Rule.Id}.included_interface_names_with_derived_types",
                out var optionValue))
                return false;

            var symbolMatches = optionValue.Split(
                                        new[]
                                        {
                                            '|'
                                        },
                                        StringSplitOptions.RemoveEmptyEntries)
                                   .Select(ConvertToSymbolMatch)
                                   .ToImmutableArray();

            return symbol.GetAllInterfacesIncludingThis()
                         .Any(baseSymbol => symbolMatches.Any(m => m(baseSymbol.Name)));
        }

        private static Func<string, bool> ConvertToSymbolMatch(string symbolName)
        {
            // doesn't have wildcard, plain starts with
            if (!symbolName.Contains('*', StringComparison.Ordinal))
                return s => s.StartsWith(symbolName, StringComparison.Ordinal);

            // use regex
            var pattern = symbolName.Trim('*');

            if (!symbolName.StartsWith("*", StringComparison.Ordinal))
                pattern = "^" + pattern;

            if (!symbolName.EndsWith("*", StringComparison.Ordinal))
                pattern += "$";

#pragma warning disable CA1307
            pattern = pattern.Replace("*", ".*");
#pragma warning restore CA1307

            return s => Regex.IsMatch(s, pattern, RegexOptions.ExplicitCapture);
        }

        private static bool IsPotentialVirtual(IPropertySymbol? property)
        {
            return property is
            {
                IsReadOnly: false,
                IsAbstract: false,
                IsStatic: false,
                IsVirtual: false,
                DeclaredAccessibility: Accessibility.Public
            };
        }

        public void AnalyzeProperty(SyntaxNodeAnalysisContext context)
        {
            var node = (PropertyDeclarationSyntax)context.Node;
            var propertySymbol = context.SemanticModel.GetDeclaredSymbol(node, context.CancellationToken);
            if (propertySymbol == null)
                return;

            if (!IsPotentialVirtual(propertySymbol))
                return;

            lock (this.potentialSymbols)
            {
                this.potentialSymbols.Add(propertySymbol);
            }
        }

        private static bool TryGetSyntaxTreeForOption(ISymbol? symbol, [NotNullWhen(true)] out SyntaxTree? tree)
        {
            switch (symbol?.Kind)
            {
                case SymbolKind.Assembly:
                case SymbolKind.Namespace when ((INamespaceSymbol)symbol).IsGlobalNamespace:
                case null:
                    tree = null;
                    return false;
                case SymbolKind.Parameter:
                    return TryGetSyntaxTreeForOption(symbol.ContainingSymbol, out tree);
                default:
                    tree = symbol.Locations[0]
                                 .SourceTree;
                    return tree != null;
            }
        }
    }
}
