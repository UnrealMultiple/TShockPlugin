using MorMorAdapter.Enumerates;
using ProtoBuf;

namespace MorMorAdapter.Model.ServerMessage;

[ProtoContract]
public class GameInitMessage : BaseMessage
{
    public GameInitMessage()
    {
        this.MessageType = PostMessageType.GamePostInit;
    }
}
