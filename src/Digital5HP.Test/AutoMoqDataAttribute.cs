namespace Digital5HP.Test
{
    using System;
    using System.Collections.Generic;

    using AutoFixture;
    using AutoFixture.Xunit2;

    // TODO: create abstract attribute
#pragma warning disable CA1813
    public class AutoMoqDataAttribute : AutoDataAttribute
#pragma warning restore CA1813
    {
        public AutoMoqDataAttribute(ICustomization customization)
            : base(
                () => Create(
                    customizations: new[]
                                    {
                                        customization,
                                    }))
        {
        }

        public AutoMoqDataAttribute(params Type[] customizationTypes)
            : base(() => Create(customizationTypes: customizationTypes))
        {
        }

        private static Fixture Create(IEnumerable<Type> customizationTypes = null, IEnumerable<ICustomization> customizations = null)
        {
            var fixture = new Fixture()
               .CustomizeDefault();

            if (customizationTypes != null)
                foreach (var customizationType in customizationTypes)
                {
                    var c = Activator.CreateInstance(customizationType);

                    if (c is not ICustomization customization)
                        throw new InvalidOperationException(
                            $"Type provided to {nameof(AutoMoqDataAttribute)} must inherit from {nameof(ICustomization)}");

                    fixture.Customize(customization);
                }

            if (customizations != null)
                foreach (var customization in customizations)
                {
                    fixture.Customize(customization);
                }

            return fixture;
        }
    }
}
