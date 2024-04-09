using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
}
