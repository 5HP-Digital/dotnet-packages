namespace Digital5HP;

using System;
using System.Linq.Expressions;
using System.Reflection;

public static class LambdaExtensions
{

    /// <summary>
    /// Sets Property Value based on Expression
    /// </summary>
    public static void SetPropertyValue<T, TValue>(this T target, Expression<Func<T, TValue>> memberLambda, TValue value)
    {
        if (memberLambda is {Body: MemberExpression memberSelectorExpression})
        {
            var property = memberSelectorExpression.Member as PropertyInfo;
            if (property != null)
            {
                property.SetValue(target, value, null);
            }
        }
    }

    /// <summary>
    /// Gets Property Value by Expression
    /// </summary>
    public static TValue GetPropertyValue<T, TValue>(this T target, Expression<Func<T, TValue>> memberLambda)
    {
        if (memberLambda is {Body: MemberExpression memberSelectorExpression})
        {
            var property = memberSelectorExpression.Member as PropertyInfo;
            if (property != null)
            {
                return (TValue)property.GetValue(target, null);
            }
        }

        return default;
    }
}
