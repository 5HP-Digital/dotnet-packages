namespace Digital5HP.Analyzers.Internals;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis;

// http://source.roslyn.io/#Microsoft.CodeAnalysis.Workspaces/Shared/Extensions/ITypeSymbolExtensions.cs,190b4ed0932458fd,references
internal static class TypeSymbolExtensions
{
    private const string DEFAULT_PARAMETER_NAME = "p";
    private const string DEFAULT_BUILT_IN_PARAMETER_NAME = "v";

    private static readonly SymbolDisplayFormat ShortNameFormat = new(
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes
                              | SymbolDisplayMiscellaneousOptions.ExpandNullable);

    public static IList<INamedTypeSymbol> GetAllInterfacesIncludingThis(this ITypeSymbol type)
    {
        var allInterfaces = type.AllInterfaces;
        if (type is INamedTypeSymbol namedType
            && namedType.TypeKind == TypeKind.Interface
            && !allInterfaces.Contains(namedType))
        {
            var result = new List<INamedTypeSymbol>(allInterfaces.Length + 1);
            result.AddRange(allInterfaces);
            result.Add(namedType);
            return result;
        }

        return allInterfaces;
    }

    public static bool InheritsFrom(this ITypeSymbol classSymbol, ITypeSymbol? baseClassType)
    {
        if (baseClassType == null)
            return false;

        var baseType = classSymbol.BaseType;
        while (baseType != null)
        {
            if (baseClassType.IsEqualTo(baseType))
                return true;

            baseType = baseType.BaseType;
        }

        return false;
    }

    public static bool Implements(this ITypeSymbol classSymbol, ITypeSymbol? interfaceType)
    {
        if (interfaceType == null)
            return false;

        return classSymbol.AllInterfaces.Any(i => interfaceType.IsEqualTo(i));
    }

    public static bool IsOrImplements(this ITypeSymbol symbol, ITypeSymbol? interfaceType)
    {
        if (interfaceType == null)
            return false;

        return GetAllInterfacesIncludingThis(symbol)
           .Any(i => interfaceType.IsEqualTo(i));
    }

    public static bool HasAttribute(this ISymbol symbol, ITypeSymbol? attributeType, Func<AttributeData, bool>? predicate = null)
    {
        if (attributeType == null)
            return false;

        foreach (var attribute in symbol.GetAttributes())
        {
            if (attributeType.IsEqualTo(attribute.AttributeClass))
                return predicate == null || predicate(attribute);
        }

        return false;
    }

    public static bool IsOrInheritFrom(this ITypeSymbol symbol, ITypeSymbol? expectedType)
    {
        if (expectedType == null)
            return false;

        return symbol.IsEqualTo(expectedType) || symbol.InheritsFrom(expectedType);
    }

    public static bool IsEqualToAny(this ITypeSymbol? symbol, params ITypeSymbol?[]? expectedTypes)
    {
        if (symbol == null || expectedTypes == null)
            return false;

        return expectedTypes.Any(t => t.IsEqualTo(symbol));
    }

    public static bool IsObject(this ITypeSymbol? symbol)
    {
        if (symbol == null)
            return false;

        return symbol.SpecialType == SpecialType.System_Object;
    }

    public static bool IsString(this ITypeSymbol? symbol)
    {
        if (symbol == null)
            return false;

        return symbol.SpecialType == SpecialType.System_String;
    }

    public static bool IsChar(this ITypeSymbol? symbol)
    {
        if (symbol == null)
            return false;

        return symbol.SpecialType == SpecialType.System_Char;
    }

    public static bool IsInt32(this ITypeSymbol? symbol)
    {
        if (symbol == null)
            return false;

        return symbol.SpecialType == SpecialType.System_Int32;
    }

    public static bool IsBoolean(this ITypeSymbol? symbol)
    {
        if (symbol == null)
            return false;

        return symbol.SpecialType == SpecialType.System_Boolean;
    }

    public static bool IsDateTime(this ITypeSymbol? symbol)
    {
        if (symbol == null)
            return false;

        return symbol.SpecialType == SpecialType.System_DateTime;
    }

    public static bool IsEnumeration([NotNullWhen(returnValue: true)] this ITypeSymbol? symbol)
    {
        return symbol != null && GetEnumerationType(symbol) != null;
    }

    public static INamedTypeSymbol? GetEnumerationType(this ITypeSymbol? symbol)
    {
        return (symbol as INamedTypeSymbol)?.EnumUnderlyingType;
    }

    public static bool IsNumberType(this ITypeSymbol? symbol)
    {
        if (symbol == null)
            return false;

        switch (symbol.SpecialType)
        {
            case SpecialType.System_Int16:
            case SpecialType.System_Int32:
            case SpecialType.System_Int64:
            case SpecialType.System_UInt16:
            case SpecialType.System_UInt32:
            case SpecialType.System_UInt64:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            case SpecialType.System_Decimal:
            case SpecialType.System_Byte:
            case SpecialType.System_SByte:
                return true;

            default:
                return false;
        }
    }

    public static bool IsUnitTestClass(this ITypeSymbol typeSymbol)
    {
        var attributes = typeSymbol.GetAttributes();
        foreach (var attribute in attributes)
        {
            var type = attribute.AttributeClass;
            while (type != null)
            {
                var ns = type.ContainingNamespace;
                if (ns.IsNamespace(
                        [
                            "Microsoft", "VisualStudio", "TestTools", "UnitTesting"
                        ])
                    || ns.IsNamespace(
                        [
                            "NUnit", "Framework"
                        ])
                    || ns.IsNamespace(
                        [
                            "Xunit"
                        ]))
                {
                    return true;
                }

                type = type.BaseType;
            }
        }

        return false;
    }

    public static bool IsSpecialType(this ITypeSymbol? symbol)
    {
        if (symbol != null)
        {
            switch (symbol.SpecialType)
            {
                case SpecialType.System_Object:
                case SpecialType.System_Void:
                case SpecialType.System_Boolean:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Decimal:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_Char:
                case SpecialType.System_String:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                    return true;
            }
        }

        return false;
    }

    public static string GetShortName(this INamespaceOrTypeSymbol symbol)
    {
        return symbol.ToDisplayString(ShortNameFormat);
    }

    public static string? CreateParameterName(this ITypeSymbol type, bool capitalize = false)
    {
        while (true)
        {
            if (type is IArrayTypeSymbol arrayType)
            {
                type = arrayType.ElementType;
                continue;
            }

            if (type is IPointerTypeSymbol pointerType)
            {
                type = pointerType.PointedAtType;
                continue;
            }

            break;
        }

        var shortName = GetParameterName(type);
        return capitalize ? shortName.ToPascalCase() : shortName.ToCamelCase();
    }

    public static IEnumerable<INamedTypeSymbol> GetBaseTypesAndThis(this INamedTypeSymbol type)
    {
        var current = type;
        while (current != null)
        {
            yield return current;
            current = current.BaseType;
        }
    }

    private static string GetParameterName(ITypeSymbol? type)
    {
        if (type == null || type.IsAnonymousType /*|| type.IsTupleType*/)
        {
            return DEFAULT_PARAMETER_NAME;
        }

        if (type.IsSpecialType() || type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
        {
            return DEFAULT_BUILT_IN_PARAMETER_NAME;
        }

        var shortName = type.GetShortName();
        return shortName.Length == 0
            ? DEFAULT_PARAMETER_NAME
            : shortName;
    }
}
