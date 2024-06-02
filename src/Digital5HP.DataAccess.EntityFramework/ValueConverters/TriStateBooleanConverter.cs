namespace Digital5HP.DataAccess.EntityFramework.ValueConverters;

public class TriStateBooleanConverter : NullableValueConverter<bool?,int >
{
    public TriStateBooleanConverter()
        : base(
                boolVal => boolVal.HasValue ? boolVal.Value ? 1 : 0 : 2,
                intVal => intVal == 2 ? null : intVal == 1)
    {
    }
}
