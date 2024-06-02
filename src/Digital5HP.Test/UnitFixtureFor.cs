namespace Digital5HP.Test
{
    using System;

    using AutoFixture;

    using Xunit;

    [Trait("Category", "Unit")]
    public abstract class UnitFixtureFor<TSut>(Action<IFixture> config = null) : FixtureFor<TSut>(config)
    {
    }
}
