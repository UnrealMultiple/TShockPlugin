using GhostView.Models;
using GhostView.Service;
using GhostView.Utils;
using TShockAPI;

namespace GhostView.Managers;

public class RespawnManager(RespawnService respawnService, RespawnCountdown respawnCountdown)
{
    public void OnPlayerSpawn(string playerName)
    {
        var player = TShock.Players.ToList().Find(p => p != null && p.Name == playerName);
        if (player?.TPlayer is null)
        {
            return;
        }

        if (respawnService.IsReconnectPlayer(playerName))
        {
            respawnService.SetGhost(playerName);
        }
        else if (player.TPlayer.dead)
        {
            respawnService.RevivePlayer(playerName);
            respawnService.TeleportToSpawn(player);
        }
    }

    public void OnLeave(string playerName)
    {
        if (!respawnCountdown.IsFinished(playerName))
        {
            respawnService.MarkReconnect(playerName);
        }
    }

    public void OnGreetPlayer(string playerName)
    {
        var player = TShock.Players.ToList().Find(p => p != null && p.Name == playerName);
        if (!respawnCountdown.IsFinished(playerName))
        {
            respawnService.SetGhost(playerName);
            var remain = respawnCountdown.GetRemainingSeconds(playerName);
            respawnService.NotifyReconnect(playerName, remain);
        }
        else if (player?.TPlayer?.dead == true)
        {
            respawnService.RevivePlayer(playerName);
        }
    }

    public void OnKillMe(string playerName)
    {
        var player = TShock.Players.ToList().Find(p => p != null && p.Name == playerName);
        if (player is null)
        {
            return;
        }
        
        var isBossAlive = BossCheckUtil.IsBossNearPlayer(player);

        var totalSeconds = respawnCountdown.StartCountdown(
            playerName,
            isBossAlive,
            respawnService.RevivePlayer
        );
        respawnService.SetGhost(playerName, totalSeconds);
    }
}