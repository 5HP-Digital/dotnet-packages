namespace Digital5HP.Test
{
    using Moq;

    using Digital5HP.Logging;

    /// <summary>
    /// Mocked <see cref="ILogger{T}"/> with loose behavior.
    /// </summary>
    public class MockLogger<T> : LoggerWrapper, ILogger<T>
    {
        public MockLogger()
            : base(new Mock<ILogger<T>>(MockBehavior.Loose).Object)
        {
        }
    }
}
