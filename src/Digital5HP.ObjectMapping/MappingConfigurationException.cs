namespace Digital5HP.ObjectMapping;

using System;

public class MappingConfigurationException : ExceptionBase
{
    public MappingConfigurationException(string message)
        : base(message)
    {
    }

    public MappingConfigurationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
