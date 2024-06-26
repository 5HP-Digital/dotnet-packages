﻿namespace Digital5HP.Analyzers.Internals;

using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

internal static class OperationExtensions
{
    public static IEnumerable<IOperation> Ancestors(this IOperation operation)
    {
        var parent = operation.Parent;
        while (parent != null)
        {
            yield return parent;
            parent = parent.Parent;
        }
    }

    public static bool IsInQueryableExpressionArgument(this IOperation operation)
    {
        var semanticModel = operation.SemanticModel;
        if (semanticModel == null)
            return false;

        foreach (var invocationOperation in operation.Ancestors().OfType<IInvocationOperation>())
        {
            var type = invocationOperation.TargetMethod.ContainingType;
            if (type.IsEqualTo(semanticModel.Compilation.GetTypeByMetadataName("System.Linq.Queryable")))
                return true;
        }

        return false;
    }

    public static bool IsInExpressionArgument(this IOperation operation)
    {
        var semanticModel = operation.SemanticModel;
        if (semanticModel == null)
            return false;

        foreach (var op in operation.Ancestors().OfType<IArgumentOperation>())
        {
            if (op.Parameter == null)
                continue;

            var type = op.Parameter.Type;
            if (type.InheritsFrom(semanticModel.Compilation.GetTypeByMetadataName("System.Linq.Expressions.Expression")))
                return true;
        }

        return false;
    }

    public static bool IsInNameofOperation(this IOperation operation)
    {
        return operation.Ancestors().OfType<INameOfOperation>().Any();
    }

    public static ITypeSymbol? GetActualType(this IOperation operation)
    {
        if (operation is IConversionOperation conversionOperation)
        {
            return GetActualType(conversionOperation.Operand);
        }

        return operation.Type;
    }

    public static IOperation UnwrapImplicitConversionOperations(this IOperation operation)
    {
        if (operation is IConversionOperation conversionOperation && conversionOperation.IsImplicit)
        {
            return UnwrapImplicitConversionOperations(conversionOperation.Operand);
        }

        return operation;
    }

    public static bool HasArgumentOfType(this IInvocationOperation operation, ITypeSymbol argumentTypeSymbol)
    {
        foreach (var arg in operation.Arguments)
        {
            if (argumentTypeSymbol.IsEqualTo(arg.Value.Type))
                return true;
        }

        return false;
    }

    public static IMethodSymbol? GetContainingMethod(this IOperation operation)
    {
        if (operation.SemanticModel == null)
            return null;

        foreach (var syntax in operation.Syntax.AncestorsAndSelf())
        {
            if (syntax is MethodDeclarationSyntax method)
                return operation.SemanticModel.GetDeclaredSymbol(method) as IMethodSymbol;
        }

        return null;
    }
}
