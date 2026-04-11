using DeltaForce.Core.Modules;
using DeltaForce.Protocol.Packets;
using DeltaForce.Protocol.Processing;

namespace DeltaForce.Core.Handlers;

public class SquadDataHandler : RequestHandlerBase<SquadDataRequestPacket, SquadDataResponsePacket>
{
    public override SquadDataResponsePacket Handle(SquadDataRequestPacket request)
    {
        var allSquads = SquadMatchManager.GetAllSquads();
        var squadInfos = new List<SquadInfo>();

        foreach (var squad in allSquads)
        {
            var members = squad.Value.Members.Select(m => new SquadMemberInfo
            {
                PlayerName = m.Name,
                IsOnline = m.Active
            }).ToArray();

            squadInfos.Add(new SquadInfo
            {
                SquadId = (int)squad.Key,
                Members = members,
                MaxMembers = SquadMatchManager.MaxSquadMembers,
                CurrentMemberCount = members.Length
            });
        }

        var response = CreateSuccessResponse(request, "Success");
        response.Squads = [.. squadInfos];

        return response;
    }
}
