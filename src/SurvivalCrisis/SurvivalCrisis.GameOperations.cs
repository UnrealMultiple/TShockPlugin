using Microsoft.Xna.Framework;
using SurvivalCrisis.SpecialEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TShockAPI;

namespace SurvivalCrisis
{
    public partial class SurvivalCrisis
    {
        internal class GameOperations
        {
            private static SurvivalCrisis Game => Instance;

            private static int CalcTraitorsCount(int participantsCount)
            {
                int traitorCount;
                if (participantsCount == 1)
                {
                    traitorCount = 0;
                }
                else if (participantsCount < 4)
                {
                    traitorCount = 1;
                }
                else
                {
                    traitorCount = participantsCount / 4;
                }
                return traitorCount;
            }
            #region GameOperations
            public static void TogglePvp(bool pvp)
            {
                foreach (var player in Game.Players)
                {
                    if (player != null)
                    {
                        if (player.Identity == PlayerIdentity.Watcher)
                        {
                            player.Pvp = false;
                        }
                        else
                        {
                            player.Pvp = pvp;
                        }
                    }
                }
            }
            public static void GhostsActive()
            {
                foreach (var player in Game.Players)
                {
                    if (player != null)
                    {
                        player.IsGhost = false;
                    }
                }
            }
            public static void GameReset()
            {
                TSPlayer.Server.SetTime(false, 0);

                Game.FinalBossType = NPCID.SkeletronPrime;//Rand.Next(Game.FinalBossTypes);
                Game.GameTime = 0;
                Game.IsFinalBattleTime = false;
                Game.FinalBossIndex = null;
                Game.FinalBossLifeMax = null;
                Game.FinalBossLifeRest = null;
                Game.IsInGame = true;
                #region ??
                Game.Participants = Game.Players.Where(player => player != null && Regions.WaitingZone.InRange(player)).ToArray();
                Game.TraitorEMPTime = 0;
                Game.ChestUnlocked = 0;
                Game.EnabledGlobalChat = false;
                Game.IsLongLongDark = false;
                Game.IsMagnetStorm = false;
                Game.IsFarce = false;
                Game.TraitorShop.Reset();
                Array.Clear(Game.ChestOpened, 0, Game.ChestOpened.Length);
                #endregion
                #region Players
                IdentifyPlayers();
                foreach (var player in Game.Players)
                {
                    if (player == null)
                    {
                        continue;
                    }
                    if (player.Identity != PlayerIdentity.Watcher)
                    {
                        player.Reset();
                        player.HideName();
                    }
                }
                #endregion
                #region Events
                var events = new SpecialEvent[]
                {
                    new BunnyTime(),
                    new BunnyRevenge(),
                    new FatalBlizzard(),
                    new LongLongDark(),
                    new MagnetStorm(),
                    new NightMare(),
                    new PainOfRelief()
                };
                Rand.Shuffle(events);
                Game.Events = new SpecialEvent[]
                {
                    new FinalEvent(),
                    events[0],
                    events[1],
                    events[2]
                };
                foreach (var @event in Game.Events)
                {
                    @event.Reset();
                }
                events[0].StartDelay = SunsetToMidnight;
                events[1].StartDelay = SunsetToMidnight + 7 * 60 * 60;
                events[2].StartDelay = SunsetToMidnight + 15 * 60 * 60;
                #endregion
                #region Effects
                ClearEffects();
                #endregion
            }
            public static void GameStart()
            {
                GameReset();
                TeleportPlayers(Regions.Hall.Center);
                TogglePvp(false);
                Game.CurrentTask = new SurvivorTask(1);
                Game.BCToAll(Game.CurrentTask.Text, Color.White);
                Game.BCToAll(Texts.TipsWhenStart, Color.White);
            }

