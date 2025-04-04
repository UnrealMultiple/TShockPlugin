using Lagrange.XocMat.Adapter.Protocol.Action;
using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Action.Receive;

[ProtoContract]
public class QueryPlayerInventoryArgs : BaseAction
{
    [ProtoMember(5)] public string Name { get; set; } = "";
}
