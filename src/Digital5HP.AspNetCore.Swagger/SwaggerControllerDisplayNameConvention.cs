namespace Digital5HP.AspNetCore.Swagger;

using System;

using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.ComponentModel;
using System.Text.RegularExpressions;

public class SwaggerControllerDisplayNameConvention : IControllerModelConvention
{
    private const string SPLIT_PASCAL_CASE_PATTERN = @"(?<=[A-Za-z])(?=[A-Z][a-z])|(?<=[a-z0-9])(?=[0-9]?[A-Z])";

    public void Apply(ControllerModel controller)
    {
        if (controller == null)
            return;

        foreach (var attribute in controller.Attributes)
        {
            if (attribute.GetType() == typeof(DisplayNameAttribute))
            {
                var routeAttribute = (DisplayNameAttribute)attribute;

                if (!string.IsNullOrWhiteSpace(routeAttribute.DisplayName))
                    controller.ControllerName = routeAttribute.DisplayName;
            }
            else
            {
                controller.ControllerName = Regex.Replace(
                    controller.ControllerName,
                    SPLIT_PASCAL_CASE_PATTERN,
                    " ",
                    RegexOptions.ExplicitCapture,
                    TimeSpan.FromSeconds(1));
            }
        }
    }
}
