using Lagrange.XocMat.Adapter.Enumerates;
using Lagrange.XocMat.Adapter.Protocol.Action;
using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Action.Receive;

[ProtoContract]
public class MapImageArgs : BaseAction
{
    [ProtoMember(5)] public ImageType ImageType { get; set; }
}
