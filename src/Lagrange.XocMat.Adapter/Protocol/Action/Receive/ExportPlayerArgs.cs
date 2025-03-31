using Lagrange.XocMat.Adapter.Protocol.Action;
using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Action.Receive;

[ProtoContract]
public class ExportPlayerArgs : BaseAction
{
    [ProtoMember(5)] public List<string> Names { get; set; } = new();
}
