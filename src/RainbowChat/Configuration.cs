using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using TShockAPI;

namespace RainbowChat;

internal class Configuration
{
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "RainbowChat.json");

    [JsonProperty("使用说明")]
    public string Text = "权限名（rainbowchat.use） /rc 渐变 用指令修改的颜色不会写进配置文件，这里改的是全体默认渐变色，开启【随机色】渐变会默认失效";

    [JsonProperty("插件开关")]
    public bool Enabled { get; set; } = true;

    [JsonProperty("错误提醒")]
    public bool ErrorMess { get; set; } = true;

    [JsonProperty("进服自动开启渐变色")]
    public bool OpenGradientForJoinServer { get; set; } = true;

    [JsonProperty("全局随机色开关")]
    public bool Random { get; set; } = true;

    [JsonProperty("全局渐变色开关")]
    public bool Gradient { get; set; } = true;

    [JsonProperty("修改渐变开始颜色")]
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color GradientStartColor { get; set; }

    [JsonProperty("修改渐变结束颜色")]
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color GradientEndColor { get; set; }

    public Configuration()
    {
        this.GradientStartColor = new Color(166, 213, 234);
        this.GradientEndColor = new Color(245, 247, 175);
    }

    #region 色彩辅助方法
    // 将十六进制颜色字符串转换为Microsoft.Xna.Framework.Color
    private static Microsoft.Xna.Framework.Color HexToXnaColor(string hexColor)
    {
        if (hexColor.StartsWith("#"))
        {
            hexColor = hexColor[1..];
        }

        byte r, g, b;
        switch (hexColor.Length)
        {
            case 6:
                r = byte.Parse(hexColor[..2], NumberStyles.HexNumber);
                g = byte.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber);
                b = byte.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber);
                break;
            case 8:
                r = byte.Parse(hexColor[..2], NumberStyles.HexNumber);
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
    public void Write()
    {
        var json = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(FilePath, json);
    }

    public static Configuration Read(string path)
    {
        if (!File.Exists(path))
        {
            var c = new Configuration();
            c.Write();
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

    #region 用于反序列化的JsonConverter 使Config看得简洁(转换器
    internal class ColorJsonConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            var colorObject = new JObject
            {
                ["R"] = value.R,
                ["G"] = value.G,
                ["B"] = value.B,
            };

            colorObject.WriteTo(writer);
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var colorObject = JObject.Load(reader);

            var r = colorObject["R"]?.Value<byte>() ?? throw new KeyNotFoundException("key 'R' is missing inside a 'Color' object");
            var g = colorObject["G"]?.Value<byte>() ?? throw new KeyNotFoundException("key 'G' is missing inside a 'Color' object");
            var b = colorObject["B"]?.Value<byte>() ?? throw new KeyNotFoundException("key 'B' is missing inside a 'Color' object");

            return new Color(r, g, b);
        }
    }
    #endregion
}