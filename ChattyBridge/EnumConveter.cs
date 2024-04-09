using Newtonsoft.Json;
using System.ComponentModel;
using System.Reflection;

namespace ChattyBridge;

internal class EnumConveter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return true;
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if(reader.Value is string type)
        {
            foreach (var item in typeof(MsgType).GetFields())
            {
                var attr = item.GetCustomAttribute<DescriptionAttribute>();
                if (attr != null && attr.Description == type)
                {
                    return item.GetValue(-1);
                }
            }
        }
        return MsgType.Unknow;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if(value is MsgType type)
        {
            var attr = typeof(MsgType).GetField(type.ToString())?.GetCustomAttribute<DescriptionAttribute>();
            if (attr != null)
            {
                writer.WriteValue(attr.Description);
            }
        }
        writer.WriteValue("unknow");
    }
}
