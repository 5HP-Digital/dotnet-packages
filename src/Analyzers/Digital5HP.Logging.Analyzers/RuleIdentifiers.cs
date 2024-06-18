namespace Digital5HP.Logging.Analyzers;

using System.Globalization;

internal static class RuleIdentifiers
{
    public const string INCORRECT_EXCEPTION_ARGUMENT_USAGE = "DHPLOG0001";
    public const string USE_VALID_MESSAGE_TEMPLATE_SYNTAX = "DHPLOG0002";
    public const string MISMATCH_BETWEEN_MESSAGE_TEMPLATE_TOKENS_AND_ARGUMENTS = "DHPLOG0003";
    public const string USE_PROPER_CONSTANT_STRUCTURED_MESSAGE_TEMPLATE = "DHPLOG0004";
    public const string USE_UNIQUE_PROPERTY_NAMES = "DHPLOG0005";
    public const string USE_PASCAL_CASE_PROPERTY_NAMES = "DHPLOG0006";
    public const string ANONYMOUS_OBJECTS_SHOULD_BE_DESTRUCTURED = "DHPLOG0007";

    public static string GetHelpUri(string identifier)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "https://github.com/5HP-Digital/dotnet-packages/blob/main/docs/Digital5HP.Logging.Analyzers.md&anchor={0}&_a=preview",
            identifier);
    }
}
