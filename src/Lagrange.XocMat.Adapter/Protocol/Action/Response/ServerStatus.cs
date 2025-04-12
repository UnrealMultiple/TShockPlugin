using Lagrange.XocMat.Adapter.Protocol.Internet;
using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Action.Response;

[ProtoContract]

public class ServerStatus : BaseActionResponse
{
    [ProtoMember(8)] public string WorldName { get; set; } = "";

    [ProtoMember(9)] public int WorldWidth { get; set; }

    [ProtoMember(10)] public int WorldHeight { get; set; }

    [ProtoMember(11)] public int WorldMode { get; set; }

    [ProtoMember(12)] public int WorldID { get; set; }

    [ProtoMember(13)] public string WorldSeed { get; set; } = "";

    [ProtoMember(14)] public TimeSpan RunTime { get; set; }

    [ProtoMember(15)] public string TShockPath { get; set; } = "";

    [ProtoMember(16)] public List<PluginInfo> Plugins { get; set; } = new();
}
