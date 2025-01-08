using Newtonsoft.Json;
using TShockAPI;

namespace Plugin;

public class Configuration
{
    [JsonProperty("插件开关", Order = 6)]
    public bool Enabled = true;

    [JsonProperty("允许PVP复活", Order = 7)]
    public bool PVP = false;

    [JsonProperty("复活提醒", Order = 8)]
    public string Mess = GetString("{0} 被圣光笼罩，瞬间复活!!!");

    [JsonProperty("复活提醒的颜色", Order = 9)]
    public int[] Colors = new int[3] { 255, 215, 0 };

    [JsonProperty("复活币的物品ID", Order = 10)]
    public int[] ItemID = new int[] { 3229 };

    #region 读取与创建配置文件方法
    public static string FilePath => Path.Combine(TShock.SavePath, "复活币.json");
    public void Write()
    {
        var json = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(FilePath, json);
    }

    public static Configuration Read(string path)
    {
        if (!File.Exists(path))
        {
            var c = new Configuration();
            c.Write();
            return c;
        }
        else
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sr = new StreamReader(fs))
            {
                var json = sr.ReadToEnd();
                var cf = JsonConvert.DeserializeObject<Configuration>(json);
                return cf!;
            }
        }
    }
    #endregion
}
