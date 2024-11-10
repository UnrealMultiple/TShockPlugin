using ProtoBuf;

namespace MorMorAdapter.Model.Action.Response;

[ProtoContract]
public class DeadRank : BaseActionResponse
{
    [ProtoMember(8)] public Dictionary<string, int> Rank { get; set; }

    public DeadRank(Dictionary<string, int> rank) : base()
    {
        this.Rank = rank;
    }
}