using MorMorAdapter.Enumerates;
using MorMorAdapter.Model.Action.Receive;
using MorMorAdapter.Model.Action.Response;
using ProtoBuf;

namespace MorMorAdapter.Model.Action;

[ProtoContract]
[ProtoInclude(301, typeof(BroadcastArgs))]
[ProtoInclude(302, typeof(MapImageArgs))]
[ProtoInclude(303, typeof(QueryPlayerInventoryArgs))]
[ProtoInclude(304, typeof(RegisterAccountArgs))]
[ProtoInclude(305, typeof(ServerCommandArgs))]
[ProtoInclude(306, typeof(BaseActionResponse))]
[ProtoInclude(307, typeof(ResetServerArgs))]
[ProtoInclude(308, typeof(ReStartServerArgs))]
[ProtoInclude(309, typeof(PlayerPasswordResetArgs))]
[ProtoInclude(310, typeof(SocketConnectStatusArgs))]
[ProtoInclude(311, typeof(QueryAccountArgs))]
[ProtoInclude(312, typeof(ExportPlayerArgs))]

public class BaseAction : BaseMessage
{
    [ProtoMember(4)] public ActionType ActionType { get; set; }

    [ProtoMember(5)] public string Echo { get; set; } = "";
}
