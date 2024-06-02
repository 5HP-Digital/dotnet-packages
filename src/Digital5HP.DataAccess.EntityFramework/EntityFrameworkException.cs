namespace Digital5HP.DataAccess.EntityFramework;

using System;

public class EntityFrameworkException : ExceptionBase
{
    public EntityFrameworkException(string message)
        : base(message)
    {
    }

    public EntityFrameworkException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}