            public static void GameEnd(PlayerIdentity winner)
            {
                GhostsActive();
                TogglePvp(false);
                CleanField();
                CountScore(winner);
                BroadcastResults(winner);
                Game.IsInGame = false;
                Game.Participants = null;
                Game.gameStartDelay = null;
                TeleportPlayers(new Point(Main.spawnTileX, Main.spawnTileY));
                foreach (var player in Game.Players)
                {
                    if (player != null)
                    {
                        player.OnGameEnd();
                        NetMessage.TrySendData((int)PacketTypes.PlayerInfo, -1, player.Index, null, player.Index);
                    }
                }
                Commands.HandleCommand(TSPlayer.Server, "/resetplayers all");
            }
            public static void GameUpdate()
            {
                #region Count Survivors
                Game.SurvivorCounts = Game.Players.Count(player => player?.Identity == PlayerIdentity.Survivor);
                if (Game.SurvivorCounts < 1)
                {
                    GameEnd(PlayerIdentity.Traitor);
                    return;
                }
                #endregion
                Game.GameTime++;
                if (Game.GameTime == PeaceTime)
                {
                    Game.BCToAll(Texts.PvpOn, Color.CornflowerBlue);
                    TogglePvp(true);
                }
                UpdateChatInterference();
                UpdateSpecialEvents();
                UpdateEffects();
                if (Game.GameTime % 70 == 0)
                {
                    if (Rand.NextDouble() < 0.35)
                    {
                        SpawnEnemiesUpdate();
                    }
                }
                if (Game.GameTime % 60 == 0)
                {
                    int timeToNextEvent = -1;
                    if (Game.GameTime < EventTime1)
                    {
                        timeToNextEvent = EventTime1 - Game.GameTime;
                    }
                    else if (Game.GameTime < EventTime2)
                    {
                        timeToNextEvent = EventTime2 - Game.GameTime;
                    }
                    else if (Game.GameTime < EventTime3)
                    {
                        timeToNextEvent = EventTime3 - Game.GameTime;
                    }
                    if (timeToNextEvent >= 0)
                    {
                        var msg = $"{timeToNextEvent / 60}s后将发生随机事件";
                        foreach (var player in Game.Players)
                        {
                            if (player?.IsGuest != false)
                            {
                                continue;
                            }
                            player.AddStatusMessage(msg);
                        }
                    }
                    if (Game.GameTime % 1800 == 0)
                    {
                        foreach (var player in Game.Players)
                        {
                            if (player?.IsGuest != false || player.Identity == PlayerIdentity.Watcher)
                            {
                                continue;
                            }
                            player.HideName();
                        }
                    }
                }
            }


            public static void TeleportPlayers(Point point)
            {
                foreach (var player in Game.Players)
                {
                    if (player?.IsGuest != false)
                    {
                        continue;
                    }
                    player.TeleportTo(point);
                }
            }

            public static void IdentifyPlayers()
            {
                var actives = Game.Players.Where(player => player != null && !player.IsGuest && Regions.WaitingZone.InRange(player)).ToArray();
                int count = actives.Length;
                int traitorCount = CalcTraitorsCount(count);
                #region Select Traitor
                bool[] selected = new bool[count];
                for (int i = 0; i < traitorCount; i++)
                {
                    int t;
                    do
                    {
                        t = Rand.Next(count);
                    }
                    while (selected[t]);
                    selected[t] = true;
                    actives[t].Identity = PlayerIdentity.Traitor;
                    actives[t].Party = PlayerIdentity.Traitor;
                    actives[t].Data.TraitorDatas.GameCounts++;
                }
                #endregion
                #region Select Survivor
                foreach (var player in actives)
                {
                    if (player.Identity == PlayerIdentity.Watcher)
                    {
                        player.Identity = PlayerIdentity.Survivor;
                        player.Party = PlayerIdentity.Survivor;
                        player.Data.SurvivorDatas.GameCounts++;
                    }
                }
                #endregion
            }

            public static void SpawnFinalBoss()
            {
                var pos = Regions.Hall.Center;
                Game.IsFinalBattleTime = true;
                Game.FinalBossIndex = NPC.NewNPC(new EntitySource_DebugCommand(), pos.X * 16, pos.Y * 16, Game.FinalBossType);
                Game.FinalBoss.lifeMax /= 4;
                Game.FinalBoss.life = Game.FinalBoss.lifeMax;
                Game.FinalBoss.defDefense = 0;
                Game.FinalBoss.defense = 0;

                Game.FinalBossLifeMax = Game.FinalBoss.lifeMax;
                Game.FinalBossLifeRest = Game.FinalBoss.lifeMax;

                TSPlayer.All.SendData(PacketTypes.NpcUpdate, string.Empty, (int)Game.FinalBossIndex);
            }

