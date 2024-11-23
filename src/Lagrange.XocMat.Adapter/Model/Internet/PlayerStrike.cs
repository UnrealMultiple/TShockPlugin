using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Model.Internet;

[ProtoContract]
public class PlayerStrike
{
    [ProtoMember(1)] public string Player { get; set; } = "";

    [ProtoMember(2)] public int Damage { get; set; }
}
