using Newtonsoft.Json;
using TShockAPI;
//using PlaceholderAPI;

namespace SwitchCommands;

public class Database
{
    public static string databasePath = Path.Combine(TShock.SavePath, "开关配置表.json");

    [JsonProperty("是否开启开关保护", Order = -3)]
    public bool SwitchEnable = true;

    [JsonProperty("试图破坏开关的警告", Order = -3)]
    public string SwitchText = "你没有权限破坏指令开关！";

    [JsonProperty("开关指令表")]
    public Dictionary<string, CommandInfo> switchCommandList = new Dictionary<string, CommandInfo>();

    #region 读取配置文件方法
    public void Write(string path)
    {
        File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
    }

    public static Database Read(string path)
    {
        if (!File.Exists(path))
        {
            var c = new Database();
            c.Write(path);
            return c;
        }
        else
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sr = new StreamReader(fs))
            {
                var json = sr.ReadToEnd();
                var cf = JsonConvert.DeserializeObject<Database>(json);
                return cf!;
            }
        }
    }
    #endregion
}