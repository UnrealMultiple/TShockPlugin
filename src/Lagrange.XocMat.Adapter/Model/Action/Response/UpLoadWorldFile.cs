using ProtoBuf;

namespace MorMorAdapter.Model.Action.Response;

[ProtoContract]
public class UpLoadWorldFile : BaseActionResponse
{
    [ProtoMember(8)] public string WorldName { get; set; }

    [ProtoMember(9)] public byte[] WorldBuffer { get; set; }
}
