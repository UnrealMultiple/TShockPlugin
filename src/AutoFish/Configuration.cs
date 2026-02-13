using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;
using TShockAPI.Hooks;

namespace AutoFish;

[Config]
internal class Configuration : JsonConfigBase<Configuration>
{
    public class BaitReward
    {
        [LocalizedPropertyName(CultureType.Chinese, "数量")]
        public int Count { get; set; }

        [LocalizedPropertyName(CultureType.Chinese, "时长")]
        public int Minutes { get; set; }
    }

    [LocalizedPropertyName(CultureType.Chinese, "启用")]
    public bool Enabled { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "启用自动钓鱼")]
    public bool GlobalAutoFishFeatureEnabled { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "玩家默认开启自动钓鱼")]
    public bool DefaultAutoFishEnabled { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "启用BUFF")]
    public bool GlobalBuffFeatureEnabled { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "玩家默认开启自动BUFF")]
    public bool DefaultBuffEnabled { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "启动多钩")]
    public bool GlobalMultiHookFeatureEnabled { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "多钩数量阈值")]
    public int GlobalMultiHookMaxNum { get; set; } = 5;

    [LocalizedPropertyName(CultureType.Chinese, "玩家默认开启多钩")]
    public bool DefaultMultiHookEnabled { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "忽略敌怪")]
    public bool GlobalBlockMonsterCatch { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "玩家默认忽略敌怪")]
    public bool DefaultBlockMonsterCatch { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "跳过动画")]
    public bool GlobalSkipFishingAnimation { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "玩家默认跳过动画")]
    public bool DefaultSkipFishingAnimation { get; set; } = false;

    [LocalizedPropertyName(CultureType.Chinese, "忽略任务鱼")]
    public bool GlobalBlockQuestFish { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "玩家默认忽略任务鱼")]
    public bool DefaultBlockQuestFish { get; set; } = false;

    [LocalizedPropertyName(CultureType.Chinese, "保护鱼饵")]
    public bool GlobalProtectValuableBaitEnabled { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "玩家默认保护鱼饵")]
    public bool DefaultProtectValuableBaitEnabled { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "消耗模式")]
    public bool GlobalConsumptionModeEnabled { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "消耗配置")]
    public Dictionary<int, BaitReward> BaitRewards { get; set; } = [];

    [LocalizedPropertyName(CultureType.Chinese, "保护鱼饵列表")]
    public List<int> ValuableBaitItemIds { get; set; } = [];

    [LocalizedPropertyName(CultureType.Chinese, "BUFF配置")]
    public Dictionary<int, int> BuffDurations { get; set; } = [];

    protected override string Filename => "AutoFish";

    protected override void SetDefault()
    {
        this.BaitRewards = new Dictionary<int, BaitReward>()
        {
            { 2002, new BaitReward { Count = 1, Minutes = 1 } }, // 蠣虫
            { 2675, new BaitReward { Count = 1, Minutes = 5 } }, // 熟手诱饵
            { 2676, new BaitReward { Count = 1, Minutes = 10 } }, // 大师诱饵
            { 3191, new BaitReward { Count = 1, Minutes = 8 } }, // 附魔夜行者
            { 3194, new BaitReward { Count = 1, Minutes = 5 } } // 蝗虫
        };

        this.ValuableBaitItemIds =
        [
             2673, // 松露虫
             1999, // 帛斑蝶
             2436, // 蓝水母
             2437, // 绿水母
             2438, // 粉水母
             2891, // 金蝴蝶
             4340, // 金蜻蜓
             2893, // 金蚱蜢
             4362, // 金瓢虫
             4419, // 金水黾
             2895 // 金蠕虫
        ];

        this.BuffDurations.Add(114, 1);
    }

    protected override void Reload(ReloadEventArgs args)
    {
        args.Player.SendSuccessMessage("[自动钓鱼] 配置重读成功");
    }
}
