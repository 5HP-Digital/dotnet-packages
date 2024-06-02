namespace Digital5HP.Logging.Internals;

using Microsoft.Extensions.Logging;

internal class LoggerWrapper(ILoggerFactory loggerFactory) : LoggerWrapperBase(loggerFactory.CreateLogger(categoryName: null))
{
}