            public static void CleanField()
            {
                Game.GroundItems.Clear();
                // Regions.GamingZone.TurnToAir();
                Regions.GamingZone.UpdateToPlayer();
            }

            public static void BroadcastResults(PlayerIdentity winner)
            {
                string message = winner switch
                {
                    PlayerIdentity.Survivor => Texts.SurvivorsWin,
                    PlayerIdentity.Traitor => Texts.TraitorsWin,
                    _ => Texts.NobodyWin
                };
                Game.BCToAll(message, Color.BlueViolet);
                var tillParticipants = Game.Participants;
                var deathTimeRank = "最速死亡: " + string.Join("    ", tillParticipants.Where(p => p.Identity != p.Party).OrderBy(p => p.SurvivedFrames).Select(p => $"{p.Data.Name}({p.Index}号): {p.SurvivedFrames / 60}s"));
                var deathCountRank = "死亡榜: " + string.Join("    ", tillParticipants.Where(p => p.Identity != p.Party).OrderBy(p => p.KilledCount).Select(p => $"{p.Data.Name}({p.Index}号): {p.KilledCount}次"));
                var killingRank = "屠夫榜: " + string.Join("    ", tillParticipants.OrderBy(p => -p.KillingCount).Select(p => $"{p.Data.Name}({p.Index}号): {p.KillingCount}杀"));
                var chestScoutingRank = "开箱榜: " + string.Join("    ", tillParticipants.OrderBy(p => -p.ChestsOpened).Select(p => $"{p.Data.Name}({p.Index}号): {p.ChestsOpened}个"));



                Game.BCToAll(deathTimeRank, Color.Green);
                Game.BCToAll(deathCountRank, Color.Green);
                Game.BCToAll(killingRank, Color.Green);
                Game.BCToAll(chestScoutingRank, Color.Green);

                var traitorList = tillParticipants.Where(p => p.Party == PlayerIdentity.Traitor).Select(p => $"{p.Data.Name}({p.Index}号)");
                var survivorList = tillParticipants.Where(p => p.Party == PlayerIdentity.Survivor).Select(p => $"{p.Data.Name}({p.Index}号)");

                Game.BCToAll(@$"背叛者们: {string.Join(", ", traitorList)}", Color.Red);
                Game.BCToAll(@$"生存者们: {string.Join(", ", survivorList)}", Color.SkyBlue);
#if false
                var traitorRank = Game.DataBase.GetTraitorRank();
                var survivorRank = Game.DataBase.GetSurvivorRank();

                foreach (var player in Game.Participants)
                {
                    if (player?.TSPlayer == null)
                    {
                        continue;
                    }
                    player.SendText($"当前排名(生存者): {survivorRank.IndexOf(player.Data) + 1}", Color.Blue);
                    player.SendText($"当前排名(背叛者): {traitorRank.IndexOf(player.Data) + 1}", Color.Blue);
                }
                TSPlayer.All.SendInfoMessage($"生存者排行:");
                for (int i = 0; i < 10 && i < survivorRank.Count; i++)
                {
                    TSPlayer.All.SendInfoMessage($"第{i + 1}名: {survivorRank[i].Name}({survivorRank[i].SurvivorDatas.TotalScore})");
                }
                TSPlayer.All.SendInfoMessage($"背叛者排行:");
                for (int i = 0; i < 10 && i < traitorRank.Count; i++)
                {
                    TSPlayer.All.SendInfoMessage($"第{i + 1}名: {traitorRank[i].Name}({traitorRank[i].TraitorDatas.TotalScore})");
                }
#endif
            }

