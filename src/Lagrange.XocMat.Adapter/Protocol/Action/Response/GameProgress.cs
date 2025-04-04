using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Action.Response;

[ProtoContract]
public class GameProgress : BaseActionResponse
{
    [ProtoMember(8)] public Dictionary<string, bool> Progress { get; set; }

    public GameProgress(Dictionary<string, bool> prog) : base()
    {
        this.Progress = prog;
    }
}
