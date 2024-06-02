namespace Digital5HP.Logging;

using System;

public interface ILoggerFactory
{
    ILogger Create(Type contextType);

    ILogger<T> Create<T>();
}
