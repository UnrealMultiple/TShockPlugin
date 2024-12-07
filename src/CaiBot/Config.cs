using Newtonsoft.Json;

namespace CaiBot;

public class Config
{
    private const string Path = "tshock/CaiBot.json";

    public static Config config = new ();
    [JsonProperty("白名单开关",Order = 1)] public bool WhiteList = true;
    [JsonProperty("密钥",Order = 2)] public string Token = "";
    [JsonProperty("白名单拦截提示的群号",Order = 3)] public long GroupNumber;
    
    [JsonProperty("同步服务器聊天",Order = 4)] public bool SycnChatFromServer;
    [JsonProperty("服务器聊天格式",Order = 5)] public string ServerChatFormat = "[Server]{0}:{1}"; //"[Server]玩家名:内容" 额外 {2}:玩家组名 {3}:玩家聊天前缀 {4}:Ec职业名
    [JsonProperty("退出服务器消息格式",Order = 6)] public string ExitServerFormat = "[Server]{0}离开了游戏";
    [JsonProperty("加入服务器消息格式",Order = 7)] public string JoinServerFormat = "[Server]{0}加入了游戏";
    
    [JsonProperty("同步群聊天",Order = 8)] public bool SycnChatFromGroup;
    [JsonProperty("群聊天发送格式",Order = 9)] public string GroupChatFormat = "[{0}]{1}:{2}"; // "[群名]玩家昵称:内容" 额外 {3}:群QQ号 {4}:发送者QQ
    [JsonProperty("群聊天自定义群名",Order = 10)] public Dictionary<long, string> CustomGroupName = new ();

    public void Write(string path = Path)
    {
        using FileStream fileStream = new (path, FileMode.Create, FileAccess.Write, FileShare.Write);
        this.Write(fileStream);
    }

    private void Write(Stream stream)
    {
        var value = JsonConvert.SerializeObject(this, Formatting.Indented);
        using StreamWriter streamWriter = new (stream);
        streamWriter.Write(value);
    }

    public static void Read(string path = Path)
    {
        var flag = !File.Exists(path);
        Config? result;
        if (flag)
        {
            result = new Config();
            result.Write(path);
        }
        else
        {
            using (FileStream fileStream = new (path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                result = Read(fileStream);
            }
        }

        config = result!;
    }

    private static Config? Read(Stream stream)
    {
        using StreamReader streamReader = new (stream);
        var result = JsonConvert.DeserializeObject<Config>(streamReader.ReadToEnd());

        return result;
    }
}