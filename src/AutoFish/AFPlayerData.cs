namespace AutoFish;

public class AFPlayerData
{
    //玩家数据表（使用字典以便按玩家名快速检索）
    private Dictionary<string, ItemData> Items { get; } = new(StringComparer.OrdinalIgnoreCase);

    internal ItemData GetOrCreatePlayerData(string name, Func<string, ItemData> factory)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Player name is required.", nameof(name));
        if (factory == null) throw new ArgumentNullException(nameof(factory));

        if (!this.Items.TryGetValue(name, out var data) || data == null)
        {
            data = factory(name);
            this.Items[name] = data;
        }

        return data;
    }

    public class ItemData(string name = "", bool autoFishEnabled = true,
        bool buffEnabled = false, int hookMaxNum = 3, bool multiHookEnabled = false,
        bool firstFishHintShown = false, bool blockMonsterCatch = false,
        bool skipFishingAnimation = true, bool protectValuableBaitEnabled = true, bool blockQuestFish = true)
    {
        public bool CanConsume()
        {
            if (this.GetRemainTimeInMinute() <= 0)
            {
                return false;
            }

            return true;
        }

        public double GetRemainTimeInMinute()
        {
            var minutesHave = (this.ConsumeOverTime - DateTime.Now).TotalMinutes;
            return minutesHave;
        }

        public (int minutes, int seconds) GetRemainTime()
        {
            var timeSpan = this.ConsumeOverTime - DateTime.Now;
            if (timeSpan.TotalSeconds <= 0)
                return (0, 0);
            return ((int) timeSpan.TotalMinutes, timeSpan.Seconds);
        }


        //玩家名字
        public string Name { get; set; } = name ?? "";

        //总开关
        public bool AutoFishEnabled { get; set; } = autoFishEnabled;

        //BUFF开关
        public bool BuffEnabled { get; set; } = buffEnabled;

        //鱼线数量上限
        public int HookMaxNum { get; set; } = hookMaxNum;

        //多钩开关
        public bool MultiHookEnabled { get; set; } = multiHookEnabled;

        //是否已提示自动钓鱼
        public bool FirstFishHintShown { get; set; } = firstFishHintShown;

        //不钓怪物
        public bool BlockMonsterCatch { get; set; } = blockMonsterCatch;

        //跳过上鱼动画
        public bool SkipFishingAnimation { get; set; } = skipFishingAnimation;

        //屏蔽任务鱼
        public bool BlockQuestFish { get; set; } = blockQuestFish;

        //保护贵重鱼饵
        public bool ProtectValuableBaitEnabled { get; set; } = protectValuableBaitEnabled;

        //记录时间，用于判定
        public DateTime ConsumeOverTime { get; set; } = DateTime.Now;

        //记录时间，仅用于提示
        public DateTime ConsumeStartTime { get; set; } = default;
    }
}