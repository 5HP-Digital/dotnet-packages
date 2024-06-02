namespace Digital5HP.Analyzers.Internals;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

public static class DocumentEditorExtensions
{
    public static DocumentEditor ReplaceToken(this DocumentEditor editor, SyntaxToken oldToken, SyntaxToken newToken)
    {
        if (oldToken.Parent is not null)
        {
            editor.ReplaceNode(
                oldToken.Parent,
                (x, _) =>
                {
                    var syntaxToken = x.FindToken(oldToken.SpanStart);
                    return x.ReplaceToken(syntaxToken, newToken);
                });
        }

        return editor;
    }
}
