using Newtonsoft.Json;
using System.ComponentModel;
using System.Reflection;

namespace Lagrange.XocMat.Adapter.Converter;

internal class MessageTypeConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return true;
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var fields = objectType.GetFields();
        var readValue = reader.Value?.ToString() ?? string.Empty;
        foreach (var field in fields)
        {
            var obj = field.GetCustomAttributes<DescriptionAttribute>();
            if (obj.Any(item => item?.Description == readValue))
            {
                return Convert.ChangeType(field.GetValue(-1), objectType);
            }
        }
        return objectType.IsEnum ? Activator.CreateInstance(objectType) : null;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {

        var field = value!.GetType().GetField(value.ToString()!)!;
        var des = field.GetCustomAttribute<DescriptionAttribute>();
        if (des != null)
        {
            writer.WriteValue(des.Description);
            return;
        }
        writer.WriteValue("");
    }
}
