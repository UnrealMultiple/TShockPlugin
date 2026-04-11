using LazyAPI.Utility;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace DeltaForce.Game.Modules;

public static class EvacuationManager
{
    private static readonly Dictionary<string, EvacuationProgress> _evacuationProgress = new();
    private static readonly Random _random = new();

    public static void Initialize()
    {
        if (Config.Instance.EvacuationPoints.Count == 0)
        {
            Console.WriteLine(GetString("[EvacuationManager] 警告: 配置文件中没有定义撤离点"));
        }
    }

    public static void Update()
    {
        if (!GameManager._isGame) return;

        var activePlayers = TShock.Players.Where(p => p?.Active == true && !p.TPlayer.ghost).ToList();

        foreach (var player in activePlayers)
        {
            CheckPlayerEvacuation(player);
        }

        UpdateEvacuationProgress();
    }

    private static void CheckPlayerEvacuation(TSPlayer player)
    {
        var evacuationPoint = GetNearbyEvacuationPoint(player);
        if (evacuationPoint == null)
        {
            ResetPlayerEvacuation(player.Name);
            return;
        }

        if (!_evacuationProgress.TryGetValue(player.Name, out var progress))
        {
            progress = new EvacuationProgress
            {
                PlayerName = player.Name,
                EvacuationPoint = evacuationPoint,
                StartTime = DateTime.Now
            };
            _evacuationProgress[player.Name] = progress;
            player.SendInfoMessage(GetString($"开始撤离... 请保持在撤离点范围内 {Config.Instance.EvacuationTimeSeconds} 秒"));
        }

        if (!IsPlayerInEvacuationPoint(player, evacuationPoint))
        {
            ResetPlayerEvacuation(player.Name);
            player.SendErrorMessage(GetString("你已离开撤离点，撤离取消！"));
            return;
        }

        var elapsedSeconds = (DateTime.Now - progress.StartTime).TotalSeconds;
        var remainingSeconds = Config.Instance.EvacuationTimeSeconds - (int)elapsedSeconds;

        if (remainingSeconds > 0 && remainingSeconds != progress.LastNotifiedSecond)
        {
            progress.LastNotifiedSecond = remainingSeconds;
            player.SendInfoMessage(GetString($"撤离倒计时: {remainingSeconds} 秒"));
        }

        if (elapsedSeconds >= Config.Instance.EvacuationTimeSeconds)
        {
            CompleteEvacuation(player, evacuationPoint);
        }
    }

    private static void UpdateEvacuationProgress()
    {
        var playersToRemove = _evacuationProgress.Keys
            .Where(name => TShock.Players.All(p => p?.Name != name || p?.Active != true))
            .ToList();

        foreach (var playerName in playersToRemove)
        {
            _evacuationProgress.Remove(playerName);
        }
    }

    private static EvacuationPoint? GetNearbyEvacuationPoint(TSPlayer player)
    {
        var playerTileX = (int)(player.X / 16);
        var playerTileY = (int)(player.Y / 16);

        return Config.Instance.EvacuationPoints
            .Where(ep => ep.IsActive)
            .FirstOrDefault(ep =>
            {
                var distance = Math.Sqrt(
                    Math.Pow(ep.X - playerTileX, 2) +
                    Math.Pow(ep.Y - playerTileY, 2)
                );
                return distance <= ep.Radius;
            });
    }

    private static bool IsPlayerInEvacuationPoint(TSPlayer player, EvacuationPoint point)
    {
        var playerTileX = (int)(player.X / 16);
        var playerTileY = (int)(player.Y / 16);

        var distance = Math.Sqrt(
            Math.Pow(point.X - playerTileX, 2) +
            Math.Pow(point.Y - playerTileY, 2)
        );

        return distance <= point.Radius;
    }

    private static void CompleteEvacuation(TSPlayer player, EvacuationPoint point)
    {
        ResetPlayerEvacuation(player.Name);

        player.SendSuccessMessage(GetString($"撤离成功！你从 {point.Name} 成功撤离！"));
        Console.WriteLine(GetString($"[EvacuationManager] 玩家 {player.Name} 从 {point.Name} 成功撤离"));

        SavePlayerInventoryAndTeleport(player);
    }

    private static void SavePlayerInventoryAndTeleport(TSPlayer player)
    {
        try
        {
            var response = InventoryManager.SavePlayerInventoryAsync(player).GetAwaiter().GetResult();

            if (response?.Success == true)
            {
                player.SendSuccessMessage(GetString("装备已保存，正在返回特勤处..."));
                Console.WriteLine(GetString($"[EvacuationManager] 玩家 {player.Name} 装备保存成功"));
            }
            else
            {
                player.SendWarningMessage(GetString("装备保存可能失败，但仍将返回特勤处"));
                Console.WriteLine(GetString($"[EvacuationManager] 玩家 {player.Name} 装备保存失败: {response?.Message}"));
            }

            DimensionsSender.SendPlayerToCustomServer(
                player,
                Config.Instance.CoreServer.Address,
                (ushort)Config.Instance.CoreServer.Port
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(GetString($"[EvacuationManager] 保存玩家 {player.Name} 装备失败: {ex.Message}"));
            player.SendErrorMessage(GetString("装备保存失败，但你可以返回特勤处"));

            DimensionsSender.SendPlayerToCustomServer(
                player,
                Config.Instance.CoreServer.Address,
                (ushort)Config.Instance.CoreServer.Port
            );
        }
    }

    private static void ResetPlayerEvacuation(string playerName)
    {
        _evacuationProgress.Remove(playerName);
    }

    public static void ResetAll()
    {
        _evacuationProgress.Clear();
    }

    public static IReadOnlyList<EvacuationPoint> GetActiveEvacuationPoints()
    {
        return Config.Instance.EvacuationPoints.Where(ep => ep.IsActive).ToList();
    }
}

public class EvacuationProgress
{
    public string PlayerName { get; set; } = string.Empty;
    public EvacuationPoint EvacuationPoint { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public int LastNotifiedSecond { get; set; } = -1;
}
