namespace Digital5HP.Text.M3U;

using System;

public class SerializationException(string message, Exception innerException = null) : AppCoreException(message, innerException)
{
}
