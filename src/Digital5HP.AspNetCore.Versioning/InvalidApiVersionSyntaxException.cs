namespace Digital5HP.AspNetCore.Versioning;

public class InvalidApiVersionSyntaxException(string message) : AppCoreException(message)
{
}
