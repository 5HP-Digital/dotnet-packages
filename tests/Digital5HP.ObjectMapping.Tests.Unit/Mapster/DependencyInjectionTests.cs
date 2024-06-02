namespace Digital5HP.ObjectMapping.Tests.Unit.Mapster
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoFixture.Xunit2;

    using FluentAssertions;

    using global::Mapster;

    using MapsterMapper;

    using Microsoft.Extensions.DependencyInjection;

    using Digital5HP.Test;

    using Xunit;

    [Trait("Category", "POC")]
    public class DependencyInjectionTests : FixtureBase
    {
        [Theory]
        [AutoData]
        public void MapsterMap_UsingInjectedService_Succeed(Foo foo)
        {
            // Arrange
            var config = new TypeAdapterConfig();
            config.NewConfig<Foo, Baz>()
                  .MapToConstructor(true)
                  .Map(dest => dest.Id, src => src.OtherId)
                  .Map(
                       dest => dest.Name,
                       src => MapContext.Current.GetService<INameFormatter>()
                                        .Format(src.Name));

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(config);
            serviceCollection.AddScoped<IMapper, ServiceMapper>();
            serviceCollection.AddTransient<INameFormatter, NameFormatter>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var mapper = serviceProvider.GetRequiredService<IMapper>();

            // Act
            var baz = mapper.Map<Baz>(foo);

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

        [Theory]
        [AutoData]
        public void MapsterMap_UsingInjectedService_WithIEnumerableProperty_Succeed(Quux quux)
        {
            // Arrange
            var config = new TypeAdapterConfig();
            config.Default.Settings.AvoidInlineMapping = true;
            config.ForType(typeof(IEnumerable<>), typeof(List<>));
            config.NewConfig<Foo, Baz>()
                  .MapToConstructor(true)
                  .Map(dest => dest.Id, src => src.OtherId)
                  .Map(
                       dest => dest.Name,
                       src => MapContext.Current.GetService<INameFormatter>()
                                        .Format(src.Name));

            config.NewConfig<Quux, Quuz>()
                  .MapToConstructor(true)
                  .Map(dest => dest.Bazes, src => src.Foos);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(config);
            serviceCollection.AddScoped<IMapper, ServiceMapper>();
            serviceCollection.AddTransient<INameFormatter, NameFormatter>();

            // Act
            Quuz result;
            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                var mapper = serviceProvider.GetRequiredService<IMapper>();
                result = mapper.Map<Quuz>(quux);
            }

            // Assert
            result.Should()
                  .NotBeNull();

            result.Bazes.Should()
                  .HaveCount(quux.Foos.Count());
        }
    }
}
