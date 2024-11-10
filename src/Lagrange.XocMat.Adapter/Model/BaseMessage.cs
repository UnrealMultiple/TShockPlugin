using MorMorAdapter.Enumerates;
using MorMorAdapter.Model.Action;
using MorMorAdapter.Model.PlayerMessage;
using MorMorAdapter.Model.ServerMessage;
using ProtoBuf;

namespace MorMorAdapter.Model;

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
