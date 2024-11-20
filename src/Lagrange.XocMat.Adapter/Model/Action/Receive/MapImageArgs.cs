using Lagrange.XocMat.Adapter.Enumerates;
using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Model.Action.Receive;

[ProtoContract]
public class MapImageArgs : BaseAction
{
    [ProtoMember(5)] public ImageType ImageType { get; set; }
}
