using Newtonsoft.Json;
using TShockAPI;

namespace PacketsStop
{
    public class Configuration
    {
        [JsonProperty("数据包名可查看")]
        public string README = "https://github.com/Pryaxis/TSAPI/blob/general-devel/TerrariaServerAPI/TerrariaApi.Server/PacketTypes.cs";

        [JsonProperty("插件指令与权限名")]
        public string README2 = "拦截";

        [JsonProperty("第一次使用输这个")]
        public string README3 = "/group addperm default 免拦截";

        [JsonProperty("拦截的数据包名")] 
        public HashSet<string> Packets { get; set; } = new HashSet<string>();

        public static readonly string FilePath = Path.Combine(TShock.SavePath, "数据包拦截.json");


        public Configuration()
        {
            Packets = new HashSet<string>
            {
                "ConnectRequest",
                "Disconnect",
                "ContinueConnecting",
                "ContinueConnecting2",
                "PlayerInfo",
                "PlayerSlot",
                "TileGetSection",
                "PlayerSpawn",
                "ProjectileNew",
                "ProjectileDestroy",
                "SyncProjectileTrackers",
            };
        }

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
            {
                var defaultConfig = new Configuration(); 
                defaultConfig.Write(path);
                return defaultConfig;
            }
            else
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var sr = new StreamReader(fs))
                {
                    var json = sr.ReadToEnd();
                    var cf = JsonConvert.DeserializeObject<Configuration>(json);
                    return cf;
                }
            }
        }
    }
}
