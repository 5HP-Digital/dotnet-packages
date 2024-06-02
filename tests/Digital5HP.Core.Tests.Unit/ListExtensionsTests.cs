namespace Digital5HP.Core.Tests.Unit
{
    using FluentAssertions;

    using System.Collections.Generic;
    using System.Linq;

    using Xunit;

    [Trait("Category", "Unit")]
    public class ListExtensionsTests
    {
        [Fact]
        public void AddIfNotNull_Succeed()
        {
            var list = new List<string>();
            var value = "test";

            list.AddIfNotNull(value, null);

            list.Count
                .Should()
                .Be(1);

            list.First()
                .Should()
                .Be(value);
        }

        [Fact]
        public void AddIfTrue_Succeed()
        {
            var list = new List<string>();
            var value = "test";

            list.AddIfTrue(true, value);
            list.AddIfTrue(true, null);
            list.AddIfTrue(false, value);

            list.Count
                .Should()
                .Be(1);

            list.First()
                .Should()
                .Be(value);
        }
    }
}
