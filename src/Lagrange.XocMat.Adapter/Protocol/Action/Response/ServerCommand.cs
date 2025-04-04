using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Action.Response;

[ProtoContract]
public class ServerCommand : BaseActionResponse
{
    [ProtoMember(8)] public List<string> Params { get; set; }

    public ServerCommand(List<string> param) : base()
    {
        this.Params = param;
    }
}
