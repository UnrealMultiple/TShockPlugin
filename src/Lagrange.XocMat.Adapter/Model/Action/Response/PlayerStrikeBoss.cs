using MorMorAdapter.Model.Internet;
using ProtoBuf;

namespace MorMorAdapter.Model.Action.Response;

[ProtoContract]
public class PlayerStrikeBoss : BaseActionResponse
{
    [ProtoMember(8)] public List<KillNpc> Damages { get; set; }
}
