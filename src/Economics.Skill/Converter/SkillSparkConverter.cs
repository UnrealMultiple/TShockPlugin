using Economics.Skill.Enumerates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Reflection;

namespace Economics.Skill.Converter;

public class SkillSparkConverter : JsonConverter
{
    public override bool CanRead => true;

    public override bool CanWrite => true;
    public override bool CanConvert(Type objectType)
    {
        return true;
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var types = new List<SkillSparkType>();
        foreach (var spark in (JArray) serializer.Deserialize(reader)!)
        {
            types.Add(GetSparkType(spark.ToString()));
        }
        return types;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is List<SkillSparkType> sparks)
        {

            writer.WriteStartArray();
            foreach (var spark in sparks)
            {
                writer.WriteValue(GetFieldDes(spark));
            }
            writer.WriteEndArray();
        }
    }

    public static string GetFieldDes(SkillSparkType type)
    {
        var field = typeof(SkillSparkType).GetField(type.ToString());
        if (field != null)
        {
            var Des = field.GetCustomAttribute<DescriptionAttribute>();
            if (Des != null)
            {
                return Des.Description;
            }
        }
        return "";
    }

    public static SkillSparkType GetSparkType(string name)
    {
        var fields = typeof(SkillSparkType).GetFields();
        foreach (var field in fields)
        {
            var Des = field.GetCustomAttribute<DescriptionAttribute>();
            if (Des != null && Des.Description == name)
            {
                return (SkillSparkType) Convert.ChangeType(field.GetValue(-1), typeof(SkillSparkType))!;
            }
        }
        return default;
    }
}