using Newtonsoft.Json;
using TShockAPI;

namespace ReFishTask;

internal class Configuration
{
    [JsonProperty("是否切换任务鱼（关掉就可以1种鱼刷1天）", Order = 0)]
    public bool SwitchTasks { get; set; } = true;


    #region 读取与创建配置文件方法
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "刷新渔夫任务.json");

    public void Write()
    {
        var json = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(FilePath, json);
    }

    public static Configuration Read()
    {
        if (!File.Exists(FilePath))
        {
            var NewConfig = new Configuration();
            new Configuration().Write();
            return NewConfig;
        }
        else
        {
            var jsonContent = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<Configuration>(jsonContent)!;
        }
    }
    #endregion

}