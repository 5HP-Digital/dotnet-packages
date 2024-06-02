namespace Digital5HP.Test
{
    using Xunit;

    [Trait("Category", "Integration")]
    public abstract class IntegrationFixtureFor<TSut> : FixtureFor<TSut>
    {
    }
}