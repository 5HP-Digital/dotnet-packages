namespace Digital5HP.Analyzers.Internals;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class ExpressionSyntaxExtensions
{

    public static ExpressionSyntax WalkDownParentheses(this ExpressionSyntax expression)
    {
        while (expression.IsKind(SyntaxKind.ParenthesizedExpression))
        {
            expression = ((ParenthesizedExpressionSyntax)expression).Expression;
        }

        return expression;
    }
    
}
