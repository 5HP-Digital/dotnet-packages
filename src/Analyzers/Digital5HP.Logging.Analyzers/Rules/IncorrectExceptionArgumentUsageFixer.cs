namespace Digital5HP.Logging.Analyzers.Rules;

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IncorrectExceptionArgumentUsageFixer))]
[Shared]
public class IncorrectExceptionArgumentUsageFixer : CodeFixProvider
{
    private const string TITLE = "Reorder exception argument";

    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(RuleIdentifiers.INCORRECT_EXCEPTION_ARGUMENT_USAGE);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                .ConfigureAwait(false);

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        if(root?.FindNode(diagnosticSpan) is not ArgumentSyntax declaration)
            return;

        context.RegisterCodeFix(
            CodeAction.Create(
                title: TITLE,
                createChangedDocument: c => MoveExceptionFirstAsync(context.Document, declaration, c),
                equivalenceKey: TITLE),
            diagnostic);
    }

    private static async Task<Document> MoveExceptionFirstAsync(Document document, ArgumentSyntax argument, CancellationToken cancellationToken)
    {
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken)
                                          .ConfigureAwait(false);
        if (semanticModel is null) return document;

        var argumentList = argument.AncestorsAndSelf()
                                   .OfType<ArgumentListSyntax>()
                                   .First();
        if (argumentList.Parent is null) return document;

        var newList = argumentList.Arguments.Remove(argument);
        var symbolInfo = semanticModel.GetSymbolInfo(argumentList.Parent, cancellationToken: cancellationToken);

        if (symbolInfo.Symbol is IMethodSymbol methodSymbol && methodSymbol.IsExtensionMethod && methodSymbol.IsStatic)
        {
            // This is a static method invocation of an extension method, so the first parameter is the
            // extended type itself; hence we insert at the second position
            newList = newList.Insert(1, argument);
        }
        else
        {
            newList = newList.Insert(0, argument);
        }

        var editor = await DocumentEditor.CreateAsync(document, cancellationToken);
        editor.ReplaceNode(argumentList, argumentList.WithArguments(newList));

        return editor.GetChangedDocument();
    }
}
