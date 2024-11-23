using Lagrange.XocMat.Adapter.Model.Internet;
using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Model.Action.Response;

[ProtoContract]
public class QueryAccount : BaseActionResponse
{
    [ProtoMember(8)] public List<Account> Accounts { get; set; } = new();
}
