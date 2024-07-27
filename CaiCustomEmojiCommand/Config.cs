using Newtonsoft.Json;

namespace CaiCustomEmojiCommand;

public class Config
{
    public const string Path = "tshock/CaiCustomEmojiCommand.json";

    public static Config config = new();

    [JsonProperty("说明", Order = -1)] public string Description =
        "EmojiID可以在wiki(https://terraria.wiki.gg/zh/wiki/%E8%A1%A8%E6%83%85)上查询, 本插件不支持跳过权限检查, 命令需要标识符(/或者.)";

    [JsonProperty("命令列表", Order = 0)] public List<EmojiCommand> EmojiCommands = new();

    public void Write(string path = Path)
    {
        using FileStream fileStream = new(path, FileMode.Create, FileAccess.Write, FileShare.Write);
        Write(fileStream);
    }

    public void Write(Stream stream)
    {
        string value = JsonConvert.SerializeObject(this, Formatting.Indented);
        using (StreamWriter streamWriter = new(stream))
        {
            streamWriter.Write(value);
        }
    }

    public static Config? Read(string path = Path)
    {
        Config? result;
        if (!File.Exists(path))
        {
            result = new Config
            {
                EmojiCommands = new List<EmojiCommand>
                {
                    new(0, "/home")
                }
            };
            result.Write(path);
        }
        else
        {
            using (FileStream fileStream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read))
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
        using (StreamReader streamReader = new(stream))
        {
            result = JsonConvert.DeserializeObject<Config>(streamReader.ReadToEnd());
        }

        return result;
    }
}