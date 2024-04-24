using Newtonsoft.Json;

namespace RewardSection
{
    public class RewardSection
    {
        [JsonProperty("球洞坐标X")]
        public int HolePositionX { get; set; }

        [JsonProperty("球洞坐标Y")]
        public int HolePositionY { get; set; }

        [JsonProperty("最少杆数")]
        public int MinSwings { get; set; }

        [JsonProperty("最多杆数")]
        public int MaxSwings { get; set; }

        [JsonProperty("本位置仅领取该奖励")]
        public bool OnlyClaimRewardAtThisLocation { get; set; }

        [JsonProperty("物品节")]
        public List<ItemSections> ItemSections { get; set; }

        [JsonProperty("指令节")]
        public List<string> CommandSections { get; set; }

        public RewardSection(int x, int y, List<ItemSections> items, List<string> commands)
        {
            HolePositionX = x;
            HolePositionY = y;
            MinSwings = 0;
            MaxSwings = 999;
            ItemSections = items;
            CommandSections = commands;
            OnlyClaimRewardAtThisLocation = false;
        }

        public ItemSections GetRandomItems()
        {
            if (ItemSections.Count == 0)
                return null;

            int maxValue = ItemSections.Sum(t => t.RelativeProbability);
            int num = LC.LRadom.Next(1, maxValue);
            foreach (ItemSections item in ItemSections)
            {
                if (item.RelativeProbability >= num)
                    return item;
                num -= item.RelativeProbability;
            }
            return null;
        }

        public string GetRandomCMD()
        {
            if (CommandSections.Count == 0)
                return null;

            int count = CommandSections.Count;
            int num = LC.LRadom.Next(1, count);
            return CommandSections[num - 1];
        }

        [JsonProperty("命中提示")]
        public string HitPrompt { get; set; } = "高尔夫击中奖励球";
    }
}

