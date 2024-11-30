using Lagrange.XocMat.Adapter.Model.Internet;
using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Model.Action.Response;

[ProtoContract]
public class PlayerStrikeBoss : BaseActionResponse
{
    [ProtoMember(8)] public List<KillNpc> Damages { get; set; } = new();
}
