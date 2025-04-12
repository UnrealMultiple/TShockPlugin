using Lagrange.XocMat.Adapter.Protocol.Action;
using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Action.Receive;

[ProtoContract]
public class ResetServerArgs : BaseAction
{
    [ProtoMember(5)] public List<string> RestCommamd { get; set; } = new();

    [ProtoMember(6)] public string StartArgs { get; set; } = "";

    [ProtoMember(7)] public bool UseFile { get; set; }

    [ProtoMember(8)] public string FileName { get; set; } = "";

    [ProtoMember(9)] public byte[] FileBuffer { get; set; } = Array.Empty<byte>();
}
