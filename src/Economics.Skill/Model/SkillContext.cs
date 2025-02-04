using Economics.Skill.JSInterpreter;
using Economics.Skill.Model.Loop;
using Economics.Skill.Model.Options;
using EconomicsAPI.Configured;
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
    public List<RedemptionRelationshipsOption> RedemptionRelationshipsOption { get; set; } = new();

    [JsonProperty("限制等级")]
    public List<string> LimitLevel { get; set; } = new();

    [JsonProperty("限制进度")]
    public List<string> LimitProgress { get; set; } = new();

    [JsonProperty("限制技能")]
    public List<int> LimitSkill { get; set; } = new();

    [JsonProperty("事件循环")]
    public LoopEvent LoopEvent { get; set; } = new();

    [JsonProperty("触发设置")]
    public SkillSparkOption SkillSpark { get; set; } = new();

    [JsonProperty("执行脚本")]
    public string? ExecuteScript
    {
        get => this.JsScript?.FilePathOrUri;
        set => this.JsScript = this.Set(value!);
    }

    [JsonIgnore]
    public JsScript? JsScript { get; set; }

    public JsScript? Set(string path)
    {
        var jistScript = new JsScript
        {
            FilePathOrUri = path
        };
        try
        {
            jistScript.Script = File.ReadAllText(Path.Combine(Interpreter.ScriptsDir, jistScript.FilePathOrUri));
        }
        catch (Exception ex)
        {
            TShock.Log.Error("无法加载{0}: {1}", path, ex.Message);
            return null;
        }
        ScriptContainer.PreprocessRequires(jistScript);
        return jistScript;
    }

    

}