using System.Text.Json;
using System.Text.Json.Serialization;

namespace Loom.TypeGenerator.ApiTypes;

internal sealed class SingleOrArrayConverter<T> : JsonConverter<T[]>
{
    public override T[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        T[] result = [];
        switch (reader.TokenType)
        {
            case JsonTokenType.StartObject:
            {
                var item = JsonSerializer.Deserialize<T>(ref reader, options);
                if (item != null)
                    result.SetValue(item, result.Length);

                break;
            }
            case JsonTokenType.StartArray:
                result = JsonSerializer.Deserialize<T[]>(ref reader, options)!;
                break;
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, T[] value, JsonSerializerOptions options)
    {
        if (value.Length == 1)
            JsonSerializer.Serialize(writer, value[0], options);
        else
            JsonSerializer.Serialize(writer, value, options);
    }
}