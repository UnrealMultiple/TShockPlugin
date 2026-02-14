using Newtonsoft.Json;
using System.IO;

namespace CaiBotLite;

public class Config
{
    private const string ConfigPath = "tshock/CaiBotLite.json";
    public static Config Settings = new ();

    [JsonProperty("白名单开关")]
    public bool WhiteList = true;

    [JsonProperty("密钥")]
    public string Token = "";

    [JsonProperty("群OpenID")]
    public string GroupOpenId = "114514";

    [JsonProperty("在线显示进度")]
    public bool ShowProcessInPlayerList = true;

    [JsonProperty("商店分组标签")]
    public string ShopTag = "生存服";

    [JsonProperty("白名单拦截提示的群号")]
    public long GroupNumber;


    /// <summary>
    /// 将配置文件写入硬盘
    /// </summary>
    internal void Write()
    {
        using FileStream fileStream = new (ConfigPath, FileMode.Create, FileAccess.Write, FileShare.Write);
        using StreamWriter streamWriter = new (fileStream);
        streamWriter.Write(JsonConvert.SerializeObject(this, JsonSettings));
    }

    /// <summary>
    /// 从硬盘读取配置文件
    /// </summary>
    internal void Read()
    {
        Config result;
        if (!File.Exists(ConfigPath))
        {
            result = new Config();
            result.Write();
        }
        else
        {
            using FileStream fileStream = new (ConfigPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using StreamReader streamReader = new (fileStream);
            result = JsonConvert.DeserializeObject<Config>(streamReader.ReadToEnd(), JsonSettings)!;
        }

        Settings = result;
    }

    private static readonly JsonSerializerSettings JsonSettings = new () { Formatting = Formatting.Indented, ObjectCreationHandling = ObjectCreationHandling.Replace };
}