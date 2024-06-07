using Newtonsoft.Json;
using TShockAPI;

namespace AnnouncementBoxPlus
{
    public class Config
    {
        public  const string FilePath = "tshock/AnnouncementBoxPlus.json";
        public static Config config= new();
        [JsonProperty("禁用广播盒")]
        public bool disabled { get; set; } = false;
        [JsonProperty("广播内容仅触发者可见")]
        public bool justWho { get; set; } = false;
        [JsonProperty("广播范围(像素)(0为无限制)")]
        public int range { get; set; } = 0;

        [JsonProperty("启用广播盒权限(AnnouncementBoxPlus.Edit)")]
        public bool usePerm { get; set; } = false;

        [JsonProperty("启用插件广播盒发送格式")]
        public bool useFormat { get; set; } = false;

        [JsonProperty("广播盒发送格式")]
        public string formation { get; set; } = "%当前时间% %玩家组名% %玩家名%:%内容% #详细可查阅文档";

        [JsonProperty("启用广播盒占位符(详细查看文档)")]
        public bool usePlaceholder { get; set; } = true;

        public void Write(string path = FilePath)
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

        public static Config Read(string path=FilePath)
        {
            if (!File.Exists(path))
            {
                new Config().Write();
                return new Config();
            }
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var sr = new StreamReader(fs))
                {
                    var cf = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
                    config = cf;
                    return cf;
                }
            }
            
        }
    }
}