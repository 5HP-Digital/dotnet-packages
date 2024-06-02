namespace Digital5HP.DataAccess;

using System;

public class DataAccessException : ExceptionBase
{
    public DataAccessException(string message)
        : base(message)
    {
    }

    public DataAccessException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