            public static void CountScore(PlayerIdentity winner)
            {
                foreach (var player in Game.Participants)
                {
                    if (player == null || !player.IsValid())
                    {
                        continue;
                    }
                    var statement = "得分: \n";
                    var traitorCount = Game.Participants.Count(p => p?.Party == PlayerIdentity.Traitor);
                    var survivorCount = Game.Participants.Count(p => p?.Party == PlayerIdentity.Survivor);
                    var score = 0.0;
                    var bossLife = Game.FinalBossLifeMax;
                    var BLPropotion = Game.IsFinalBattleTime ? (double)Game.FinalBossLifeRest / (double)Game.FinalBossLifeMax : 1;
                    var bossScore = 50 * Game.Participants.Length;
                    if (player.Party == PlayerIdentity.Traitor)
                    {
                        var fromKilling = player.KilledCount * 30;
                        var fromBoss = BLPropotion * bossScore / traitorCount;
                        score += fromKilling;
                        score += fromBoss;
                        statement += $"  +{fromKilling}(击杀{player.KilledCount}人)\n";
                        statement += $"  +{(int)fromBoss}(boss剩余{(int)(BLPropotion * 100)}%血量)\n";
                    }
                    else
                    {
                        if (bossLife != null)
                        {
                            var fromBoss = (player.DamageCaused / (double)bossLife) * (1 - BLPropotion) * bossScore;
                            score += fromBoss;
                            statement += $"  +{(int)fromBoss}(对boss造成{(int)(player.DamageCaused / (double)bossLife * 100)}%伤害)\n";
                        }
                    }
                    var fromChest = player.ChestsOpened;
                    score += fromChest;
                    statement += $"  +{fromChest}(打开{player.ChestsOpened}个箱子)\n";

                    score += player.PerformanceScore;
                    statement += $"  +{player.PerformanceScore}(表现分)\n";

                    if (player.Party == player.Identity)
                    {
                        var fromAlive = 20;
                        score += fromAlive;
                        statement += $"  +{fromAlive}(存活)";
                    }
                    if (player.Party == winner)
                    {
                        score *= 1.2;
                        statement += $"  +20%(胜利)\n";
                    }
                    else
                    {
                        score *= 0.9;
                        statement += $"  -10%(失败)\n";
                    }
                    var data = player.Data.SurvivorDatas;
                    if (player.Party == PlayerIdentity.Traitor)
                    {
                        data = player.Data.TraitorDatas;
                    }
                    data.TotalScore += (int)score;
                    player.Data.Coins += (int)score;
                    statement += $"共计获得{(int)score}分, 当前分数{data.TotalScore}";
                    player.SendText(statement, Color.YellowGreen);
                }
            }

