using Lagrange.XocMat.Adapter.Protocol.Action;
using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Action.Receive;

[ProtoContract]
public class QueryAccountArgs : BaseAction
{
    [ProtoMember(5)] public string? Target { get; set; }
}
