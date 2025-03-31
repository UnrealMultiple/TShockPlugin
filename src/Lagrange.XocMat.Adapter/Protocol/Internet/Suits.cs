using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Internet;

[ProtoContract]
public class Suits
{
    [ProtoMember(1)] public Item[] armor { get; set; } = Array.Empty<Item>();
    //染料
    [ProtoMember(2)] public Item[] dye { get; set; } = Array.Empty<Item>();
}
