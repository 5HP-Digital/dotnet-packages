namespace Digital5HP.Identity;

using System;

public class TokenException : ExceptionBase
{
    public TokenException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}