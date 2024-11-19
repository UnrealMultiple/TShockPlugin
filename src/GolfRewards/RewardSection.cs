using Newtonsoft.Json;

namespace RewardSection;

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
        this.HolePositionX = x;
        this.HolePositionY = y;
        this.MinSwings = 0;
        this.MaxSwings = 999;
        this.ItemSections = items;
        this.CommandSections = commands;
        this.OnlyClaimRewardAtThisLocation = false;
    }

    public ItemSections? GetRandomItems()
    {
        if (this.ItemSections.Count == 0)
        {
            return null;
        }

        var maxValue = this.ItemSections.Sum(t => t.RelativeProbability);
        var num = LC.LRadom.Next(1, maxValue);
        foreach (var item in this.ItemSections)
        {
            if (item.RelativeProbability >= num)
            {
                return item;
            }

            num -= item.RelativeProbability;
        }
        return null;
    }

    public string? GetRandomCMD()
    {
        if (this.CommandSections.Count == 0)
        {
            return null;
        }

        var count = this.CommandSections.Count;
        var num = LC.LRadom.Next(1, count);
        return this.CommandSections[num - 1];
    }

    [JsonProperty("命中提示")]
    public string HitPrompt { get; set; } = "高尔夫击中奖励球";
}