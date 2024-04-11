using Newtonsoft.Json;
using System.ComponentModel;

namespace ShortCommand;

public class CMD
{
    [JsonProperty("原始命令")]
    public string SourceCommand = "warp {0}";

    [JsonProperty("新的命令")]
    public string NewCommand = "传送";

    [JsonProperty("余段补充")]
    public bool Supplement = false;

    [JsonProperty("阻止原始")]
    public bool NotSource = false;

    [JsonProperty("限制条件")]
    public ConditionType Condition = ConditionType.None;

    [JsonProperty("冷却秒数")]
    public int CD = 0;

    [JsonProperty("冷却共享")]
    public bool ShareCD = false;
}

public class Config
{
    [JsonProperty("命令表")]
    [Description("命令表")]
    public List<CMD> Commands = new List<CMD>();

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
        using var fs = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.Write);
        this.Write(fs);
    }

    /// <summary>
    /// 通过文件流写入配置
    /// </summary>
    /// <param name="stream">文件流</param>
    public void Write(Stream stream)//给定流文件写
    {
        var data = JsonConvert.SerializeObject(this, Formatting.Indented);
        using var sw = new StreamWriter(stream);
        sw.Write(data);
        sw.Close();
    }
}