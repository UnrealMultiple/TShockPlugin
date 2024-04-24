using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using TShockAPI;

namespace RainbowChat
{
    internal class Configuration
    {
        // 配置文件存放路径
        public static readonly string FilePath = Path.Combine(TShock.SavePath, "RainbowChat.json");

        [JsonProperty("渐变开始颜色")]
        private string? _gradientStartColorHex = "#615EF2";

        [JsonProperty("渐变结束颜色")]
        private string? _gradientEndColorHex = "#D6C053";

        [JsonProperty("修改渐变开始颜色")]
        public Microsoft.Xna.Framework.Color GradientStartColor
        {
            get => HexToXnaColor(_gradientStartColorHex!);
            set => _gradientStartColorHex = XnaColorToHex(value);
        }

        [JsonProperty("修改渐变结束颜色")]
        public Microsoft.Xna.Framework.Color GradientEndColor
        {
            get => HexToXnaColor(_gradientEndColorHex!);
            set => _gradientEndColorHex = XnaColorToHex(value);
        }

        #region 色彩转换辅助方法

        // 将十六进制颜色字符串转换为Microsoft.Xna.Framework.Color
        private static Microsoft.Xna.Framework.Color HexToXnaColor(string hexColor)
        {
            if (hexColor.StartsWith("#"))
                hexColor = hexColor.Substring(1);

            byte r, g, b, a;
            switch (hexColor.Length)
            {
                case 6:
                    r = byte.Parse(hexColor.Substring(0, 2), NumberStyles.HexNumber);
                    g = byte.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber);
                    b = byte.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber);
                    a = 255; // 默认全透明
                    break;
                case 8:
                    r = byte.Parse(hexColor.Substring(0, 2), NumberStyles.HexNumber);
                    g = byte.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber);
                    b = byte.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber);
                    a = byte.Parse(hexColor.Substring(6, 2), NumberStyles.HexNumber);
                    break;
                default:
                    throw new ArgumentException("Invalid hexadecimal color string.");
            }

            return new Microsoft.Xna.Framework.Color(r, g, b, a);
        }

        // 将Microsoft.Xna.Framework.Color转换为十六进制颜色字符串
        private static string XnaColorToHex(Microsoft.Xna.Framework.Color xnaColor)
        {
            return $"#{xnaColor.PackedValue.ToString("X8").Substring(2)}";
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
    }
}