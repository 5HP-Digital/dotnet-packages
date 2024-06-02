namespace Digital5HP.Logging.Analyzers.Internals;

internal class MessageTemplateDiagnostic(int startIndex,
                                 int length,
                                 string? diagnostic = null,
                                 bool mustBeRemapped = true) : MessageTemplateToken(startIndex, length)
{
    public string? Diagnostic { get; } = diagnostic;

    public bool MustBeRemapped { get; } = mustBeRemapped;
}