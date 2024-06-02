namespace Digital5HP.DataAccess.EntityFramework.ValueConverters;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class BooleanToIntConverter : ValueConverter<bool, int>
{
    public BooleanToIntConverter()
        : base(
            boolVal => boolVal ? 1 : 0,
            intVal => intVal == 1)
    {
    }
}
