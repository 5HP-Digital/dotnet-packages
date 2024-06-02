namespace Digital5HP.ObjectMapping.Tests.Unit
{
    using System;

    using Digital5HP.Test;
    using Digital5HP.ObjectMapping.Tests.Unit.Mapster;

    using FluentAssertions;

    using Moq;

    using Xunit;

    public class MapperProviderTests : UnitFixtureFor<MapperProvider>
    {
        private readonly Mock<IServiceProvider> serviceProviderMock;

        public MapperProviderTests()
        {
            this.serviceProviderMock = this.CreateMock<IServiceProvider>();
        }

        protected override MapperProvider CreateSut()
        {
            return new (this.serviceProviderMock.Object);
        }

        [Fact]
        public void Get_Succeed()
        {
            // Arrange
            this.serviceProviderMock.Setup(x => x.GetService(It.IsAny<Type>()))
                .Returns(this.CreateMock<IMapper<SourceClass>>().Object);

            // Act
            var result = this.Sut.Get<SourceClass>();

            // Assert
            result.Should()
                  .NotBeNull();

            this.serviceProviderMock.Verify(x => x.GetService(typeof(IMapper<SourceClass>)), Times.Once);
        }

        [Fact]
        public void Get_MapperNotFound_Succeed()
        {
            // Arrange
            this.serviceProviderMock.Setup(x => x.GetService(It.IsAny<Type>()))
                .Returns((object)null);

            // Act
            var result = Record.Exception(() => this.Sut.Get<SourceClass>());

            // Assert
            result.Should()
                  .NotBeNull()
                  .And.BeOfType<MappingException>();

            result.As<MappingException>()
                  .InnerException.Should()
                  .NotBeNull()
                  .And.BeOfType<InvalidOperationException>();

            this.serviceProviderMock.Verify(x => x.GetService(typeof(IMapper<SourceClass>)), Times.Once);
        }
    }
}
