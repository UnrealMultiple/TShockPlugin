using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Internet;

[ProtoContract]
public class PluginInfo
{
    [ProtoMember(1)] public string Name { get; set; } = "";

    [ProtoMember(2)] public string Author { get; set; } = "";

    [ProtoMember(3)] public string Description { get; set; } = "";
}
