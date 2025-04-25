using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace lybra;

public class EnActorColorConverter : JsonConverter<EnActorColor>
{
    public override EnActorColor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string enumString = reader.GetString();
            if (Enum.TryParse(typeof(EnActorColor), enumString, true, out object enumValue))
            {
                return (EnActorColor)enumValue;
            }
        }
        
        throw new JsonException($"Unable to convert '{reader.GetString()}' to enum type {typeof(EnActorColor)}");
    }

    public override void Write(Utf8JsonWriter writer, EnActorColor value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}