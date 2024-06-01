using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using System;
using System.Collections.Generic;

namespace Respawn
{
    [ApiVersion(2, 1)]
    public class Respawn : TerrariaPlugin
    {
        public override string Author => "leader，肝帝熙恩"; 
        public override string Description => "原地复活";
        public override string Name => "Respawn";
        public override Version Version => new Version(1, 0, 1); 
        private long TimerCount = 0;
        private Dictionary<int, (DateTime DeathTime, Vector2 DeathPosition)> playerDeathRecords = new Dictionary<int, (DateTime, Vector2)>();
        private int respawnTime;

        public Respawn(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            GetDataHandlers.KillMe.Register(OnKillMe);
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GetDataHandlers.KillMe.UnRegister(OnKillMe);
                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
            }
            base.Dispose(disposing);
        }

        private void OnKillMe(object? sender, GetDataHandlers.KillMeEventArgs e)
        {
            if (!playerDeathRecords.ContainsKey(e.Player.Index))
            {
                playerDeathRecords[e.Player.Index] = (DateTime.UtcNow, e.Player.TPlayer.position);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            TimerCount++;

            if (TimerCount % 60 == 0)
            {
                List<int> playersToRespawn = new List<int>();

                foreach (var kvp in playerDeathRecords)
                {
                    TSPlayer player = TShock.Players[kvp.Key];
                    var (deathTime, position) = kvp.Value;
                    respawnTime = TShock.Config.Settings.RespawnSeconds;
                    foreach (NPC npc in Main.npc)
                    {
                        if (npc.active && (npc.boss || npc.type == 13 || npc.type == 14 || npc.type == 15) &&
                            Math.Abs(player.TPlayer.Center.X - npc.Center.X) + Math.Abs(player.TPlayer.Center.Y - npc.Center.Y) < 4000f)
                        {
                            respawnTime = TShock.Config.Settings.RespawnBossSeconds;
                            break;
                        }
                    }
                    if (DateTime.UtcNow - deathTime > TimeSpan.FromSeconds(respawnTime))
                    {
                        playersToRespawn.Add(kvp.Key);

                        //player.Spawn(PlayerSpawnContext.ReviveFromDeath);
                        player.Teleport(position.X, position.Y, 1);
                    }
                }

                // 移除已复活玩家的记录，防止重复处理
                foreach (int playerId in playersToRespawn)
                {
                    playerDeathRecords.Remove(playerId);
                }
            }
        }
    }
}
