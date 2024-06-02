namespace Digital5HP.Logging;

using System;

/// <summary>
/// Indicates that the marked method logs data using a message template and (optional) arguments.
/// The name of the parameter which contains the message template should be given in the constructor.
/// </summary>
/// <example>
/// <code>
/// [StructuredMessageTemplateMethod("messageTemplate")]
/// public void Information(string messageTemplate, params object[] propertyValues)
/// {
///     // Do something
/// }
///
/// public void Foo()
/// {
///     Information("Hello, {Name}!") // Warning: Non-existing argument in message template.
/// }
/// </code>
/// </example>
/// <remarks>
/// Initializes a new instance of the <see cref="StructuredMessageTemplateMethodAttribute"/> class.
/// </remarks>
/// <param name="messageTemplateParameterName">Name of the message template parameter.</param>
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method)]
public sealed class StructuredMessageTemplateMethodAttribute(string messageTemplateParameterName) : Attribute
{

    /// <summary>
    /// Gets the name of the message template parameter.
    /// </summary>
    /// <value>The name of the message template parameter.</value>
    public string MessageTemplateParameterName { get; } = messageTemplateParameterName;
}
