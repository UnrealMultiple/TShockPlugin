using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WorldModify
{
    public class RectangleConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Rectangle val = (Rectangle)value;
            JObject.FromObject(new { val.X, val.Y, val.Width, val.Height }).WriteTo(writer, Array.Empty<JsonConverter>());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject o = JObject.Load(reader);
            int valueOrDefault = GetTokenValue(o, "x").GetValueOrDefault();
            int valueOrDefault2 = GetTokenValue(o, "y").GetValueOrDefault();
            int valueOrDefault3 = GetTokenValue(o, "width").GetValueOrDefault();
            int valueOrDefault4 = GetTokenValue(o, "height").GetValueOrDefault();
            return new Rectangle(valueOrDefault, valueOrDefault2, valueOrDefault3, valueOrDefault4);
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        private static int? GetTokenValue(JObject o, string tokenName)
        {
            JToken val = default;
            return o.TryGetValue(tokenName, StringComparison.InvariantCultureIgnoreCase, out val) ? new int?((int)val) : null;
        }
    }
}
