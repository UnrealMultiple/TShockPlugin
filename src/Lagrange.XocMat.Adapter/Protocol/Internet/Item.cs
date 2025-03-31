using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Internet;


[ProtoContract]
public class Item
{
    [ProtoMember(1)] public int netID { get; set; }

    [ProtoMember(2)] public int prefix { get; set; }

    [ProtoMember(3)] public int stack { get; set; }

    public Item(int netID, int prefix, int stack)
    {
        this.netID = netID;
        this.prefix = prefix;
        this.stack = stack;
    }
}
