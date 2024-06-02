namespace Digital5HP.Logging.Analyzers.Internals;

using System;
using System.Globalization;

/// <summary>
/// A message template token representing a log event property.
/// </summary>
internal sealed class PropertyToken : MessageTemplateToken
{
    private readonly int? position;

    public PropertyToken(int startIndex, string propertyName, string rawText)
        : base(startIndex, rawText.Length)
    {
        this.PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        this.RawText = rawText ?? throw new ArgumentNullException(nameof(rawText));

        if (int.TryParse(this.PropertyName, NumberStyles.None, CultureInfo.InvariantCulture, out var p)
            && p >= 0)
        {
            this.position = p;
        }
    }

    /// <summary>
    /// The property name.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// True if the property name is a positional index; otherwise, false.
    /// </summary>
    public bool IsPositional => this.position.HasValue;

    internal string RawText { get; }

    /// <summary>
    /// Try to get the integer value represented by the property name.
    /// </summary>
    /// <param name="pos">The integer value, if present.</param>
    /// <returns>True if the property is positional, otherwise false.</returns>
    public bool TryGetPositionalValue(out int pos)
    {
        if (this.position == null)
        {
            pos = 0;
            return false;
        }

        pos = this.position.Value;
        return true;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>
    /// A string that represents the current object.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public override string ToString() => this.RawText;
}