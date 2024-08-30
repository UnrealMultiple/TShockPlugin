using Newtonsoft.Json;

namespace TeleportRequest;

public class Config
{
    [JsonProperty("间隔秒数")]
    public int IntervalInSeconds = 3;
    [JsonProperty("超时次数")]
    public int TimeoutCount = 3;

    public void Write(string path)
    {
        using FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write);
        Write(stream);
    }

    public void Write(Stream stream)
    {
        string value = JsonConvert.SerializeObject((object)this, (Formatting)1);
        using StreamWriter streamWriter = new StreamWriter(stream);
        streamWriter.Write(value);
    }

    public static Config Read(string path)
    {
        if (!File.Exists(path))
        {
            return new Config();
        }
        using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Read(stream);
    }

    public static Config Read(Stream stream)
    {
        using StreamReader streamReader = new StreamReader(stream);
        return JsonConvert.DeserializeObject<Config>(streamReader.ReadToEnd());
    }
}
