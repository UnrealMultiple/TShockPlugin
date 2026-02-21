using Newtonsoft.Json;

namespace PermaBuff;

public class Config
{
    [JsonProperty("不可添加buff")]
    public HashSet<int> LimitBuffs = new HashSet<int>();

    [JsonProperty("刷新间隔(秒)")]
    public double RefreshIntervalSeconds { get; set; } = 5.0;
    /// <summary>
    /// 读取配置文件内容
    /// </summary>
    /// <param name="Path">配置文件路径</param>
    /// <returns></returns>
    public static Config Read(string Path)
    {
        if (!File.Exists(Path))
        {
            return new Config();
        }

        using var fs = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Read(fs);
    }

    /// <summary>
    /// 通过文件流读取文件内容
    /// </summary>
    /// <param name="stream">流</param>
    /// <returns></returns>
    public static Config Read(Stream stream)//给定流文件进行读取
    {
        using var sr = new StreamReader(stream);
        var cf = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
        return cf ?? new Config();
    }

    /// <summary>
    /// 写入配置
    /// </summary>
    /// <param name="Path">配置文件路径</param>
    public void Write(string Path)//给定路径进行写
    {
        using (var fs = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.Write))
        { this.Write(fs); }
    }

    /// <summary>
    /// 通过文件流写入配置
    /// </summary>
    /// <param name="stream">文件流</param>
    public void Write(Stream stream)//给定流文件写
    {
        var data = JsonConvert.SerializeObject(this, Formatting.Indented);
        var sw = new StreamWriter(stream);
        sw.Write(data);
        sw.Close();
    }
}