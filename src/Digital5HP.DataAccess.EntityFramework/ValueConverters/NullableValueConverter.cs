//Original source: https://github.com/dotnet/efcore/blob/main/src/EFCore/Storage/ValueConversion/ValueConverter%60.cs

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Digital5HP.DataAccess.EntityFramework.ValueConverters;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading;

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
///     Defines conversions from an object of one type in a model to an object of the same or
///     different type in the store.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="ValueConverter{TModel,TProvider}" /> class.
/// </remarks>
/// <param name="convertToProviderExpression"> An expression to convert objects when writing data to the store. </param>
/// <param name="convertFromProviderExpression"> An expression to convert objects when reading data from the store. </param>
/// <param name="mappingHints">
///     Hints that can be used by the <see cref="ITypeMappingSource" /> to create data types with appropriate
///     facets for the converted data.
/// </param>
public abstract class NullableValueConverter<TModel, TProvider>(
    [NotNull] Expression<Func<TModel, TProvider>> convertToProviderExpression,
    [NotNull] Expression<Func<TProvider, TModel>> convertFromProviderExpression,
    ConverterMappingHints mappingHints = null) : ValueConverter(convertToProviderExpression, convertFromProviderExpression, mappingHints)
{
    private Func<object, object> convertToProvider;
    private Func<object, object> convertFromProvider;

    private static Func<object, object> RunConverter<TIn, TOut>(Expression<Func<TIn, TOut>> convertExpression)
    {
        var compiled = convertExpression.Compile();

        return v => compiled((TIn)v);
    }

    /// <summary>
    ///     Gets the function to convert objects when writing data to the store,
    ///     setup to handle nulls, boxing, and non-exact matches of simple types.
    /// </summary>
    public override Func<object, object> ConvertToProvider
        => NonCapturingLazyInitializer.EnsureInitialized(
            ref this.convertToProvider, this, c => RunConverter(c.ConvertToProviderExpression));

    /// <summary>
    ///     Gets the function to convert objects when reading data from the store,
    ///     setup to handle nulls, boxing, and non-exact matches of simple types.
    /// </summary>
    public override Func<object, object> ConvertFromProvider
        => NonCapturingLazyInitializer.EnsureInitialized(
            ref this.convertFromProvider, this, c => RunConverter(c.ConvertFromProviderExpression));

    /// <summary>
    ///     Gets the expression to convert objects when writing data to the store,
    ///     exactly as supplied and may not handle
    ///     nulls, boxing, and non-exact matches of simple types.
    /// </summary>
    public new virtual Expression<Func<TModel, TProvider>> ConvertToProviderExpression
        => (Expression<Func<TModel, TProvider>>)base.ConvertToProviderExpression;

    /// <summary>
    ///     Gets the expression to convert objects when reading data from the store,
    ///     exactly as supplied and may not handle
    ///     nulls, boxing, and non-exact matches of simple types.
    /// </summary>
    public new virtual Expression<Func<TProvider, TModel>> ConvertFromProviderExpression
        => (Expression<Func<TProvider, TModel>>)base.ConvertFromProviderExpression;

    /// <summary>
    ///     The CLR type used in the EF model.
    /// </summary>
    public override Type ModelClrType
        => typeof(TModel);

    /// <summary>
    ///     The CLR type used when reading and writing from the store.
    /// </summary>
    public override Type ProviderClrType
        => typeof(TProvider);

    internal static class NonCapturingLazyInitializer
    {
        public static TValue EnsureInitialized<TParam, TValue>(
            ref TValue target,
            TParam param,
            [NotNull] Func<TParam, TValue> valueFactory)
            where TValue : class
        {
            var tmp = Volatile.Read(ref target);
            if (tmp != null)
            {
                return tmp;
            }

            Interlocked.CompareExchange(ref target, valueFactory(param), null);

            return target!;
        }

        public static TValue EnsureInitialized<TParam1, TParam2, TValue>(
            ref TValue target,
            TParam1 param1,
            TParam2 param2,
            [NotNull] Func<TParam1, TParam2, TValue> valueFactory)
            where TValue : class
        {
            var tmp = Volatile.Read(ref target);
            if (tmp != null)
            {
                return tmp;
            }

            Interlocked.CompareExchange(ref target, valueFactory(param1, param2), null);

            return target!;
        }

        public static TValue EnsureInitialized<TValue>(
            ref TValue target,
            [NotNull] TValue value)
            where TValue : class
        {
            var tmp = Volatile.Read(ref target);
            if (tmp != null)
            {
                return tmp;
            }

            Interlocked.CompareExchange(ref target, value, null);

            return target!;
        }

        public static TValue EnsureInitialized<TParam, TValue>(
            ref TValue target,
            TParam param,
            [NotNull] Action<TParam> valueFactory)
            where TValue : class
        {
            if (Volatile.Read(ref target) != null)
            {
                return target!;
            }

            valueFactory(param);

            return Volatile.Read(ref target)!;
        }
    }
}
