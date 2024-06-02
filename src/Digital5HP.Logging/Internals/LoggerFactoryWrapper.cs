namespace Digital5HP.Logging.Internals;

using System;

public class LoggerFactoryWrapper(Microsoft.Extensions.Logging.ILoggerFactory loggerFactory) : ILoggerFactory
{
    private readonly Microsoft.Extensions.Logging.ILoggerFactory loggerFactory = loggerFactory;

    public ILogger Create(Type contextType)
    {
        return new LoggerWrapper(this.loggerFactory);
    }

    public ILogger<T> Create<T>()
    {
        return new LoggerWrapper<T>(this.loggerFactory);
    }
}
