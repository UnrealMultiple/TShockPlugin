using Newtonsoft.Json;

namespace CaiBot;

public class Config
{
    private const string ConfigPath = "tshock/CaiBot.json";
    public static Config Settings = new ();

    [JsonProperty("白名单开关", Order = 1)] 
    public bool WhiteList = true;
    
    [JsonProperty("密钥", Order = 2)] 
    public string Token = "";

    [JsonProperty("白名单拦截提示的群号", Order = 3)]
    public long GroupNumber;

    [JsonProperty("同步服务器聊天", Order = 4)] 
    public bool SyncChatFromServer;
    
    [JsonProperty("服务器聊天格式", Order = 5)] 
    public string ServerChatFormat = "[Server]{0}:{1}"; //"[Server]玩家名:内容" 额外 {2}:玩家组名 {3}:玩家聊天前缀 {4}:Ec职业名
    
    [JsonProperty("退出服务器消息格式", Order = 6)] 
    public string ExitServerFormat = "[Server]{0}离开了游戏";
    
    [JsonProperty("加入服务器消息格式", Order = 7)] 
    public string JoinServerFormat = "[Server]{0}加入了游戏";

    [JsonProperty("同步群聊天", Order = 8)] 
    public bool SyncChatFromGroup;
    
    [JsonProperty("群聊天发送格式", Order = 9)] 
    public string GroupChatFormat = "[{0}]{1}:{2}"; // "[群名]玩家昵称:内容" 额外 {3}:群QQ号 {4}:发送者QQ
    
    [JsonProperty("群聊天自定义群名", Order = 10)] 
    public Dictionary<long, string> CustomGroupName = new ();

    [JsonProperty("群聊天忽略用户列表", Order = 11)]
    public List<long> GroupChatIgnoreUsers = new ();

    [JsonProperty("群Ban封禁通知", Order = 12)] 
    public bool GroupBanNotice = true;
    
    [JsonProperty("BotAPI地址", Order = 13)] 
    public string BotApi = "api.terraria.ink:22334";

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