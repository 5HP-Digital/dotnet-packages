namespace Digital5HP.Test;

using System;

using AutoFixture;

public class DefaultCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        // AutoData doesn't support DateOnly and TimeOnly yet
        fixture.Register(() => DateOnly.FromDateTime(fixture.Create<DateTime>()));
        fixture.Register(() => TimeOnly.FromDateTime(fixture.Create<DateTime>()));
    }
}
