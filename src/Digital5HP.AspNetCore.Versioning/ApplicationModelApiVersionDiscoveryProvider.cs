namespace Digital5HP.AspNetCore.Versioning;

using System;
using System.Linq;

using Microsoft.AspNetCore.Mvc.ApplicationModels;

public class ApplicationModelApiVersionDiscoveryProvider : IApplicationModelProvider
{
    public void OnProvidersExecuting(ApplicationModelProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Discover all API versions
        foreach (var controllerModel in context.Result.Controllers)
        {
            DiscoverControllerVersion(controllerModel);

            foreach (var actionModel in controllerModel.Actions)
            {
                DiscoverActionVersion(actionModel);
            }
        }
    }

    public void OnProvidersExecuted(ApplicationModelProviderContext context)
    {
    }

    public int Order => -999; // make sure executes right after DefaultApplicationModelProvider (-1000)

    private static void DiscoverControllerVersion(ControllerModel controller)
    {
        ArgumentNullException.ThrowIfNull(controller);

        var introducedInAttribute = controller.Attributes.OfType<IntroducedInAttribute>()
                                              .SingleOrDefault();

        if (introducedInAttribute == null)
            return;

        ApiVersionCollection.Instance.Add(introducedInAttribute.Version);
    }

    private static void DiscoverActionVersion(ActionModel action)
    {
        ArgumentNullException.ThrowIfNull(action);

        var introducedInAttribute = action.Attributes.OfType<IntroducedInAttribute>()
                                          .SingleOrDefault();

        if (introducedInAttribute == null)
            return;

        ApiVersionCollection.Instance.Add(introducedInAttribute.Version);
    }
}
