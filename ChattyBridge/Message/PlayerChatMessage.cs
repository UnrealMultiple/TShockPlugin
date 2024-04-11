using Newtonsoft.Json;
using TShockAPI;

namespace ChattyBridge.Message;

internal class PlayerChatMessage : PlayerMessage
{
    [JsonProperty("text")]
    public string Text { get; init; }
    public PlayerChatMessage(TSPlayer player, string text) : base(player)
    {
        Text = text;
        Type = MsgType.Chat;
    }
    public PlayerChatMessage()
    {

    }
}
