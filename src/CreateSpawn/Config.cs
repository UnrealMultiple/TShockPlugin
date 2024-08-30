using Newtonsoft.Json;


namespace CreateSpawn;



public class Config
{
    public int centreX { get; set; } = 0;

    public int CountY { get; set; } = 0;

    [JsonProperty("微调X")]
    public int AdjustX { get; set; } = 0;

    [JsonProperty("微调Y")]
    public int AdjustY { get; set; } = 0;


    /// <summary>
    /// 通过文件流读取文件内容
    /// </summary>
    /// <param name="Path">路径</param>
    /// <returns></returns>
    public static Config Read(string Path)//给定流文件进行读取
    {
        return !File.Exists(Path) ? new() : JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path)) ?? new();
    }

    /// <summary>
    /// 写入配置
    /// </summary>
    /// <param name="Path">配置文件路径</param>
    public void Write(string Path)
    {
        File.WriteAllText(Path, JsonConvert.SerializeObject(this, Formatting.Indented));
    }

}