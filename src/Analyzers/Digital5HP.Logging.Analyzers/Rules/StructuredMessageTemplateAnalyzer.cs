namespace Digital5HP.Logging.Analyzers.Rules;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

using Digital5HP.Analyzers.Internals;
using Digital5HP.Logging.Analyzers.Internals;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class StructuredMessageTemplateAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor ExceptionUsageRule = new DiagnosticDescriptor(
        RuleIdentifiers.INCORRECT_EXCEPTION_ARGUMENT_USAGE,
        title: "Incorrect exception argument usage",
        messageFormat: "Incorrect exception argument usage",
        RuleCategories.USAGE,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description:
        "Checks that exceptions are passed to the exception argument, and not as a normal property, with a code fix to correct it.",
        helpLinkUri: RuleIdentifiers.GetHelpUri(RuleIdentifiers.INCORRECT_EXCEPTION_ARGUMENT_USAGE));

    private static readonly DiagnosticDescriptor MessageTemplateSyntaxRule = new DiagnosticDescriptor(
        RuleIdentifiers.USE_VALID_MESSAGE_TEMPLATE_SYNTAX,
        title: "Use valid message template syntax",
        messageFormat: "Use valid message template syntax",
        RuleCategories.USAGE,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description:
        "Checks message templates for correct syntax and emits an error if there's a violation of the templating syntax.",
        helpLinkUri: RuleIdentifiers.GetHelpUri(RuleIdentifiers.USE_VALID_MESSAGE_TEMPLATE_SYNTAX));

    private static readonly DiagnosticDescriptor PropertyBindingRule = new DiagnosticDescriptor(
        RuleIdentifiers.MISMATCH_BETWEEN_MESSAGE_TEMPLATE_TOKENS_AND_ARGUMENTS,
        title: "Property names must match the supplied arguments",
        messageFormat: "Property names must match the supplied arguments",
        RuleCategories.USAGE,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Checks coherence between the message template tokens and the supplied arguments.",
        helpLinkUri: RuleIdentifiers.GetHelpUri(
            RuleIdentifiers.MISMATCH_BETWEEN_MESSAGE_TEMPLATE_TOKENS_AND_ARGUMENTS));

    private static readonly DiagnosticDescriptor ConstantMessageTemplateRule = new DiagnosticDescriptor(
        RuleIdentifiers.USE_PROPER_CONSTANT_STRUCTURED_MESSAGE_TEMPLATE,
        title: "Use proper structured message template",
        messageFormat: "Use proper structured message template",
        RuleCategories.USAGE,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description:
        "Checks that message templates are constant strings. This ensures that events with different data/format arguments can still be detected as instances of the same event.",
        helpLinkUri: RuleIdentifiers.GetHelpUri(RuleIdentifiers.USE_PROPER_CONSTANT_STRUCTURED_MESSAGE_TEMPLATE));

    private static readonly DiagnosticDescriptor UniquePropertyRule = new DiagnosticDescriptor(
        RuleIdentifiers.USE_UNIQUE_PROPERTY_NAMES,
        title: "Use unique property names",
        messageFormat: "Use unique property names",
        RuleCategories.USAGE,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Checks that all property names in a message template are unique.",
        helpLinkUri: RuleIdentifiers.GetHelpUri(RuleIdentifiers.USE_UNIQUE_PROPERTY_NAMES));

    private static readonly DiagnosticDescriptor PascalCasePropertyRule = new DiagnosticDescriptor(
        RuleIdentifiers.USE_PASCAL_CASE_PROPERTY_NAMES,
        title: "Use pascal case property names",
        messageFormat: "Use pascal case property names",
        RuleCategories.USAGE,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Checks that all property names in a message template are PascalCased.",
        helpLinkUri: RuleIdentifiers.GetHelpUri(RuleIdentifiers.USE_PASCAL_CASE_PROPERTY_NAMES));

    private static readonly DiagnosticDescriptor AnonymousObjectsRule = new DiagnosticDescriptor(
        RuleIdentifiers.ANONYMOUS_OBJECTS_SHOULD_BE_DESTRUCTURED,
        title: "Use destructuring for anonymous objects",
        messageFormat: "Use destructuring for anonymous objects",
        RuleCategories.USAGE,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Checks that all anonymous objects passed to the logger are destructured.",
        helpLinkUri: RuleIdentifiers.GetHelpUri(RuleIdentifiers.ANONYMOUS_OBJECTS_SHOULD_BE_DESTRUCTURED));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        ExceptionUsageRule,
        MessageTemplateSyntaxRule,
        PropertyBindingRule,
        ConstantMessageTemplateRule,
        UniquePropertyRule,
        PascalCasePropertyRule,
        AnonymousObjectsRule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze);

        context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        var info = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken);
        if (info.Symbol is not IMethodSymbol method)
        {
            return;
        }

        // is Digital5HP.Logging even present in the compilation?
        var messageTemplateAttribute = context.SemanticModel.Compilation.GetTypeByMetadataName("Digital5HP.Logging.StructuredMessageTemplateMethodAttribute");
        if (messageTemplateAttribute == null)
        {
            return;
        }

        // is it a logging method?
        var attributes = method.GetAttributes();
        var attributeData = attributes.FirstOrDefault(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, messageTemplateAttribute));
        if (attributeData == null)
        {
            return;
        }

        var messageTemplateName = attributeData.ConstructorArguments.First().Value as string;

        // check for errors in the MessageTemplate
        var arguments = new List<SourceArgument>();
        var properties = new List<PropertyToken>();
        var hasErrors = false;
        var literalSpan = default(TextSpan);
        var exactPositions = true;
        string stringText = "";
        var invocationArguments = invocation.ArgumentList.Arguments;

        foreach (var argument in invocationArguments)
        {
            var parameter = argument.DetermineParameter(context.SemanticModel, true, context.CancellationToken);

            if(parameter == null || parameter.Name != messageTemplateName) continue;

            string messageTemplate;

            // is it a simple string literal?
            if (argument.Expression is LiteralExpressionSyntax literal)
            {
                stringText = literal.Token.Text;
                exactPositions = true;

                messageTemplate = literal.Token.ValueText;
            }
            else
            {
                // can we at least get a computed constant value for it?
                var constantValue = context.SemanticModel.GetConstantValue(argument.Expression, context.CancellationToken);
                if (!constantValue.HasValue || constantValue.Value is not string constString)
                {
                    INamedTypeSymbol? StringType() =>
                        context.SemanticModel.Compilation.GetTypeByMetadataName(typeof(string).FullName!);

                    if (context.SemanticModel.GetSymbolInfo(argument.Expression, context.CancellationToken)
                               .Symbol is IFieldSymbol field
                        && field.Name == "Empty"
                        && SymbolEqualityComparer.Default.Equals(field.Type, StringType()))
                    {
                        constString = "";
                    }
                    else
                    {
                        context.ReportDiagnostic(ConstantMessageTemplateRule, argument.Expression);
                        continue;
                    }
                }

                // we can't map positions back from the computed string into the real positions
                exactPositions = false;
                messageTemplate = constString;
            }

            literalSpan = argument.Expression.GetLocation()
                                  .SourceSpan;

            var messageTemplateDiagnostics = MessageTemplateParser.Tokenize(messageTemplate);

            foreach (var templateDiagnostic in messageTemplateDiagnostics)
            {
                if (templateDiagnostic is PropertyToken property)
                {
                    properties.Add(property);
                    continue;
                }

                if (templateDiagnostic is MessageTemplateDiagnostic diagnostic)
                {
                    hasErrors = true;
                    ReportDiagnostic(ref context, ref literalSpan, stringText, exactPositions, MessageTemplateSyntaxRule, diagnostic);
                }
            }

            var messageTemplateArgumentIndex = invocationArguments.IndexOf(argument);
            arguments = invocationArguments.Skip(messageTemplateArgumentIndex + 1)
                                           .Select(
                                                x =>
                                                {
                                                    var location = x.GetLocation()
                                                                    .SourceSpan;
                                                    return new SourceArgument(x, location.Start, location.Length);
                                                })
                                           .ToList();

            break;
        }

        // do properties match up?
        if (!hasErrors && literalSpan != default && (arguments.Count > 0 || properties.Count > 0))
        {
            var diagnostics = PropertyBindingHelper.AnalyzeProperties(properties, arguments);
            foreach (var diagnostic in diagnostics)
            {
                ReportDiagnostic(
                    ref context,
                    ref literalSpan,
                    stringText,
                    exactPositions,
                    PropertyBindingRule,
                    diagnostic);
            }

            // check that all anonymous objects have destructuring hints in the message template
            if (arguments.Count == properties.Count)
            {
                for (var i = 0; i < arguments.Count; i++)
                {
                    var argument = arguments[i];
                    var argumentInfo = context.SemanticModel.GetTypeInfo(
                        argument.Argument.Expression,
                        context.CancellationToken);
                    if (argumentInfo.Type?.IsAnonymousType ?? false)
                    {
                        var property = properties[i];
                        if (!property.RawText.StartsWith("{@", StringComparison.Ordinal))
                        {
                            ReportDiagnostic(
                                ref context,
                                ref literalSpan,
                                stringText,
                                exactPositions,
                                AnonymousObjectsRule,
                                new MessageTemplateDiagnostic(
                                    property.StartIndex,
                                    property.Length,
                                    property.PropertyName));
                        }
                    }
                }
            }

            // are there duplicate property names?
            var usedNames = new HashSet<string>();
            foreach (var property in properties)
            {
                if (!property.IsPositional && !usedNames.Add(property.PropertyName))
                {
                    ReportDiagnostic(
                        ref context,
                        ref literalSpan,
                        stringText,
                        exactPositions,
                        UniquePropertyRule,
                        new MessageTemplateDiagnostic(property.StartIndex, property.Length, property.PropertyName));
                }

                var firstCharacter = property.PropertyName[0];
                if (!char.IsDigit(firstCharacter) && !char.IsUpper(firstCharacter))
                {
                    ReportDiagnostic(
                        ref context,
                        ref literalSpan,
                        stringText,
                        exactPositions,
                        PascalCasePropertyRule,
                        new MessageTemplateDiagnostic(property.StartIndex, property.Length, property.PropertyName));
                }
            }
        }

        // is this an overload where the exception argument is used?
        var exception = context.SemanticModel.Compilation.GetTypeByMetadataName(typeof(Exception).FullName!);
        if (HasConventionalExceptionParameter(method))
        {
            return;
        }

        // is there an overload with the exception argument?
        if (!method.ContainingType.GetMembers()
                   .OfType<IMethodSymbol>()
                   .Any(x => x.Name == method.Name && HasConventionalExceptionParameter(x)))
        {
            return;
        }

        // check whether any of the format arguments is an exception
        foreach (var argument in invocationArguments.Where(
            arg => IsException(
                context.SemanticModel.GetTypeInfo(arg.Expression)
                       .Type)))
        {

            context.ReportDiagnostic(
                Diagnostic.Create(
                    ExceptionUsageRule,
                    argument.GetLocation(),
                    argument.Expression.ToFullString()));
        }

        // Check if there is an Exception parameter at position 1 (position 2 for static extension method invocations)?
        bool HasConventionalExceptionParameter(IMethodSymbol methodSymbol)
        {
            return SymbolEqualityComparer.Default.Equals(
                       methodSymbol.Parameters.FirstOrDefault()
                                  ?.Type,
                       exception)
                   || (methodSymbol.IsExtensionMethod
                       && SymbolEqualityComparer.Default.Equals(
                           methodSymbol.Parameters.Skip(1)
                                       .FirstOrDefault()
                                      ?.Type,
                           exception));
        }

        bool IsException(ITypeSymbol? type)
        {
            for (var symbol = type; symbol != null; symbol = symbol.BaseType)
            {
                if (SymbolEqualityComparer.Default.Equals(symbol, exception))
                {
                    return true;
                }
            }
            return false;
        }
    }

    private static void ReportDiagnostic(ref SyntaxNodeAnalysisContext context, ref TextSpan literalSpan, string stringText, bool exactPositions, DiagnosticDescriptor rule, MessageTemplateDiagnostic diagnostic)
    {
        TextSpan textSpan;
        if (diagnostic.MustBeRemapped)
        {
            if (!exactPositions)
            {
                textSpan = literalSpan;
            }
            else
            {
                var remappedStart = GetPositionInLiteral(stringText, diagnostic.StartIndex);
                var remappedEnd = GetPositionInLiteral(stringText, diagnostic.StartIndex + diagnostic.Length);
                textSpan = new TextSpan(literalSpan.Start + remappedStart, remappedEnd - remappedStart);
            }
        }
        else
        {
            textSpan = new TextSpan(diagnostic.StartIndex, diagnostic.Length);
        }
        var sourceLocation = Location.Create(context.Node.SyntaxTree, textSpan);
        context.ReportDiagnostic(Diagnostic.Create(rule, sourceLocation, diagnostic.Diagnostic));
    }

    /// <summary>
    /// Remaps a string position into the position in a string literal
    /// </summary>
    /// <param name="literal">The literal string as string</param>
    /// <param name="unescapedPosition">The position in the non literal string</param>
    /// <returns></returns>
    private static int GetPositionInLiteral(string literal, int unescapedPosition)
    {
        if (literal[0] == '@')
        {
            for (var i = 2; i < literal.Length; i++)
            {
                var c = literal[i];

                if (c == '"' && i + 1 < literal.Length && literal[i + 1] == '"')
                {
                    i++;
                }
                unescapedPosition--;

                if (unescapedPosition == -1)
                {
                    return i;
                }
            }
        }
        else
        {
            for (var i = 1; i < literal.Length; i++)
            {
                var c = literal[i];

                if (c == '\\' && i + 1 < literal.Length)
                {
                    c = literal[++i];
                    if (c is 'x' or 'u' or 'U')
                    {
                        var max = Math.Min((c == 'U' ? 8 : 4) + i + 1, literal.Length);
                        for (i++; i < max; i++)
                        {
                            c = literal[i];
                            if (!IsHexDigit(c))
                            {
                                break;
                            }
                        }
                        i--;
                    }
                }
                unescapedPosition--;

                if (unescapedPosition == -1)
                {
                    return i;
                }
            }
        }

        return unescapedPosition;
    }

    /// <summary>
    /// Returns true if the Unicode character is a hexadecimal digit.
    /// </summary>
    /// <param name="c">The Unicode character.</param>
    /// <returns>true if the character is a hexadecimal digit 0-9, A-F, a-f.</returns>
    private static bool IsHexDigit(char c)
    {
        return c is (>= '0' and <= '9') or (>= 'A' and <= 'F') or (>= 'a' and <= 'f');
    }
}
