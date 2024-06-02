namespace Digital5HP;

using System;

public interface ITimeProvider
{
    DateTime Now { get; }
}