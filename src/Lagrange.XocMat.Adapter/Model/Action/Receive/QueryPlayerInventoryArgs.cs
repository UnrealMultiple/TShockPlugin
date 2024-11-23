using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Model.Action.Receive;

[ProtoContract]
public class QueryPlayerInventoryArgs : BaseAction
{
    [ProtoMember(5)] public string Name { get; set; } = "";
}
