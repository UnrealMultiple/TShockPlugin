using Newtonsoft.Json;

namespace ProgressBag;

public class Award
{
    public int netID = 22;

    public int prefix = 0;

    public int stack = 99;
}

public class Bag
{
    [JsonProperty("礼包名称")]
    public string Name { get; set; } = "新手礼包";

    [JsonProperty("进度限制")]
    public List<string> Limit { get; set; } = new();

    [JsonProperty("礼包奖励")]
    public List<Award> Award { get; set; } = new();

    [JsonProperty("执行命令")]
    public List<string> Command { get; set; } = new();

    [JsonProperty("已领取玩家")]
    public List<string> Receive { get; set; } = new();

    [JsonProperty("可领取组")]
    public List<string> Group { get; set; } = new();
}

public class Config
{
    [JsonProperty("礼包")]
    public List<Bag> Bag = new();

    public void Reset()
    {
        foreach (var bag in this.Bag)
        {
            bag.Receive.Clear();
        }
        this.Write(Plugin.PATH);
    }


    public static Config Read(string Path)//给定文件进行读
    {
        if (!File.Exists(Path))
        {
            return new Config();
        }

        using var fs = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Read(fs);
    }
    public static Config Read(Stream stream)//给定流文件进行读取
    {
        using var sr = new StreamReader(stream);
        var cf = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd())!;
        return cf;
    }
    public void Write(string Path)//给定路径进行写
    {
        using var fs = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.Write);
        this.Write(fs);
    }
    public void Write(Stream stream)//给定流文件写
    {
        var str = JsonConvert.SerializeObject(this, Formatting.Indented);
        using var sw = new StreamWriter(stream);
        sw.Write(str);
    }
}