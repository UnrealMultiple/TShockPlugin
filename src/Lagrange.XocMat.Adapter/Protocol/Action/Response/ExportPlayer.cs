using Lagrange.XocMat.Adapter.Protocol.Internet;
using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Action.Response;

[ProtoContract]
internal class ExportPlayer : BaseActionResponse
{
    [ProtoMember(8)] public List<PlayerFile> PlayerFiles { get; set; } = new();
}
