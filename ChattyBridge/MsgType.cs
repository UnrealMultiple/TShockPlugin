using System.ComponentModel;

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
