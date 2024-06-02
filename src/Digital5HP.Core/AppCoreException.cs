namespace Digital5HP;

using System;

public class AppCoreException : ExceptionBase
{
    public AppCoreException(string message)
        : base(message)
    {
    }

    public AppCoreException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
