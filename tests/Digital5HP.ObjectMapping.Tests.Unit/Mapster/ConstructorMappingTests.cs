namespace Digital5HP.ObjectMapping.Tests.Unit.Mapster
{
    using System.Linq;

    using AutoFixture.Xunit2;

    using FluentAssertions;

    using global::Mapster;

    using Digital5HP.Test;

    using Xunit;

    [Trait("Category", "POC")]
    public class ConstructorMappingTests : FixtureBase
    {

        [Theory]
        [AutoData]
        public void CtorParamWitCustomMapping_Mapster(Foo foo)
        {
            TypeAdapterConfig.GlobalSettings.RequireExplicitMapping = true;
            TypeAdapterConfig.GlobalSettings.RequireDestinationMemberSource = true;

            TypeAdapterConfig<Foo, Bar>.NewConfig()
                                       .MapToConstructor(true)
                                       .Map(
                                            dest => dest.Id,
                                            src => src.OtherId);

            // Act
            TypeAdapterConfig.GlobalSettings.Compile();

            var bar = foo.Adapt<Bar>();

            // Assert
            bar.Should()
               .NotBeNull()
               .And.Match((Bar o) => o.Id == foo.OtherId);
        }

        [Theory]
        [AutoData]
        public void CtorWithCustomMappingAndConverter_Mapster(Foo foo)
        {
            TypeAdapterConfig.GlobalSettings.RequireExplicitMapping = true;
            TypeAdapterConfig.GlobalSettings.RequireDestinationMemberSource = true;

            TypeAdapterConfig<Foo, Baz>.NewConfig()
                                       .MapToConstructor(true)
                                       .Map(
                                            dest => dest.Id,
                                            src => src.OtherId)
                                       .Map(
                                            dest => dest.Name,
                                            src => new string(
                                                src.Name.Reverse()
                                                   .ToArray()));

            // Act
            TypeAdapterConfig.GlobalSettings.Compile();

            var baz = foo.Adapt<Baz>();

            // Assert

            baz.Should()
               .NotBeNull();

            baz.Id.Should()
               .Be(foo.OtherId);

            baz.Name.Should()
               .Be(
                    new string(
                        foo.Name.Reverse()
                           .ToArray()));
        }
    }
}
