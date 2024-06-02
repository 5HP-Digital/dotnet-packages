namespace Digital5HP.Logging.Internals;

using Microsoft.Extensions.Logging;

internal class LoggerWrapper<T>(ILoggerFactory loggerFactory) : LoggerWrapperBase(loggerFactory.CreateLogger<T>()), Logging.ILogger<T>
{
}
