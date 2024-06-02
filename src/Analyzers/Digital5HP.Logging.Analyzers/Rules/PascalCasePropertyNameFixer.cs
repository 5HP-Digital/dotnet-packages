namespace Digital5HP.Logging.Analyzers.Rules;

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

using Digital5HP.Analyzers.Internals;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PascalCasePropertyNameFixer))]
[Shared]
public class PascalCasePropertyNameFixer : CodeFixProvider
{
    private const char STRINGIFICATION_PREFIX = '$';
    private const char DESTRUCTURING_PREFIX = '@';
    private const string TITLE = "Pascal case the property";

    public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(RuleIdentifiers.USE_PASCAL_CASE_PROPERTY_NAMES);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null) return;

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var declaration = root.FindNode(diagnosticSpan);

        context.RegisterCodeFix(
            CodeAction.Create(
                title: TITLE,
                createChangedDocument: c => PascalCaseThePropertiesAsync(
                                           context.Document,
                                           declaration.DescendantNodesAndSelf()
                                                      .OfType<LiteralExpressionSyntax>()
                                                      .First(),
                                           c),
                equivalenceKey: TITLE),
            diagnostic);
    }

    private static async Task<Document> PascalCaseThePropertiesAsync(Document document, LiteralExpressionSyntax node, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null) return document;

        var oldToken = node.Token;
        if (oldToken.Parent is null) return document;

        var sb = new StringBuilder();
        if (oldToken.Text.StartsWith("@", StringComparison.Ordinal))
        {
            sb.Append('@');
        }
        sb.Append('"');

        var interpolatedString = (InterpolatedStringExpressionSyntax)SyntaxFactory.ParseExpression("$" + oldToken);
        foreach (var child in interpolatedString.Contents)
        {
            switch (child)
            {
                case InterpolatedStringTextSyntax text:
                    sb.Append(text.TextToken.ToString());
                    break;
                case InterpolationSyntax interpolation:
                    AppendAsPascalCase(sb, interpolation.ToString());
                    break;
            }
        }
        sb.Append('"');

        var newToken = SyntaxFactory.ParseToken(sb.ToString());

        var editor = await DocumentEditor.CreateAsync(document, cancellationToken);
        editor.ReplaceToken(oldToken, newToken);

        return editor.GetChangedDocument();
    }

    private static void AppendAsPascalCase(StringBuilder sb, string input)
    {
        var uppercaseChar = true;
        var skipTheRest = false;
        for (var i = 0; i < input.Length; i++)
        {
            var current = input[i];
            if ((i < 2 && current == '{') || current is STRINGIFICATION_PREFIX or DESTRUCTURING_PREFIX)
            {
                sb.Append(current);
                continue;
            }
            if (skipTheRest || current is ',' or ':' or '}')
            {
                skipTheRest = true;
                sb.Append(current);
                continue;
            }
            if (current == '_')
            {
                uppercaseChar = true;
                continue;
            }
            sb.Append(uppercaseChar ? char.ToUpperInvariant(current) : current);
            uppercaseChar = false;
        }
    }
}
