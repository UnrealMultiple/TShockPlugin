using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattyBridge;

public enum MsgType
{
    [Description("unknow")]
    Unknow,

    [Description("player_chat")]
    Chat,

    [Description("player_leave")]
    Leave,

    [Description("player_join")]
    Join
}
