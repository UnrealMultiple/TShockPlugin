using ProtoBuf;

namespace MorMorAdapter.Model.Action.Receive;

[ProtoContract]
public class ReStartServerArgs : BaseAction
{
    [ProtoMember(1)] public string StartArgs { get; set; }
}
