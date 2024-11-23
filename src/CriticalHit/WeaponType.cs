using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CriticalHit;

public enum WeaponType
{
    Melee,
    Ranged,
    Magic,
    Explosive
}
public class WeaponTypeDictionaryConverter : JsonConverter
{
    private static readonly Dictionary<WeaponType, string> DisplayNames = new Dictionary<WeaponType, string>
    {
        { WeaponType.Melee, "近战" },
        { WeaponType.Ranged, "远程" },
        { WeaponType.Magic, "魔法" },
        { WeaponType.Explosive, "爆炸" },
    };

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        var dict = (Dictionary<WeaponType, CritMessage>) value!;

        writer.WriteStartObject();
        foreach (var pair in dict)
        {
            writer.WritePropertyName(DisplayNames[pair.Key]);
            serializer.Serialize(writer, pair.Value);
        }
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var result = new Dictionary<WeaponType, CritMessage>();

        var jsonObject = JObject.Load(reader);
        foreach (var property in jsonObject.Properties())
        {
            var weaponType = DisplayNames.First(x => x.Value == property.Name).Key;
            var critMessage = property.Value.ToObject<CritMessage>(serializer)!;
            result.Add(weaponType, critMessage);
        }

        return result;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Dictionary<WeaponType, CritMessage>);
    }
}