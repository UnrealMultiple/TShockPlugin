using DeltaForce.Protocol.Packets;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace DeltaForce.Game.Modules;

public static class SpawnManager
{
    private static readonly Random _random = new();
    private static readonly Dictionary<int, SpawnPoint> _assignedSpawnPoints = new();

    public static void AssignSpawnPointsAndTeleport(SquadInfo[] squads)
    {
        _assignedSpawnPoints.Clear();

        var availableSpawnPoints = Config.Instance.SpawnPoints.ToList();

        if (availableSpawnPoints.Count == 0)
        {
            Console.WriteLine("[SpawnManager] 警告: 配置文件中没有定义出生点");
            return;
        }

        foreach (var squad in squads)
        {
            if (squad.Members.Length == 0) continue;

            SpawnPoint? spawnPoint = null;

            var teamSpecificSpawn = availableSpawnPoints.FirstOrDefault(sp => sp.TeamId == squad.SquadId);
            if (teamSpecificSpawn != null)
            {
                spawnPoint = teamSpecificSpawn;
                availableSpawnPoints.Remove(teamSpecificSpawn);
            }
            else
            {
                var generalSpawns = availableSpawnPoints.Where(sp => sp.TeamId == null).ToList();
                if (generalSpawns.Count > 0)
                {
                    spawnPoint = generalSpawns[_random.Next(generalSpawns.Count)];
                    availableSpawnPoints.Remove(spawnPoint);
                }
                else if (availableSpawnPoints.Count > 0)
                {
                    spawnPoint = availableSpawnPoints[_random.Next(availableSpawnPoints.Count)];
                    availableSpawnPoints.Remove(spawnPoint);
                }
            }

            if (spawnPoint != null)
            {
                _assignedSpawnPoints[squad.SquadId] = spawnPoint;
                TeleportSquadToSpawn(squad, spawnPoint);
            }
        }
    }

    private static void TeleportSquadToSpawn(SquadInfo squad, SpawnPoint spawnPoint)
    {
        int range = Config.Instance.SpawnRange;

        foreach (var member in squad.Members)
        {
            var player = TShock.Players.FirstOrDefault(p => p?.Name == member.PlayerName && p?.Active == true);
            if (player == null) continue;

            int offsetX = _random.Next(-range, range + 1);
            int offsetY = _random.Next(-range, range + 1);

            int targetX = spawnPoint.X + offsetX;
            int targetY = spawnPoint.Y + offsetY;

            targetX = Math.Clamp(targetX, 1, Main.maxTilesX - 1);
            targetY = Math.Clamp(targetY, 1, Main.maxTilesY - 1);

            player.Teleport(targetX * 16, targetY * 16);
            player.SendSuccessMessage($"你已传送到出生点: {spawnPoint.Name}");

            Console.WriteLine($"[SpawnManager] 玩家 {member.PlayerName} (小队 {squad.SquadId}) 传送到 {spawnPoint.Name} ({targetX}, {targetY})");
        }
    }

    public static SpawnPoint? GetSquadSpawnPoint(int squadId)
    {
        return _assignedSpawnPoints.TryGetValue(squadId, out var spawnPoint) ? spawnPoint : null;
    }

    public static void ClearAssignments()
    {
        _assignedSpawnPoints.Clear();
    }
}
