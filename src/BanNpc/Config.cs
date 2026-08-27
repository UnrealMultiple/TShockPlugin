using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;
using LazyAPI;
using System.Collections.Generic;

namespace BanNpc;

[Config]
public class Config : JsonConfigBase<Config>
{
    protected override string Filename => "BanNpc";

    public HashSet<int> BanNpcs { get; set; } = new HashSet<int>();

    public List<BossRateConfig> BossRateLimitList { get; set; } = new List<BossRateConfig>();
}

public class BossRateConfig
{
    public int NpcId { get; set; }
    public int PlayerCooldownSeconds { get; set; } = 0;
    public int GlobalMaxAlive { get; set; } = 2;
    public string DenyMessage { get; set; } = "召唤过于频繁";
    public bool BroadcastDeny { get; set; } = false;
    public List<RateWindow> RateWindows { get; set; } = new List<RateWindow>();
}

public class RateWindow
{
    public int WindowSeconds { get; set; }
    public int MaxCount { get; set; }
}