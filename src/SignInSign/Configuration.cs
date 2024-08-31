using Newtonsoft.Json;
using System.Text;
using TShockAPI;

namespace SignInSign;

public class Configuration
{
    public static string ConfigPath = Path.Combine(TShock.SavePath, "告示牌登录.json");

    [JsonProperty("是否开启注册登录功能", Order = -3)]
    public bool SignEnable = true;
    [JsonProperty("记录角色密码", Order = -3)]
    public bool PassInfo = false;

    [JsonProperty("对登录玩家显示告示牌", Order = -2)]
    public bool SignEnable1 = true;

    [JsonProperty("是否允许点击告示牌", Order = -2)]
    public bool SignEnable2 = true;
    [JsonProperty("点击告示牌是否发广播", Order = -2)]
    public bool SignEnable3 = false;

    [JsonProperty("创建告示牌的内容,重设指令:/gs r")]
    public string SignText = "欢迎来到开荒服！！\n本服支持PE/PC跨平台联机游玩\n每25分钟清理世界与Boss战排名统计\n更多指令教学请输入/help\n点击告示牌可进行传送\n\nTShock官方群：816771079\n";
    [JsonProperty("点击告示牌的广播/仅使用者可见")]
    public string SignText2 = "在本告示牌依序输入2次：\n[c/F7CCF0:123456]  进行注册登录。";
    [JsonProperty("试图破坏告示牌的广播")]
    public string SignText3 = "此告示牌不可被修改!";
    [JsonProperty("点击告示牌执行什么指令")]
    public string[] CmdList { get; set; } = new string[0];
    [JsonProperty("点击告示牌给什么BUFF")]
    public int[] BuffID { get; set; } = new int[] { };
    [JsonProperty("点击告示牌BUFF时长/分钟")]
    public int BuffTime { get; set; } = 10;
    [JsonProperty("点击告示牌给什么物品")]
    public int[] ItemID { get; set; } = new int[] { };
    [JsonProperty("点击告示牌给物品数量")]
    public int ItemStack { get; set; } = 1;

    [JsonProperty("点击告示牌是否传送,设置指令:/gs s")]
    public bool Teleport { get; set; } = false;
    [JsonProperty("点击告示牌传送坐标.X")]
    public float Teleport_X { get; set; } = 0;
    [JsonProperty("点击告示牌传送坐标.Y")]
    public float Teleport_Y { get; set; } = 0;
    [JsonProperty("点击告示牌传送特效")]
    public byte Style { get; set; } = 10;

    public static Configuration Reload()
    {
        Configuration? c = null;

        if (File.Exists(ConfigPath))
        {
            c = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(ConfigPath));
        }

        if (c == null)
        {
            c = new Configuration();
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(c, Formatting.Indented));
        }

        return c;
    }

    public void Write(string path)
    {
        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
        using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
        {
            var str = JsonConvert.SerializeObject(this, Formatting.Indented);
            sw.Write(str);
        }
    }

    #region 读取配置文件方法
    // 从文件读取配置
    public static Configuration Read(string path)
    {
        if (!File.Exists(path))
        {
            var c = new Configuration();
            c.Write(path);
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