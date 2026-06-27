using System;
using System.IO;
using Newtonsoft.Json;

namespace RealTime;

public class RealTimeConfig
{
    [JsonProperty("同步现实时间")]
    public bool EnableRealTimeSync { get; set; } = true;

    [JsonProperty("同步间隔秒")]
    public int RealTimeSyncInterval { get; set; } = 8;

    [JsonProperty("事件自动结束")]
    public bool EnableEventAutoEnd { get; set; } = true;

    [JsonProperty("血月日食持续时间分钟")]
    public int BloodMoonEclipseDuration { get; set; } = 20;

    [JsonProperty("霜月万圣节持续时间分钟")]
    public int PumpkinMoonSnowMoonDuration { get; set; } = 40;

    [JsonProperty("渔夫任务刷新")]
    public bool EnableAnglerQuest { get; set; } = true;

    [JsonProperty("月相刷新")]
    public bool EnableMoonPhase { get; set; } = true;

    [JsonProperty("NPC综合刷新间隔分钟")]
    public int NpcRefreshInterval { get; set; } = 24;

    [JsonProperty("老人自动召唤")]
    public bool EnableOldManSpawn { get; set; } = true;

    [JsonProperty("拜月教徒自动召唤")]
    public bool EnableCultistSpawn { get; set; } = true;

    [JsonProperty("旅商商品刷新")]
    public bool EnableTravelShopRefresh { get; set; } = true;

    [JsonProperty("旅商常驻")]
    public bool EnableTravelNPCStay { get; set; } = true;

    [JsonProperty("随机天气")]
    public bool EnableWeatherChange { get; set; } = true;

    [JsonProperty("天气更新间隔分钟")]
    public int WeatherChangeInterval { get; set; } = 10;

    [JsonProperty("夜间NPC入住")]
    public bool EnableNightNpcSpawn { get; set; } = true;

    [JsonProperty("事件强制PVP")]
    public bool EnableEventForcePvp { get; set; } = true;

    [JsonProperty("事件禁止切换队伍")]
    public bool EnableEventTeamLock { get; set; } = true;

    public static RealTimeConfig Load(string path)
    {
        if (!File.Exists(path))
        {
            var cfg = new RealTimeConfig();
            cfg.Save(path);
            return cfg;
        }
        var json = File.ReadAllText(path);
        var config = JsonConvert.DeserializeObject<RealTimeConfig>(json);
        return config ?? new RealTimeConfig();
    }

    public void Save(string path)
    {
        var json = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(path, json);
    }
}