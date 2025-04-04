using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Action.Receive;

[ProtoContract]
public class PrivatMsgArgs : BroadcastArgs
{
    [ProtoMember(7)] public string Name { get; set; } = "";
}
