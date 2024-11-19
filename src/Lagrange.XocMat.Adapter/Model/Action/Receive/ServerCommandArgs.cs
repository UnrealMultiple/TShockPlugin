using ProtoBuf;

namespace MorMorAdapter.Model.Action.Receive;

[ProtoContract]
public class ServerCommandArgs : BaseAction
{
    [ProtoMember(5)] public string Text { get; set; } = "";
}
