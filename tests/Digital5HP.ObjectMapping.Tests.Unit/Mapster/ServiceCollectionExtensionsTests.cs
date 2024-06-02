namespace Digital5HP.ObjectMapping.Tests.Unit.Mapster
{
    using System;

    using global::Mapster;

    using MapsterMapper;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using Digital5HP.Logging;
    using Digital5HP.ObjectMapping.Mapster;
    using Digital5HP.Test;

    using Xunit;

    [Trait("Category", "Unit")]
    public class ServiceCollectionExtensionsTests : FixtureBase
    {
        [Fact]
        public void AddObjectMapping_Succeed()
        {
            // Arrange
            var serviceCollectionMock = this.CreateMock<IServiceCollection>();
            serviceCollectionMock.Setup(x => x.Add(It.IsAny<ServiceDescriptor>()))
                                 .Verifiable();
            serviceCollectionMock.SetupGet(x => x.Count)
                                 .Returns(0);

            // Act
            serviceCollectionMock.Object.AddObjectMapping(typeof(TestMappingProfile).Assembly);

            // Assert
            serviceCollectionMock.Verify(
                x => x.Add(
                    It.Is<ServiceDescriptor>(
                        v => v.Lifetime == ServiceLifetime.Transient
                             && v.ServiceType == typeof(IMapper<SourceClass>)
                             && v.ImplementationType == typeof(Mapper<SourceClass>))),
                Times.Once);
            serviceCollectionMock.Verify(
                x => x.Add(
                    It.Is<ServiceDescriptor>(
                        v => v.Lifetime == ServiceLifetime.Transient
                             && v.ServiceType == typeof(IMapperProvider)
                             && v.ImplementationType == typeof(MapperProvider))),
                Times.Once);
            serviceCollectionMock.Verify(
                x => x.Add(
                    It.Is<ServiceDescriptor>(
                        v => v.Lifetime == ServiceLifetime.Singleton
                             && v.ServiceType == typeof(TypeAdapterConfig)
                             && v.ImplementationFactory != null && this.TestTypeAdapterConfigFactory(v.ImplementationFactory))),
                Times.Once);
            serviceCollectionMock.Verify(
                x => x.Add(
                    It.Is<ServiceDescriptor>(
                        v => v.Lifetime == ServiceLifetime.Scoped
                             && v.ServiceType == typeof(IMapper)
                             && v.ImplementationType == typeof(ServiceMapper))),
                Times.Once);
        }

        private bool TestTypeAdapterConfigFactory(Func<IServiceProvider, object> factory)
        {
            // Arrange
            var loggerMock = this.CreateMock<ILogger>();
            loggerMock.Setup(x => x.LogDebug(It.IsAny<string>(), It.IsAny<object[]>()))
                      .Verifiable();

            var serviceProviderMock = this.CreateMock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(ILogger)))
                               .Returns(loggerMock.Object);

            // Act
            var result = factory(serviceProviderMock.Object);

            // Assert
            loggerMock.Verify(x => x.LogDebug(It.IsAny<string>(), It.IsAny<object[]>()), Times.Exactly(2));

            return result is TypeAdapterConfig config
                   && config.RequireExplicitMapping
                   && config.RequireDestinationMemberSource
                   && config.Default.Settings.AvoidInlineMapping == true;
        }
    }
}
