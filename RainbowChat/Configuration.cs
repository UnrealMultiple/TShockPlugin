using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using System.Text;
using TShockAPI;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace RainbowChat
{
    internal class Configuration
    {
        // 配置文件存放路径
        public static readonly string FilePath = Path.Combine(TShock.SavePath, "RainbowChat.json");

        [JsonProperty("使用说明")]
        public string Text = "权限名（rainbowchat.use） /rc 渐变 用指令修改的颜色不会写进配置文件，这里改的是全体默认渐变色，开启【随机色】渐变会默认失效";

        [JsonProperty("进服自动开启渐变色")]
        public bool Enable = false;

        [JsonProperty("修改渐变开始颜色")]
        [JsonConverter(typeof(ColorJsonConverter))]
        public Microsoft.Xna.Framework.Color GradientStartColor { get; set; }

        [JsonProperty("修改渐变结束颜色")]
        [JsonConverter(typeof(ColorJsonConverter))]
        public Microsoft.Xna.Framework.Color GradientEndColor { get; set; }

        //赋个默认值 避免色盲不会调色
        public Configuration()
        {
            GradientStartColor = new Microsoft.Xna.Framework.Color(r: 166, g: 213, b: 234); 
            GradientEndColor = new Microsoft.Xna.Framework.Color(r: 245, g: 247, b: 175); 
        }

        #region 色彩辅助方法

        // 将十六进制颜色字符串转换为Microsoft.Xna.Framework.Color
        private static Microsoft.Xna.Framework.Color HexToXnaColor(string hexColor)
        {
            if (hexColor.StartsWith("#"))
                hexColor = hexColor.Substring(1);

            byte r, g, b;
            switch (hexColor.Length)
            {
                case 6:
                    r = byte.Parse(hexColor.Substring(0, 2), NumberStyles.HexNumber);
                    g = byte.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber);
                    b = byte.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber);
                    break;
                case 8:
                    r = byte.Parse(hexColor.Substring(0, 2), NumberStyles.HexNumber);
                    g = byte.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber);
                    b = byte.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber);
                    break;
                default:
                    throw new ArgumentException("无效的十六进制颜色字符串。");
            }

            return new Microsoft.Xna.Framework.Color(r, g, b);
        }
        #endregion

        #region 读取与创建配置文件方法

        //创建 写入你 👆 上面的参数
        public void Write(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
            {
                var str = JsonConvert.SerializeObject(this, Formatting.Indented);
                sw.Write(str);
            }
        }

        // 从文件读取配置
        public static Configuration Read(string path)
        {
            if (!File.Exists(path))
            {
                var c = new Configuration();
                c.Write(path);
                return c;
            }
            else
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var sr = new StreamReader(fs))
                {
                    var json = sr.ReadToEnd();
                    var cf = JsonConvert.DeserializeObject<Configuration>(json);
                    return cf!;
                }
            }
        }
        #endregion


        #region 用于反序列化的JsonConverter 使Config看得简洁

        internal class ColorJsonConverter : JsonConverter<Color>
        {
            public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
            {
                JObject colorObject = new JObject
                {
                    ["R"] = value.R,
                    ["G"] = value.G,
                    ["B"] = value.B,
                };

                colorObject.WriteTo(writer);
            }

            public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                JObject colorObject = JObject.Load(reader);

                byte r = colorObject["R"].Value<byte>();
                byte g = colorObject["G"].Value<byte>();
                byte b = colorObject["B"].Value<byte>();

                return new Color(r, g, b);
            }
        } 
        #endregion
    }
}