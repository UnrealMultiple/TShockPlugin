using TShockAPI;

namespace ChattyBridge.Message;

internal class PlayerLeaveMessage : PlayerMessage
{
    public PlayerLeaveMessage(TSPlayer player) : base(player)
    {
        this.Type = MsgType.Leave;
    }
    public PlayerLeaveMessage()
    {

    }
}