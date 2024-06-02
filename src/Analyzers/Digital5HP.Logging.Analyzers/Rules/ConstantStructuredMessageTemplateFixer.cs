namespace Digital5HP.Logging.Analyzers.Rules;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

using Digital5HP.Analyzers.Internals;
using Digital5HP.Logging.Analyzers.Internals;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConstantStructuredMessageTemplateFixer))]
[Shared]
public class ConstantStructuredMessageTemplateFixer : CodeFixProvider
{
    private const string TITLE = "Convert to proper constant structured message template";
    private const string CONVERSION_NAME = "LoggingAnalyzer-";

    public override ImmutableArray<string> FixableDiagnosticIds =>
        [RuleIdentifiers.USE_PROPER_CONSTANT_STRUCTURED_MESSAGE_TEMPLATE];

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null) return;

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        if (root.FindNode(diagnosticSpan) is not ArgumentSyntax declaration || declaration.Parent is null)
            return;

        if (declaration.Parent.Parent is InvocationExpressionSyntax logger)
        {
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken)
                                             .ConfigureAwait(false);
            if(semanticModel is null)
                return;

            switch (declaration.Expression)
            {
                case InvocationExpressionSyntax inv when ModelExtensions.GetSymbolInfo(semanticModel, inv.Expression)
                                                                        .Symbol is IMethodSymbol symbol
                                                         && symbol.ToString()!.StartsWith(
                                                             "string.Format(",
                                                             StringComparison.Ordinal)
                                                         && inv.ArgumentList.Arguments.Count > 0:
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            TITLE,
                            c => this.ConvertStringFormatToMessageTemplateAsync(context.Document, inv, logger, c),
                            TITLE),
                        diagnostic);
                    break;
                case InterpolatedStringExpressionSyntax interpolatedString:
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            TITLE,
                            c => this.ConvertInterpolationToMessageTemplateAsync(
                                context.Document,
                                interpolatedString,
                                logger,
                                c),
                            TITLE),
                        diagnostic);
                    break;
                default:
                    {
                        if (declaration.Expression.DescendantNodesAndSelf()
                                       .OfType<LiteralExpressionSyntax>()
                                       .Any())
                        {
                            context.RegisterCodeFix(
                                CodeAction.Create(
                                    TITLE,
                                    c => this.ConvertStringConcatToMessageTemplateAsync(
                                        context.Document,
                                        declaration.Expression,
                                        logger,
                                        c),
                                    TITLE),
                                diagnostic);
                        }

                        break;
                    }
            }
        }
    }

    private static async Task<Document> InlineFormatAndArgumentsIntoLoggerStatementAsync(Document document, ExpressionSyntax originalTemplateExpression, InvocationExpressionSyntax logger, InterpolatedStringExpressionSyntax format, List<ExpressionSyntax> expressions, CancellationToken cancellationToken)
    {
        var loggerArguments = logger.ArgumentList.Arguments;
        var argumentIndex = loggerArguments.IndexOf(x => x.Expression == originalTemplateExpression);

        var sb = new StringBuilder();
        if (format.StringStartToken.ValueText.Contains("@", StringComparison.Ordinal))
        {
            sb.Append('@');
        }
        sb.Append('"');

        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (syntaxRoot is null) return document;

        var usedNames = new HashSet<string>();
        var argumentExpressions = new List<ExpressionSyntax?>();

        var indexFromOriginalLoggingArguments = argumentIndex + 1;
        foreach (var child in format.Contents)
        {
            switch (child)
            {
                case InterpolatedStringTextSyntax text:
                    sb.Append(text.TextToken.ToString());
                    break;
                case InterpolationSyntax interpolation:
                    string expressionText = interpolation.Expression.ToString();
                    ExpressionSyntax? correspondingArgument;
                    string name;
                    if (expressionText.StartsWith(CONVERSION_NAME, StringComparison.Ordinal) && int.TryParse(expressionText.Substring(CONVERSION_NAME.Length), out var index))
                    {
                        correspondingArgument = expressions.ElementAtOrDefault(index);

                        if (correspondingArgument != null)
                        {
                            name = semanticModel.GenerateNameForExpression(correspondingArgument, true).NullWhenWhitespace() ?? "Error";
                        }
                        else // in case this string.format is faulty
                        {
                            correspondingArgument = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
                            name = "Error";
                        }
                    }
                    else
                    {
                        correspondingArgument = loggerArguments.ElementAtOrDefault(indexFromOriginalLoggingArguments++)?.Expression;
                        if (!string.IsNullOrWhiteSpace(expressionText))
                        {
                            name = expressionText;
                        }
                        else if (correspondingArgument != null)
                        {
                            name = semanticModel.GenerateNameForExpression(correspondingArgument, true).NullWhenWhitespace() ?? "Error";
                        }
                        else // in case this string.format is faulty
                        {
                            correspondingArgument = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
                            name = "Error";
                        }
                    }

                    argumentExpressions.Add(correspondingArgument);

                    sb.Append('{');

                    var attempt = 0;
                    string lastAttempt;
                    while (!usedNames.Add(lastAttempt = attempt == 0 ? name : name + attempt))
                    {
                        attempt++;
                    }

                    sb.Append(lastAttempt);

                    if (interpolation.AlignmentClause != null)
                        sb.Append(interpolation.AlignmentClause);

                    if (interpolation.FormatClause != null)
                        sb.Append(interpolation.FormatClause);

                    sb.Append('}');
                    break;
            }
        }

        sb.Append('"');
        var messageTemplate = SyntaxFactory.Argument(SyntaxFactory.ParseExpression(sb.ToString()));

        var seperatedSyntax = loggerArguments.Replace(loggerArguments[argumentIndex], messageTemplate);

        // remove any arguments that we've put into argumentExpressions
        if (indexFromOriginalLoggingArguments > argumentIndex + 1)
        {
            for (var i = Math.Min(indexFromOriginalLoggingArguments, seperatedSyntax.Count) - 1; i > argumentIndex; i--)
            {
                seperatedSyntax = seperatedSyntax.RemoveAt(i);
            }
        }

        seperatedSyntax = seperatedSyntax.InsertRange(argumentIndex + 1, argumentExpressions.Select(x => SyntaxFactory.Argument(x ?? SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression))));

        var newLogger = logger.WithArgumentList(SyntaxFactory.ArgumentList(seperatedSyntax)).WithAdditionalAnnotations(Formatter.Annotation);
        return document.WithSyntaxRoot(syntaxRoot.ReplaceNode(logger, newLogger));
    }

    private async Task<Document> ConvertInterpolationToMessageTemplateAsync(Document document, InterpolatedStringExpressionSyntax interpolatedString, InvocationExpressionSyntax logger, CancellationToken cancellationToken)
    {
        GetFormatStringAndExpressionsFromInterpolation(interpolatedString, out var format, out var expressions);

        return await InlineFormatAndArgumentsIntoLoggerStatementAsync(document, interpolatedString, logger, format, expressions, cancellationToken).ConfigureAwait(false);
    }

    private static void GetFormatStringAndExpressionsFromInterpolation(InterpolatedStringExpressionSyntax interpolatedString, out InterpolatedStringExpressionSyntax format, out List<ExpressionSyntax> expressions)
    {
        var sb = new StringBuilder();
        var replacements = new List<string>();
        var interpolations = new List<ExpressionSyntax>();
        foreach (var child in interpolatedString.Contents)
        {
            switch (child)
            {
                case InterpolatedStringTextSyntax text:
                    sb.Append(text.TextToken.ToString());
                    break;
                case InterpolationSyntax interpolation:
                    var argumentPosition = interpolations.Count;
                    interpolations.Add(interpolation.Expression);

                    sb.Append('{');
                    sb.Append(replacements.Count);
                    sb.Append('}');

                    replacements.Add($"{{{CONVERSION_NAME}{argumentPosition}{interpolation.AlignmentClause}{interpolation.FormatClause}}}");

                    break;
            }
        }

        format = (InterpolatedStringExpressionSyntax)SyntaxFactory.ParseExpression(
            "$\""
            + string.Format(CultureInfo.InvariantCulture, sb.ToString(), replacements.ToArray<object>())
            + "\"");
        expressions = interpolations;
    }

    private async Task<Document> ConvertStringFormatToMessageTemplateAsync(Document document, InvocationExpressionSyntax stringFormat, InvocationExpressionSyntax logger, CancellationToken cancellationToken)
    {
        GetFormatStringAndExpressionsFromStringFormat(stringFormat, out var format, out var expressions);

        return await InlineFormatAndArgumentsIntoLoggerStatementAsync(document, stringFormat, logger, format, expressions, cancellationToken).ConfigureAwait(false);
    }

    private static void GetFormatStringAndExpressionsFromStringFormat(InvocationExpressionSyntax stringFormat, out InterpolatedStringExpressionSyntax format, out List<ExpressionSyntax> expressions)
    {
        var arguments = stringFormat.ArgumentList.Arguments;
        var formatString = ((LiteralExpressionSyntax)arguments[0].Expression).Token.ToString();
        var interpolatedString = (InterpolatedStringExpressionSyntax)SyntaxFactory.ParseExpression("$" + formatString);

        var sb = new StringBuilder();
        var replacements = new List<string>();
        foreach (var child in interpolatedString.Contents)
        {
            switch (child)
            {
                case InterpolatedStringTextSyntax text:
                    sb.Append(text.TextToken.ToString());
                    break;
                case InterpolationSyntax interpolation:
                    int argumentPosition;
                    if (interpolation.Expression is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.NumericLiteralExpression) && literal.Token.Value != null)
                    {
                        argumentPosition = (int)literal.Token.Value;
                    }
                    else
                    {
                        argumentPosition = -1;
                    }

                    sb.Append('{');
                    sb.Append(replacements.Count);
                    sb.Append('}');

                    replacements.Add($"{{{CONVERSION_NAME}{argumentPosition}{interpolation.AlignmentClause}{interpolation.FormatClause}}}");

                    break;
            }
        }

        format = (InterpolatedStringExpressionSyntax)SyntaxFactory.ParseExpression(
            "$\""
            + string.Format(CultureInfo.InvariantCulture, sb.ToString(), replacements.ToArray<object>())
            + "\"");
        expressions = arguments.Skip(1).Select(x => x.Expression).ToList();
    }

    private async Task<Document> ConvertStringConcatToMessageTemplateAsync(Document document, ExpressionSyntax stringConcat, InvocationExpressionSyntax logger, CancellationToken cancellationToken)
    {
        GetFormatStringAndExpressionsFromStringConcat(stringConcat, out var format, out var expressions);

        return await InlineFormatAndArgumentsIntoLoggerStatementAsync(document, stringConcat, logger, format, expressions, cancellationToken).ConfigureAwait(false);
    }

    private static void GetFormatStringAndExpressionsFromStringConcat(ExpressionSyntax stringConcat, out InterpolatedStringExpressionSyntax format, out List<ExpressionSyntax> expressions)
    {
        var concatExpressions = new List<ExpressionSyntax>();
        void FindExpressions(ExpressionSyntax exp)
        {
            switch (exp)
            {
                case BinaryExpressionSyntax binary when binary.OperatorToken.IsKind(SyntaxKind.PlusToken):
                    FindExpressions(binary.Left);
                    FindExpressions(binary.Right);
                    break;
                case ParenthesizedExpressionSyntax parens:
                    FindExpressions(parens.Expression);
                    break;
                case LiteralExpressionSyntax literal:
                    concatExpressions.Add(literal);
                    break;
                default:
                    concatExpressions.Add(exp.Parent is ParenthesizedExpressionSyntax paren ? paren : exp);
                    break;
            }
        }
        FindExpressions(stringConcat);

        var sb = new StringBuilder();
        var shouldUseVerbatim = false;
        var argumentPosition = 0;
        foreach (var child in concatExpressions)
        {
            switch (child)
            {
                case LiteralExpressionSyntax literal:
                    sb.Append(literal.Token.ValueText);
                    shouldUseVerbatim |= literal.Token.Text.StartsWith("@", StringComparison.Ordinal) && ContainsQuotesOrLineBreaks(literal.Token.ValueText);
                    break;
                case { }:

                    sb.Append('{');
                    sb.Append(CONVERSION_NAME);
                    sb.Append(argumentPosition++);
                    sb.Append('}');

                    break;
            }
        }

        if (shouldUseVerbatim)
        {
            for (var i = 0; i < sb.Length; i++)
            {
                if (IsForbiddenInVerbatimString(sb[i]))
                {
                    shouldUseVerbatim = false;
                    break;
                }
            }
        }

        var text = ObjectDisplay.FormatLiteral(sb.ToString(), useQuotes: true, escapeNonPrintable: !shouldUseVerbatim);

        format = (InterpolatedStringExpressionSyntax)SyntaxFactory.ParseExpression("$" + text);
        expressions = concatExpressions.Where(x => !(x is LiteralExpressionSyntax)).ToList();
    }

    private static bool IsForbiddenInVerbatimString(char c)
    {
        switch (c)
        {
            case '\a':
            case '\b':
            case '\f':
            case '\v':
            case '\0':
                return true;
        }

        return false;
    }

    private static bool ContainsQuotesOrLineBreaks(string s)
    {
        foreach (var c in s)
        {
            if (c == '\r' || c == '\n' || c == '"')
            {
                return true;
            }
        }

        return false;
    }
}
