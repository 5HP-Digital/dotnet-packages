namespace Digital5HP.Logging.Analyzers.Rules;

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DestructingAnonymousObjectsFixer))]
[Shared]
public class DestructingAnonymousObjectsFixer : CodeFixProvider
{
    private const string TITLE = "Add destructuring hint for anonymous object";

    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(RuleIdentifiers.ANONYMOUS_OBJECTS_SHOULD_BE_DESTRUCTURED);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        context.RegisterCodeFix(
            CodeAction.Create(
                title: TITLE,
                createChangedSolution: c => AddDestructuringHintAsync(context.Document, diagnosticSpan, c),
                equivalenceKey: TITLE),
            diagnostic);

        return Task.CompletedTask;
    }

    private static async Task<Solution> AddDestructuringHintAsync(Document document, TextSpan textSpan, CancellationToken cancellationToken)
    {
        var text = await document.GetTextAsync(cancellationToken)
                                 .ConfigureAwait(false);

        text = text.Replace(start: textSpan.Start + 1, length: 0, newText: "@"); // textSpan: "{Name}" -> "{@Name}"
        document = document.WithText(text);

        return document.Project.Solution;
    }
}
