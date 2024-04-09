using System.ComponentModel;

namespace Economics.Skill.Enumerates;

[Flags]
public enum SkillSparkType
{
    [Description("")]
    Null = 
        0b0000000,

    [Description("主动")]
    Take = 
        0b00000001,

    [Description("血量")]
    HP = 
        0b00000100,

    [Description("蓝量")]
    MP = 
        0b00001000,

    [Description("CD")]
    CD = 
        0b00010000,

    [Description("死亡")]
    Death = 
        0b00100000,

    [Description("击杀")]
    Kill = 
        0b01000000,

    [Description("击打")]
    Strike = 
        0b10000000
}
