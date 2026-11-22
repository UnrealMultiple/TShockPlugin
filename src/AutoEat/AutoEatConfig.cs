using Newtonsoft.Json;
using TShockAPI;

namespace AutoEat;

/// <summary>
/// AutoEat 配置文件（tshock/AutoEat.json）。
/// </summary>
public sealed class AutoEatConfig
{
    /// <summary>食物物品对应的饱食 buff 档位：26 吃得好 / 206 很满意 / 207 吃得很饱（值越高档位越高）。</summary>
    public static readonly Dictionary<int, int> FoodTiers = new()
    {
        [26] = 0,
        [206] = 1,
        [207] = 2,
    };

    private static string ConfigPath => Path.Combine(TShock.SavePath, "AutoEat.json");

    public static AutoEatConfig Current { get; private set; } = new();

    [JsonProperty("全局启用")]
    public bool GlobalEnabled = true;

    [JsonProperty("默认开启")]
    public bool DefaultEnabled = true;

    [JsonProperty("检查间隔(秒)")]
    public int CheckIntervalSeconds = 1;

    [JsonProperty("刚过期判定(秒)")]
    public int ExpireWindowSeconds = 30;

    [JsonProperty("进食提示")]
    public bool Notice = true;

    /// <summary>
    /// 「手动取消」判定容差（秒）。以插件记录的「预期自然过期时刻」为基准：
    /// buff 消失时刻落在预期到期前该秒数之内 → 自然过期，自动续；明显早于预期（超出该秒数）→ 视为手动取消，不自动续。
    /// （服务端 buffTime 读数恒为 60 且不递减、不可信，故不按剩余时长判定。）
    /// </summary>
    [JsonProperty("手动取消判定(秒)")]
    public int ManualCancelThresholdSeconds = 3;

    /// <summary>续期提示合并窗口（秒）：窗口内的多次续期合并为一条消息（避免多瓶药水刷屏）。</summary>
    [JsonProperty("提示合并窗口(秒)")]
    public int NoticeBatchSeconds = 3;

    [JsonProperty("续食物")]
    public RenewFoodConfig RenewFood = new();

    [JsonProperty("续buff")]
    public RenewBuffConfig RenewBuff = new();

    public sealed class RenewFoodConfig
    {
        [JsonProperty("启用")]
        public bool Enabled = true;

        /// <summary>同档食物吃完时，允许吃低档食物替补（高档食物始终可用作替补升级）。</summary>
        [JsonProperty("允许低档替补")]
        public bool AllowLowerTier = true;

        /// <summary>永不自动食用的食物物品 ID（如稀有收藏食物）。</summary>
        [JsonProperty("忽略物品ID")]
        public List<int> IgnoredItemIds = new();

        /// <summary>非空时只自动食用列表内的食物物品 ID。</summary>
        [JsonProperty("仅白名单物品ID")]
        public List<int> WhitelistItemIds = new();
    }

    public sealed class RenewBuffConfig
    {
        [JsonProperty("启用")]
        public bool Enabled = true;

        /// <summary>允许自动续上的 buff ID 列表；仅续「激活过且刚过期（非手动取消）」的。续期方式为真实消耗背包里能给出该 buff 的药水/食物（按其原时长）。</summary>
        [JsonProperty("buff列表")]
        public List<int> BuffList = new() { 26, 206, 207 };

        /// <summary>背包里没有能给出该 buff 的物品时，是否用服务器指令 SetBuff 兜底续上（默认关闭：优先真实喝药）。</summary>
        [JsonProperty("无药水时指令兜底")]
        public bool FallbackSetBuff = false;

        /// <summary>仅在「无药水时指令兜底」开启时生效：指令续 buff 给予的时长（秒）。真实喝药时以物品原时长为准。</summary>
        [JsonProperty("时长(秒)")]
        public int DurationSeconds = 480;
    }

    /// <summary>当前配置下需要跟踪的 buff（食物档位 ∪ 续buff列表）。</summary>
    public HashSet<int> GetTrackedBuffs()
    {
        var set = new HashSet<int>(FoodTiers.Keys);
        foreach (var b in RenewBuff.BuffList)
        {
            if (b > 0)
            {
                set.Add(b);
            }
        }
        return set;
    }

    /// <summary>修正非法配置值。</summary>
    public void Normalize()
    {
        CheckIntervalSeconds = Math.Max(1, CheckIntervalSeconds);
        ExpireWindowSeconds = Math.Max(1, ExpireWindowSeconds);
        ManualCancelThresholdSeconds = Math.Max(0, ManualCancelThresholdSeconds);
        NoticeBatchSeconds = Math.Max(0, NoticeBatchSeconds);
        RenewBuff.DurationSeconds = Math.Max(1, RenewBuff.DurationSeconds);
        RenewFood.IgnoredItemIds = RenewFood.IgnoredItemIds.Distinct().ToList();
        RenewFood.WhitelistItemIds = RenewFood.WhitelistItemIds.Distinct().ToList();
        RenewBuff.BuffList = RenewBuff.BuffList.Where(b => b > 0).Distinct().ToList();
    }

    public static void Load()
    {
        try
        {
            Current = File.Exists(ConfigPath)
                ? JsonConvert.DeserializeObject<AutoEatConfig>(File.ReadAllText(ConfigPath)) ?? new AutoEatConfig()
                : new AutoEatConfig();
        }
        catch (Exception ex)
        {
            TShock.Log.Error($"[AutoEat] 配置文件读取失败，使用默认配置: {ex.Message}");
            Current = new AutoEatConfig();
        }

        Current.Normalize();
        Current.Save();
    }

    public void Save()
    {
        try
        {
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
        catch (Exception ex)
        {
            TShock.Log.Error($"[AutoEat] 配置文件写入失败: {ex.Message}");
        }
    }
}
