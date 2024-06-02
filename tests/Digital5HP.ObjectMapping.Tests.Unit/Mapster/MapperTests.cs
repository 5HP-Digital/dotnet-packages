namespace Digital5HP.ObjectMapping.Tests.Unit.Mapster
{
    using System;
    using System.Collections.Generic;

    using AutoFixture.Xunit2;

    using FluentAssertions;

    using MapsterMapper;

    using Moq;

    using Digital5HP.ObjectMapping.Mapster;
    using Digital5HP.Test;

    using Xunit;

    public class MapperTests : UnitFixtureFor<Mapper<SourceClass>>
    {
        private readonly Mock<IMapper> mapperMock;

        public MapperTests()
        {
            this.mapperMock = this.CreateMock<IMapper>();
        }

        protected override Mapper<SourceClass> CreateSut()
        {
            return new(this.mapperMock.Object, this.Logger.Object);
        }

        [Theory]
        [AutoData]
        public void Map_Succeed(SourceClass source)
        {
            // Arrange
            this.mapperMock.Setup(x => x.Map<SourceClass, TargetClass>(It.IsAny<SourceClass>()))
                .Returns(this.Create<TargetClass>());

            // Act
            var result = this.Sut.Map<TargetClass>(source);

            // Assert
            result.Should()
                  .NotBeNull();

            this.mapperMock.Verify(x => x.Map<SourceClass, TargetClass>(source), Times.Once);
        }

        [Theory]
        [AutoData]
        public void Map_InternalMapThrowException_LogsAndThrows(SourceClass source)
        {
            // Arrange
            this.mapperMock.Setup(x => x.Map<SourceClass, TargetClass>(It.IsAny<SourceClass>()))
                .Throws<InvalidOperationException>();

            // Act
            var result = Record.Exception(() => this.Sut.Map<TargetClass>(source));

            // Assert
            result.Should()
                  .NotBeNull()
                  .And.BeOfType<MappingException>()
                  .And.Match((MappingException ex) => ex.InnerException is InvalidOperationException);

            this.mapperMock.Verify(x => x.Map<SourceClass, TargetClass>(source), Times.Once);
            this.Logger.VerifyLogError<Mapper<SourceClass>, InvalidOperationException>(Times.Once());
        }

        [Theory]
        [AutoData]
        public void MapCollection_Succeed(SourceClass[] source)
        {
            // Arrange
            this.mapperMock.Setup(x => x.Map<IEnumerable<SourceClass>, IEnumerable<TargetClass>>(It.IsAny<IEnumerable<SourceClass>>()))
                .Returns(this.CreateMany<TargetClass>(source.Length));

            // Act
            var result = this.Sut.Map<TargetClass>(source);

            // Assert
            result.Should()
                  .NotBeNull()
                  .And.HaveCount(source.Length);

            this.mapperMock.Verify(x => x.Map<IEnumerable<SourceClass>, IEnumerable<TargetClass>>(source), Times.Once);
        }

        [Theory]
        [AutoData]
        public void MapCollection_InternalMapThrowException_LogsAndThrows(IEnumerable<SourceClass> source)
        {
            // Arrange
            this.mapperMock.Setup(x => x.Map<IEnumerable<SourceClass>, IEnumerable<TargetClass>>(It.IsAny<IEnumerable<SourceClass>>()))
                .Throws<InvalidOperationException>();

            // Act
            var result = Record.Exception(() => this.Sut.Map<TargetClass>(source));

            // Assert
            result.Should()
                  .NotBeNull()
                  .And.BeOfType<MappingException>()
                  .And.Match((MappingException ex) => ex.InnerException is InvalidOperationException);

            this.mapperMock.Verify(x => x.Map<IEnumerable<SourceClass>, IEnumerable<TargetClass>>(source), Times.Once);
            this.Logger.VerifyLogError<Mapper<SourceClass>, InvalidOperationException>(Times.Once());
        }
    }
}
