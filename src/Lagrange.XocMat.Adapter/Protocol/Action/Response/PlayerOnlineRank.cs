using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Action.Response;

[ProtoContract]
public class PlayerOnlineRank : BaseActionResponse
{
    [ProtoMember(8)] public Dictionary<string, int> OnlineRank { get; set; }

    public PlayerOnlineRank(Dictionary<string, int> rank) : base()
    {
        this.OnlineRank = rank;
    }
}
