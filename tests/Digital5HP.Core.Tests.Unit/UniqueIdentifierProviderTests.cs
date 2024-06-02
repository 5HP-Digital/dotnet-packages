namespace Digital5HP.Core.Tests.Unit
{
    using System;

    using FluentAssertions;

    using Digital5HP.Test;

    using Xunit;

    [Trait("Category", "Unit")]
    public class UniqueIdentifierProviderTests : UnitFixtureFor<IUniqueIdentifierProvider>
    {

        protected override IUniqueIdentifierProvider CreateSut()
        {
            return UniqueIdentifierProvider.Current;
        }

        [Fact]
        public void NewGuid_ReturnsGuid()
        {
            // Act
            var result = this.Sut.Generate<Guid>();

            // Assert
            result.Should()
                  .NotBeEmpty();
        }

        [Fact]
        public void UniqueIdentifierProvider_Generate_ThrowNotSupportedException()
        {
            // Act
            var result = Record.Exception(() => this.Sut.Generate<int>());

            // Assert
            result.Should()
                  .BeOfType<NotSupportedException>();
        }
    }
}
