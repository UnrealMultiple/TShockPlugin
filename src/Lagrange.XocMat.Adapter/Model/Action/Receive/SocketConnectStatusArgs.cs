using Lagrange.XocMat.Adapter.Enumerates;
using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Model.Action.Receive;

[ProtoContract]
public class SocketConnectStatusArgs : BaseAction
{
    [ProtoMember(5)] public SocketConnentType Status { get; set; }
}
