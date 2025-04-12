using Lagrange.XocMat.Adapter.Protocol.Action;
using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Action.Receive;

[ProtoContract]
public class ReStartServerArgs : BaseAction
{
    [ProtoMember(1)] public string StartArgs { get; set; } = "";
}
