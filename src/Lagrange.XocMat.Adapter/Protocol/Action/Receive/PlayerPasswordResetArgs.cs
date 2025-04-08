using Lagrange.XocMat.Adapter.Protocol.Action;
using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Action.Receive;

[ProtoContract]
public class PlayerPasswordResetArgs : BaseAction
{
    [ProtoMember(5)] public string Name { get; set; } = "";

    [ProtoMember(6)] public string Password { get; set; } = "";
}
