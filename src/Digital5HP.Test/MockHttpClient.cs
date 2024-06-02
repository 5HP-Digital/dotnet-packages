namespace Digital5HP.Test
{
    using System;
    using System.Linq.Expressions;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Moq;
    using Moq.Language;
    using Moq.Language.Flow;
    using Moq.Protected;

    public class MockHttpClient
    {
        private const string SEND_ASYNC_METHOD_NAME = "SendAsync";

        public HttpClient Client { get; }

        private readonly Mock<HttpMessageHandler> mockHttpMessageHandler;

        public MockHttpClient()
        {
            this.mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            this.Client = new HttpClient(this.mockHttpMessageHandler.Object, disposeHandler: false);
        }

        #region Setup

        public ISetup<HttpMessageHandler, Task<HttpResponseMessage>> Setup()
        {
            return this.mockHttpMessageHandler.Protected()
                       .Setup<Task<HttpResponseMessage>>(
                            SEND_ASYNC_METHOD_NAME,
                            ItExpr.IsAny<HttpRequestMessage>(),
                            ItExpr.IsAny<CancellationToken>());
        }

        public ISetupSequentialResult<Task<HttpResponseMessage>> SetupSequence()
        {
            return this.mockHttpMessageHandler.Protected()
                       .SetupSequence<Task<HttpResponseMessage>>(
                            SEND_ASYNC_METHOD_NAME,
                            ItExpr.IsAny<HttpRequestMessage>(),
                            ItExpr.IsAny<CancellationToken>());
        }

        #endregion

        #region Verify

        public void Verify(Expression<Func<HttpRequestMessage, bool>> requestMatch,
                           Expression<Func<CancellationToken, bool>> cancellationTokenMatch,
                           Times times = default)
        {
            ArgumentNullException.ThrowIfNull(requestMatch);
            ArgumentNullException.ThrowIfNull(cancellationTokenMatch);

            this.VerifyInternal(requestMatch, cancellationTokenMatch, times);
        }

        public void Verify(Expression<Func<HttpRequestMessage, bool>> requestMatch,
                           Times times = default)
        {
            ArgumentNullException.ThrowIfNull(requestMatch);

            this.VerifyInternal(requestMatch, null, times);
        }

        public void Verify(Expression<Func<HttpRequestMessage, bool>> requestMatch,
                           Func<Times> times)
        {
            times ??= () => default;

            this.VerifyInternal(requestMatch, null, times());
        }

        public void Verify(Times times = default)
        {
            this.VerifyInternal(null, null, times);
        }

        public void Verify(Func<Times> times)
        {
            times ??= () => default;

            this.VerifyInternal(null, null, times());
        }

        #endregion

        private void VerifyInternal(Expression<Func<HttpRequestMessage, bool>> requestMatch,
                                    Expression<Func<CancellationToken, bool>> cancellationTokenMatch,
                                    Times times)
        {
            var cancellationTokenExpression = cancellationTokenMatch != null
                ? ItExpr.Is(cancellationTokenMatch)
                : ItExpr.IsAny<CancellationToken>();

            var requestExpression = requestMatch != null
                ? ItExpr.Is(requestMatch)
                : ItExpr.IsAny<HttpRequestMessage>();

            this.mockHttpMessageHandler.Protected()
                .Verify(
                     SEND_ASYNC_METHOD_NAME,
                     times,
                     requestExpression,
                     cancellationTokenExpression);
        }
    }
}
