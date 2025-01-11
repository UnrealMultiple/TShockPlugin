using Newtonsoft.Json;
using System.ComponentModel;
using System.Reflection;

namespace ChattyBridge;

internal class EnumConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return true;
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.Value is string type)
        {
            foreach (var item in typeof(MsgType).GetFields())
            {
                var attr = item.GetCustomAttribute<DescriptionAttribute>();
                if (attr != null && attr.Description == type)
                {
                    return (MsgType) Convert.ChangeType(item.GetValue(-1), typeof(MsgType))!;
                }
            }
        }
        return MsgType.Unknown;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is MsgType type)
        {
            var field = value.GetType().GetField(type.ToString());
            var des = field?.GetCustomAttribute<DescriptionAttribute>();
            if (des != null)
            {
                writer.WriteValue(des.Description);
                return;
            }
        }
        writer.WriteValue("unknown");
    }
}