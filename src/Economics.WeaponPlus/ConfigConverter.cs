using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace TerrariaMap;

internal class ConfigConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Config);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);
        var setting = new Config();
        var attrs = typeof(Config)
            .GetFields()
            .Where(x => x.IsDefined(typeof(JsonPropertyAttribute)))
            .Select(x => (x, x.GetCustomAttribute<JsonPropertyAttribute>()!))
            .ToDictionary(x => x.Item2.PropertyName!, x => x.x.Name);
        foreach (var pro in obj.Properties())
        {
            var field = typeof(Config).GetField(pro.Name)!;
            if (attrs.TryGetValue(pro.Name, out var fieldName))
            {
                field = typeof(Config).GetField(fieldName)!;
            }

            field.SetValue(setting, Convert.ChangeType(pro.Value, field.FieldType));
        }
        return setting;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is Config config)
        {

            var fields = typeof(Config).GetFields();
            writer.WriteStartObject();
            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute<JsonPropertyAttribute>();
                if (attr != null)
                {
                    var fileName = config.EnableEnglish ? field.Name : attr!.PropertyName!;
                    var fileValue = field.GetValue(config);
                    writer.WritePropertyName(fileName);
                    writer.WriteValue(fileValue);
                }
            }
            writer.WriteEndObject();
        }
    }
}