            [Obsolete("待补充")]
            public static void ToNextTask()
            {
                Game.CurrentTask = null;
            }
            #endregion
            #region GlobalChatInterference
            public static void InterferenceChat(int timeInTicks)
            {
                Game.TraitorEMPTime = Math.Max(Game.TraitorEMPTime, timeInTicks);
            }
            private static void UpdateChatInterference()
            {
                if (Game.TraitorEMPTime > 0)
                {
                    Game.TraitorEMPTime--;
                    if (Game.TraitorEMPTime == 0)
                    {
                        Game.BCToAll(Texts.InterferenceEnded, Color.SeaShell);
                    }
                }
            }
            #endregion
            #region SpawnEnemies
            private static void SpawnEnemiesUpdate()
            {
                int playersInSpheres = Regions.Spheres.CountPlayers(true);
                int playersInMaze = Regions.Maze.CountPlayers(true);
                int playersInCaveEx = Regions.CaveEx.CountPlayers(true);
                int playersInHell = Regions.Hell.CountPlayers(true);

                int enemiesInSpheres = Main.npc.Count(npc => npc.active && Regions.Spheres.InRange(npc));
                int enemiesInMaze = Main.npc.Count(npc => npc.active && Regions.Maze.InRange(npc));
                int enemiesInCaveEx = Main.npc.Count(npc => npc.active && Regions.CaveEx.InRange(npc) && npc.lifeMax > 450);
                int enemiesInHell = Main.npc.Count(npc => npc.active && Regions.Hell.InRange(npc) && npc.lifeMax > 300);

                int spheresMax = 15 * playersInSpheres;
                int mazeMax = 5 * playersInMaze;
                int caveExMax = 9 * playersInCaveEx;
                int hellMax = 7 * playersInHell;

                int spheresAverage = playersInSpheres == 0 ? 0 : (spheresMax - enemiesInSpheres) / playersInSpheres;
                int mazeAverage = playersInMaze == 0 ? 0 : (mazeMax - enemiesInMaze) / playersInMaze;
                int caveExAverage = playersInCaveEx == 0 ? 0 : (caveExMax - enemiesInCaveEx) / playersInCaveEx;
                int hellAverage = playersInHell == 0 ? 0 : (hellMax - enemiesInHell) / playersInHell;

                foreach (var player in Game.Participants)
                {
                    if (!player.IsValid()|| player.Identity == PlayerIdentity.Watcher)
                    {
                        continue;
                    }
                    if (Regions.Spheres.InRange(player))
                    {
                        TrySpawnEnemies(player, spheresAverage,
                            (NPCID.Harpy, 0.0003),
                            (NPCID.CursedSkull, 0.000025)
                            );
                    }
                    else if (Regions.Maze.InRange(player))
                    {
                        TrySpawnEnemies(player, mazeAverage,
                            (NPCID.FireImp, 0.000833),
                            (NPCID.SkeletonSniper, 0.00024),
                            (NPCID.CursedHammer, 0.00008),
                            (NPCID.EnchantedSword, 0.00008)
                            );
                    }
                    else if (Regions.CaveEx.InRange(player))
                    {
                        TrySpawnEnemies(player, caveExAverage,
                            (NPCID.ChaosElemental, 0.002),
                            (NPCID.IlluminantBat, 0.008),
                            (NPCID.SkeletonArcher, 0.002),
                            (NPCID.Hellbat, 0.01),
                            (NPCID.Lavabat, 0.007),
                            (NPCID.DiggerHead, 0.0012),
                            (NPCID.Medusa, 0.0075)
                            );
                    }
                    else if (Regions.Hell.InRange(player))
                    {
                        const int scaler = 10;
                        int hellSkeleton = Rand.Next(277, 280 + 1);
                        TrySpawnEnemies(player, caveExAverage,
                            (NPCID.BoneSerpentHead, 0.0010 * scaler),
                            (NPCID.Mimic, 0.0003 * scaler),
                            (hellSkeleton, 0.0003 * scaler),
                            (NPCID.AngryBones, 0.0070 * scaler),
                            (NPCID.Hellbat, 0.0030 * scaler),
                            (NPCID.Lavabat, 0.0025 * scaler),
                            (NPCID.Medusa, 0.0055 * scaler)
                            );
                    }
                }
            }
            public static void TrySpawnEnemies(GamePlayer player, int n, params (int type, double probability)[] npcs)
            {
                if (Game.IsLongLongDark)
                {
                    for (int i = 0; i < npcs.Length; i++)
                    {
                        npcs[i].probability *= 1.2;
                    }
                }
                var x = (int)(player.TPlayer.position.X / 16);
                var y = (int)(player.TPlayer.position.Y / 16);
                for (int i = 0; i < 120; i++)
                {
                    for (int j = 0; j < 120; j++)
                    {
                        if (i > 80 || j > 45)
                        {
                            if (
                                !Main.tile[x + i, y + j + 0].active() && !Main.tile[x + i + 1, y + j + 0].active() &&
                                !Main.tile[x + i, y + j + 1].active() && !Main.tile[x + i + 1, y + j + 1].active() &&
                                !Main.tile[x + i, y + j + 2].active() && !Main.tile[x + i + 1, y + j + 2].active()
                                )
                            {
                                var npc = Rand.Next(npcs);
                                if (Rand.NextDouble() < 0.066 && Rand.NextDouble() < npc.probability)
                                {
                                    int idx = NPC.NewNPC(new EntitySource_DebugCommand(), (x + i) * 16, (y + j) * 16, npc.type);
                                    TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", idx);
                                    n--;
                                }
                                if (n <= 0)
                                {
                                    return;
                                }
                            }
                            if (
                                !Main.tile[x - i, y + j + 0].active() && !Main.tile[x - i + 1, y + j + 0].active() &&
                                !Main.tile[x - i, y + j + 1].active() && !Main.tile[x - i + 1, y + j + 1].active() &&
                                !Main.tile[x - i, y + j + 2].active() && !Main.tile[x - i + 1, y + j + 2].active()
                                )
                            {
                                var npc = Rand.Next(npcs);
                                if (Rand.NextDouble() < 0.066 && Rand.NextDouble() < npc.probability)
                                {
                                    int idx = NPC.NewNPC(new EntitySource_DebugCommand(), (x - i) * 16, (y + j) * 16, npc.type);
                                    TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", idx);
                                    n--;
                                }
                                if (n <= 0)
                                {
                                    return;
                                }
                            }
                            if (
                                !Main.tile[x + i, y - j + 0].active() && !Main.tile[x + i + 1, y - j + 0].active() &&
                                !Main.tile[x + i, y - j + 1].active() && !Main.tile[x + i + 1, y - j + 1].active() &&
                                !Main.tile[x + i, y - j + 2].active() && !Main.tile[x + i + 1, y - j + 2].active()
                                )
                            {
                                var npc = Rand.Next(npcs);
                                if (Rand.NextDouble() < 0.066 && Rand.NextDouble() < npc.probability)
                                {
                                    int idx = NPC.NewNPC(new EntitySource_DebugCommand(), (x + i) * 16, (y - j) * 16, npc.type);
                                    TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", idx);
                                    n--;
                                }
                                if (n <= 0)
                                {
                                    return;
                                }
                            }
                            if (
                                !Main.tile[x - i, y - j + 0].active() && !Main.tile[x - i + 1, y - j + 0].active() &&
                                !Main.tile[x - i, y - j + 1].active() && !Main.tile[x - i + 1, y - j + 1].active() &&
                                !Main.tile[x - i, y - j + 2].active() && !Main.tile[x - i + 1, y - j + 2].active()
                                )
                            {
                                var npc = Rand.Next(npcs);
                                if (Rand.NextDouble() < 0.066 && Rand.NextDouble() < npc.probability)
                                {
                                    int idx = NPC.NewNPC(new EntitySource_DebugCommand(), (x - i) * 16, (y - j) * 16, npc.type);
                                    TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", idx);
                                    n--;
                                }
                                if (n <= 0)
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region SpecialEvents
            private static void UpdateSpecialEvents()
            {
                foreach (var Event in Game.Events)
                {
                    if (!Event.Active)
                    {
                        if (!Event.Triggered)
                        {
                            if (Event.StartDelay == 0)
                            {
                                Event.Start();
                            }
                            else
                            {
                                Event.StartDelay--;
                            }
                        }
                    }
                    else
                    {
                        Event.Update();
                        Event.TimeLeft--;
                        if (Event.TimeLeft == 0)
                        {
                            Event.End();
                        }
                    }
                }
            }
            #endregion
            #region Effects
            public static void AddEffect(Effect effect)
            {
                effect.Apply();
                Game.Effects.Enqueue(effect);
            }
            private static void UpdateEffects()
            {
                int count = Game.Effects.Count;
                while (count-- > 0)
                {
                    var effect = Game.Effects.Dequeue();
                    effect.Update();
                    if (!effect.IsEnd)
                    {
                        Game.Effects.Enqueue(effect);
                    }
                }
            }
            public static void RemoveEffect(Effect effect)
            {
                int count = Game.Effects.Count;
                while (count-- > 0)
                {
                    var e = Game.Effects.Dequeue();
                    if (effect == e)
                    {
                        e.Abort();
                        break;
                    }
                    Game.Effects.Enqueue(e);
                }
            }
            public static void ClearEffects()
            {
                foreach (var effect in Game.Effects)
                {
                    effect.Abort();
                }
                Game.Effects.Clear();
            }
            #endregion
        }
    }
}
