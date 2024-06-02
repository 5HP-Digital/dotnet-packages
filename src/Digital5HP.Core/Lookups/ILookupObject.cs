namespace Digital5HP.Lookups;

using System;

public interface ILookupObject<out TEnum> : ILookupObject
    where TEnum : Enum
{
    TEnum EnumValue { get; }
}

public interface ILookupObject
{
    string Name { get; }

    string Code { get; }
}
