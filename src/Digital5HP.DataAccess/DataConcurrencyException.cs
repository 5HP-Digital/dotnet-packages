namespace Digital5HP.DataAccess;

using System;

public class DataConcurrencyException : ExceptionBase
{
    public DataConcurrencyException(string message)
        : base(message)
    {
    }

    public DataConcurrencyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
