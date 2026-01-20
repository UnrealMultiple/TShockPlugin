namespace AutoFish.Data;

public class AFPlayerData
{
    //玩家数据表（使用字典以便按玩家名快速检索）
    private Dictionary<string, ItemData> Items { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     获取或创建玩家数据入口，内部使用私有工厂方法。
    /// </summary>
    public ItemData GetOrCreatePlayerData(string name, Func<string, ItemData> factory)
    {
        return GetOrCreate(name, factory);
    }

    /// <summary>
    ///     私有的获取或创建逻辑，隐藏底层字典实现。
    /// </summary>
    private ItemData GetOrCreate(string name, Func<string, ItemData> factory)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Player name is required.", nameof(name));
        if (factory == null) throw new ArgumentNullException(nameof(factory));

        if (!Items.TryGetValue(name, out var data) || data == null)
        {
            data = factory(name);
            Items[name] = data;
        }

        return data;
    }

    /// <summary>
    ///     玩家自动钓鱼相关配置。
    /// </summary>
    public class ItemData
    {
        public ItemData(string name = "", bool autoFishEnabled = true, bool consumptionEnabled = true,
            bool buffEnabled = false, int hookMaxNum = 3, bool multiHookEnabled = false,
            bool firstFishHintShown = false, bool skipNonStackableLoot = true, bool blockMonsterCatch = false,
            bool skipFishingAnimation = true, bool protectValuableBaitEnabled = true)
        {
            Name = name ?? "";
            AutoFishEnabled = autoFishEnabled;
            ConsumptionEnabled = consumptionEnabled;
            BuffEnabled = buffEnabled;
            HookMaxNum = hookMaxNum;
            MultiHookEnabled = multiHookEnabled;
            FirstFishHintShown = firstFishHintShown;
            SkipNonStackableLoot = skipNonStackableLoot;
            BlockMonsterCatch = blockMonsterCatch;
            SkipFishingAnimation = skipFishingAnimation;
            ProtectValuableBaitEnabled = protectValuableBaitEnabled;
        }

        //玩家名字
        public string Name { get; set; }

        //总开关
        public bool AutoFishEnabled { get; set; }

        //消耗模式开关
        public bool ConsumptionEnabled { get; set; }

        //BUFF开关
        public bool BuffEnabled { get; set; }

        //鱼线数量上限
        public int HookMaxNum { get; set; } = 3;

        //多钩开关
        public bool MultiHookEnabled { get; set; } = true;

        //是否已提示自动钓鱼
        public bool FirstFishHintShown { get; set; }

        //过滤不可堆叠物品
        public bool SkipNonStackableLoot { get; set; } = true;

        //不钓怪物
        public bool BlockMonsterCatch { get; set; }

        //跳过上鱼动画
        public bool SkipFishingAnimation { get; set; } = true;

        //保护贵重鱼饵
        public bool ProtectValuableBaitEnabled { get; set; } = true;

        //记录时间
        public DateTime LogTime { get; set; }
    }
}