using Lagrange.XocMat.Adapter.Protocol.Internet;
using ProtoBuf;
namespace Lagrange.XocMat.Adapter.Protocol.Action.Response;

[ProtoContract]
public class PlayerInventory : BaseActionResponse
{
    [ProtoMember(8)] public PlayerData? PlayerData { get; set; }

    public PlayerInventory(PlayerData? data) : base()
    {
        this.PlayerData = data;
    }
}
