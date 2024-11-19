using Newtonsoft.Json;
using TShockAPI;

namespace ChattyBridge.Message;

internal class PlayerChatMessage : PlayerMessage
{
    [JsonProperty("text")]
    public string Text { get; init; }
    public PlayerChatMessage(TSPlayer player, string text) : base(player)
    {
        this.Text = text;
        this.Type = MsgType.Chat;
    }
    public PlayerChatMessage()
    {
        this.Text = "";
    }
}