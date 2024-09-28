using Economics.RPG.Model;
using Newtonsoft.Json;

namespace Economics.RPG.Converter;

internal class LevelConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return true;
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var level = reader.Value?.ToString();
        return level == null
            ? null
            : (object) new Level()
            {
                Name = level
            };
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is Level level)
        {
            writer.WriteValue(level.Name);
        }
        else
        {
            writer.WriteValue("");
        }
    }
}