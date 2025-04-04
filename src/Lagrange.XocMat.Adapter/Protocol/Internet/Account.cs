using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Internet;

[ProtoContract]
public class Account
{
    [ProtoMember(1)] public string Name { get; set; } = "";

    [ProtoMember(2)] public string IP { get; set; } = "";

    [ProtoMember(3)] public int ID { get; set; }

    [ProtoMember(4)] public string Group { get; set; } = "";

    [ProtoMember(5)] public string UUID { get; set; } = "";

    [ProtoMember(6)] public string Password { get; set; } = "";

    [ProtoMember(7)] public string RegisterTime { get; set; } = "";

    [ProtoMember(8)] public string LastLoginTime { get; set; } = "";

}
