using Newtonsoft.Json;
using TShockAPI;

namespace ChattyBridge.Message;

public class PlayerMessage
{
    [JsonProperty("server_name")]
    public string ServerName { get; set; } = Plugin.Config.ServerName;

    [JsonProperty("name")]
    public string Name { get; init; }

    [JsonProperty("prefix")]
    public string Prefix { get; init; }

    [JsonProperty("group")]
    public string Group { get; init; }

    [JsonProperty("rgb")]
    public byte[] RGB { get; init; } = new byte[3];

    [JsonProperty("type")]
    [JsonConverter(typeof(EnumConveter))]
    public MsgType Type { get; init; }

    public PlayerMessage(TSPlayer player)
    {
        Name = player.Name;
        Prefix = player.Group.Prefix;
        Group = player.Group.Name;
        RGB = new byte[3]
        {
            player.Group.R,
            player.Group.G,
            player.Group.B,
        };
    }
    public PlayerMessage()
    {

    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}
