namespace Digital5HP.ObjectMapping.Tests.Unit.Mapster
{
    using FluentAssertions;

    using global::Mapster;
    using global::Mapster.Models;

    using Digital5HP.ObjectMapping.Mapster;
    using Digital5HP.Test;

    using Xunit;

    public class MappingProfileTests : UnitFixtureFor<TestMappingProfile>
    {
        protected override TestMappingProfile CreateSut()
        {
            return new ();
        }

        [Fact]
        public void Configure_Succeed()
        {
            // Arrange
            var config = new TypeAdapterConfig();
            var mappingConfiguration = new MappingConfiguration<SourceClass>(config);

            // Act
            this.Sut.ConfigureInternal(mappingConfiguration);

            // Asset
            config.RuleMap.Should()
                  .ContainKey(new TypeTuple(typeof(SourceClass), typeof(TargetClass)));
        }
    }
}
