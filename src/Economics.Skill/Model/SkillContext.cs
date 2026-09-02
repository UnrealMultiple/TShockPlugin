using Economics.Skill.Scripting;
using Economics.Skill.Model.Options;
using Economics.Core.ConfigFiles;
using Newtonsoft.Json;
using TShockAPI;

namespace Economics.Skill.Model;

public class SkillContext
{
    [JsonProperty("名称")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("喊话")]
    public string Broadcast { get; set; } = string.Empty;

    [JsonProperty("技能唯一")]
    public bool SkillUnique { get; set; }

    [JsonProperty("全服唯一")]
    public bool SkillUniqueAll { get; set; }

    [JsonProperty("隐藏")]
    public bool Hidden { get; set; }

    [JsonProperty("技能价格")]
    public List<RedemptionRelationshipsOption> RedemptionRelationshipsOption { get; set; } = [];

    [JsonProperty("限制等级")]
    public List<string> LimitLevel { get; set; } = [];

    [JsonProperty("限制进度")]
    public List<string> LimitProgress { get; set; } = [];

    [JsonProperty("限制技能")]
    public List<int> LimitSkill { get; set; } = [];

    [JsonProperty("触发设置")]
    public SkillSparkOption SkillSpark { get; set; } = new();

    [JsonProperty("技能等级设置")]
    public Dictionary<int, List<RedemptionRelationshipsOption>> SkillLevelOptions { get; set; } = [];

    private string? _executeScript;

    /// <summary>是否在每次触发时重置脚本全局变量（false=保留，true=每次归零）。</summary>
    [JsonProperty("重置变量")]
    public bool ResetVariables { get; set; }

    [JsonProperty("执行脚本")]
    public string? ExecuteScript
    {
        get => _executeScript;
        set
        {
            _executeScript = value;
            ScriptLocation = ResolveScript(value);
        }
    }

    /// <summary>解析后的脚本文件绝对路径（缓存 key），无效脚本为 <c>null</c>。</summary>
    [JsonIgnore]
    public string? ScriptLocation { get; private set; }

    private static string? ResolveScript(string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        if (!SkillScripts.Manager.TryResolve(key, out var location))
        {
            TShock.Log.Error("无法加载{0}: 脚本文件不存在", key);
            return null;
        }

        // reload 时（重新反序列化配置会再次走到这里）只需让运行时注意到文件变化：
        // 未修改的文件不会重读/重编译，已修改的文件在下次触发时才重建。
        SkillScripts.Reload(key);
        return location;
    }
}