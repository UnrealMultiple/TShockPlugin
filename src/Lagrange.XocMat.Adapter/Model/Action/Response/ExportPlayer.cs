using MorMorAdapter.Model.Internet;
using ProtoBuf;

namespace MorMorAdapter.Model.Action.Response;

[ProtoContract]
internal class ExportPlayer : BaseActionResponse
{
    [ProtoMember(8)] public List<PlayerFile> PlayerFiles { get; set; } = new();
}
