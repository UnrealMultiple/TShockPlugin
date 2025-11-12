using GhostView.Models;
using GhostView.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace GhostView.Service;

public class RespawnService
{
    private readonly RespawnCountdown _respawnCountdown = new ();
    private readonly HashSet<string> _reconnectPlayers = new ();

    public void MarkReconnect(string playerName)
    {
        this._reconnectPlayers.Add(playerName);
    }

    private void UnmarkReconnect(string playerName)
    {
        this._reconnectPlayers.Remove(playerName);
    }

    public bool IsReconnectPlayer(string playerName)
    {
        return this._reconnectPlayers.Contains(playerName);
    }

    public void SetGhost(string playerName, double? totalSeconds = null)
    {
        var player = TShock.Players.ToList().Find(p => p != null && p.Name == playerName);
        if (player?.TPlayer is null)
        {
            return;
        }

        player.TPlayer.ghost = true;
        if (this.IsReconnectPlayer(playerName))
        {
            player.TPlayer.active = false;
        }

        if (!this.IsReconnectPlayer(playerName))
        {
            var remaining = totalSeconds ?? this._respawnCountdown.GetRemainingSeconds(playerName);
            player.SendMessage(
                string.Format(GetString("你已死亡，剩余{0:F1}s后复活"), remaining),
                Color.Yellow
            );
        }

        TSPlayer.All.SendData(PacketTypes.PlayerUpdate, "", player.Index);
    }

    public void RevivePlayer(string playerName)
    {
        var player = TShock.Players.ToList().Find(p => p != null && p.Name == playerName);
        if (player?.TPlayer is null)
        {
            return;
        }

        player.TPlayer.ghost = false;
        player.TPlayer.active = true;
        TSPlayer.All.SendData(PacketTypes.PlayerUpdate, "", player.Index);
        BuffUtils.ClearDebuffs(player);
        TSPlayer.All.SendData(PacketTypes.PlayerBuff, "", player.Index);
        if (this.IsReconnectPlayer(playerName))
        {
            player.TPlayer.statLife = Math.Max(100, player.TPlayer.statLife / 2);
            player.SendData(PacketTypes.PlayerHp, "", player.Index);
        }

        this.UnmarkReconnect(playerName);
    }

    public void NotifyReconnect(string playerName, double remainSeconds)
    {
        var player = TShock.Players.ToList().Find(p => p != null && p.Name == playerName);

        player?.SendMessage(
            string.Format(GetString("你在离线期间仍未结束复活冷却，剩余{0:F1}s"), remainSeconds),
            Color.Yellow
        );
    }

    public void TeleportToSpawn(TSPlayer player)
    {
        if (player.TPlayer == null)
        {
            return;
        }

        var spawnX = player.TPlayer.SpawnX;
        var spawnY = player.TPlayer.SpawnY;

        if (spawnX < 0 || spawnY < 0)
        {
            spawnX = Main.spawnTileX;
            spawnY = Main.spawnTileY;
        }

        float worldX = (spawnX * 16) + 8;
        float worldY = (spawnY * 16) + 8;

        player.Teleport(worldX, worldY);
    }

    
}