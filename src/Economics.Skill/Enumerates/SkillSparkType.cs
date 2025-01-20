using System.ComponentModel;

namespace Economics.Skill.Enumerates;

[Flags]
public enum SkillSparkType
{
    [Description("")]
    Null,

    [Description("主动")]
    Take,

    [Description("血量")]
    HP,

    [Description("蓝量")]
    MP,

    [Description("CD")]
    CD,

    [Description("死亡")]
    Death,

    [Description("击杀")]
    Kill,

    [Description("击打")]
    Strike,

    [Description("冲刺")]
    Dash,

    [Description("装备")]
    Armor,

    [Description("跳跃")]
    Jump,

    [Description("受击")]
    Struck,

    [Description("BUFF")]
    Buff,

    [Description("环境")]
    Environment,

    [Description("技能")]
    Skill
}