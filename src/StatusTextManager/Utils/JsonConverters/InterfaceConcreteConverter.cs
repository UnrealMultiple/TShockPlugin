using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StatusTextManager.Utils.Attrs;
using System.Reflection;

namespace StatusTextManager.Utils.JsonConverters;

public class InterfaceConcreteConverter : JsonConverter
{
    public override bool CanRead => true;
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType)
    {
        return objectType.IsInterface;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        try
        {
            var jsonObj = JObject.Load(reader);
            object? target = null;
            if (jsonObj.TryGetValue("TypeName", out var jsonTypeName) && jsonTypeName is JValue)
            {
                foreach (var t in objectType.GetCustomAttribute<ImplementsAttribute>()!.ImplementsTypes)
                {
                    var propInfo = t.GetProperty("TypeName", BindingFlags.Public | BindingFlags.Static);
                    if (propInfo == null || propInfo.PropertyType != typeof(string))
                    {
                        continue;
                    }

                    if ((string?) propInfo.GetValue(null) == jsonTypeName.Value<string>())
                    {
                        target = Activator.CreateInstance(t);
                        break;
                    }
                }
            }

            if (target == null)
            {
                throw new Exception("Could not find a corresponding concrete class");
            }

            serializer.Populate(jsonObj.CreateReader(), target);
            return target;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to convert", ex);
        }
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}