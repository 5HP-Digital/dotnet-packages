namespace Digital5HP.DataAccess.EntityFramework.ValueConverters;

using System.Collections.Generic;
using System.Text.Json;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class DictionaryToJsonConverter : ValueConverter<IDictionary<string, string>, string>
{
    public DictionaryToJsonConverter()
        : base(dictionary => ConvertToJson(dictionary), jsonString => ConvertFromJson(jsonString))
    {
    }

    private static string ConvertToJson(IDictionary<string, string> dictionary)
    {
        return JsonSerializer.Serialize(dictionary);
    }

    private static IDictionary<string, string> ConvertFromJson(string jsonString)
    {
        return JsonSerializer.Deserialize<IDictionary<string, string>>(jsonString);
    }
}
