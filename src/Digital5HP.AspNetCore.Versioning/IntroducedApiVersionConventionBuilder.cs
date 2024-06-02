namespace Digital5HP.AspNetCore.Versioning;

using System;

using Asp.Versioning;
using Asp.Versioning.Conventions;

using Microsoft.AspNetCore.Mvc.ApplicationModels;

public class IntroducedApiVersionConventionBuilder : ApiVersionConventionBuilder
{
    private readonly ApiVersion startVersion;
    private readonly ApiVersion currentVersion;
    private readonly ApiVersionConventionBuilder apiVersionConventionBuilder;

    internal ApiVersionContainer AllVersions { get; }

    /// <summary>
    /// Use this convention to allow use of the <see cref="IntroducedInAttribute"/> and <see cref="RemovedAsOfAttribute"/>.
    /// Configure the range of APIs that are available. Only the current API version
    /// will be supported. All other API versions will be marked as deprecated.
    /// </summary>
    /// <param name="startVersion">The lowest API version that is available.</param>
    /// <param name="currentVersion">The current supported API version.</param>
    public IntroducedApiVersionConventionBuilder(string startVersion, string currentVersion)
        : this(
            ApiVersionConverter.Convert(startVersion),
            ApiVersionConverter.Convert(currentVersion))
    {
    }

    /// <summary>
    /// Use this convention to allow use of the <see cref="IntroducedInAttribute"/> and <see cref="RemovedAsOfAttribute"/>.
    /// Configure the range of APIs that are available. Only the current API version
    /// will be supported. All other API versions will be marked as deprecated.
    /// </summary>
    /// <param name="startVersion">The lowest API version that is available.</param>
    /// <param name="currentVersion">The current supported API version.</param>
    public IntroducedApiVersionConventionBuilder(ApiVersion startVersion, ApiVersion currentVersion)
        : this(startVersion, currentVersion, new ApiVersionConventionBuilder())
    {
    }

    internal IntroducedApiVersionConventionBuilder(ApiVersion startVersion,
                                                   ApiVersion currentVersion,
                                                   ApiVersionConventionBuilder apiVersionConventionBuilder)
    {
        this.startVersion = startVersion;
        this.currentVersion = currentVersion;
        this.AllVersions = new ApiVersionContainer(startVersion, currentVersion);
        this.apiVersionConventionBuilder = apiVersionConventionBuilder;
    }

    /// <inheritdoc cref="IApiVersionConventionBuilder.ApplyTo" />
    public override bool ApplyTo(ControllerModel controller)
    {
        ArgumentNullException.ThrowIfNull(controller);

        var controllerIntroducedInVersion = controller.GetIntroducedVersion();
        var controllerRemovedAsOfVersion = controller.GetRemovedVersion();

        ValidateControllerVersions(controller, controllerIntroducedInVersion, controllerRemovedAsOfVersion);

        if (this.UseApiConvention(controllerIntroducedInVersion, controllerRemovedAsOfVersion))
        {
            return this.apiVersionConventionBuilder.ApplyTo(controller);
        }

        var controllerConventionBuilder = this.apiVersionConventionBuilder.Controller(controller.ControllerType);
        this.SetControllerApiVersions(controllerConventionBuilder, controllerIntroducedInVersion, controllerRemovedAsOfVersion);
        this.SetActionApiVersions(controller, controllerIntroducedInVersion, controllerRemovedAsOfVersion, controllerConventionBuilder);

        this.apiVersionConventionBuilder.ApplyTo(controller);

        return true;
    }

    private bool UseApiConvention(ApiVersion controllerIntroducedInVersion, ApiVersion controllerRemovedAsOfVersion)
    {
        return controllerIntroducedInVersion == null
               || controllerIntroducedInVersion > this.currentVersion
               || (controllerRemovedAsOfVersion != null && controllerRemovedAsOfVersion <= this.startVersion);
    }

    private static void ValidateControllerVersions(ControllerModel controllerModel,
                                                   ApiVersion controllerIntroducedInVersion,
                                                   ApiVersion controllerRemovedAsOfVersion)
    {
        if (controllerIntroducedInVersion == null || controllerRemovedAsOfVersion == null)
        {
            return;
        }

        if (controllerIntroducedInVersion == controllerRemovedAsOfVersion)
        {
            throw new InvalidOperationException(
                $"({controllerModel.ControllerType}) API version cannot be introduced and removed in the same version.");
        }

        if (controllerRemovedAsOfVersion < controllerIntroducedInVersion)
        {
            throw new InvalidOperationException(
                $"({controllerModel.ControllerType}) API version cannot be removed before it is introduced.");
        }
    }

    private void SetActionApiVersions(ControllerModel controllerModel,
                                      ApiVersion controllerIntroducedInVersion,
                                      ApiVersion controllerRemovedAsOfVersion,
                                      IControllerConventionBuilder controller)
    {
        foreach (var actionModel in controllerModel.Actions)
        {
            var actionModelIntroduced = actionModel.GetIntroducedVersion();
            ValidateActionModel(controllerModel, controllerIntroducedInVersion, actionModelIntroduced, actionModel);

            var actionIntroducedVersion = actionModelIntroduced ?? controllerIntroducedInVersion;
            var actionRemovedVersion = actionModel.GetRemovedVersion() ?? controllerRemovedAsOfVersion;

            this.SetActionApiVersions(actionModel, controller, actionIntroducedVersion, actionRemovedVersion);
        }
    }

    private static void ValidateActionModel(ControllerModel controllerModel,
                                            ApiVersion controllerIntroducedInVersion,
                                            ApiVersion actionModelIntroduced,
                                            ActionModel actionModel)
    {
        if (actionModelIntroduced != null && actionModelIntroduced < controllerIntroducedInVersion)
        {
            throw new InvalidOperationException(
                $"Action ({actionModel.ActionName}) version cannot be"
                + $" introduced before controller ({controllerModel.ControllerName}) version.");
        }
    }

    private void SetActionApiVersions(ActionModel actionModel,
                                      IControllerConventionBuilder controller,
                                      ApiVersion introducedVersion,
                                      ApiVersion removedVersion)
    {
        var actionSupportedVersions = this.AllVersions.GetSupportedVersions(introducedVersion, removedVersion);
        var actionDeprecatedVersions = this.AllVersions.GetDeprecatedVersions(introducedVersion, removedVersion);

        var action = controller.Action(actionModel.ActionMethod);
        action.HasApiVersions(actionSupportedVersions);
        action.HasDeprecatedApiVersions(actionDeprecatedVersions);
    }

    private void SetControllerApiVersions(IControllerConventionBuilder controller,
                                          ApiVersion introducedVersion,
                                          ApiVersion removedVersion)
    {
        var supportedVersions =
            this.AllVersions.GetSupportedVersions(introducedVersion, removedVersion);

        var deprecatedVersions =
            this.AllVersions.GetDeprecatedVersions(introducedVersion, removedVersion);

        controller.HasApiVersions(supportedVersions);
        controller.HasDeprecatedApiVersions(deprecatedVersions);
    }
}
