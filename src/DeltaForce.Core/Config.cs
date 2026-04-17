using DeltaForce.Core.Enitys;
using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;
using Newtonsoft.Json;

namespace DeltaForce.Core;

[Config]
public class Config : JsonConfigBase<Config>
{
    protected override string Filename => "DeltaForce";

    [JsonProperty("match_seconds")]
    public int MatchSeconds { get; set; } = 60;


    [JsonProperty("game_server")]
    public ServerEnity GameServer { get; set; } = new();

    [JsonProperty("socket_server")]
    public ServerEnity Socket { get; set; } = new();

    [JsonProperty("delta_items")]
    public List<DeltaItemEnity> Items { get; set; } = [];
}
