namespace Digital5HP.ObjectMapping;

using System;

public class MappingException : ExceptionBase
{
    public MappingException(string message)
        : base(message)
    {
    }

    public MappingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
