using Lagrange.XocMat.Adapter.Protocol.Action;
using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Action.Receive;

[ProtoContract]
public class ServerCommandArgs : BaseAction
{
    [ProtoMember(5)] public string Text { get; set; } = "";
}
