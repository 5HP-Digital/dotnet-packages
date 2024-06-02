namespace Digital5HP.Analyzers.Rules;

using System;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using Digital5HP.Analyzers.Internals;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class OverrideTypeIdForAllowMultipleValidationAttributeAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        RuleIdentifiers.OVERRIDE_TYPEID_VALIDATION_ATTRIBUTE,
        title: "Override TypeId property for ValidationAttribute with AllowMultiple",
        messageFormat: "Override TypeId property",
        RuleCategories.DESIGN,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "",
        helpLinkUri: RuleIdentifiers.GetHelpUri(RuleIdentifiers.OVERRIDE_TYPEID_VALIDATION_ATTRIBUTE));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze);

        context.RegisterSymbolAction(Analyze, SymbolKind.NamedType);
    }

    private static void Analyze(SymbolAnalysisContext context)
    {
        var validationAttributeType = context.Compilation.GetTypeByMetadataName("System.ComponentModel.DataAnnotations.ValidationAttribute");
        var attributeUsageAttributeType = context.Compilation.GetTypeByMetadataName("System.AttributeUsageAttribute");
        if (validationAttributeType == null || attributeUsageAttributeType == null)
            return;

        var symbol = (INamedTypeSymbol)context.Symbol;
        if (symbol.IsAbstract)
            return;

        if (!symbol.InheritsFrom(validationAttributeType))
            return;

        if (HasAttributeUsageAttributeWithAllowMultiple(symbol, validationAttributeType, attributeUsageAttributeType)
            && !HasProperty(symbol, IsTypeIdProperty))
            context.ReportDiagnostic(Rule, symbol);
    }

    private static bool HasAttributeUsageAttributeWithAllowMultiple(INamedTypeSymbol? symbol, ITypeSymbol validationAttributeSymbol, ITypeSymbol attributeUsageSymbol)
    {
        while (symbol != null && !symbol.IsEqualTo(validationAttributeSymbol))
        {
            if (symbol.HasAttribute(
                    attributeUsageSymbol,
                    attrData =>
                    {
                        foreach (var pair in attrData.NamedArguments)
                        {
                            if (pair.Key == nameof(AttributeUsageAttribute.AllowMultiple) && pair.Value.Value is true)
                                return true;
                        }

                        return false;
                    }))
                return true;

            symbol = symbol.BaseType;
        }

        return false;
    }

    private static bool IsTypeIdProperty(IPropertySymbol symbol)
    {
        return symbol.Name == nameof(Attribute.TypeId)
               && symbol.Type.IsObject()
               && symbol.DeclaredAccessibility == Accessibility.Public
               && !symbol.IsStatic;
    }

    private static bool HasProperty(INamedTypeSymbol parentType, Func<IPropertySymbol, bool> predicate)
    {
        return parentType.GetMembers()
                         .OfType<IPropertySymbol>()
                         .Any(predicate);
    }
}
