using MorMorAdapter.Model.Internet;
using ProtoBuf;

namespace MorMorAdapter.Model.Action.Response;

[ProtoContract]
public class QueryAccount : BaseActionResponse
{
    [ProtoMember(8)] public List<Account> Accounts { get; set; } = new();
}
