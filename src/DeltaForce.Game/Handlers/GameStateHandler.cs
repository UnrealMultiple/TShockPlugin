using DeltaForce.Game.Modules;
using DeltaForce.Protocol.Packets;
using DeltaForce.Protocol.Processing;

namespace DeltaForce.Game.Handlers;

public class GameStateHandler : RequestHandlerBase<GameStatePacket, GameStateResponsePacket>
{
    public override GameStateResponsePacket Handle(GameStatePacket packet)
    {
        bool isGameStarting = packet.IsGameActive && !GameManager._isGame;

        if (isGameStarting)
        {
            ChestLootManager.RequestAndDistributeItemsAsync().GetAwaiter().GetResult();
            var squ = CommunicationManager.GetSquadData().Result;
            if (squ != null)
            {
                GameManager.squadInfo = squ.Squads;

                SpawnManager.AssignSpawnPointsAndTeleport(squ.Squads);
            }

            EvacuationManager.ResetAll();
            EvacuationManager.Initialize();
        }
        GameManager._isGame = packet.IsGameActive;

        return new GameStateResponsePacket
        {
            RequestId = packet.RequestId,
            Success = true,
            Message = "游戏状态处理成功",
            IsGameActive = GameManager._isGame,
            GameMode = packet.GameMode,
            GameTime = packet.GameTime
        };
    }
}
