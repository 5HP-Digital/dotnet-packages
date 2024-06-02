namespace Digital5HP.Logging.Analyzers.Internals;

using Microsoft.CodeAnalysis.CSharp.Syntax;

internal class SourceArgument(ArgumentSyntax argument, int startIndex, int length)
{
    public ArgumentSyntax Argument { get; } = argument;

    public int StartIndex { get; } = startIndex;

    public int Length { get; } = length;
}
