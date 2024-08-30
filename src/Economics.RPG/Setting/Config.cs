using Economics.RPG.Converter;
using Economics.RPG.Model;
using Newtonsoft.Json;
using TShockAPI;

namespace Economics.RPG.Setting;

internal class Config
{
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

    public void Init()
    {
        foreach (var (name, level) in RPG)
        {
            level.Name = name;
            if (!RPG.TryGetValue(level.Parent?.Name, out var info) || info == null)
            {
                if (level.Parent.Name != DefaultLevel.Name)
                {
                    TShock.Log.ConsoleError($"等级 {name} 空引用等级 {level.Parent.Name}");
                    level.Parent = null;
                }
                else
                {
                    level.Parent = DefaultLevel;
                }
            }
            else
            {
                level.Parent = info;
            }
        }

        foreach (var (name, level) in RPG)
        {
            level.AllParentLevels = GetLevelAllParent(name);
            level.AllPermission = GetLevelAllPerm(name);
            level.RankLevels = GetRankLevel(name);
        }
        DefaultLevel.RankLevels = GetRankLevel(DefaultLevel.Name);
    }

    public List<Level> GetRankLevel(string name)
    {
        var result = new List<Level>();
        foreach (var level in RPG.Values)
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
        var level = GetLevel(name);
        while (level != null)
        {
            if (level.Parent?.Name == name || level.Parent?.Name == DefaultLevel.Name)
            {
                break;
            }
            perms.AddRange(level.AppendPermsssions);
            level = level.Parent;
        }
        return perms.ToHashSet();
    }

    public Level? GetLevel(string name)
    {
        if (RPG.TryGetValue(name, out var info))
            return info;
        return null;
    }

    public HashSet<Level> GetLevelAllParent(string name)
    {
        var parents = new HashSet<Level>();
        var level = GetLevel(name);
        while (level != null)
        {
            if (level.Parent?.Name == DefaultLevel.Name)
            {
                break;
            }
            if (level.Parent?.Name == name)
            {
                TShock.Log.ConsoleError($"{level.Name} 与 {name} 之间循环引用");
                break;
            }
            parents.Add(level);
            level = level.Parent;
        }
        return parents;
    }
}