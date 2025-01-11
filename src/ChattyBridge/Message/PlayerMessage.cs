using Newtonsoft.Json;
using TShockAPI;

namespace ChattyBridge.Message;

public class PlayerMessage
{
    [JsonProperty("server_name")]
    public string ServerName { get; set; } = Config.Instance.ServerName;

    [JsonProperty("name")]
    public string Name { get; init; }

    [JsonProperty("prefix")]
    public string Prefix { get; init; }

    [JsonProperty("group")]
    public string Group { get; init; }

    [JsonProperty("rgb")]
    public byte[] RGB { get; init; } = new byte[3];

    [JsonProperty("type")]
    [JsonConverter(typeof(EnumConverter))]
    public MsgType Type { get; init; }

    public PlayerMessage(TSPlayer player)
    {
        this.Name = player.Name;
        this.Prefix = player.Group.Prefix;
        this.Group = player.Group.Name;
        this.RGB = new byte[3]
        {
            player.Group.R,
            player.Group.G,
            player.Group.B,
        };
    }
    public PlayerMessage()
    {
        this.Name = "";
        this.Prefix = "";
        this.Group = "";
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}