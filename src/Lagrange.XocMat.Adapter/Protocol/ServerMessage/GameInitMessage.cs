using Lagrange.XocMat.Adapter.Enumerates;
using Lagrange.XocMat.Adapter.Protocol;
using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.ServerMessage;

[ProtoContract]
public class GameInitMessage : BaseMessage
{
    public GameInitMessage()
    {
        this.MessageType = PostMessageType.GamePostInit;
    }
}
