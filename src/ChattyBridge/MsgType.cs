using System.ComponentModel;

namespace ChattyBridge;

public enum MsgType
{
    [Description("unknown")]
    Unknown,

    [Description("player_chat")]
    Chat,

    [Description("player_leave")]
    Leave,

    [Description("player_join")]
    Join
}