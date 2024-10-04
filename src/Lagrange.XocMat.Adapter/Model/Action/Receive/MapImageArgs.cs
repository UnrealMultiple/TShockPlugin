using MorMorAdapter.Enumerates;
using ProtoBuf;

namespace MorMorAdapter.Model.Action.Receive;

[ProtoContract]
public class MapImageArgs : BaseAction
{
    [ProtoMember(5)] public ImageType ImageType { get; set; }
}
