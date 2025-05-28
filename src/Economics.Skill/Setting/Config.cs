using Economics.Core.ConfigFiles;
using Economics.Skill.Model;
using Newtonsoft.Json;


namespace Economics.Skill.Setting;

public class Config : JsonConfigBase<Config>
{
    protected override string Filename => "Skill.json";

    [JsonProperty("绑定技能最大数量")]
    public int SkillMaxCount { get; set; }

    [JsonProperty("单武器绑定最大技能数量")]
    public int WeapoeBindMaxCount { get; set; }

    [JsonProperty("禁止拉怪表")]
    public HashSet<int> BanPullNpcs { get; set; } = new();

    [JsonProperty("禁止伤怪表")]
    public HashSet<int> BanStrikeNpcs { get; set; } = new();

    [JsonProperty("最大显示页")]
    public int PageMax { get; set; } = 20;

    [JsonProperty("技能列表")]
    public List<SkillContext> SkillContexts { get; set; } = new();

    protected override void SetDefault()
    {
        this.SkillContexts = new()
        {
            new SkillContext()
            {
                LoopEvent = new()
                {
                    ProjectileLoops = []

                }
            }
        };
    }

    public SkillContext? GetSkill(int index)
    {
        return index < 1 || index > this.SkillContexts.Count ? null : this.SkillContexts[index - 1];
    }
}