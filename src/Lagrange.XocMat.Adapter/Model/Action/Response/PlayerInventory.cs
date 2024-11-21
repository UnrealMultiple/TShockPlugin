using Lagrange.XocMat.Adapter.Model.Internet;
using ProtoBuf;
namespace Lagrange.XocMat.Adapter.Model.Action.Response;

[ProtoContract]
public class PlayerInventory : BaseActionResponse
{
    [ProtoMember(8)] public PlayerData? PlayerData { get; set; }

    public PlayerInventory(PlayerData? data) : base()
    {
        this.PlayerData = data;
    }
}
