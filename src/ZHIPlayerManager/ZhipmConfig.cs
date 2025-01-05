using Newtonsoft.Json;
using TShockAPI;

namespace ZHIPlayerManager;

public class ZhipmConfig
{
    static readonly string configPath = Path.Combine(TShock.SavePath + "/Zhipm", "ZhiPlayerManager.json");

    /// <summary>
    /// 从文件中导出
    /// </summary>
    /// <returns></returns>
    public static ZhipmConfig LoadConfigFile()
    {
        if (!Directory.Exists(TShock.SavePath + "/Zhipm"))
        {
            Directory.CreateDirectory(TShock.SavePath + "/Zhipm");
        }
        if (!File.Exists(configPath))
        {
            File.WriteAllText(configPath, JsonConvert.SerializeObject(new ZhipmConfig(
                true, true, true, false, true, false, true, true, 20, 5, new List<int> { 68 }, new List<int>(), true, 40, 1.0f, false
                ), Formatting.Indented));
        }

        return JsonConvert.DeserializeObject<ZhipmConfig>(File.ReadAllText(configPath))!;
    }

    public void SaveConfigFile()
    {
        File.WriteAllText(configPath, JsonConvert.SerializeObject(this, Formatting.Indented));
    }

    public ZhipmConfig()
    {
        this.AdditionalCreaturesForDamageLeaderboard = new();
        this.CreaturesTreatedAsRareForKills = new();
    }

    public ZhipmConfig(
        bool enableOnlineTimeTracking,
        bool enableDeathCountTracking,
        bool enableNpcKillTracking,
        bool enablePointTracking,
        bool defaultKillFontVisibleToPlayers,
        bool defaultPointFontVisibleToPlayers,
        bool enableBossDamageLeaderboard,
        bool enablePlayerAutoBackup,
        int defaultAutoBackupIntervalInMinutes,
        int maxBackupsPerPlayer,
        List<int> additionalCreaturesForDamageLeaderboard,
        List<int> creaturesTreatedAsRareForKills,
        bool allowPlayerRespawnAtLastDeathPoint,
        int respawnCostPoints,
        float pointsLossMultiplierOnDeath,
        bool enableSpecialNameBan
    )
    {
        this.EnableOnlineTimeTracking = enableOnlineTimeTracking;
        this.EnableDeathCountTracking = enableDeathCountTracking;
        this.EnableNpcKillTracking = enableNpcKillTracking;
        this.EnablePointTracking = enablePointTracking;
        this.DefaultKillFontVisibleToPlayers = defaultKillFontVisibleToPlayers;
        this.DefaultPointFontVisibleToPlayers = defaultPointFontVisibleToPlayers;
        this.EnableBossDamageLeaderboard = enableBossDamageLeaderboard;
        this.EnablePlayerAutoBackup = enablePlayerAutoBackup;
        this.DefaultAutoBackupIntervalInMinutes = defaultAutoBackupIntervalInMinutes;
        this.MaxBackupsPerPlayer = maxBackupsPerPlayer;
        this.AdditionalCreaturesForDamageLeaderboard = additionalCreaturesForDamageLeaderboard;
        this.CreaturesTreatedAsRareForKills = creaturesTreatedAsRareForKills;
        this.AllowPlayerRespawnAtLastDeathPoint = allowPlayerRespawnAtLastDeathPoint;
        this.RespawnCostPoints = respawnCostPoints;
        this.PointsLossMultiplierOnDeath = pointsLossMultiplierOnDeath;
        this.EnableSpecialNameBan = enableSpecialNameBan;

    }

    [JsonProperty("是否启用在线时长统计")]
    public bool EnableOnlineTimeTracking { get; set; }

    [JsonProperty("是否启用死亡次数统计")]
    public bool EnableDeathCountTracking { get; set; }

    [JsonProperty("是否启用击杀NPC统计")]
    public bool EnableNpcKillTracking { get; set; }

    [JsonProperty("是否启用点数统计")]
    public bool EnablePointTracking { get; set; }

    [JsonProperty("默认击杀字体是否对玩家显示")]
    public bool DefaultKillFontVisibleToPlayers { get; set; }

    [JsonProperty("默认点数字体是否对玩家显示")]
    public bool DefaultPointFontVisibleToPlayers { get; set; }

    [JsonProperty("是否启用击杀Boss伤害排行榜")]
    public bool EnableBossDamageLeaderboard { get; set; }

    [JsonProperty("是否启用玩家自动备份")]
    public bool EnablePlayerAutoBackup { get; set; }

    [JsonProperty("默认自动备份的时间_单位分钟_若为0代表关闭")]
    public int DefaultAutoBackupIntervalInMinutes { get; set; }

    [JsonProperty("每个玩家最多几个备份存档")]
    public int MaxBackupsPerPlayer { get; set; }

    [JsonProperty("哪些生物也包含进击杀伤害排行榜")]
    public List<int> AdditionalCreaturesForDamageLeaderboard { get; set; }

    [JsonProperty("哪些生物也当成稀有生物进行击杀记录")]
    public List<int> CreaturesTreatedAsRareForKills { get; set; }

    [JsonProperty("是否允许玩家回溯上次死亡点")]
    public bool AllowPlayerRespawnAtLastDeathPoint { get; set; }

    [JsonProperty("每次死亡回溯消耗点数")]
    public int RespawnCostPoints { get; set; }

    [JsonProperty("死亡时丢失点数乘数")]
    public float PointsLossMultiplierOnDeath { get; set; }

    [JsonProperty("是否允许特殊名字")]
    public bool EnableSpecialNameBan { get; set; }
}