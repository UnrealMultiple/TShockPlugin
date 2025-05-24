using Economics.RPG.Converter;
using Economics.Core.ConfigFiles;
using Economics.Core.Model;
using Newtonsoft.Json;

namespace Economics.RPG.Model;

public class Level
{
    [JsonProperty("聊天前缀")]
    public string ChatPrefix { get; set; } = string.Empty;

    [JsonProperty("聊天颜色")]
    public int[] ChatRGB { get; set; } = new int[3];

    [JsonProperty("聊天后缀")]
    public string ChatSuffix { get; set; } = string.Empty;

    [JsonProperty("聊天格式")]
    public string ChatFormat { get; set; } = string.Empty;

    [JsonProperty("升级广播")]
    public string RankBroadcast { get; set; } = string.Empty;

    [JsonProperty("唯一职业")]
    public bool SoleOccupation { get; set; }

    [JsonProperty("手持武器")]
    public HashSet<int> SelectedWeapon { get; set; } = new();

    [JsonProperty("进度限制")]
    public HashSet<string> Limit { get; set; } = new();

    [JsonProperty("升级指令")]
    public string[] RankCommands { get; set; } = Array.Empty<string>();

    [JsonProperty("附加权限")]
    public List<string> AppendPermsssions { get; set; } = new();

    [JsonProperty("升级奖励")]
    public Item[] RewardGoods { get; set; } = Array.Empty<Item>();

    [JsonProperty("升级消耗")]
    public List<RedemptionRelationshipsOption> RedemptionRelationshipsOption { get; set; } = new();

    [JsonProperty("父等级")]
    [JsonConverter(typeof(LevelConverter))]
    public Level? Parent { get; set; }

    [JsonIgnore]
    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public HashSet<Level> AllParentLevels { get; set; } = new();

    [JsonIgnore]
    public HashSet<string> AllPermission { get; set; } = new();

    [JsonIgnore]
    public List<Level> RankLevels { get; set; } = new();
}