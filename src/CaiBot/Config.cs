using Newtonsoft.Json;

namespace CaiBot;

public class Config
{
    public const string Path = "tshock/CaiBot.json";

    public static Config config = new ();
    
    [JsonProperty("白名单开关")] public bool WhiteList = true;

    [JsonProperty("密钥")] public string Token = "";
    
    [JsonProperty("白名单拦截提示的群号")] public long GroupNumber;
    
    [JsonProperty("同步群聊天")] public bool SycnChatFromGroup;
    
    [JsonProperty("群聊天发送格式")] public string GroupChatFormat = "[{0}]{1}:{2}"; // "[群名]玩家昵称:内容" 额外 {3}:群QQ号 {4}:发送者QQ
    
    [JsonProperty("群聊天自定义群名")] public Dictionary<long, string> CustomGroupName = new ();
    
    [JsonProperty("同步服务器聊天")] public bool SycnChatFromServer;

    [JsonProperty("服务器聊天格式")] public string ServerChatFormat = "[Server]{0}:{1}";
    
    [JsonProperty("加入服务器消息格式")] public string JoinServerFormat = "[Server]{0}加入了游戏";
    
    [JsonProperty("退出服务器消息格式")] public string ExitServerFormat = "[Server]{0}离开了游戏";
    

    
    public void Write(string path = Path)
    {
        using FileStream fileStream = new (path, FileMode.Create, FileAccess.Write, FileShare.Write);
        this.Write(fileStream);
    }

    public void Write(Stream stream)
    {
        var value = JsonConvert.SerializeObject(this, Formatting.Indented);
        using (StreamWriter streamWriter = new (stream))
        {
            streamWriter.Write(value);
        }
    }

    public static Config? Read(string path = Path)
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
        return result;
    }

    public static Config? Read(Stream stream)
    {
        Config? result;
        using (StreamReader streamReader = new (stream))
        {
            result = JsonConvert.DeserializeObject<Config>(streamReader.ReadToEnd());
        }

        return result;
    }
}