using System.ComponentModel;

namespace Economics.Skill.Enumerates;

public enum AIStyleType
{
    [Description("空")]
    None = -1,

    [Description("环绕")]
    Revolve,

    [Description("悬浮")]
    Hover,

    [Description("跟踪")]
    Trace
}