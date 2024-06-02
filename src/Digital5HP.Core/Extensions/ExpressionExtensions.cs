namespace Digital5HP;

using System;
using System.Linq.Expressions;
using System.Reflection;

public static class ExpressionExtensions
{
    /// <summary>
    /// Gets the member name that the lambda expression references.
    /// </summary>
    public static string GetMemberName(this LambdaExpression memberSelector)
    {
        ArgumentNullException.ThrowIfNull(memberSelector);

        string NameSelector(Expression e, string name)
        {
            switch (e.NodeType)
            {
                case ExpressionType.Parameter:
                    return ConcatParts(name, ((ParameterExpression)e).Name);
                case ExpressionType.MemberAccess:
                    var memberExpr = (MemberExpression)e;
                    // not a property (local member?)
                    if (memberExpr.Member is not PropertyInfo)
                        return name;
                    // static property
                    if (memberExpr.Expression is null)
                        return ConcatParts(
                            ConcatParts(memberExpr.Member.ReflectedType?.Name, memberExpr.Member.Name),
                            name);
                    // drill one more level into property
                    return ConcatParts(NameSelector(memberExpr.Expression, memberExpr.Member.Name), name);
                case ExpressionType.Call:
                    var methodExpr = (MethodCallExpression)e;
                    return ConcatParts(
                        NameSelector(
                            methodExpr.Object,
                            $"{methodExpr.Method.Name}()"),
                        name);
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    return NameSelector(((UnaryExpression)e).Operand, name);
                case ExpressionType.Invoke:
                    return NameSelector(((InvocationExpression)e).Expression, name);
                case ExpressionType.ArrayLength:
                    return ConcatParts(NameSelector(((UnaryExpression)e).Operand, name), "Length");
                case ExpressionType.ArrayIndex:
                    var binExpr = (BinaryExpression)e;
                    return ConcatParts(NameSelector(binExpr.Left, "") + "[]", name);
                case ExpressionType.Constant:
                    var constExpr = (ConstantExpression)e;
                    return ConcatParts(name, constExpr.Value?.ToString());
                default:
                    throw new NotSupportedException($"Expression node type '{e.NodeType}' is not supported.");
            }
        }

        string ConcatParts(string left, string right)
        {
            left = left.TrimEnd('.');
            right = right.TrimStart('.');

            if (string.IsNullOrWhiteSpace(left))
                return right;

            return string.IsNullOrWhiteSpace(right) ? left : string.Join(".", left, right);
        }

        return NameSelector(memberSelector.Body, string.Empty);
    }
}
