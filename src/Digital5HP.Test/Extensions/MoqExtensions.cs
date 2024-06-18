#pragma warning disable CA2000
#pragma warning disable VSTHRD200
namespace Digital5HP.Test
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading.Tasks;

    using Moq;
    using Moq.Language;
    using Moq.Language.Flow;

    public static class MoqExtensions
    {
        /// <summary>
        /// Marks the expectation as verifiable and will return a completed task, meaning that a call
        /// to <see cref="Mock.Verify()"/> will check if this particular
        /// expectation was met.
        /// </summary>
        public static IReturnsResult<TMock> VerifiableAsync<TMock>(this IReturnsThrows<TMock, Task> setup)
            where TMock : class
        {
            ArgumentNullException.ThrowIfNull(setup);

            return setup.Returns(Task.CompletedTask);
        }

        /// <summary>
        /// Marks the expectation as verifiable and will return a completed task, meaning that a call
        /// to <see cref="Mock.Verify()"/> will check if this particular
        /// expectation was met.
        /// </summary>
        public static IReturnsResult<TMock> VerifiableAsync<TMock>(this IReturnsThrows<TMock, ValueTask> setup)
            where TMock : class
        {
            ArgumentNullException.ThrowIfNull(setup);

            return setup.Returns(ValueTask.CompletedTask);
        }

        public static IReturnsResult<HttpMessageHandler> ReturnsAsync(
            this IReturnsThrows<HttpMessageHandler, Task<HttpResponseMessage>> setup,
            HttpStatusCode statusCode,
            string content = null)
        {
            var response = new HttpResponseMessage(statusCode);

            if (content != null)
            {
                response.Content = new StringContent(content);
            }

            return setup.ReturnsAsync(response);
        }

        public static ISetupSequentialResult<Task<HttpResponseMessage>> ReturnsAsync(
            this ISetupSequentialResult<Task<HttpResponseMessage>> setup,
            HttpStatusCode statusCode,
            string content = null)
        {
            var response = new HttpResponseMessage(statusCode);

            if (content != null)
            {
                response.Content = new StringContent(content);
            }

            return setup.ReturnsAsync(response);
        }

        #region OutCallback

        public delegate void OutAction<TOut>(out TOut outVal);
        public delegate void OutAction<in T1, TOut>(T1 arg1, out TOut outVal);
        public delegate void OutAction<in T1, in T2, TOut>(T1 arg1, T2 arg2, out TOut outVal);
        public delegate void OutAction<in T1, in T2, in T3, TOut>(T1 arg1, T2 arg2, T3 arg3, out TOut outVal);
        public delegate void OutAction<in T1, in T2, in T3, in T4, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, out TOut outVal);

        /// <summary>
        /// Configures a callback for method with an <see langword="out"/> parameter and a return type.
        /// </summary>
        public static IReturnsThrows<TMock, TReturn> OutCallback<TMock, TReturn, TOut>(this ICallback<TMock, TReturn> mock, OutAction<TOut> action)
            where TMock : class
        {
            return OutCallbackInternal(mock, action);
        }

        /// <summary>
        /// Configures a callback for an <see langword="out"/> parameter.
        /// </summary>
        public static ICallbackResult OutCallback<TOut>(this ICallback mock, OutAction<TOut> action)
        {
            return OutCallbackInternal(mock, action);
        }

        /// <summary>
        /// Configures a callback for method with an <see langword="out"/> parameter and a return type.
        /// </summary>
        public static IReturnsThrows<TMock, TReturn> OutCallback<TMock, TReturn, T1, TOut>(this ICallback<TMock, TReturn> mock, OutAction<T1, TOut> action)
            where TMock : class
        {
            return OutCallbackInternal(mock, action);
        }

        /// <summary>
        /// Configures a callback for an <see langword="out"/> parameter.
        /// </summary>
        public static ICallbackResult OutCallback<T1, TOut>(this ICallback mock, OutAction<T1, TOut> action)
        {
            return OutCallbackInternal(mock, action);
        }

        /// <summary>
        /// Configures a callback for method with an <see langword="out"/> parameter and a return type.
        /// </summary>
        public static IReturnsThrows<TMock, TReturn> OutCallback<TMock, TReturn, T1, T2, TOut>(this ICallback<TMock, TReturn> mock, OutAction<T1, T2, TOut> action)
            where TMock : class
        {
            return OutCallbackInternal(mock, action);
        }

        /// <summary>
        /// Configures a callback for an <see langword="out"/> parameter.
        /// </summary>
        public static ICallbackResult OutCallback<T1, T2, TOut>(this ICallback mock, OutAction<T1, T2, TOut> action)
        {
            return OutCallbackInternal(mock, action);
        }

        /// <summary>
        /// Configures a callback for method with an <see langword="out"/> parameter and a return type.
        /// </summary>
        public static IReturnsThrows<TMock, TReturn> OutCallback<TMock, TReturn, T1, T2, T3, TOut>(this ICallback<TMock, TReturn> mock, OutAction<T1, T2, T3, TOut> action)
            where TMock : class
        {
            return OutCallbackInternal(mock, action);
        }

        /// <summary>
        /// Configures a callback for an <see langword="out"/> parameter.
        /// </summary>
        public static ICallbackResult OutCallback<T1, T2, T3, TOut>(this ICallback mock, OutAction<T1, T2, T3, TOut> action)
        {
            return OutCallbackInternal(mock, action);
        }

        /// <summary>
        /// Configures a callback for method with an <see langword="out"/> parameter and a return type.
        /// </summary>
        public static IReturnsThrows<TMock, TReturn> OutCallback<TMock, TReturn, T1, T2, T3, T4, TOut>(this ICallback<TMock, TReturn> mock, OutAction<T1, T2, T3, T4, TOut> action)
            where TMock : class
        {
            return OutCallbackInternal(mock, action);
        }

        /// <summary>
        /// Configures a callback for an <see langword="out"/> parameter.
        /// </summary>
        public static ICallbackResult OutCallback<T1, T2, T3, T4, TOut>(this ICallback mock, OutAction<T1, T2, T3, T4, TOut> action)
        {
            return OutCallbackInternal(mock, action);
        }

        private static IReturnsThrows<TMock, TReturn> OutCallbackInternal<TMock, TReturn>(ICallback<TMock, TReturn> mock, object action)
            where TMock : class
        {
            ArgumentNullException.ThrowIfNull(mock);

            var methodCall = mock.GetType()
                                 .GetProperty("Setup")
                                 ?.GetValue(mock);

            mock.GetType()
                .Assembly.GetType("Moq.MethodCall")
               ?.InvokeMember(
                     "SetCallbackBehavior",
                     BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                     null,
                     methodCall,
                     [action],
                     CultureInfo.InvariantCulture);

            return mock as IReturnsThrows<TMock, TReturn>;
        }

        private static ICallbackResult OutCallbackInternal(ICallback mock, object action)
        {
            ArgumentNullException.ThrowIfNull(mock);

            var methodCall = mock.GetType()
                                 .GetProperty("Setup")
                                ?.GetValue(mock);

            mock.GetType()
                .Assembly.GetType("Moq.MethodCall")
               ?.InvokeMember(
                     "SetCallbackBehavior",
                     BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                     null,
                     methodCall,
                     [action],
                     CultureInfo.InvariantCulture);

            return (ICallbackResult)mock;
        }

        #endregion
    }
}
