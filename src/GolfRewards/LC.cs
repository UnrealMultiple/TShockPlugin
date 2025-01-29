using System.Collections.Concurrent;
using TShockAPI;

namespace RewardSection;

internal class LC
{
    public static ConfigFile LConfig { get; set; } = new();
    internal static string LConfigPath => Path.Combine(TShock.SavePath, "高尔夫奖励.json");
    public static ConcurrentDictionary<int, LPlayer> LPlayers { get; set; } = new();
    public static Random LRadom = new Random();

    //public static void RI()
    //{
    //    LConfig = new ConfigFile();
    //    // 初始化 LPlayers 为 ConcurrentDictionary
    //    LPlayers = new ConcurrentDictionary<int, LPlayer>();
    //}

    public static void RC()
    {
        try
        {
            if (!File.Exists(LConfigPath))
            {
                TShock.Log.ConsoleError(GetString("未找到高尔夫奖励配置文件，已为您创建！"));
            }
            LConfig = ConfigFile.Read(LConfigPath);
            LConfig.Write(LConfigPath);
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(GetString($"高尔夫奖励插件错误配置读取错误:{ex}"));
        }
    }
}