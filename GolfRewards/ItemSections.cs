using Newtonsoft.Json;
using System;

namespace RewardSection
{
    public class ItemSections
    {
        [JsonProperty("物品ID")]
        public int ItemId { get; set; }

        [JsonProperty("物品数量")]
        public int ItemStack { get; set; }

        public ItemSections(int id, int stack, int prefix)
        {
            ItemId = id;
            ItemStack = stack;
            ItemPrefix = prefix;
        }

        [JsonProperty("相对概率")]
        public int RelativeProbability { get; set; } = 1;

        [JsonProperty("物品前缀")]
        public int ItemPrefix { get; set; } = 0;
    }
}
