using Lagrange.XocMat.Adapter.Protocol.Internet;
using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Action.Response;

[ProtoContract]
public class PlayerStrikeBoss : BaseActionResponse
{
    [ProtoMember(8)] public List<KillNpc> Damages { get; set; } = new();
}
