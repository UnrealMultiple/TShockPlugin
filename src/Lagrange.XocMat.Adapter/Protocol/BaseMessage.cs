using Lagrange.XocMat.Adapter.Enumerates;
using Lagrange.XocMat.Adapter.Protocol.Action;
using Lagrange.XocMat.Adapter.Protocol.PlayerMessage;
using Lagrange.XocMat.Adapter.Protocol.ServerMessage;
using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol;

[ProtoContract]
[ProtoInclude(101, typeof(BasePlayerMessage))]
[ProtoInclude(102, typeof(BaseAction))]
[ProtoInclude(103, typeof(GameInitMessage))]
public class BaseMessage
{
    [ProtoMember(1)] public PostMessageType MessageType { get; set; }

    [ProtoMember(2)] public string ServerName { get; set; } = Plugin.Config.SocketConfig.ServerName;

    [ProtoMember(3)] public string Token { get; set; } = Plugin.Config.SocketConfig.Token;
}
