namespace Digital5HP.Lookups;

using System;

public abstract record LookupObject<TEnum>(string Name, string Code, TEnum EnumValue) : ILookupObject<TEnum>
    where TEnum : Enum;
