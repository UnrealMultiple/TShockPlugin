using Lagrange.XocMat.Adapter.Protocol.Internet;
using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Action.Response;

[ProtoContract]
public class ServerOnline : BaseActionResponse
{
    [ProtoMember(8)] public List<PlayerInfo> Players { get; set; }

    [ProtoMember(9)] public int MaxCount { get; set; } = TShockAPI.TShock.Config.Settings.MaxSlots;

    [ProtoMember(10)] public int OnlineCount { get; set; }

    public ServerOnline(List<PlayerInfo> players) : base()
    {
        this.Players = players;
        this.OnlineCount = players.Count;
    }
}
