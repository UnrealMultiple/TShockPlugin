using DeltaForce.Core;
using DeltaForce.Protocol.Packets;

namespace DeltaForce.Core.Modules;

public class CommunicationManager
{
    public async static Task<GameStateResponsePacket?> StartGameASync()
    {
        var packet = new GameStatePacket()
        {
            IsGameActive = true,
        };
        return await DeltaAction.DeltaServer.RequestAsync<GameStatePacket, GameStateResponsePacket>(packet);
    }
}
