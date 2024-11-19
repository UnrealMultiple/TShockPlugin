using Newtonsoft.Json;

namespace RewardSection;

public class ConfigFile
{
    public static ConfigFile Read(string path)
    {
        if (!File.Exists(path))
        {
            return new ConfigFile
            {
                RewardTable = new List<RewardSection>
                {
                    new RewardSection(0, 0, new List<ItemSections> { new ItemSections(757, 1, 0) }, new List<string> { "/time noon" }),
                    new RewardSection(0, 0, new List<ItemSections> { new ItemSections(757, 1, 0) }, new List<string> { "/time night" })
                }
            };
        }

        using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        using (var streamReader = new StreamReader(fileStream))
        {
            var configFile = JsonConvert.DeserializeObject<ConfigFile>(streamReader.ReadToEnd())!;
            ConfigR?.Invoke(configFile);
            return configFile;
        }
    }

    public static ConfigFile Read(Stream stream)
    {
        using (var streamReader = new StreamReader(stream))
        {
            var configFile = JsonConvert.DeserializeObject<ConfigFile>(streamReader.ReadToEnd())!;
            ConfigR?.Invoke(configFile);
            return configFile;
        }
    }

    public void Write(string path)
    {
        using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
        {
            this.Write(fileStream);
        }
    }

    public void Write(Stream stream)
    {
        var value = JsonConvert.SerializeObject(this, Formatting.Indented);
        using (var streamWriter = new StreamWriter(stream))
        {
            streamWriter.Write(value);
        }
    }

    [JsonProperty("奖励表")]
    public List<RewardSection> RewardTable { get; set; } = new List<RewardSection>();

    public static Action<ConfigFile>? ConfigR;
}