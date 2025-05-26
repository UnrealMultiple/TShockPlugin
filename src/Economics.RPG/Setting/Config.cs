using Economics.Core.ConfigFiles;
using Economics.RPG.Converter;
using Economics.RPG.Model;
using Newtonsoft.Json;
using TShockAPI;

namespace Economics.RPG.Setting;

public class Config : JsonConfigBase<Config>
{
    protected override string Filename => "RPG.json";

    [JsonProperty("RPG信息")]
    public Dictionary<string, Level> RPG { get; set; } = new();

    [JsonProperty("重置职业执行命令")]
    public List<string> ResetCommand { get; set; } = new();

    [JsonProperty("重置职业广播")]
    public string ResetBroadcast { get; set; } = string.Empty;

    [JsonProperty("重置后踢出")]
    public bool ResetKick { get; set; } = false;

    [JsonProperty("默认等级")]
    [JsonConverter(typeof(LevelConverter))]
    public Level DefaultLevel { get; set; } = new();

    protected override void SetDefault()
    {
        this.RPG["战士"] = new()
        {
            SelectedWeapon = [],
            Name = "战士",
            Parent = this.DefaultLevel,
            RedemptionRelationshipsOption = [new()],
            RewardGoods = [new()]
        };
    }

    protected override void Initialize()
    {
        foreach (var (name, level) in this.RPG)
        {
            level.Name = name;
            var parentName = level.Parent?.Name ?? "";
            if (!this.RPG.TryGetValue(parentName, out var info) || info == null)
            {
                if (parentName != this.DefaultLevel.Name)
                {
                    TShock.Log.ConsoleError(GetString($"等级 {name} 空引用等级 {parentName}"));
                    level.Parent = null;
                }
                else
                {
                    level.Parent = this.DefaultLevel;
                }
            }
            else
            {
                level.Parent = info;
            }
        }

        foreach (var (name, level) in this.RPG)
        {
            level.AllParentLevels = this.GetLevelAllParent(name);
            level.AllPermission = this.GetLevelAllPerm(name);
            level.RankLevels = this.GetRankLevel(name);
        }
        this.DefaultLevel.RankLevels = this.GetRankLevel(this.DefaultLevel.Name);
    }

    public List<Level> GetRankLevel(string name)
    {
        var result = new List<Level>();
        foreach (var level in this.RPG.Values)
        {
            if (level.Parent?.Name == name)
            {
                result.Add(level);
            }
        }
        return result;
    }

    private HashSet<string> GetLevelAllPerm(string name)
    {
        var perms = new List<string>();
        var level = this.GetLevel(name);
        while (level != null)
        {
            perms.AddRange(level.AppendPermsssions);
            if (level.Parent?.Name == name || level.Parent?.Name == this.DefaultLevel.Name)
            {
                break;
            }
            level = level.Parent;
        }
        return [.. perms];
    }

    public Level? GetLevel(string name)
    {
        return this.RPG.TryGetValue(name, out var info) ? info : null;
    }

    public HashSet<Level> GetLevelAllParent(string name)
    {
        var parents = new HashSet<Level>();
        var level = this.GetLevel(name);
        while (level != null)
        {
            if (level.Parent?.Name == this.DefaultLevel.Name)
            {
                break;
            }
            if (level.Parent?.Name == name)
            {
                TShock.Log.ConsoleError(GetString($"{level.Name} 与 {name} 之间循环引用"));
                break;
            }
            parents.Add(level);
            level = level.Parent;
        }
        return parents;
    }
}