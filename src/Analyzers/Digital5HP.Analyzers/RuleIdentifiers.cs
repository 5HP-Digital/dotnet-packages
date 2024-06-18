namespace Digital5HP.Analyzers;

using System.Globalization;

internal static class RuleIdentifiers
{
    public const string MAKE_PROPERTY_VIRTUAL = "DHP0001";
    public const string OVERRIDE_TYPEID_VALIDATION_ATTRIBUTE = "DHP0002";

    public static string GetHelpUri(string identifier)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "https://github.com/5HP-Digital/dotnet-packages/blob/main/docs/Digital5HP.Analyzers.md&anchor={0}&_a=preview",
            identifier);
    }
}
