using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Internet;

[ProtoContract]
public class PlayerFile
{
    [ProtoMember(1)] public byte[] Buffer { get; set; } = Array.Empty<byte>();

    [ProtoMember(2)] public string Name { get; set; } = string.Empty;

    [ProtoMember(3)] public bool Active { get; set; } = true;
}
