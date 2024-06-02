namespace Digital5HP.AspNetCore.Versioning;

using System.Linq;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc.ApplicationModels;

internal static class CommonModelExtensions
{
    internal static ApiVersion GetIntroducedVersion(this ICommonModel model)
    {
        return model.Attributes
                    .OfType<IntroducedInAttribute>()
                    .Select(a => a.Version)
                    .SingleOrDefault();
    }

    internal static ApiVersion GetRemovedVersion(this ICommonModel model)
    {
        return model.Attributes
                    .OfType<RemovedAsOfAttribute>()
                    .Select(a => a.Version)
                    .SingleOrDefault();
    }
}
