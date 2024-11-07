using MorMorAdapter.Enumerates;
using ProtoBuf;

namespace MorMorAdapter.Model.Action.Receive;

[ProtoContract]
public class SocketConnectStatusArgs : BaseAction
{
    [ProtoMember(5)] public SocketConnentType Status { get; set; }
}
