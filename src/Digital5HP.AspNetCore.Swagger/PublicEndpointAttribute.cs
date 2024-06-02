namespace Digital5HP.AspNetCore.Swagger;

using System;

/// <summary>
/// Identifies an action that should be included in the public Swagger documentation.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public abstract class PublicEndpointAttribute : Attribute
{
}
