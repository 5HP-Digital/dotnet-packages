#nullable disable
namespace Digital5HP.Analyzers.Internals;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class SemanticModelExtensions
{
    /// <summary>
    /// Given an expression node, tries to generate an appropriate name that can be used for
    /// that expression.
    /// </summary>
    /// <remarks>Lifted from https://github.com/dotnet/roslyn/blob/c5c72d57af0ee9c615ee6a810394ea4e92d8d913/src/Workspaces/CSharp/Portable/Extensions/SemanticModelExtensions.cs#L200 </remarks>
    public static string GenerateNameForExpression(
        this SemanticModel semanticModel, ExpressionSyntax expression, bool capitalize = false)
    {
        // Try to find a usable name node that we can use to name the
        // parameter.  If we have an expression that has a name as part of it
        // then we try to use that part.
        var current = expression;
        while (true)
        {
            current = current.WalkDownParentheses();

            if (current.Kind() == SyntaxKind.IdentifierName)
            {
                return ((IdentifierNameSyntax)current).Identifier.ValueText.ToPascalCase();
            }

            if (current is MemberAccessExpressionSyntax)
            {
                return ((MemberAccessExpressionSyntax)current).Name.Identifier.ValueText.ToPascalCase();
            }

            if (current is MemberBindingExpressionSyntax)
            {
                return ((MemberBindingExpressionSyntax)current).Name.Identifier.ValueText.ToPascalCase();
            }

            if (current is ConditionalAccessExpressionSyntax)
            {
                current = ((ConditionalAccessExpressionSyntax)current).WhenNotNull;
            }
            else if (current is CastExpressionSyntax)
            {
                current = ((CastExpressionSyntax)current).Expression;
            }
            //else if (current is DeclarationExpressionSyntax)
            //{
            //    var decl = (DeclarationExpressionSyntax)current;
            //    var name = decl.Designation as SingleVariableDesignationSyntax;
            //    if (name == null)
            //    {
            //        break;
            //    }

            //    return name.Identifier.ValueText.ToCamelCase();
            //}
            else
            {
                break;
            }
        }

        // Otherwise, figure out the type of the expression and generate a name from that
        // instead.
        var info = semanticModel.GetTypeInfo(expression);

        // If we can't determine the type, then fallback to some placeholders.
        var type = info.Type;
        return type?.CreateParameterName(capitalize);
    }
}
