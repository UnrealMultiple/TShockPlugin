using Lagrange.XocMat.Adapter.Enumerates;
using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Model.ServerMessage;

[ProtoContract]
public class GameInitMessage : BaseMessage
{
    public GameInitMessage()
    {
        this.MessageType = PostMessageType.GamePostInit;
    }
}
