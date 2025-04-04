using ProtoBuf;


namespace Lagrange.XocMat.Adapter.Protocol.Action.Response;

[ProtoContract]
public class MapImage : BaseActionResponse
{
    [ProtoMember(8)] public byte[] Buffer { get; set; }

    public MapImage(byte[] buffer) : base()
    {
        this.Buffer = buffer;
    }
}
