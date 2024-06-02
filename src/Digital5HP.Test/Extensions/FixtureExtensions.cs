namespace Digital5HP.Test
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using AutoFixture;
    using AutoFixture.AutoMoq;
    using AutoFixture.Kernel;

    public static class FixtureExtensions
    {
        private static readonly ICustomization[] DefaultCustomizations =
        {
            new AutoMoqCustomization
            {
                ConfigureMembers = true,
            },
            new DefaultCustomization(),
        };

        public static T CustomizeDefault<T>([NotNull] this T fixture)
            where T : IFixture
        {
            foreach (var customization in DefaultCustomizations)
            {
                fixture.Customize(customization);
            }

            return fixture;
        }

        public static int CreateIntInRange(this IFixture fixture, int min, int max)
        {
            return (fixture.Create<int>() % (max - min + 1)) + min;
        }

        public static long CreateLongInRange(this IFixture fixture, long min, long max)
        {
            return (fixture.Create<long>() % (max - min + 1)) + min;
        }

        public static decimal CreateDecimalInRange(this IFixture fixture, decimal min, decimal max)
        {
            var request = new RangedNumberRequest(typeof(decimal), min, max);
            var context = new SpecimenContext(fixture);
            var generator = new RandomRangedNumberGenerator();

            var result = (decimal)generator.Create(request, context);
            return result;
        }

        public static DateTime CreateDateInRange(this IFixture fixture, DateTime min, DateTime max)
        {
            var ticks = fixture.CreateLongInRange(min.Ticks, max.Ticks);

            return new DateTime(ticks);
        }
    }
}
