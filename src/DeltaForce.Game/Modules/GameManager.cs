using Microsoft.Xna.Framework;
using DeltaForce.Protocol.Packets;
using LazyAPI.Utility;
using TShockAPI;

namespace DeltaForce.Game.Modules;

public class GameManager
{
    private static int _gameDuration;

    internal static bool _isGame;

    internal static SquadInfo[]? squadInfo;

    private static int _lastNotifiedMinute = -1;

    public static int GetPlayerTeam(TSPlayer player)
    {
        if(squadInfo != null)
        {
            return squadInfo.FirstOrDefault(x => x.Members.Any(f => f.PlayerName == player.Name))?.SquadId ?? 0;
        }
        return 0;
    }

    public static int GetGameDuration()
    {
        return _gameDuration;
    }

    public static void GameLoop()
    {
        if(!_isGame)
            return;

        _gameDuration++;

        EvacuationManager.Update();

        CheckAllPlayersLeft();

        SendTimeRemainingNotification();

        if(_gameDuration >= Config.Instance.MatchMinute * 60)
        {
            EndGame();
        }
    }

    private static void SendTimeRemainingNotification()
    {
        var totalSeconds = Config.Instance.MatchMinute * 60;
        var remainingSeconds = totalSeconds - _gameDuration;
        var remainingMinutes = remainingSeconds / 60;

        if (remainingSeconds <= 10 && remainingSeconds > 0)
        {
            if (remainingSeconds != _lastNotifiedMinute)
            {
                _lastNotifiedMinute = remainingSeconds;
                BroadcastToActivePlayers(GetString($"[游戏结束倒计时] 还有 {remainingSeconds} 秒！"), Color.OrangeRed);
            }
        }
        else if (remainingSeconds == 30 && remainingSeconds != _lastNotifiedMinute)
        {
            _lastNotifiedMinute = remainingSeconds;
            BroadcastToActivePlayers(GetString("[游戏提示] 还有 30 秒游戏结束！请尽快撤离！"), Color.Yellow);
        }
        else if (remainingSeconds == 60 && remainingSeconds != _lastNotifiedMinute)
        {
            _lastNotifiedMinute = remainingSeconds;
            BroadcastToActivePlayers(GetString("[游戏提示] 还有 1 分钟游戏结束！"), Color.Yellow);
        }
        else if (remainingMinutes == 5 && remainingMinutes != _lastNotifiedMinute && remainingSeconds % 60 == 0)
        {
            _lastNotifiedMinute = remainingMinutes;
            BroadcastToActivePlayers(GetString("[游戏提示] 还有 5 分钟游戏结束！"), Color.LightGreen);
        }
        else if (remainingMinutes == 3 && remainingMinutes != _lastNotifiedMinute && remainingSeconds % 60 == 0)
        {
            _lastNotifiedMinute = remainingMinutes;
            BroadcastToActivePlayers(GetString("[游戏提示] 还有 3 分钟游戏结束！"), Color.LightGreen);
        }
        else if (remainingMinutes == 1 && remainingMinutes != _lastNotifiedMinute && remainingSeconds % 60 == 0)
        {
            _lastNotifiedMinute = remainingMinutes;
            BroadcastToActivePlayers(GetString("[游戏提示] 还有 1 分钟游戏结束！请尽快撤离！"), Color.Orange);
        }
    }

    private static void BroadcastToActivePlayers(string message, Color color)
    {
        foreach (var player in TShock.Players)
        {
            if (player?.Active == true && !player.TPlayer.ghost)
            {
                player.SendMessage(message, color);
            }
        }

        TShock.Log.ConsoleInfo(GetString($"[GameManager] {message}"));
    }

    public static void CheckAllPlayersLeft()
    {
        var activePlayers = TShock.Players.Where(p => p?.Active == true).ToList();

        if (activePlayers.Count == 0 && _isGame)
        {
            TShock.Log.ConsoleInfo(GetString("[GameManager] 所有玩家已离开服务器，重置游戏状态"));
            ResetGameState();
        }
    }

    public static void ResetGameState()
    {
        _isGame = false;
        _gameDuration = 0;
        _lastNotifiedMinute = -1;
        squadInfo = null;

        EvacuationManager.ResetAll();
        SpawnManager.ClearAssignments();
        TShock.Log.ConsoleInfo(GetString("[GameManager] 游戏状态已重置，等待下一局游戏开始"));
    }

    private static void EndGame()
    {
        _isGame = false;
        _gameDuration = 0;
        _lastNotifiedMinute = -1;
        EvacuationManager.ResetAll();
        SpawnManager.ClearAssignments();

        foreach(var player in TShock.Players)
        {
            if(player != null && player.Active)
            {
                player.SendErrorMessage(GetString("游戏结束了！"));
                player.SendErrorMessage(GetString("你未能撤离成功！已丢失所有装备物品！"));
                GameUitls.ClearPlayerInventory(player, false);
                DimensionsSender.SendPlayerToCustomServer(player, Config.Instance.CoreServer.Address, (ushort)Config.Instance.CoreServer.Port);
            }
        }
    }
}
