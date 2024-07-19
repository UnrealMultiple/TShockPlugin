using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
