using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OnlineGiftPackage
{
    public class GiftConverter : JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;
        public override bool CanConvert(Type objectType)
        {
            return typeof(Gift) == objectType;
        }

        // 优化 ReadJson 方法，加入异常处理和状态验证
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                JObject giftObject = JObject.Load(reader);
                Gift gift = new();

                gift.物品名称 = (string)giftObject["物品名称"];
                gift.物品ID = (int)giftObject["物品ID"];

                JArray? 数量Array = giftObject["物品数量"] as JArray;
                gift.物品数量 = new int[2];
                gift.物品数量[0] = (int)数量Array[0];
                gift.物品数量[1] = (int)数量Array[1];

                gift.所占概率 = (int)giftObject["所占概率"];

                return gift;
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException($"无法反序列化Gift: {ex.Message}", ex);
            }
        }


        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Gift gift = (Gift)value;

            writer.WriteStartObject();
            writer.WritePropertyName("物品名称");
            writer.WriteValue(gift.物品名称);
            writer.WritePropertyName("物品ID");
            writer.WriteValue(gift.物品ID);
            writer.WritePropertyName("物品数量");
            writer.WriteStartArray();
            writer.WriteValue(gift.物品数量[0]);
            writer.WriteValue(gift.物品数量[1]);
            writer.WriteEndArray();
            writer.WritePropertyName("所占概率");
            writer.WriteValue(gift.所占概率);
            writer.WriteEndObject();
        }
    }
}