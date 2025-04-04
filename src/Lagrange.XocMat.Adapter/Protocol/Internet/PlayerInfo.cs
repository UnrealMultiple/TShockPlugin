using ProtoBuf;
using TShockAPI;

namespace Lagrange.XocMat.Adapter.Protocol.Internet;

[ProtoContract]
public class PlayerInfo
{
    [ProtoMember(1)] public int Index { get; set; }

    [ProtoMember(2)] public string Name { get; set; }

    [ProtoMember(3)] public string Group { get; set; }

    [ProtoMember(4)] public string Prefix { get; set; }

    [ProtoMember(5)] public bool IsLogin { get; set; }

    public PlayerInfo(TSPlayer ply)
    {
        this.Index = ply.Index;
        this.Name = ply.Name;
        this.Group = ply.Group.Name;
        this.Prefix = ply.Group.Prefix;
        this.IsLogin = ply.IsLoggedIn;
    }
}
