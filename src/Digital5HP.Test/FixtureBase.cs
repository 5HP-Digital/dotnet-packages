namespace Digital5HP.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using AutoFixture;

    using Moq;

    using Digital5HP.Logging;

    public abstract class FixtureBase : IDisposable
    {
        protected FixtureBase(Action<IFixture> config = null)
        {
            this.Fixture = new Fixture()
               .CustomizeDefault();

            config?.Invoke(this.Fixture);
        }

        protected IFixture Fixture { get; }

        /// <summary>
        /// Creates an instance of the specified type with auto-generated property values
        /// </summary>
        /// <typeparam name="T">The type to create</typeparam>
        protected T Create<T>()
        {
            return this.Fixture.Create<T>();
        }

        /// <summary>
        /// Creates an instance of the specified type with auto-generated property values and applies an action over it.
        /// </summary>
        /// <param name="action">Action to apply on created instance.</param>
        /// <typeparam name="T">The type to create</typeparam>
        protected T Create<T>(Action<T> action)
        {
            var created = this.Fixture.Create<T>();
            action?.Invoke(created);
            return created;
        }

        /// <summary>
        /// Creates many instances of the specified type with auto-generated property values.
        /// </summary>
        /// <typeparam name="T">The type to create.</typeparam>
        protected IEnumerable<T> CreateMany<T>()
        {
            return this.Fixture.CreateMany<T>();
        }

        /// <summary>
        /// Creates an instance of the specified type using its internal constructor.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="types">The parameter types of the internal constructor.</param>
        /// <param name="parameterValues">The values to pass to the constructor.</param>
#pragma warning disable CA1822
        protected T CreateWithInternalCtor<T>(Type[] types = null, object[] parameterValues = null)
#pragma warning restore CA1822
        {
            types ??= Array.Empty<Type>();
            var ctorInfo = typeof(T).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                types,
                null);

            if (ctorInfo == null)
                return default;

            try
            {
                return (T) ctorInfo.Invoke(parameterValues);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        /// <summary>
        /// Creates an instance of the specified type without auto-generated property values
        /// </summary>
        /// <typeparam name="T">The type to create</typeparam>
        protected T CreateWithoutAutoProperties<T>()
        {
            return this.Fixture.Build<T>()
                       .WithAutoProperties()
                       .Create();
        }

        /// <summary>
        /// Creates the specified number of  instances of the specified type
        /// with auto-generated property values
        /// </summary>
        /// <param name="number">The number of instances to create</param>
        /// <typeparam name="T">The type to create</typeparam>
        protected IEnumerable<T> CreateMany<T>(int number)
        {
            return this.Fixture.CreateMany<T>(number)
                       .ToArray();
        }

        /// <summary>
        /// Initializes and instance of the Mock with Default behavior
        /// </summary>
        /// <typeparam name="T">The type of Mock to create</typeparam>
        /// <returns></returns>
#pragma warning disable CA1822
        protected Mock<T> CreateMock<T>(bool forceStrict = true)
#pragma warning restore CA1822
            where T : class
        {
            return new Mock<T>(forceStrict ? MockBehavior.Strict : MockBehavior.Default);
        }

#pragma warning disable CA1822
        protected MockHttpClient CreateMockHttpClient()
#pragma warning restore CA1822
        {
            return new MockHttpClient();
        }

        /// <summary>
        /// Creates a Mock for <see cref="ILogger{T}"/> for type <typeparamref name="T"/>.
        /// <para>
        /// The Mock is set up with all log methods verifiable.
        /// </para>
        /// </summary>
        protected Mock<ILogger<T>> CreateLogger<T>()
        {
            var mock = this.CreateMock<ILogger<T>>();

#pragma warning disable DHPLOG0004
            mock.Setup(x => x.LogDebug(It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogDebug(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogTrace(It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogTrace(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogInformation(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogWarning(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogError(It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogCritical(It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogCritical(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
#pragma warning restore DHPLOG0004

            return mock;
        }

        /// <summary>
        /// Creates a Mock for <see cref="ILogger"/>.
        /// <para>
        /// The Mock is set up with all log methods verifiable.
        /// </para>
        /// </summary>
        protected Mock<ILogger> CreateTypelessLogger()
        {
            var mock = this.CreateMock<ILogger>();

#pragma warning disable DHPLOG0004
            mock.Setup(x => x.LogDebug(It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogDebug(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogTrace(It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogTrace(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogInformation(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogWarning(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogError(It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogCritical(It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            mock.Setup(x => x.LogCritical(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
#pragma warning restore DHPLOG0004

            return mock;
        }

        /// <summary>
        /// Registers a specific instance for a specific type, in order to make
        /// that instance a shared instance, no matter how many times the
        /// Fixture is asked to create an instance of that type.
        /// </summary>
        /// <typeparam name="T">
        /// The type for which <paramref name="instance"/> should be registered.
        /// </typeparam>
        /// <param name="instance">The instance to register.</param>
        /// <remarks>
        /// <para>
        /// Registering an instance of a specific type in the
        /// <see cref="Fixture" /> effectively 'locks' the type to that
        /// specific instance. The registered <paramref name="instance" /> becomes a
        /// shared instance. No matter how many times the Fixture instance is
        /// asked to create an instance of <typeparamref name="T" />, the
        /// shared item is returned.
        /// </para>
        /// <para>
        /// If you are familiar with DI Container lifetime management, the
        /// following parallel may be helpful. If not, skip the next paragraph.
        /// </para>
        /// <para>
        /// Most DI Containers come with several built-in lifetime scopes. The
        /// two most common lifetime scopes are Transient (a new instance is
        /// created for every request) and Singleton (the same instance is used
        /// for all requests) (don't confuse the Singleton lifetime scope with
        /// the Singleton design pattern; they are related, but different). By
        /// default, Fixture uses the Transient lifetime scope: it creates a
        /// new instance for every request. However, using the RegisterInSingletonLifetimeScope method,
        /// effectively changes the lifetime scope for that particular type to
        /// Singleton.
        /// </para>
        /// </remarks>
        /// <example>
        /// This example demonstrates that when registering an instance of the
        /// custom class MyClass in a Fixture instance, the Fixture instance
        /// will return the originally registered MyClass instance every time
        /// it's asked to create an instance of MyClass.
        /// <code>
        /// var original = new MyClass();
        /// this.Fixture.RegisterInSingletonLifetimeScope(original);
        ///
        /// var actual1 = this.Create&lt;MyClass&gt;();
        /// var actual2 = this.Create&lt;MyClass&gt;();
        ///
        /// // actual1 and actual2 are equal, and equal to original
        /// Assert.Same(actual1, actual2);
        /// Assert.Same(original, actual1);
        /// Assert.Same(original, actual2);
        /// </code>
        /// </example>
        protected void RegisterInSingletonLifetimeScope<T>(T instance)
            where T : class
        {
            this.Fixture.Inject(instance);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
