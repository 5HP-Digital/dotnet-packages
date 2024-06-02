namespace Digital5HP.Swagger.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using Asp.Versioning;
    using Asp.Versioning.ApiExplorer;

    using AutoFixture;
    using AutoFixture.Xunit2;

    using Digital5HP.AspNetCore.Swagger;
    using Digital5HP.Test;

    using FluentAssertions;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;

    using Moq;

    using Swashbuckle.AspNetCore.SwaggerGen;

    using Xunit;

    public class SwaggerCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            // ApiVersion
            fixture.Register(() => new ApiVersion(fixture.CreateIntInRange(0, 999), fixture.CreateIntInRange(0, 999)));
        }
    }

    /// <summary>
    /// Tests the methods in the <see cref="ConfigureSwaggerOptions"/> class.
    /// </summary>
    public class ConfigureSwaggerOptionsTests : UnitFixtureFor<ConfigureSwaggerOptions>
    {
        private readonly Mock<IApiVersionDescriptionProvider> mockApiVersionDescriptionProvider;
        private readonly Mock<IOptions<ServiceConfiguration>> mockServiceConfigOptions;

        public ConfigureSwaggerOptionsTests()
            : base(f => f.Customize(new SwaggerCustomization()))
        {
            this.mockServiceConfigOptions = this.CreateMock<IOptions<ServiceConfiguration>>();
            this.mockApiVersionDescriptionProvider = this.CreateMock<IApiVersionDescriptionProvider>();
        }

        protected override ConfigureSwaggerOptions CreateSut()
        {
            return new ConfigureSwaggerOptions(
                this.mockServiceConfigOptions.Object,
                this.mockApiVersionDescriptionProvider.Object);
        }

        /// <summary>
        /// Ensures <see cref="ConfigureSwaggerOptions.Configure"/> configures <see cref="SwaggerGenOptions"/> objects
        /// such that there is a public and an internal Swagger document for each API version.
        /// </summary>
        /// <param name="apiVersions">The API version identifiers.</param>
        [Theory]
        [AutoMoqData(typeof(SwaggerCustomization))]
        public void Configure_AtLeastOneApiVersion_AddsPublicAndInternalSwaggerDocsForEachApiVersion(IReadOnlyCollection<ApiVersion> apiVersions)
        {
            // Arrange
            this.SetUpSwaggerTitle(this.Create<string>());

            var apiVersionDescriptions = apiVersions
                                        .Select(
                                             version => new ApiVersionDescription(version, version.ToString()))
                                        .ToImmutableList();

            // Set up the IApiVersionDescriptionProvider
            this.mockApiVersionDescriptionProvider.Setup(provider => provider.ApiVersionDescriptions)
                .Returns(apiVersionDescriptions);

            // Set up the rest of the test
            var swaggerGenOptions = new SwaggerGenOptions();

            // Act
            this.Sut.Configure(swaggerGenOptions);

            // Assert
            var swaggerDocNames = swaggerGenOptions.SwaggerGeneratorOptions.SwaggerDocs.Keys;
            foreach (var version in apiVersions)
            {
                // Ensure there is a public doc and an internal doc
                swaggerDocNames.Should()
                               .ContainSingle(docName => docName == version.ToString());
                swaggerDocNames.Should()
                               .ContainSingle(
                                    docName => docName
                                               == $"{version}{ConfigureSwaggerOptions.INTERNAL_DOC_NAME_SUFFIX}");
            }
        }

        /// <summary>
        /// Ensures <see cref="ConfigureSwaggerOptions.Configure"/> sets the title of each Swagger doc to the value
        /// specified in the given <see cref="IConfiguration"/>.
        /// </summary>
        /// <param name="expectedTitle">The value to set as the "title" configuration value.</param>
        [Theory]
        [AutoData]
        public void Configure_TitleConfigValueNotNull_SetsSwaggerDocTitleToConfigValue(string expectedTitle)
        {
            // Arrange

            // Set up the IConfiguration
            this.SetUpSwaggerTitle(expectedTitle);

            // Set up the IApiVersionDescriptionProvider
            this.mockApiVersionDescriptionProvider.Setup(provider => provider.ApiVersionDescriptions)
                .Returns(
                     this.CreateMany<ApiVersionDescription>()
                         .ToList());

            // Set up the rest of the test
            var swaggerGenOptions = new SwaggerGenOptions();

            // Act
            this.Sut.Configure(swaggerGenOptions);

            // Assert
            foreach (var doc in swaggerGenOptions.SwaggerGeneratorOptions.SwaggerDocs.Values)
            {
                doc.Title.Should()
                   .BeEquivalentTo(expectedTitle);
            }
        }

        /// <summary>
        /// Sets up the mock <see cref="IConfiguration"/> to return the specified title when getting the Swagger title config value.
        /// </summary>
        /// <param name="expectedTitle">The title the mock <see cref="IConfiguration"/> should return.</param>
        private void SetUpSwaggerTitle(string expectedTitle)
        {
            this.mockServiceConfigOptions.SetupGet(x => x.Value)
                .Returns(
                     new ServiceConfiguration
                     {
                         Title = expectedTitle
                     });
        }
    }
}
