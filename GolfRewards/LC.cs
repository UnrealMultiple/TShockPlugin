using System;
using System.IO;
using TShockAPI;

namespace RewardSection
{
    internal class LC
    {
        public static ConfigFile LConfig { get; set; }
        internal static string LConfigPath => Path.Combine(TShock.SavePath, "高尔夫奖励.json");
        public static LPlayer[] LPlayers { get; set; }
        public static Random LRadom = new Random();

        public static void RI()
        {
            LConfig = new ConfigFile();
            LPlayers = new LPlayer[256];
        }

        public static void RC()
        {
            try
            {
                if (!File.Exists(LConfigPath))
                {
                    TShock.Log.ConsoleError("未找到高尔夫奖励配置文件，已为您创建！");
                }
                LConfig = ConfigFile.Read(LConfigPath);
                LConfig.Write(LConfigPath);
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError("高尔夫奖励插件错误配置读取错误:" + ex.ToString());
            }
        }
    }
}
