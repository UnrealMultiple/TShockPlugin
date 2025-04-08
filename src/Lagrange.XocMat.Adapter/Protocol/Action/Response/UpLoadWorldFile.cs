using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Action.Response;

[ProtoContract]
public class UpLoadWorldFile : BaseActionResponse
{
    [ProtoMember(8)] public string WorldName { get; set; } = "";

    [ProtoMember(9)] public byte[] WorldBuffer { get; set; } = Array.Empty<byte>();
}
