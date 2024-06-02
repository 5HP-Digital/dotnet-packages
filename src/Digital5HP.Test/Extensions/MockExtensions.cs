namespace Digital5HP.Test
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Microsoft.Extensions.Configuration;

    using Moq;

    public static class MockExtensions
    {
        public static Mock<T> MockFluentInterface<T>(this Mock<T> mock)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(mock);

            var isAnyMethodInfo = typeof(It).GetMethod(nameof(It.IsAny));

            var parameterExpression = Expression.Parameter(typeof(T));

            var fluentMethods =
                typeof(T).GetMethods()
                         .Where(methodInfo => methodInfo.ReturnType == typeof(T))
                         .ToArray();

            foreach (var fluentMethod in fluentMethods)
            {
                var parameters = fluentMethod.GetParameters()
                                             .Select(
                                                  parameterInfo =>
                                                      Expression.Call(
                                                          isAnyMethodInfo?.MakeGenericMethod(
                                                              parameterInfo.ParameterType) ?? throw new InvalidOperationException(),
                                                          Array.Empty<Expression>()))
                                             .Cast<Expression>()
                                             .ToArray();

                var fluentExpression = Expression.Lambda<Func<T, T>>(
                    Expression.Call(parameterExpression, fluentMethod, parameters),
                    [parameterExpression]);

                mock.Setup(fluentExpression)
                    .Returns(mock.Object);
            }

            return mock;
        }

        public static void SetupConfigurationGetValue(this Mock<IConfiguration> mock, string value)
        {
            ArgumentNullException.ThrowIfNull(mock);

            var configurationSectionMock = new Mock<IConfigurationSection>(MockBehavior.Strict);
            configurationSectionMock.Setup(x => x.Value)
                                    .Returns(value);
            configurationSectionMock.Setup(x => x.Path)
                                    .Returns(string.Empty);

            mock.Setup(x => x.GetSection(It.IsAny<string>()))
                .Returns(configurationSectionMock.Object);
        }

        public static void VerifyConfigurationGetValue(this Mock<IConfiguration> mock,
                                                       string key,
                                                       Times? times = null)
        {
            ArgumentNullException.ThrowIfNull(mock);

            mock.Verify(x => x.GetSection(It.Is<string>(v => v == key)), times ?? Times.AtLeastOnce());
        }

        public static void UnfreezeAndVerify(this Mock<ITimeProvider> mock, Times? times = null)
        {
            ArgumentNullException.ThrowIfNull(mock);

            Digital5HP.TimeProvider.ResetProvider();
            mock.Verify(provider => provider.Now, times ?? Times.AtLeastOnce());
        }
    }
}
