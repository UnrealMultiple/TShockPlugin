using Newtonsoft.Json;

namespace AutoStoreItems;
public class CommentConverter : JsonConverter
{
    public string Comment { get;}
    public CommentConverter(string comment)
    {
        this.Comment = comment;
    }

    public override bool CanConvert(Type objectType)
    {
        return true;
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        return reader.Value;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        writer.WriteValue(value);
        writer.WriteComment(this.Comment);
    }
}
