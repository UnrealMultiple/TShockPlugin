using Lagrange.XocMat.Adapter.Model.Internet;
using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Model.Action.Response;

[ProtoContract]
internal class ExportPlayer : BaseActionResponse
{
    [ProtoMember(8)] public List<PlayerFile> PlayerFiles { get; set; } = new();
}
