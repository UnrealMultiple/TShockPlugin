using TShockAPI;

namespace ChattyBridge.Message;

internal class PlayerJoinMessage : PlayerMessage
{
    public PlayerJoinMessage(TSPlayer player) : base(player)
    {
        this.Type = MsgType.Join;
    }
    public PlayerJoinMessage()
    {

    }
}