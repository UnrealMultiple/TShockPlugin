using System;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using TShockAPI;

namespace InvincibilityPlugin
{
    public class Configuration
    {
        public static readonly string FilePath = Path.Combine(TShock.SavePath, "限时无敌.json");
        [JsonProperty("插件自带god无敌开启提示")]
        public bool EnableInvincibleReminder { get; set; } = true;

        [JsonProperty("god无敌开启提示自定义文本")]
        public string CustomInvincibleReminderText { get; set; } = "";

        [JsonProperty("god无敌关闭提示自定义文本")]
        public string CustomInvincibleDisableText { get; set; } = "你的无敌已关闭！";

        [JsonProperty("插件自带无敌帧开启提示")]
        public bool EnableFrameReminder { get; set; } = true;

        [JsonProperty("无敌帧开始提示文本")]
        public string CustomStartFrameText { get; set; } = "";

        [JsonProperty("无敌帧结束提示文本")]
        public string CustomEndFrameText { get; set; } = "无敌帧结束了！";

        public void Write(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                var str = JsonConvert.SerializeObject(this, Formatting.Indented);
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(str);
                }
            }
        }

        public static Configuration Read(string path)
        {
            if (!File.Exists(path))
                return new Configuration();
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var sr = new StreamReader(fs))
                {
                    var cf = JsonConvert.DeserializeObject<Configuration>(sr.ReadToEnd());
                    return cf;
                }
            }
        }
    }
}
