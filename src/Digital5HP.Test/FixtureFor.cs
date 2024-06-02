#pragma warning disable RVLOG0004
namespace Digital5HP.Test
{
    using System;

    using AutoFixture;

    using Moq;

    using Digital5HP.Logging;

    public abstract class FixtureFor<TSut> : FixtureBase
    {
        private readonly Lazy<Mock<ILogger<TSut>>> lazyLogger;

        private readonly Lazy<TSut> lazySut;

        protected FixtureFor(Action<IFixture> config = null) : base(config)
        {
            this.lazySut = new Lazy<TSut>(this.CreateSutInternal);
            this.lazyLogger = new Lazy<Mock<ILogger<TSut>>>(this.CreateLogger);
        }

        /// <summary>
        /// The Sut (System Under Test)
        /// </summary>
        protected TSut Sut => this.lazySut.Value;

        protected Mock<ILogger<TSut>> Logger => this.lazyLogger.Value;

        /// <summary>
        /// Called by the base class to create an instance of the SUT (System Under Test)
        /// </summary>
        protected abstract TSut CreateSut();

        /// <summary>
        /// Creates a Mock for <see cref="ILogger{T}"/> for the current SUT type <typeparamref name="TSut"/>.
        /// <para>
        /// The Mock is set up with all log methods verifiable.
        /// </para>
        /// </summary>
        protected Mock<ILogger<TSut>> CreateLogger()
        {
            return this.CreateLogger<TSut>();
        }

        /// <summary>
        /// Setup constructor statements.
        /// </summary>
        protected virtual void SetupConstructor()
        {
        }

        /// <summary>
        /// Verify constructor statements.
        /// </summary>
        protected virtual void VerifyConstructor()
        {
        }

        private TSut CreateSutInternal()
        {
            this.SetupConstructor();

            var sut = this.CreateSut();

            this.VerifyConstructor();

            return sut;
        }
    }
}
