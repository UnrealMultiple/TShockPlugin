using DeltaForce.Game.Enitys;
using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;
using Newtonsoft.Json;

namespace DeltaForce.Game;

[Config]
public class Config : JsonConfigBase<Config>
{
    protected override string Filename => "DeltaGame";

    [JsonProperty("socket_server")]
    public SocketOption SocketOption { get; set; } = new();

    [JsonProperty("core_server")]
    public SocketOption CoreServer { get; set; } = new();

    [JsonProperty("match_minute")]
    public int MatchMinute { get; set; } = 10;

    [JsonProperty("ready_second")]
    public int ReadySecond { get; set; } = 30;

    [JsonProperty("spawn_points")]
    public List<SpawnPoint> SpawnPoints { get; set; } = [];

    [JsonProperty("spawn_range")]
    public int SpawnRange { get; set; } = 40;

    [JsonProperty("evacuation_points")]
    public List<EvacuationPoint> EvacuationPoints { get; set; } = [];

    [JsonProperty("evacuation_time_seconds")]
    public int EvacuationTimeSeconds { get; set; } = 10;
}

public class SpawnPoint
{
    [JsonProperty("name")]
    public string Name { get; set; } = "出生点";

    [JsonProperty("x")]
    public int X { get; set; }

    [JsonProperty("y")]
    public int Y { get; set; }

    [JsonProperty("team_id")]
    public int? TeamId { get; set; }
}

public class EvacuationPoint
{
    [JsonProperty("name")]
    public string Name { get; set; } = "撤离点";

    [JsonProperty("x")]
    public int X { get; set; }

    [JsonProperty("y")]
    public int Y { get; set; }

    [JsonProperty("radius")]
    public int Radius { get; set; } = 5;

    [JsonProperty("is_active")]
    public bool IsActive { get; set; } = true;
}
