namespace Digital5HP.ObjectMapping.Mapster;

using global::Mapster;

using MapsterMapper;

public static class ObjectExtensions
{
    public static TResult InnerAdapt<TResult>(this object source)
    {
        var mapper = MapContext.Current.GetService<IMapper>();

        return mapper.Map<TResult>(source);
    }
}
