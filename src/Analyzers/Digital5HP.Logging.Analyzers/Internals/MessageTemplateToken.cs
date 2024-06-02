namespace Digital5HP.Logging.Analyzers.Internals;

/// <summary>
/// An element parsed from a message template string.
/// </summary>
/// <remarks>
/// Construct a <see cref="MessageTemplateToken"/>.
/// </remarks>
/// <param name="startIndex">The token's start index in the template.</param>
/// <param name="length"></param>
internal abstract class MessageTemplateToken(int startIndex, int length)
{

    /// <summary>
    /// The token's start index in the template.
    /// </summary>
    public int StartIndex { get; set; } = startIndex;

    /// <summary>
    /// The token's length.
    /// </summary>
    public int Length { get; set; } = length;
}