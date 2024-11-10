using Newtonsoft.Json;

namespace CaiPacketDebug;

public class Config
{
    public const string Path = "tshock/CaiPacketDebug.json";

    public static Config Settings = new ();

    [JsonProperty("C->S")] public DebugSettings ClientToServer = new ();
    [JsonProperty("S->C")] public DebugSettings ServerToClient = new ();

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

        Settings = result!;
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

public class DebugSettings
{
    [JsonProperty("自启动")] public bool DebugAfterInit;
    [JsonProperty("排除数据包")] public int[] ExcludePackets = { 114, 514 };
    [JsonProperty("白名单模式")] public bool WhiteListMode;
    [JsonProperty("白名单模式数据包")] public int[] WhiteListPackets = { 1, 2, 3 };
}