using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Timer = System.Timers.Timer;
using Microsoft.Xna.Framework;

namespace XiuXianShouYuan
{
    [ApiVersion(2, 1)]
    public class XiuXianShouYuan : TerrariaPlugin
    {
        #region 基础配置
        private static readonly string ConfigPath = Path.Combine(TShock.SavePath, "XiuXianConfig.json");
        private static readonly string DataPath = Path.Combine(TShock.SavePath, "XiuXianData.json");
        private readonly Timer _cultivationTimer = new Timer(30000);
        private readonly Timer _lifeCheckTimer = new Timer(5000);
        private readonly Timer _topUiRefreshTimer = new Timer(1800000);
        private readonly Timer _chatUiRefreshTimer = new Timer(1800000);
        private readonly Timer _onlineRewardTimer = new Timer(300000);
        private StatusManager statusManager;
        
        private readonly Dictionary<int, BossBattle> _activeBossBattles = new Dictionary<int, BossBattle>();
        private readonly Timer _bossBattleCheckTimer = new Timer(5000);
        private readonly Dictionary<int, int> _playerLastHitBy = new Dictionary<int, int>();

        // 事件系统
        private readonly Dictionary<string, EventInfo> _activeEvents = new Dictionary<string, EventInfo>();
        private readonly Timer _eventCheckTimer = new Timer(60000);

        // 在线时长追踪
        private readonly Dictionary<string, DateTime> _playerLoginTime = new Dictionary<string, DateTime>();

        public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
        public override string Author => "泷白";
        public override Version Version => new Version(1, 2, 3);
        public override string Description => "独立境界职业修仙系统，包含完整事件命座功能和丰富的突破奖励";

        public XiuXianShouYuan(Main game) : base(game)
        {
            Order = 1;
            _cultivationTimer.AutoReset = true;
            _lifeCheckTimer.AutoReset = true;
            _topUiRefreshTimer.AutoReset = true;
            _chatUiRefreshTimer.AutoReset = true;
            _bossBattleCheckTimer.AutoReset = true;
            _eventCheckTimer.AutoReset = true;
            _onlineRewardTimer.AutoReset = true;
            statusManager = new StatusManager();
        }
        #endregion

        #region 事件系统
        private class EventInfo
        {
            public string EventName { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public List<string> Participants { get; set; } = new List<string>();
            public bool IsTimeBased { get; set; } = true;
            public Func<bool> GameEventCondition { get; set; }
            public bool IsActive => IsTimeBased ? 
                (DateTime.Now >= StartTime && DateTime.Now <= EndTime) : 
                (GameEventCondition?.Invoke() ?? false);
        }

        private void InitializeEvents()
        {
            // 时间事件
            var bloodMoon = new EventInfo
            {
                EventName = "血月",
                StartTime = DateTime.Today.AddDays(1 - DateTime.Today.Day).AddHours(18),
                EndTime = DateTime.Today.AddDays(7 - DateTime.Today.Day).AddHours(6),
                IsTimeBased = true
            };
            _activeEvents["血月"] = bloodMoon;

            var pumpkinMoon = new EventInfo
            {
                EventName = "南瓜月",
                StartTime = DateTime.Today.AddDays(15 - DateTime.Today.Day).AddHours(18),
                EndTime = DateTime.Today.AddDays(21 - DateTime.Today.Day).AddHours(6),
                IsTimeBased = true
            };
            _activeEvents["南瓜月"] = pumpkinMoon;

            var frostMoon = new EventInfo
            {
                EventName = "霜月",
                StartTime = DateTime.Today.AddDays(25 - DateTime.Today.Day).AddHours(18),
                EndTime = DateTime.Today.AddDays(DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month) - DateTime.Today.Day).AddHours(6),
                IsTimeBased = true
            };
            _activeEvents["霜月"] = frostMoon;

            // 游戏内事件
            var solarEclipse = new EventInfo
            {
                EventName = "日食",
                IsTimeBased = false,
                GameEventCondition = () => Main.eclipse
            };
            _activeEvents["日食"] = solarEclipse;

            var goblinArmy = new EventInfo
            {
                EventName = "哥布林军团",
                IsTimeBased = false,
                GameEventCondition = () => Main.invasionType == 1 && Main.invasionSize > 0
            };
            _activeEvents["哥布林军团"] = goblinArmy;

            var pirateInvasion = new EventInfo
            {
                EventName = "海盗入侵",
                IsTimeBased = false,
                GameEventCondition = () => Main.invasionType == 2 && Main.invasionSize > 0
            };
            _activeEvents["海盗入侵"] = pirateInvasion;

            var snowLegion = new EventInfo
            {
                EventName = "雪人军团",
                IsTimeBased = false,
                GameEventCondition = () => Main.invasionType == 3 && Main.invasionSize > 0
            };
            _activeEvents["雪人军团"] = snowLegion;

            var martianMadness = new EventInfo
            {
                EventName = "火星人入侵",
                IsTimeBased = false,
                GameEventCondition = () => Main.invasionType == 4 && Main.invasionSize > 0
            };
            _activeEvents["火星人入侵"] = martianMadness;

            var oldOnesArmy = new EventInfo
            {
                EventName = "撒旦军队",
                IsTimeBased = false,
                GameEventCondition = () => Main.invasionType == 5 && Main.invasionSize > 0
            };
            _activeEvents["撒旦军队"] = oldOnesArmy;

            var slimeRain = new EventInfo
            {
                EventName = "史莱姆雨",
                IsTimeBased = false,
                GameEventCondition = () => Main.slimeRain
            };
            _activeEvents["史莱姆雨"] = slimeRain;

            var sandstorm = new EventInfo
            {
                EventName = "沙尘暴",
                IsTimeBased = false,
                GameEventCondition = () => IsSandstormActive()
            };
            _activeEvents["沙尘暴"] = sandstorm;

            var rain = new EventInfo
            {
                EventName = "下雨",
                IsTimeBased = false,
                GameEventCondition = () => Main.raining
            };
            _activeEvents["下雨"] = rain;

            var windyDay = new EventInfo
            {
                EventName = "大风天",
                IsTimeBased = false,
                GameEventCondition = () => Main._shouldUseWindyDayMusic
            };
            _activeEvents["大风天"] = windyDay;

            var party = new EventInfo
            {
                EventName = "派对",
                IsTimeBased = false,
                GameEventCondition = () => IsPartyActive()
            };
            _activeEvents["派对"] = party;

            _eventCheckTimer.Start();
        }

        private bool IsSandstormActive()
        {
            try
            {
                return Main.IsItAHappyWindyDay && Main.windSpeedTarget > 0.5f;
            }
            catch
            {
                return false;
            }
        }

        private bool IsPartyActive()
        {
            try
            {
                return NPC.AnyNPCs(Terraria.ID.NPCID.PartyGirl);
            }
            catch
            {
                return false;
            }
        }

        private void CheckEvents(object sender, ElapsedEventArgs e)
        {
            try
            {
                foreach (var eventInfo in _activeEvents.Values)
                {
                    bool wasActive = eventInfo.Participants.Any();
                    bool isNowActive = eventInfo.IsActive;

                    if (isNowActive && !wasActive)
                    {
                        TSPlayer.All.SendMessage($"★★★ {eventInfo.EventName}事件已开始！参与事件可获得命座点数 ★★★", Color.Orange);
                        
                        if (!eventInfo.IsTimeBased)
                        {
                            foreach (var player in TShock.Players.Where(p => p != null && p.Active && p.IsLoggedIn))
                            {
                                AddPlayerToEvent(player, eventInfo.EventName);
                            }
                        }
                    }
                    else if (!isNowActive && wasActive)
                    {
                        eventInfo.Participants.Clear();
                        TSPlayer.All.SendMessage($"★★★ {eventInfo.EventName}事件已结束 ★★★", Color.Gray);
                    }
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"检查事件失败: {ex.Message}");
            }
        }

        private bool IsPlayerInActiveEvent(TSPlayer player)
        {
            return _activeEvents.Values.Any(e => e.IsActive && e.Participants.Contains(player.Name));
        }

        private void AddPlayerToEvent(TSPlayer player, string eventName)
        {
            if (_activeEvents.TryGetValue(eventName, out var eventInfo) && eventInfo.IsActive)
            {
                if (!eventInfo.Participants.Contains(player.Name))
                {
                    eventInfo.Participants.Add(player.Name);
                    player.SendSuccessMessage($"你已加入 {eventName} 事件！");
                }
            }
        }

        private void ProcessEventKillReward(TSPlayer player, string eventName)
        {
            if (_activeEvents.TryGetValue(eventName, out var eventInfo) && eventInfo.IsActive)
            {
                var data = XiuXianData.GetPlayer(player.Name);
                
                int pointsReward = eventName switch
                {
                    "血月" => 2,
                    "南瓜月" => 3,
                    "霜月" => 4,
                    "日食" => 5,
                    "哥布林军团" => 2,
                    "海盗入侵" => 3,
                    "雪人军团" => 3,
                    "火星人入侵" => 6,
                    "撒旦军队" => 4,
                    "史莱姆雨" => 1,
                    "沙尘暴" => 1,
                    "下雨" => 1,
                    "大风天" => 1,
                    "派对" => 1,
                    _ => 1
                };

                data.ConstellationPoints += pointsReward;
                player.SendSuccessMessage($"在{eventName}中表现卓越！获得{pointsReward}点命座点数");

                UpdateChatUI(player, data);
                UpdateTopUI(player, data);
            }
        }

        private void CheckEventMonsterKill(NPC npc)
        {
            string npcName = npc.GivenOrTypeName.ToLower();

            foreach (var eventInfo in _activeEvents.Values.Where(e => e.IsActive))
            {
                if (IsMonsterInEvent(npcName, eventInfo.EventName))
                {
                    var players = TShock.Players.Where(p => p != null && p.Active && p.IsLoggedIn);
                    foreach (var player in players)
                    {
                        if (IsPlayerInActiveEvent(player))
                        {
                            AddPlayerToEvent(player, eventInfo.EventName);
                        }
                    }
                    break;
                }
            }
        }

        private bool IsMonsterInEvent(string npcName, string eventName)
        {
            return eventName switch
            {
                "血月" => npcName.Contains("血") || npcName.Contains("僵尸") || npcName.Contains("小丑") || npcName.Contains("滴滴怪"),
                "南瓜月" => npcName.Contains("南瓜") || npcName.Contains("树精") || npcName.Contains("无头骑士") || npcName.Contains("哀木"),
                "霜月" => npcName.Contains("冰雪") || npcName.Contains("圣诞") || npcName.Contains("雪人") || npcName.Contains("圣诞坦克"),
                "日食" => npcName.Contains("飞眼") || npcName.Contains("沼泽怪") || npcName.Contains("弗兰肯") || npcName.Contains("吸血鬼"),
                "哥布林军团" => npcName.Contains("哥布林") || npcName.Contains("哥布林弓箭手") || npcName.Contains("哥布林术士"),
                "海盗入侵" => npcName.Contains("海盗") || npcName.Contains("海盗船长") || npcName.Contains("海盗水手"),
                "雪人军团" => npcName.Contains("雪人") || npcName.Contains("冰雪女王"),
                "火星人入侵" => npcName.Contains("火星") || npcName.Contains("飞碟") || npcName.Contains("火星工程师"),
                "撒旦军队" => npcName.Contains("黑暗魔法师") || npcName.Contains("食人魔") || npcName.Contains("双足翼龙"),
                "史莱姆雨" => npcName.Contains("史莱姆") && !npcName.Contains("国王"),
                _ => false
            };
        }
        #endregion

        #region Boss战观战系统 - 修复版
        private class BossBattle
        {
            public int NpcIndex { get; set; }
            public string BossName { get; set; }
            public List<string> GhostPlayers { get; set; } = new List<string>();
            public DateTime StartTime { get; set; } = DateTime.Now;
            public bool IsBossDespawned { get; set; } = false;
        }

        private void OnBossSpawn(NpcSpawnEventArgs args)
        {
            try
            {
                NPC npc = Main.npc[args.NpcId];
                if (npc.boss && npc.type != Terraria.ID.NPCID.TargetDummy)
                {
                    string bossName = npc.GivenOrTypeName;
                    
                    var battle = new BossBattle
                    {
                        NpcIndex = args.NpcId,
                        BossName = bossName
                    };
                    
                    _activeBossBattles[args.NpcId] = battle;
                    
                    TShock.Log.Info($"Boss战开始: {bossName} (ID: {args.NpcId})");
                    TSPlayer.All.SendMessage($"★★★ {bossName} 已降临！被Boss击杀的修士将化为灵魂观战 ★★★", Color.Orange);
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"处理Boss生成事件失败: {ex.Message}");
            }
        }

        private void OnNpcStrike(NpcStrikeEventArgs args)   
        {
            try
            {
                NPC npc = args.Npc;
                if (npc != null && npc.boss && args.Player.whoAmI >= 0 && args.Player.whoAmI < Main.player.Length)
                {
                    _playerLastHitBy[args.Player.whoAmI] = npc.whoAmI;
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"处理NPC攻击事件失败: {ex.Message}");
            }
        }

        private void CheckBossBattles(object sender, ElapsedEventArgs e)
        {
            try
            {
                var toRemove = new List<int>();
                
                foreach (var battle in _activeBossBattles.Values)
                {
                    // 检查Boss是否已死亡或消失
                    bool bossDeadOrGone = battle.NpcIndex >= Main.npc.Length || 
                                         !Main.npc[battle.NpcIndex].active || 
                                         Main.npc[battle.NpcIndex].life <= 0;

                    if (bossDeadOrGone && !battle.IsBossDespawned)
                    {
                        battle.IsBossDespawned = true;
                        
                        // Boss死亡或消失，复活所有观战玩家
                        foreach (var playerName in battle.GhostPlayers.ToList())
                        {
                            var player = TShock.Players.FirstOrDefault(p => p != null && p.Name == playerName);
                            if (player != null)
                            {
                                SafeRevivePlayer(player);
                                player.SendSuccessMessage($"★★★ {battle.BossName} 已被击败，你已复活！ ★★★");
                            }
                        }
                        
                        TShock.Log.Info($"Boss战结束: {battle.BossName}");
                        toRemove.Add(battle.NpcIndex);
                    }
                    else if (!bossDeadOrGone)
                    {
                        // Boss仍然存在，检查是否有活着的玩家
                        bool anyAlivePlayers = false;
                        foreach (var player in TShock.Players.Where(p => p != null && p.Active && p.IsLoggedIn && !p.Dead))
                        {
                            if (!battle.GhostPlayers.Contains(player.Name))
                            {
                                anyAlivePlayers = true;
                                break;
                            }
                        }

                        // 如果没有活着的玩家，结束Boss战
                        if (!anyAlivePlayers && battle.GhostPlayers.Count > 0)
                        {
                            battle.IsBossDespawned = true;
                            foreach (var playerName in battle.GhostPlayers.ToList())
                            {
                                var player = TShock.Players.FirstOrDefault(p => p != null && p.Name == playerName);
                                if (player != null)
                                {
                                    SafeRevivePlayer(player);
                                    player.SendWarningMessage($"★★★ 全体修士阵亡，{battle.BossName} 已离去 ★★★");
                                }
                            }
                            toRemove.Add(battle.NpcIndex);
                        }
                    }
                }
                
                foreach (var npcIndex in toRemove)
                {
                    _activeBossBattles.Remove(npcIndex);
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"检查Boss战失败: {ex.Message}");
            }
        }

        private void SetPlayerAsGhost(TSPlayer player, int bossNpcIndex)
        {
            try
            {
                if (_activeBossBattles.TryGetValue(bossNpcIndex, out var battle))
                {
                    if (!battle.GhostPlayers.Contains(player.Name))
                    {
                        battle.GhostPlayers.Add(player.Name);
                    }
                    
                    player.TPlayer.ghost = true;
                    player.TPlayer.dead = true;
                    player.SendData(PacketTypes.PlayerUpdate, "", player.Index);
                    player.SendData(PacketTypes.PlayerHp, "", player.Index);
                    
                    player.SendInfoMessage("你已化为灵魂状态，可以观战Boss战");
                    player.SendInfoMessage("Boss被击败后你将自动复活，或使用 /反魂 自己 消耗寿元复活");
                    
                    // 记录灵魂状态到玩家数据
                    var data = XiuXianData.GetPlayer(player.Name);
                    data.IsGhostState = true;
                    data.GhostBattleNpcIndex = bossNpcIndex;
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"设置玩家幽灵状态失败: {ex.Message}");
            }
        }

        private void SafeRevivePlayer(TSPlayer player)
        {
            try
            {
                var data = XiuXianData.GetPlayer(player.Name);
                
                // 传送到安全出生点
                int spawnX = Main.spawnTileX * 16;
                int spawnY = Main.spawnTileY * 16;
                player.Teleport(spawnX, spawnY);
                
                player.TPlayer.ghost = false;
                player.TPlayer.dead = false;
                player.TPlayer.statLife = player.TPlayer.statLifeMax2;
                player.SendData(PacketTypes.PlayerUpdate, "", player.Index);
                player.SendData(PacketTypes.PlayerHp, "", player.Index);
                
                // 清除灵魂状态标记
                data.IsGhostState = false;
                data.GhostBattleNpcIndex = -1;
                
                // 从所有Boss战中移除
                foreach (var battle in _activeBossBattles.Values)
                {
                    battle.GhostPlayers.Remove(player.Name);
                }
                
                TShock.Log.Info($"安全复活玩家: {player.Name}");
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"安全复活玩家失败: {ex.Message}");
            }
        }

        private void RevivePlayer(TSPlayer player)
        {
            try
            {
                var data = XiuXianData.GetPlayer(player.Name);
                
                // 传送到安全出生点
                int spawnX = Main.spawnTileX * 16;
                int spawnY = Main.spawnTileY * 16;
                player.Teleport(spawnX, spawnY);
                
                player.TPlayer.ghost = false;
                player.TPlayer.dead = false;
                player.TPlayer.statLife = player.TPlayer.statLifeMax2;
                player.SendData(PacketTypes.PlayerUpdate, "", player.Index);
                player.SendData(PacketTypes.PlayerHp, "", player.Index);
                
                // 清除灵魂状态标记
                data.IsGhostState = false;
                data.GhostBattleNpcIndex = -1;
                
                foreach (var battle in _activeBossBattles.Values)
                {
                    battle.GhostPlayers.Remove(player.Name);
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"复活玩家失败: {ex.Message}");
            }
        }

        private void SoulReturn(CommandArgs args)
        {
            try
            {
                if (args.Parameters.Count == 0 || args.Parameters[0] != "自己")
                {
                    args.Player.SendInfoMessage("用法: /反魂 自己");
                    args.Player.SendInfoMessage("效果: 消耗寿元复活自己（被Boss击杀后使用）");
                    return;
                }
                
                var data = XiuXianData.GetPlayer(args.Player.Name);
                
                // 检查是否在灵魂状态
                if (!data.IsGhostState)
                {
                    args.Player.SendErrorMessage("你不在灵魂状态，无需使用此命令");
                    return;
                }
                
                int cost = XiuXianConfig.Instance.SoulReturnCost;
                
                if (data.LifeYears <= cost)
                {
                    args.Player.SendErrorMessage($"寿元不足！复活需要消耗 {cost} 年寿元");
                    return;
                }
                
                data.LifeYears -= cost;
                SafeRevivePlayer(args.Player);
                
                args.Player.SendSuccessMessage($"已消耗 {cost} 年寿元复活，剩余寿元: {data.LifeYears:F1}年");
                
                UpdateChatUI(args.Player, data);
                UpdateTopUI(args.Player, data);
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"执行反魂命令失败: {ex.Message}");
                args.Player.SendErrorMessage("反魂失败，请稍后再试");
            }
        }

        private void AdminSoulReturn(CommandArgs args)
        {
            try
            {
                if (args.Parameters.Count == 0)
                {
                    int count = 0;
                    foreach (var battle in _activeBossBattles.Values)
                    {
                        foreach (var playerName in battle.GhostPlayers.ToList())
                        {
                            var player = TShock.Players.FirstOrDefault(p => p != null && p.Name == playerName);
                            if (player != null)
                            {
                                SafeRevivePlayer(player);
                                player.SendSuccessMessage($"管理员 {args.Player.Name} 已将你复活");
                                count++;
                            }
                        }
                    }
                    
                    args.Player.SendSuccessMessage($"已复活 {count} 名灵魂状态玩家");
                    return;
                }
                
                string targetName = string.Join(" ", args.Parameters);
                var target = TShock.Players.FirstOrDefault(p => p != null && p.Name == targetName);
                
                if (target == null)
                {
                    args.Player.SendErrorMessage($"找不到玩家: {targetName}");
                    return;
                }
                
                var targetData = XiuXianData.GetPlayer(targetName);
                if (!targetData.IsGhostState)
                {
                    args.Player.SendErrorMessage($"玩家 {targetName} 不在灵魂状态");
                    return;
                }
                
                SafeRevivePlayer(target);
                target.SendSuccessMessage($"管理员 {args.Player.Name} 已将你复活");
                args.Player.SendSuccessMessage($"已复活玩家: {targetName}");
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"执行管理员反魂命令失败: {ex.Message}");
                args.Player.SendErrorMessage("反魂失败，请稍后再试");
            }
        }

        private void OnDead(object sender, GetDataHandlers.KillMeEventArgs args)
        {
            try
            {
                var player = args.Player;
                if (player == null) return;

                var data = XiuXianData.GetPlayer(player.Name);
                var realmInfo = XiuXianConfig.Instance.GetRealmInfoForStarSign(data.Realm, data.StarSign);
                
                // 计算死亡惩罚
                int penalty = CalculateDeathPenalty(data, realmInfo);
                
                bool killedByBoss = false;
                int bossNpcIndex = -1;
                
                if (_playerLastHitBy.TryGetValue(player.Index, out int lastHitByNpcIndex))
                {
                    if (lastHitByNpcIndex >= 0 && lastHitByNpcIndex < Main.npc.Length)
                    {
                        NPC lastNpc = Main.npc[lastHitByNpcIndex];
                        if (lastNpc.active && lastNpc.boss && lastNpc.life > 0)
                        {
                            killedByBoss = true;
                            bossNpcIndex = lastHitByNpcIndex;
                        }
                    }
                }
                
                if (!killedByBoss)
                {
                    for (int i = 0; i < Main.npc.Length; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.active && npc.boss && npc.life > 0)
                        {
                            float distance = Vector2.Distance(player.TPlayer.Center, npc.Center);
                            if (distance < 1000f)
                            {
                                killedByBoss = true;
                                bossNpcIndex = i;
                                break;
                            }
                        }
                    }
                }
                
                if (killedByBoss && bossNpcIndex != -1)
                {
                    SetPlayerAsGhost(player, bossNpcIndex);
                    player.SendInfoMessage("你被Boss击杀，已进入灵魂观战状态");
                }
                else
                {
                    data.LifeYears = Math.Max(1, data.LifeYears - penalty);
                    player.SendWarningMessage($"道陨之际！寿元减少{penalty}年 (剩余: {data.LifeYears:F1}年)");
                    
                    if (data.LifeYears <= 1 && !player.Group.HasPermission("shouyuan.admin"))
                    {
                        data.LifeDepletionTime = DateTime.Now;
                        player.SendWarningMessage($"★★★ 寿元已耗尽！{XiuXianConfig.Instance.LifeDepletionCountdown}秒后将被踢出服务器 ★★★");
                    }
                }

                if (_playerLastHitBy.ContainsKey(player.Index))
                {
                    _playerLastHitBy.Remove(player.Index);
                }

                UpdateChatUI(player, data);
                UpdateTopUI(player, data);
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"玩家死亡事件处理失败: {ex.Message}");
            }
        }

        private int CalculateDeathPenalty(XiuXianData data, RealmInfo realmInfo)
        {
            // 凡人境界只扣1年寿元
            if (data.Realm == "凡人")
                return 1;

            // 根据境界等级计算惩罚，境界越高惩罚越大
            int basePenalty = 5;
            int levelBonus = realmInfo.Level * 2;
            int randomBonus = new Random().Next(5, 16);
            
            return basePenalty + levelBonus + randomBonus;
        }
        #endregion

        #region 在线时长奖励系统
        private void ProcessOnlineRewards(object sender, ElapsedEventArgs e)
        {
            try
            {
                foreach (var player in TShock.Players.Where(p => p != null && p.Active && p.IsLoggedIn))
                {
                    try
                    {
                        var data = XiuXianData.GetPlayer(player.Name);
                        
                        // 增加在线时长
                        data.TotalOnlineMinutes += 5;
                        
                        // 每5分钟给予基础寿元奖励
                        float baseReward = 0.5f;
                        data.LifeYears += baseReward;
                        
                        // 随机触发顿悟事件
                        if (new Random().Next(0, 100) < 10) // 10%几率
                        {
                            float enlightenmentReward = new Random().Next(1, 6);
                            data.LifeYears += enlightenmentReward;
                            player.SendMessage($"★★★ 修炼途中豁然开朗，触发顿悟奖励 {enlightenmentReward:F1} 年寿元！ ★★★", Color.Gold);
                        }
                        
                        // 长时间在线额外奖励（每30分钟）
                        if (data.TotalOnlineMinutes % 30 == 0)
                        {
                            float longOnlineReward = 3.0f;
                            data.LifeYears += longOnlineReward;
                            player.SendMessage($"★★★ 天道感念你修行不懈，赐予 {longOnlineReward:F1} 年寿元！ ★★★", Color.LightGreen);
                        }
                        
                        // 新手保护：寿元低于50年时额外奖励
                        if (data.LifeYears < 50 && data.TotalOnlineMinutes < 180) // 3小时内的新手
                        {
                            float newbieReward = 2.0f;
                            data.LifeYears += newbieReward;
                            player.SendMessage($"★★★ 天道眷顾新人修士，赐予 {newbieReward:F1} 年寿元！ ★★★", Color.Cyan);
                        }
                        
                        UpdateChatUI(player, data);
                        UpdateTopUI(player, data);
                    }
                    catch (Exception ex)
                    {
                        TShock.Log.Error($"处理 {player.Name} 在线奖励失败: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"处理在线奖励失败: {ex.Message}");
            }
        }

        private void StartOnlineTimer(TSPlayer player)
        {
            if (!_playerLoginTime.ContainsKey(player.Name))
            {
                _playerLoginTime[player.Name] = DateTime.Now;
            }
        }

        private void StopOnlineTimer(TSPlayer player)
        {
            if (_playerLoginTime.ContainsKey(player.Name))
            {
                _playerLoginTime.Remove(player.Name);
            }
        }
        #endregion

        #region 寿元耗尽处理
        private void CheckPlayerLife(object sender, ElapsedEventArgs e)
        {
            foreach (var player in TShock.Players.Where(p => p != null && p.Active && p.IsLoggedIn))
            {
                try
                {
                    var data = XiuXianData.GetPlayer(player.Name);

                    if (player.Group.HasPermission("shouyuan.admin"))
                        continue;

                    if (data.LifeYears <= 1)
                    {
                        if (data.LifeDepletionTime == DateTime.MinValue)
                        {
                            data.LifeDepletionTime = DateTime.Now;
                            data.LifeDepletionWarned = false;
                            TShock.Log.Info($"玩家 {player.Name} 寿元耗尽，开始倒计时");
                        }

                        double elapsedSeconds = (DateTime.Now - data.LifeDepletionTime).TotalSeconds;
                        double remainingSeconds = XiuXianConfig.Instance.LifeDepletionCountdown - elapsedSeconds;

                        if (remainingSeconds <= 0)
                        {
                            player.Kick("寿元已耗尽！请使用/修仙转生或联系管理员", true);
                            TShock.Log.Info($"玩家 {player.Name} 因寿元耗尽被踢出");

                            data.LifeDepletionWarned = false;
                            data.LifeDepletionTime = DateTime.MinValue;
                            continue;
                        }

                        if (!data.LifeDepletionWarned || (int)remainingSeconds % 5 == 0)
                        {
                            player.SendWarningMessage($"★★★ 寿元已耗尽！{remainingSeconds:F0}秒后将被踢出服务器 ★★★");
                            player.SendWarningMessage("★★★ 请立即使用 /修仙转生 或联系管理员 ★★★");
                            data.LifeDepletionWarned = true;
                        }
                    }
                    else if (data.LifeDepletionTime != DateTime.MinValue)
                    {
                        data.LifeDepletionWarned = false;
                        data.LifeDepletionTime = DateTime.MinValue;
                        player.SendSuccessMessage("寿元已恢复，倒计时取消");
                    }
                }
                catch (Exception ex)
                {
                    TShock.Log.Error($"{player.Name}寿元检查失败: {ex.Message}");
                }
            }
        }

        private void OnPlayerPreLogin(PlayerPreLoginEventArgs args)
        {
            try
            {
                var playerName = args.Player.Name;
                var data = XiuXianData.GetPlayer(playerName);

                bool isAdmin = false;
                var account = TShock.UserAccounts.GetUserAccountByName(playerName);
                if (account != null)
                {
                    var group = TShock.Groups.GetGroupByName(account.Group);
                    isAdmin = group != null && group.HasPermission("shouyuan.admin");
                }

                if (data.LifeYears <= 1 && !isAdmin)
                {
                    args.Handled = true;
                    args.Player.Kick("寿元已耗尽！请联系管理员使用/寿元转生", true);
                    TShock.Log.Info($"玩家 {playerName} 登录时因寿元耗尽被阻止登录");
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"玩家登录前检查失败: {ex.Message}");
            }
        }
        #endregion

        #region 事件处理
        private void OnAccountCreate(AccountCreateEventArgs args)
        {
            try
            {
                var data = XiuXianData.GetPlayer(args.Account.Name);
                data.Realm = "凡人";
                data.LifeYears = XiuXianConfig.Instance.InitialLifeYears;
                data.StarSign = "未选择";
                data.DharmaName = "";
                data.LifeDepletionWarned = false;
                data.LifeDepletionTime = DateTime.MinValue;
                data.KilledBosses = new HashSet<string>();
                data.ConstellationPoints = 0;
                data.TotalOnlineMinutes = 0;
                data.IsGhostState = false;
                data.GhostBattleNpcIndex = -1;

                var player = TShock.Players.FirstOrDefault(p => p?.Account?.Name == args.Account.Name);
                if (player != null)
                {
                    player.Group = TShock.Groups.GetGroupByName("修仙弟子");
                    player.SendSuccessMessage("你已被分配到修仙弟子权限组");
                }
                TShock.Log.Info($"新修士诞生: {args.Account.Name}");
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"创建账号事件处理失败: {ex.Message}");
            }
        }

        private void OnBossKilled(NpcKilledEventArgs args)
        {
            try
            {
                NPC npc = args.npc;
                if (npc.boss && npc.type != Terraria.ID.NPCID.TargetDummy)
                {
                    string bossName = npc.GivenOrTypeName;
                    TShock.Log.Info($"检测到Boss击杀: {bossName}");

                    int lastDamagePlayer = npc.lastInteraction;
                    if (lastDamagePlayer >= 0 && lastDamagePlayer < 255)
                    {
                        var player = TShock.Players[lastDamagePlayer];
                        if (player != null && player.Active && player.IsLoggedIn)
                        {
                            var data = XiuXianData.GetPlayer(player.Name);
                            
                            string normalizedBossName = NormalizeBossName(bossName);
                            if (!data.KilledBosses.Contains(normalizedBossName))
                            {
                                data.KilledBosses.Add(normalizedBossName);
                                player.SendSuccessMessage($"★★★ 你已击败 {bossName}，修为大增！ ★★★");
                                TShock.Log.Info($"玩家 {player.Name} 击败Boss {bossName}，标准化名称: {normalizedBossName}");
                                
                                // 给予Boss进度奖励
                                GiveBossProgressReward(player, data, bossName);
                                
                                CheckEventBossKill(player, bossName);
                                
                                UpdateChatUI(player, data);
                                UpdateTopUI(player, data);
                            }
                        }
                    }
                }

                CheckEventMonsterKill(npc);
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"处理Boss击杀失败: {ex.Message}");
            }
        }

        private void GiveBossProgressReward(TSPlayer player, XiuXianData data, string bossName)
        {
            try
            {
                // 根据Boss难度给予不同的寿元奖励
                float lifeReward = GetBossLifeReward(bossName);
                if (lifeReward > 0)
                {
                    data.LifeYears += lifeReward;
                    player.SendMessage($"★★★ 击败 {bossName}，天道赐福获得 {lifeReward:F1} 年寿元！ ★★★", Color.Gold);
                }
                
                // 给予命座点数奖励
                int constellationReward = GetBossConstellationReward(bossName);
                if (constellationReward > 0)
                {
                    data.ConstellationPoints += constellationReward;
                    player.SendMessage($"★★★ 击败 {bossName}，获得 {constellationReward} 点命座点数！ ★★★", Color.Purple);
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"给予Boss进度奖励失败: {ex.Message}");
            }
        }

        private float GetBossLifeReward(string bossName)
        {
            return bossName.ToLower() switch
            {
                var name when name.Contains("月亮领主") => 50.0f,
                var name when name.Contains("拜月教邪教徒") => 30.0f,
                var name when name.Contains("石巨人") => 25.0f,
                var name when name.Contains("世纪之花") => 20.0f,
                var name when name.Contains("机械") => 15.0f, // 机械三王
                var name when name.Contains("血肉墙") => 10.0f,
                var name when name.Contains("骷髅王") => 8.0f,
                var name when name.Contains("蜂王") => 6.0f,
                var name when name.Contains("克苏鲁之眼") => 5.0f,
                var name when name.Contains("世界吞噬怪") => 5.0f,
                var name when name.Contains("克苏鲁之脑") => 5.0f,
                var name when name.Contains("史莱姆王") => 3.0f,
                _ => 2.0f
            };
        }

        private int GetBossConstellationReward(string bossName)
        {
            return bossName.ToLower() switch
            {
                var name when name.Contains("月亮领主") => 10,
                var name when name.Contains("拜月教邪教徒") => 8,
                var name when name.Contains("石巨人") => 6,
                var name when name.Contains("世纪之花") => 5,
                var name when name.Contains("机械") => 4,
                var name when name.Contains("血肉墙") => 3,
                _ => 2
            };
        }

        private void CheckEventBossKill(TSPlayer player, string bossName)
        {
            if (bossName.Contains("血月") || bossName.Contains("血腥"))
            {
                AddPlayerToEvent(player, "血月");
                ProcessEventKillReward(player, "血月");
            }
            else if (bossName.Contains("南瓜") || bossName.Contains("万圣节"))
            {
                AddPlayerToEvent(player, "南瓜月");
                ProcessEventKillReward(player, "南瓜月");
            }
            else if (bossName.Contains("霜月") || bossName.Contains("圣诞"))
            {
                AddPlayerToEvent(player, "霜月");
                ProcessEventKillReward(player, "霜月");
            }
            else if (bossName.Contains("日食") || bossName.Contains("飞眼") || bossName.Contains("沼泽怪"))
            {
                AddPlayerToEvent(player, "日食");
                ProcessEventKillReward(player, "日食");
            }
            else if (bossName.Contains("哥布林"))
            {
                AddPlayerToEvent(player, "哥布林军团");
                ProcessEventKillReward(player, "哥布林军团");
            }
            else if (bossName.Contains("海盗"))
            {
                AddPlayerToEvent(player, "海盗入侵");
                ProcessEventKillReward(player, "海盗入侵");
            }
            else if (bossName.Contains("雪人") || bossName.Contains("冰雪女王"))
            {
                AddPlayerToEvent(player, "雪人军团");
                ProcessEventKillReward(player, "雪人军团");
            }
            else if (bossName.Contains("火星") || bossName.Contains("飞碟"))
            {
                AddPlayerToEvent(player, "火星人入侵");
                ProcessEventKillReward(player, "火星人入侵");
            }
        }
        
        private string NormalizeBossName(string bossName)
        {
            if (string.IsNullOrEmpty(bossName))
                return "";
            
            return bossName.ToLowerInvariant().Replace(" ", "").Replace("'", "").Replace("-", "");
        }

        private void OnPlayerChat(PlayerChatEventArgs args)
        {
            try
            {
                var data = XiuXianData.GetPlayer(args.Player.Name);

                if (!string.IsNullOrEmpty(data.DharmaName))
                {
                    string dharmaPrefix = $"[c/{data.NameColor}:{data.DharmaName}] ";
                    args.RawText = dharmaPrefix + args.RawText;
                }

                if (args.RawText.Contains("寿元") || args.RawText.Contains("寿命"))
                {
                    args.Player.SendInfoMessage($"当前剩余寿元: {data.LifeYears:F1}年");
                    args.Handled = true;
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"玩家聊天事件处理失败: {ex.Message}");
            }
        }

        private void OnPlayerDamage(object sender, GetDataHandlers.PlayerDamageEventArgs args)
        {
            try
            {
                var player = args.Player;
                if (player == null || player.Dead || args.Damage < 30)
                    return;

                var data = XiuXianData.GetPlayer(player.Name);
                var realmInfo = XiuXianConfig.Instance.GetRealmInfoForStarSign(data.Realm, data.StarSign);
                
                // 根据境界计算受伤惩罚
                int penalty = CalculateDamagePenalty(data, realmInfo);
                data.LifeYears = Math.Max(1, data.LifeYears - penalty);

                UpdateChatUI(player, data);
                UpdateTopUI(player, data);

                player.SendWarningMessage($"根基受损！寿元减少{penalty}年 (剩余: {data.LifeYears:F1}年)");

                if (data.LifeYears <= 1 && !player.Group.HasPermission("shouyuan.admin"))
                {
                    data.LifeDepletionTime = DateTime.Now;
                    player.SendWarningMessage($"★★★ 寿元已耗尽！{XiuXianConfig.Instance.LifeDepletionCountdown}秒后将被踢出服务器 ★★★");
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"玩家受伤事件处理失败: {ex.Message}");
            }
        }

        private int CalculateDamagePenalty(XiuXianData data, RealmInfo realmInfo)
        {
            // 凡人境界受伤惩罚较小
            if (data.Realm == "凡人")
                return new Random().Next(1, 3);

            // 根据境界等级计算惩罚
            int basePenalty = 3;
            int levelBonus = realmInfo.Level;
            int randomBonus = new Random().Next(2, 6);
            
            return basePenalty + levelBonus + randomBonus;
        }

        private void OnPlayerPostLogin(PlayerPostLoginEventArgs args)
        {
            try
            {
                var player = args.Player;
                var data = XiuXianData.GetPlayer(player.Name);

                // 检查灵魂状态
                if (data.IsGhostState)
                {
                    // 检查对应的Boss战是否还在进行
                    bool shouldRemainGhost = false;
                    if (data.GhostBattleNpcIndex != -1 && 
                        _activeBossBattles.TryGetValue(data.GhostBattleNpcIndex, out var battle))
                    {
                        if (battle.NpcIndex < Main.npc.Length && 
                            Main.npc[battle.NpcIndex].active && 
                            Main.npc[battle.NpcIndex].life > 0)
                        {
                            // Boss战仍在进行，保持灵魂状态
                            shouldRemainGhost = true;
                            SetPlayerAsGhost(player, data.GhostBattleNpcIndex);
                            player.SendInfoMessage("Boss战仍在进行，你仍处于灵魂观战状态");
                        }
                    }
                    
                    if (!shouldRemainGhost)
                    {
                        // Boss战已结束，安全复活玩家
                        SafeRevivePlayer(player);
                        player.SendSuccessMessage("Boss战已结束，你已自动复活");
                    }
                }

                // 开始在线计时
                StartOnlineTimer(player);

                player.SendMessage("====================================", Microsoft.Xna.Framework.Color.Gold);
                player.SendMessage($"欢迎来到 [c/FF69B4:{XiuXianConfig.Instance.ServerName}]", Microsoft.Xna.Framework.Color.White);
                player.SendMessage("====================================", Microsoft.Xna.Framework.Color.Gold);

                ShowFullStatus(player, data);
                UpdateChatUI(player, data);
                UpdateTopUI(player, data);

                if (data.StarSign == "未选择")
                {
                    player.SendMessage("★★★ 请使用 /选择星宿 开启修仙之旅 ★★★", Microsoft.Xna.Framework.Color.Yellow);
                }

                if (data.LifeYears <= 1)
                {
                    data.LifeDepletionTime = DateTime.Now;
                    double remainingSeconds = XiuXianConfig.Instance.LifeDepletionCountdown;
                    player.SendErrorMessage($"★★★ 警告：寿元已耗尽！{remainingSeconds:F0}秒后将被踢出服务器 ★★★");
                    
                    if (!player.Group.HasPermission("shouyuan.admin"))
                    {
                        player.SendWarningMessage("★★★ 请立即使用 /修仙转生 或联系管理员 ★★★");
                    }
                }

                if (player.Group.HasPermission("shouyuan.admin"))
                    player.SendSuccessMessage("★★★ 你拥有修仙仙尊管理权限 ★★★");

                CheckActiveEventsForPlayer(player);
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"玩家登录后处理失败: {ex.Message}");
            }
        }

        private void CheckActiveEventsForPlayer(TSPlayer player)
        {
            foreach (var eventInfo in _activeEvents.Values)
            {
                if (eventInfo.IsActive)
                {
                    if (!eventInfo.IsTimeBased && !eventInfo.Participants.Contains(player.Name))
                    {
                        AddPlayerToEvent(player, eventInfo.EventName);
                    }
                    
                    player.SendMessage($"★★★ {eventInfo.EventName}事件正在进行中！输入 /查看事件 参与 ★★★", Color.Orange);
                }
            }
        }

        private void OnPlayerLogout(PlayerLogoutEventArgs args)
        {
            try
            {
                XiuXianData.SavePlayer(args.Player.Name);
                statusManager.RemoveText(args.Player);
                StopOnlineTimer(args.Player);
            }
            catch (Exception ex) { TShock.Log.Error($"玩家登出处理失败: {ex.Message}"); }
        }

        private void OnReload(ReloadEventArgs args)
        {
            try
            {
                XiuXianConfig.Load(ConfigPath);
                args.Player.SendSuccessMessage("修仙法则已重新感悟！");

                _topUiRefreshTimer.Interval = XiuXianConfig.Instance.TopUIRefreshInterval;
                _chatUiRefreshTimer.Interval = XiuXianConfig.Instance.ChatUIRefreshInterval * 60000;
                
                RC();
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"重载配置失败: {ex.Message}");
            }
        }

        private void OnRegionEntered(RegionHooks.RegionEnteredEventArgs args)
        {
            try
            {
                if (args.Region.Name.Contains("灵脉") || args.Region.Name.Contains("洞天"))
                {
                    var player = args.Player;
                    player.SendSuccessMessage("进入灵脉之地，修炼速度提升！");
                    var data = XiuXianData.GetPlayer(player.Name);
                    data.InHolyLand = true;
                    UpdateChatUI(player, data);
                    UpdateTopUI(player, data);
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"进入区域事件处理失败: {ex.Message}");
            }
        }

        private void OnRegionLeft(RegionHooks.RegionLeftEventArgs args)
        {
            try
            {
                if (args.Region.Name.Contains("灵脉") || args.Region.Name.Contains("洞天"))
                {
                    var player = args.Player;
                    player.SendInfoMessage("离开灵脉之地，修炼恢复正常");
                    var data = XiuXianData.GetPlayer(player.Name);
                    data.InHolyLand = false;
                    UpdateChatUI(player, data);
                    UpdateTopUI(player, data);
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"离开区域事件处理失败: {ex.Message}");
            }
        }

        private void OnServerJoin(JoinEventArgs args)
        {
            try
            {
                var player = TShock.Players[args.Who];
                if (player != null && !XiuXianData.Players.ContainsKey(player.Name))
                {
                    var data = XiuXianData.GetPlayer(player.Name);

                    player.SendMessage("====================================", Microsoft.Xna.Framework.Color.Gold);
                    player.SendMessage($"欢迎来到 [c/FF69B4:{XiuXianConfig.Instance.ServerName}]", Microsoft.Xna.Framework.Color.White);
                    player.SendMessage("====================================", Microsoft.Xna.Framework.Color.Gold);
                    player.SendMessage($"新修士，初始寿元: {data.LifeYears}年", Microsoft.Xna.Framework.Color.LightGreen);
                    player.SendMessage("使用 /选择星宿 开启你的修仙之路", Microsoft.Xna.Framework.Color.Yellow);

                    UpdateChatUI(player, data);
                    UpdateTopUI(player, data);

                    if (data.LifeYears <= 1 && !player.Group.HasPermission("shouyuan.admin"))
                    {
                        data.LifeDepletionTime = DateTime.Now;
                        double remainingSeconds = XiuXianConfig.Instance.LifeDepletionCountdown;
                        player.SendErrorMessage($"★★★ 警告：寿元已耗尽！{remainingSeconds:F0}秒后将被踢出服务器 ★★★");
                        player.SendWarningMessage("★★★ 请立即使用 /修仙转生 或联系管理员 ★★★");
                    }
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"玩家加入事件处理失败: {ex.Message}");
            }
        }

        private void OnServerLeave(LeaveEventArgs args)
        {
            try
            {
                var player = TShock.Players[args.Who];
                if (player != null)
                {
                    XiuXianData.SavePlayer(player.Name);
                    statusManager.RemoveText(player);
                    StopOnlineTimer(player);
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"玩家离开事件处理失败: {ex.Message}");
            }
        }
        #endregion

        #region 初始化与钩子注册
        public override void Initialize()
        {
            PlayerHooks.PlayerChat += OnPlayerChat;
            PlayerHooks.PlayerPostLogin += OnPlayerPostLogin;
            PlayerHooks.PlayerPreLogin += OnPlayerPreLogin;
            PlayerHooks.PlayerLogout += OnPlayerLogout;

            GetDataHandlers.PlayerDamage += OnPlayerDamage;
            GetDataHandlers.KillMe += OnDead;

            GeneralHooks.ReloadEvent += OnReload;

            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.ServerJoin.Register(this, OnServerJoin);
            ServerApi.Hooks.ServerLeave.Register(this, OnServerLeave);
            ServerApi.Hooks.NpcKilled.Register(this, OnBossKilled);
            ServerApi.Hooks.NpcSpawn.Register(this, OnBossSpawn);
            ServerApi.Hooks.NpcStrike.Register(this, OnNpcStrike);
            AccountHooks.AccountCreate += OnAccountCreate;
            RegionHooks.RegionEntered += OnRegionEntered;
            RegionHooks.RegionLeft += OnRegionLeft;

            _cultivationTimer.Elapsed += Cultivate;
            _lifeCheckTimer.Elapsed += CheckPlayerLife;
            _topUiRefreshTimer.Elapsed += RefreshTopUIForAllPlayers;
            _chatUiRefreshTimer.Elapsed += RefreshChatUIForAllPlayers;
            _bossBattleCheckTimer.Elapsed += CheckBossBattles;
            _eventCheckTimer.Elapsed += CheckEvents;
            _onlineRewardTimer.Elapsed += ProcessOnlineRewards;

            // 优化命令注册，移除重复命令
            Commands.ChatCommands.Add(new Command("shouyuan.player", ShowStatus, "状态"));
            Commands.ChatCommands.Add(new Command("shouyuan.player", CultivationCommand, "修仙"));
            Commands.ChatCommands.Add(new Command("shouyuan.admin", AdminForceRebirth, "寿元转生"));
            Commands.ChatCommands.Add(new Command("shouyuan.admin", AdminAdjustLife, "调整寿元"));
            Commands.ChatCommands.Add(new Command("shouyuan.player", SetDharmaName, "法号"));
            Commands.ChatCommands.Add(new Command("shouyuan.player", ChooseStarSign, "选择星宿"));
            Commands.ChatCommands.Add(new Command("shouyuan.player", ViewConstellation, "命座"));
            Commands.ChatCommands.Add(new Command("shouyuan.admin", SetServerName, "设置服务器名称"));
            Commands.ChatCommands.Add(new Command("shouyuan.admin", SetChatUIOffset, "设置聊天ui偏移", "chatuioffset"));
            Commands.ChatCommands.Add(new Command("shouyuan.admin", SetTopUIOffset, "设置顶部ui偏移", "topuioffset"));
            Commands.ChatCommands.Add(new Command("shouyuan.admin", AddRealmCondition, "添加境界条件"));
            Commands.ChatCommands.Add(new Command("shouyuan.admin", AddRealmReward, "添加境界奖励"));
            Commands.ChatCommands.Add(new Command("shouyuan.admin", SetTopUIRefreshInterval, "设置顶部ui刷新间隔", "topuirefresh"));
            Commands.ChatCommands.Add(new Command("shouyuan.admin", SetChatUIRefreshInterval, "设置聊天ui刷新间隔", "chatuirefresh"));
            Commands.ChatCommands.Add(new Command("shouyuan.admin", SetStarSignIcon, "设置星宿图标", "setsignicon"));
            Commands.ChatCommands.Add(new Command("shouyuan.admin", AddRealmBuff, "添加境界buff"));
            Commands.ChatCommands.Add(new Command("shouyuan.admin", CRC, new string[] { "重读修仙", "reloadxiuxian" }));
            Commands.ChatCommands.Add(new Command("shouyuan.player", ResetCultivation, "散尽修为"));
            Commands.ChatCommands.Add(new Command("shouyuan.admin", ResetAllPlayersCultivation, "仙道重开"));
            Commands.ChatCommands.Add(new Command("shouyuan.player", SoulReturn, "反魂"));
            Commands.ChatCommands.Add(new Command("shouyuan.admin", AdminSoulReturn, "反魂"));
            Commands.ChatCommands.Add(new Command("shouyuan.player", UpgradeConstellation, "升级命座"));
            Commands.ChatCommands.Add(new Command("shouyuan.player", CheckEventsCommand, "查看事件"));

            CreateDefaultPermissions();
        }

        private void CreateDefaultPermissions()
        {
            try
            {
                if (!TShock.Groups.GroupExists("修仙弟子"))
                {
                    TShock.Groups.AddGroup("修仙弟子", "default", "0,255,0", "修仙系统玩家权限");
                    TShock.Groups.AddPermissions("修仙弟子", new List<string> { "shouyuan.player" });
                }

                if (!TShock.Groups.GroupExists("修仙仙尊"))
                {
                    TShock.Groups.AddGroup("修仙仙尊", "修仙弟子", "255,0,0", "修仙系统管理员权限");
                    TShock.Groups.AddPermissions("修仙仙尊", new List<string> { "shouyuan.player", "shouyuan.admin" });
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"创建权限组失败: {ex.Message}");
            }
        }

        private void OnInitialize(EventArgs args)
        {
            try
            {
                XiuXianConfig.Load(ConfigPath);
                XiuXianData.Load(DataPath);

                _cultivationTimer.Start();
                _lifeCheckTimer.Start();
                _bossBattleCheckTimer.Start();
                _onlineRewardTimer.Start();

                _topUiRefreshTimer.Interval = XiuXianConfig.Instance.TopUIRefreshInterval;
                _topUiRefreshTimer.Start();

                _chatUiRefreshTimer.Interval = XiuXianConfig.Instance.ChatUIRefreshInterval * 60000;
                _chatUiRefreshTimer.Start();

                InitializeEvents();

                TSPlayer.All.SendSuccessMessage($"[{XiuXianConfig.Instance.ServerName}] 天地灵气已复苏！输入 /修仙 开启修炼之路");
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"修仙系统初始化失败: {ex.Message}");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 清理事件钩子
                PlayerHooks.PlayerChat -= OnPlayerChat;
                PlayerHooks.PlayerPostLogin -= OnPlayerPostLogin;
                PlayerHooks.PlayerPreLogin -= OnPlayerPreLogin;
                PlayerHooks.PlayerLogout -= OnPlayerLogout;

                GetDataHandlers.PlayerDamage -= OnPlayerDamage;
                GetDataHandlers.KillMe -= OnDead;

                GeneralHooks.ReloadEvent -= OnReload;

                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnServerJoin);
                ServerApi.Hooks.ServerLeave.Deregister(this, OnServerLeave);
                ServerApi.Hooks.NpcKilled.Deregister(this, OnBossKilled);
                ServerApi.Hooks.NpcSpawn.Deregister(this, OnBossSpawn);
                ServerApi.Hooks.NpcStrike.Deregister(this, OnNpcStrike);
                AccountHooks.AccountCreate -= OnAccountCreate;
                RegionHooks.RegionEntered -= OnRegionEntered;
                RegionHooks.RegionLeft -= OnRegionLeft;

                // 停止并释放定时器
                _cultivationTimer.Stop();
                _cultivationTimer.Dispose();
                _lifeCheckTimer.Stop();
                _lifeCheckTimer.Dispose();
                _topUiRefreshTimer.Stop();
                _topUiRefreshTimer.Dispose();
                _chatUiRefreshTimer.Stop();
                _chatUiRefreshTimer.Dispose();
                _bossBattleCheckTimer.Stop();
                _bossBattleCheckTimer.Dispose();
                _eventCheckTimer.Stop();
                _eventCheckTimer.Dispose();
                _onlineRewardTimer.Stop();
                _onlineRewardTimer.Dispose();

                XiuXianData.Save(DataPath);
            }
            base.Dispose(disposing);
        }
        #endregion

        #region 可视化功能
        private void ShowFullStatus(TSPlayer player, XiuXianData data)
        {
            var realmInfo = XiuXianConfig.Instance.GetRealmInfoForStarSign(data.Realm, data.StarSign);
            var starSign = XiuXianConfig.Instance.StarSigns.FirstOrDefault(s => s.Name == data.StarSign);

            string progressBar = GenerateProgressBar(data.CultivationProgress,
                XiuXianConfig.Instance.GetNextRealmForStarSign(realmInfo, data.StarSign)?.BreakthroughReq ?? 100);

            player.SendMessage("═══════════ 修仙状态 ═══════════", Microsoft.Xna.Framework.Color.Cyan);
            player.SendMessage($"服务器: [c/FF69B4:{XiuXianConfig.Instance.ServerName}]", Microsoft.Xna.Framework.Color.White);

            if (starSign != null)
            {
                player.SendMessage($"星宿: [c/{starSign.Color.Hex3()}:{starSign.Name}] ([c/FFD700:{starSign.TerrariaClass}])", Microsoft.Xna.Framework.Color.White);
            }
            else
            {
                player.SendMessage($"星宿: [c/FF0000:未选择]", Microsoft.Xna.Framework.Color.White);
            }

            string realmDisplay = data.StarSign != "未选择" ? $"{data.Realm}境（{data.StarSign}）" : $"{data.Realm}境";
            player.SendMessage($"境界: [c/00FF00:{realmDisplay}]", Microsoft.Xna.Framework.Color.White);

            var lifeColor = data.LifeYears > 100 ? "00FF00" :
                            data.LifeYears > 1 ? "FFFF00" :
                            "FF0000";
            string lifeText = data.LifeYears > 1 ?
                $"{data.LifeYears:F1}年" :
                "寿元耗尽";
            player.SendMessage($"剩余寿元: [c/{lifeColor}:{lifeText}]", Microsoft.Xna.Framework.Color.White);

            if (!string.IsNullOrEmpty(data.DharmaName))
            {
                player.SendMessage($"法号: [c/FF69B4:{data.DharmaName}]", Microsoft.Xna.Framework.Color.White);
            }

            player.SendMessage($"修炼进度: [c/00BFFF:{data.CultivationProgress}%]", Microsoft.Xna.Framework.Color.White);
            player.SendMessage(progressBar, Microsoft.Xna.Framework.Color.LightBlue);

            player.SendMessage($"命座: [c/9370DB:{data.ConstellationLevel}/7] (点数: {data.ConstellationPoints})", Microsoft.Xna.Framework.Color.White);
            player.SendMessage($"转生次数: [c/FFD700:{data.RebirthCount}]", Microsoft.Xna.Framework.Color.White);
            player.SendMessage($"在线时长: [c/87CEEB:{data.TotalOnlineMinutes}分钟]", Microsoft.Xna.Framework.Color.White);

            if (data.KilledBosses.Count > 0)
            {
                player.SendMessage($"已击杀Boss: {string.Join(", ", data.KilledBosses)}", Microsoft.Xna.Framework.Color.Orange);
            }

            if (data.InHolyLand)
            {
                player.SendMessage("修炼环境: [c/00FF00:灵脉之地]", Microsoft.Xna.Framework.Color.White);
            }

            var activeEvents = _activeEvents.Values.Where(e => e.IsActive).ToList();
            if (activeEvents.Count > 0)
            {
                player.SendMessage("═══════════ 活跃事件 ═══════════", Microsoft.Xna.Framework.Color.Orange);
                foreach (var eventInfo in activeEvents)
                {
                    player.SendMessage($"★ {eventInfo.EventName}", Microsoft.Xna.Framework.Color.Yellow);
                }
            }

            if (data.LifeYears <= 1 && data.LifeDepletionTime != DateTime.MinValue)
            {
                double elapsedSeconds = (DateTime.Now - data.LifeDepletionTime).TotalSeconds;
                double remainingSeconds = XiuXianConfig.Instance.LifeDepletionCountdown - elapsedSeconds;
                
                player.SendMessage("═══════════ 警告 ═══════════", Microsoft.Xna.Framework.Color.Red);
                player.SendMessage($"★★★ 寿元已耗尽！{remainingSeconds:F0}秒后将被踢出 ★★★", Microsoft.Xna.Framework.Color.Red);
                player.SendMessage("★★★ 请立即使用 /修仙转生 ★★★", Microsoft.Xna.Framework.Color.Red);
            }

            player.SendMessage("══════════════════════════════", Microsoft.Xna.Framework.Color.Cyan);
        }

        private void UpdateChatUI(TSPlayer player, XiuXianData data)
        {
            var sb = new StringBuilder();
            var config = XiuXianConfig.Instance;

            int offsetX = config.ChatUIOffsetX;
            int offsetY = config.ChatUIOffsetY;

            if (offsetY > 0)
            {
                sb.Append(new string('\n', offsetY));
            }

            string xOffset = (offsetX > 0) ? new string(' ', offsetX) : "";

            string realmDisplay = data.StarSign != "未选择" ? $"{data.Realm}境（{data.StarSign}）" : $"{data.Realm}境";
            sb.AppendLine($"{xOffset}{$"境界: {realmDisplay}".Color(Color.LightGreen)}");

            if (data.StarSign != "未选择")
            {
                var starSign = config.StarSigns.FirstOrDefault(s => s.Name == data.StarSign);
                if (starSign != null)
                {
                    sb.AppendLine($"{xOffset}{$"星宿: {starSign.Name} ({starSign.TerrariaClass})".Color(starSign.Color)}");
                }
            }

            var lifeColor = data.LifeYears > 100 ? Color.LightGreen :
                           data.LifeYears > 1 ? Color.Yellow :
                           Color.Red;
            string lifeText = data.LifeYears > 1 ?
                $"{data.LifeYears:F1}年" :
                "寿元耗尽";
            sb.AppendLine($"{xOffset}{$"寿元: {lifeText}".Color(lifeColor)}");

            var realmInfo = config.GetRealmInfoForStarSign(data.Realm, data.StarSign);
            var nextRealm = config.GetNextRealmForStarSign(realmInfo, data.StarSign);
            int nextReq = nextRealm?.BreakthroughReq ?? 100;
            sb.AppendLine($"{xOffset}{CreateExpBar(data.CultivationProgress, nextReq, 20, Color.Cyan, Color.Gray, Color.LightBlue)}");

            sb.AppendLine($"{xOffset}{$"命座: {data.ConstellationLevel}/7 (点数:{data.ConstellationPoints})".Color(Color.Purple)}");
            sb.AppendLine($"{xOffset}{$"在线: {data.TotalOnlineMinutes}分钟".Color(Color.SkyBlue)}");

            if (!string.IsNullOrEmpty(data.DharmaName))
            {
                sb.AppendLine($"{xOffset}{$"法号: {data.DharmaName}".Color(Color.HotPink)}");
            }

            var activeEvents = _activeEvents.Values.Where(e => e.IsActive).ToList();
            if (activeEvents.Count > 0)
            {
                sb.AppendLine($"{xOffset}{$"事件: {string.Join(", ", activeEvents.Select(e => e.EventName))}".Color(Color.Orange)}");
            }

            if (data.LifeYears <= 1 && data.LifeDepletionTime != DateTime.MinValue)
            {
                double elapsedSeconds = (DateTime.Now - data.LifeDepletionTime).TotalSeconds;
                double remainingSeconds = XiuXianConfig.Instance.LifeDepletionCountdown - elapsedSeconds;
                sb.AppendLine($"{xOffset}{$"★★★ {remainingSeconds:F0}秒后踢出 ★★★".Color(Color.Red)}");
            }

            player.SendMessage(sb.ToString(), Color.White);
        }

        private void UpdateTopUI(TSPlayer player, XiuXianData data)
        {
            var sb = new StringBuilder();
            var config = XiuXianConfig.Instance;

            if (config.TopUIOffsetY > 0)
            {
                sb.Append(new string('\n', config.TopUIOffsetY));
            }

            int absXOffset = Math.Abs(config.TopUIOffsetX);
            string xOffset = new string(' ', absXOffset);

            Func<string, string> applyXOffset = line =>
            {
                if (config.TopUIOffsetX < 0)
                    return line + xOffset;
                else
                    return xOffset + line;
            };

            sb.AppendLine(applyXOffset($"玩家名称: {player.Name}".Color(Color.LightSkyBlue)));
            
            string realmDisplay = data.StarSign != "未选择" ? $"{data.Realm}境（{data.StarSign}）" : $"{data.Realm}境";
            sb.AppendLine(applyXOffset($"玩家境界: {realmDisplay}".Color(Color.LightGreen)));
            sb.AppendLine(applyXOffset($"转生次数: {data.RebirthCount}".Color(Color.Yellow)));
            sb.AppendLine(applyXOffset($"在线时长: {data.TotalOnlineMinutes}分钟".Color(Color.SkyBlue)));

            var realmInfo = config.GetRealmInfoForStarSign(data.Realm, data.StarSign);
            var nextRealm = config.GetNextRealmForStarSign(realmInfo, data.StarSign);
            int expMax = nextRealm?.BreakthroughReq ?? 100;
            sb.AppendLine(applyXOffset(CreateExpBar(data.CultivationProgress, expMax, 20, Color.Cyan, Color.DarkSlateGray, Color.LightBlue, "修炼进度: ")));

            string starIcon = "[i:65]";
            if (data.StarSign != "未选择")
            {
                var starSign = config.StarSigns.FirstOrDefault(s => s.Name == data.StarSign);
                if (starSign != null && starSign.Icon != 0)
                {
                    starIcon = $"[i:{starSign.Icon}]";
                }
            }

            var lifeColor = data.LifeYears > 100 ? Color.LightGreen :
                            data.LifeYears > 1 ? Color.Yellow :
                            Color.Red;
            string lifeText = data.LifeYears > 1 ?
                $"{data.LifeYears:F1}年" :
                "寿元耗尽";
            sb.AppendLine(applyXOffset($"{starIcon} {$"寿元: {lifeText}".Color(lifeColor)}"));

            sb.AppendLine(applyXOffset($"命座: {data.ConstellationLevel}/7 (点数:{data.ConstellationPoints})".Color(Color.Purple)));

            if (!string.IsNullOrEmpty(data.DharmaName))
                sb.AppendLine(applyXOffset($"法号: {data.DharmaName}".Color(Color.HotPink)));

            var activeEvents = _activeEvents.Values.Where(e => e.IsActive).ToList();
            if (activeEvents.Count > 0)
            {
                sb.AppendLine(applyXOffset($"事件: {string.Join(", ", activeEvents.Select(e => e.EventName))}".Color(Color.Orange)));
            }

            if (data.LifeYears <= 1 && data.LifeDepletionTime != DateTime.MinValue)
            {
                double elapsedSeconds = (DateTime.Now - data.LifeDepletionTime).TotalSeconds;
                double remainingSeconds = XiuXianConfig.Instance.LifeDepletionCountdown - elapsedSeconds;
                sb.AppendLine(applyXOffset($"⚠ {remainingSeconds:F0}秒后踢出".Color(Color.Red)));
            }

            statusManager.AddOrUpdateText(player, "top_xiuxian_info", sb.ToString().TrimEnd());
        }

        private string CreateExpBar(int current, int max, int width,
            Color filledColor, Color emptyColor, Color textColor, string prefix = "")
        {
            if (max <= 0) max = 1;
            double percent = Math.Min(1.0, (double)current / max);

            int filledBlocks = (int)Math.Ceiling(percent * width);
            if (percent > 0 && filledBlocks == 0) filledBlocks = 1;
            if (current == 0) filledBlocks = 0;

            int emptyBlocks = width - filledBlocks;

            return $"{prefix}{$"【".Color(filledColor)}" +
                   $"{new string('=', filledBlocks).Color(filledColor)}" +
                   $"{new string(' ', emptyBlocks).Color(emptyColor)}" +
                   $"{$"】".Color(filledColor)} " +
                   $"{$"{(int)(percent * 100)}%".Color(textColor)}";
        }

        private string GenerateProgressBar(int progress, int max)
        {
            int barLength = 20;
            int filled = (int)Math.Round((double)progress / max * barLength);
            filled = Math.Min(barLength, Math.Max(0, filled));

            var sb = new StringBuilder("[");
            sb.Append(new string('|', filled));
            sb.Append(new string('.', barLength - filled));
            sb.Append($"] {progress}/{max} ({progress * 100 / max}%)");

            return sb.ToString();
        }
        #endregion

        #region UI刷新功能
        private void RefreshTopUIForAllPlayers(object sender, ElapsedEventArgs e)
        {
            try
            {
                foreach (var player in TShock.Players.Where(p => p != null && p.Active && p.IsLoggedIn))
                {
                    try
                    {
                        var data = XiuXianData.GetPlayer(player.Name);
                        UpdateTopUI(player, data);
                    }
                    catch (Exception ex)
                    {
                        TShock.Log.Error($"刷新 {player.Name} 的顶部UI失败: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"刷新顶部UI失败: {ex.Message}");
            }
        }

        private void RefreshChatUIForAllPlayers(object sender, ElapsedEventArgs e)
        {
            try
            {
                foreach (var player in TShock.Players.Where(p => p != null && p.Active && p.IsLoggedIn))
                {
                    try
                    {
                        var data = XiuXianData.GetPlayer(player.Name);
                        UpdateChatUI(player, data);
                    }
                    catch (Exception ex)
                    {
                        TShock.Log.Error($"刷新 {player.Name} 的聊天UI失败: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"刷新聊天UI失败: {ex.Message}");
            }
        }
        #endregion

        #region 指令实现
        private void CheckEventsCommand(CommandArgs args)
        {
            var sb = new StringBuilder();
            sb.AppendLine("══════════ 当前事件 ══════════");
            
            bool hasActiveEvents = false;
            foreach (var eventInfo in _activeEvents.Values)
            {
                if (eventInfo.IsActive)
                {
                    hasActiveEvents = true;
                    
                    if (eventInfo.IsTimeBased)
                    {
                        var timeLeft = eventInfo.EndTime - DateTime.Now;
                        sb.AppendLine($"{eventInfo.EventName}事件 - 剩余时间: {timeLeft:hh\\:mm\\:ss}");
                    }
                    else
                    {
                        sb.AppendLine($"{eventInfo.EventName}事件 - 进行中");
                    }
                    
                    sb.AppendLine($"参与方式: 在事件期间击杀对应怪物可获得命座点数");
                    sb.AppendLine($"当前参与人数: {eventInfo.Participants.Count}");
                    sb.AppendLine("─");
                }
            }

            if (!hasActiveEvents)
            {
                sb.AppendLine("当前无活跃事件");
                sb.AppendLine("定期事件:");
                sb.AppendLine("- 血月: 每月1-7号晚上");
                sb.AppendLine("- 南瓜月: 每月15-21号晚上");
                sb.AppendLine("- 霜月: 每月25-31号晚上");
                sb.AppendLine("");
                sb.AppendLine("游戏内事件:");
                sb.AppendLine("- 日食、哥布林军团、海盗入侵、雪人军团");
                sb.AppendLine("- 火星人入侵、撒旦军队、史莱姆雨等");
            }

            sb.AppendLine("══════════════════════════════");

            args.Player.SendMessage(sb.ToString(), Color.Cyan);
        }

        private void UpgradeConstellation(CommandArgs args)
        {
            var data = XiuXianData.GetPlayer(args.Player.Name);

            if (data.StarSign == "未选择")
            {
                args.Player.SendErrorMessage("请先选择星宿！");
                return;
            }

            if (data.ConstellationLevel >= 7)
            {
                args.Player.SendErrorMessage("命座已满级！");
                return;
            }

            int pointsRequired = (data.ConstellationLevel + 1) * 10;

            if (data.ConstellationPoints < pointsRequired)
            {
                args.Player.SendErrorMessage($"升级命座需要 {pointsRequired} 点命座点数，当前只有 {data.ConstellationPoints} 点");
                args.Player.SendInfoMessage("通过参与各种事件可获得命座点数");
                return;
            }

            data.ConstellationPoints -= pointsRequired;
            data.ConstellationLevel++;

            var constellationEffects = XiuXianConfig.Instance.GetConstellationEffects(data.StarSign, data.ConstellationLevel);
            if (constellationEffects.Count >= data.ConstellationLevel)
            {
                string effect = constellationEffects[data.ConstellationLevel - 1];
                args.Player.SendMessage($"★★★ 命座升级: {data.ConstellationLevel}/7 ★★★", Microsoft.Xna.Framework.Color.Cyan);
                args.Player.SendInfoMessage($"获得命座效果: {effect}");
            }

            ApplyConstellationEffects(args.Player, data);

            UpdateChatUI(args.Player, data);
            UpdateTopUI(args.Player, data);
        }

        private void ApplyConstellationEffects(TSPlayer player, XiuXianData data)
        {
            try
            {
                if (data.ConstellationLevel > 0)
                {
                    var buffs = XiuXianConfig.Instance.GetConstellationBuffs(data.StarSign, data.ConstellationLevel);
                    
                    foreach (var buff in buffs)
                    {
                        player.SetBuff(buff.BuffID, buff.Duration * 60);
                        player.SendSuccessMessage($"获得命座Buff: {TShock.Utils.GetBuffName(buff.BuffID)}");
                    }
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"应用命座效果失败: {ex.Message}");
            }
        }

        private void SetStarSignIcon(CommandArgs args)
        {
            if (args.Parameters.Count < 2)
            {
                args.Player.SendInfoMessage("用法: /设置星宿图标 <星宿名称> <物品ID>");
                args.Player.SendInfoMessage("示例: /设置星宿图标 紫微帝星 5005");
                return;
            }

            string signName = args.Parameters[0];
            if (!int.TryParse(args.Parameters[1], out int itemId))
            {
                args.Player.SendErrorMessage("无效的物品ID，必须为整数");
                return;
            }

            var sign = XiuXianConfig.Instance.StarSigns.FirstOrDefault(s => s.Name.Equals(signName, StringComparison.OrdinalIgnoreCase));
            if (sign == null)
            {
                args.Player.SendErrorMessage($"找不到星宿: {signName}");
                return;
            }

            sign.Icon = itemId;
            XiuXianConfig.Save(ConfigPath);

            args.Player.SendSuccessMessage($"已将星宿 {sign.Name} 的图标设置为物品ID: {itemId}");

            foreach (var player in TShock.Players.Where(p => p != null && p.Active))
            {
                var data = XiuXianData.GetPlayer(player.Name);
                UpdateTopUI(player, data);
            }
        }

        private void ShowStatus(CommandArgs args)
        {
            try
            {
                var data = XiuXianData.GetPlayer(args.Player.Name);
                ShowFullStatus(args.Player, data);
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"显示状态失败: {ex.Message}");
            }
        }

        private void SetServerName(CommandArgs args)
        {
            if (args.Parameters.Count == 0)
            {
                args.Player.SendInfoMessage($"当前服务器名称: {XiuXianConfig.Instance.ServerName}");
                args.Player.SendInfoMessage("用法: /设置服务器名称 <新名称>");
                return;
            }

            string newName = string.Join(" ", args.Parameters);
            if (newName.Length > 30)
            {
                args.Player.SendErrorMessage("服务器名称过长，最多30个字符");
                return;
            }

            XiuXianConfig.Instance.ServerName = newName;
            XiuXianConfig.Save(ConfigPath);

            args.Player.SendSuccessMessage($"服务器名称已更新为: {newName}");
            TSPlayer.All.SendMessage($"★★★ 服务器更名为: {newName} ★★★", Microsoft.Xna.Framework.Color.Gold);
        }

        private void SetChatUIOffset(CommandArgs args)
        {
            if (args.Parameters.Count < 2)
            {
                args.Player.SendInfoMessage("用法: /设置聊天ui偏移 <X偏移> <Y偏移>");
                args.Player.SendInfoMessage($"当前偏移: X={XiuXianConfig.Instance.ChatUIOffsetX}, Y={XiuXianConfig.Instance.ChatUIOffsetY}");
                return;
            }

            if (!int.TryParse(args.Parameters[0], out int x) || !int.TryParse(args.Parameters[1], out int y))
            {
                args.Player.SendErrorMessage("无效的偏移值，必须为整数");
                return;
            }

            if (x < -100 || y < -100 || x > 100 || y > 100)
            {
                args.Player.SendErrorMessage("偏移值范围: X(-100-100), Y(-100-100)");
                return;
            }

            XiuXianConfig.Instance.ChatUIOffsetX = x;
            XiuXianConfig.Instance.ChatUIOffsetY = y;
            XiuXianConfig.Save(ConfigPath);

            args.Player.SendSuccessMessage($"聊天UI偏移已更新: X={x}, Y={y}");

            foreach (var player in TShock.Players.Where(p => p != null && p.Active))
            {
                var data = XiuXianData.GetPlayer(player.Name);
                UpdateChatUI(player, data);
            }
        }

        private void SetTopUIOffset(CommandArgs args)
        {
            if (args.Parameters.Count < 2)
            {
                args.Player.SendInfoMessage("用法: /设置顶部ui偏移 <X偏移> <Y偏移>");
                args.Player.SendInfoMessage($"当前偏移: X={XiuXianConfig.Instance.TopUIOffsetX}, Y={XiuXianConfig.Instance.TopUIOffsetY}");
                return;
            }

            if (!int.TryParse(args.Parameters[0], out int x) || !int.TryParse(args.Parameters[1], out int y))
            {
                args.Player.SendErrorMessage("无效的偏移值，必须为整数");
                return;
            }

            if (x < -1000 || y < -1000 || x > 100 || y > 100)
            {
                args.Player.SendErrorMessage("偏移值范围: X(-1000-100), Y(-1000-100)");
                return;
            }

            XiuXianConfig.Instance.TopUIOffsetX = x;
            XiuXianConfig.Instance.TopUIOffsetY = y;
            XiuXianConfig.Save(ConfigPath);

            args.Player.SendSuccessMessage($"顶部UI偏移已更新: X={x}, Y={y}");

            foreach (var player in TShock.Players.Where(p => p != null && p.Active))
            {
                var data = XiuXianData.GetPlayer(player.Name);
                UpdateTopUI(player, data);
            }
        }

        private void SetTopUIRefreshInterval(CommandArgs args)
        {
            if (args.Parameters.Count == 0)
            {
                args.Player.SendInfoMessage($"当前顶部UI刷新间隔: {XiuXianConfig.Instance.TopUIRefreshInterval}毫秒");
                args.Player.SendInfoMessage("用法: /设置顶部ui刷新间隔 <毫秒数>");
                return;
            }

            if (!int.TryParse(args.Parameters[0], out int interval) || interval < 1 || interval > 3600000)
            {
                args.Player.SendErrorMessage("无效的间隔值，必须为1-3600000之间的整数（1秒-1小时）");
                return;
            }

            XiuXianConfig.Instance.TopUIRefreshInterval = interval;
            XiuXianConfig.Save(ConfigPath);

            _topUiRefreshTimer.Interval = interval;

            args.Player.SendSuccessMessage($"顶部UI刷新间隔已更新为: {interval}毫秒");
        }

        private void SetChatUIRefreshInterval(CommandArgs args)
        {
            if (args.Parameters.Count == 0)
            {
                args.Player.SendInfoMessage($"当前聊天UI刷新间隔: {XiuXianConfig.Instance.ChatUIRefreshInterval}分钟");
                args.Player.SendInfoMessage("用法: /设置聊天ui刷新间隔 <分钟数>");
                return;
            }

            if (!int.TryParse(args.Parameters[0], out int minutes) || minutes < 1 || minutes > 3600000)
            {
                args.Player.SendErrorMessage("无效的间隔值，必须为1-3600000之间的整数（分钟）");
                return;
            }

            XiuXianConfig.Instance.ChatUIRefreshInterval = minutes;
            XiuXianConfig.Save(ConfigPath);

            _chatUiRefreshTimer.Interval = minutes * 60000;

            args.Player.SendSuccessMessage($"聊天UI刷新间隔已更新为: {minutes}分钟");
        }

        private void AdminForceRebirth(CommandArgs args)
        {
            if (args.Parameters.Count == 0)
            {
                args.Player.SendInfoMessage("用法: /寿元转生 <玩家名>");
                return;
            }

            string playerName = string.Join(" ", args.Parameters);
            var targetData = XiuXianData.GetPlayer(playerName);
            if (targetData == null)
            {
                args.Player.SendErrorMessage($"找不到玩家: {playerName}");
                return;
            }

            targetData.RebirthCount++;
            
            if (targetData.LifeYears <= 1)
            {
                targetData.LifeYears = XiuXianConfig.Instance.InitialLifeYears;
            }
            else
            {
                targetData.LifeYears = 60 + (targetData.RebirthCount * 20);
            }
            
            targetData.CultivationProgress = 0;
            targetData.Realm = "凡人";
            targetData.ConstellationLevel = 0;
            targetData.ConstellationPoints = 0;
            targetData.KilledBosses.Clear();
            targetData.LifeDepletionWarned = false;
            targetData.LifeDepletionTime = DateTime.MinValue;
            targetData.IsGhostState = false;
            targetData.GhostBattleNpcIndex = -1;
            
            XiuXianData.SavePlayer(targetData.Name);

            args.Player.SendSuccessMessage($"已为 {playerName} 转生，当前寿元: {targetData.LifeYears:F1}年");

            var onlinePlayer = TSPlayer.FindByNameOrID(playerName);
            if (onlinePlayer.Count > 0)
            {
                onlinePlayer[0].SendSuccessMessage($"管理员已为你转生！当前寿元: {targetData.LifeYears:F1}年");
                UpdateChatUI(onlinePlayer[0], targetData);
                UpdateTopUI(onlinePlayer[0], targetData);
            }
        }

        private void AdminAdjustLife(CommandArgs args)
        {
            if (args.Parameters.Count < 2)
            {
                args.Player.SendErrorMessage("格式: /调整寿元 <玩家名> <数值>");
                return;
            }

            if (!int.TryParse(args.Parameters[1], out int amount))
            {
                args.Player.SendErrorMessage("无效的数值");
                return;
            }

            string playerName = args.Parameters[0];
            var targetData = XiuXianData.GetPlayer(playerName);
            if (targetData == null)
            {
                args.Player.SendErrorMessage($"找不到玩家: {playerName}");
                return;
            }

            targetData.LifeYears += amount;

            if (targetData.LifeYears > 1)
            {
                targetData.LifeDepletionWarned = false;
                targetData.LifeDepletionTime = DateTime.MinValue;
            }

            args.Player.SendSuccessMessage($"已将 {playerName} 寿元调整为: {targetData.LifeYears:F1}年");

            var onlinePlayer = TSPlayer.FindByNameOrID(playerName);
            if (onlinePlayer.Count > 0)
            {
                onlinePlayer[0].SendInfoMessage($"管理员已将你的寿元调整为: {targetData.LifeYears:F1}年");
                UpdateChatUI(onlinePlayer[0], targetData);
                UpdateTopUI(onlinePlayer[0], targetData);
            }
        }

        private void SetDharmaName(CommandArgs args)
        {
            if (args.Parameters.Count == 0)
            {
                args.Player.SendInfoMessage("用法: /法号 <自定义法号>");
                args.Player.SendInfoMessage($"当前法号: {(string.IsNullOrEmpty(args.Player.Name) ? "未设置" : args.Player.Name)}");
                return;
            }

            string newName = string.Join(" ", args.Parameters);
            if (newName.Length > 15)
            {
                args.Player.SendErrorMessage("法号过长，最多15个字符");
                return;
            }

            var data = XiuXianData.GetPlayer(args.Player.Name);
            data.DharmaName = newName;
            data.NameColor = GetRandomColorHex();

            args.Player.SendSuccessMessage($"法号已更新为: {newName}");
            UpdateChatUI(args.Player, data);
            UpdateTopUI(args.Player, data);
        }

        private string GetRandomColorHex()
        {
            var colors = new[] { "FF69B4", "00BFFF", "7CFC00", "FFD700", "9370DB", "FF6347", "20B2AA" };
            return colors[new Random().Next(colors.Length)];
        }

        private void ChooseStarSign(CommandArgs args)
        {
            var data = XiuXianData.GetPlayer(args.Player.Name);

            if (data.StarSign != "未选择")
            {
                args.Player.SendErrorMessage("星宿已选定，无法更改！");
                return;
            }

            if (args.Parameters.Count == 0)
            {
                args.Player.SendMessage("════════════ 星宿职业 ════════════", Microsoft.Xna.Framework.Color.Yellow);

                foreach (var sign in XiuXianConfig.Instance.StarSigns)
                {
                    args.Player.SendMessage($"{sign.Name} ({sign.TerrariaClass}): {sign.ShortDesc}", sign.Color);
                }

                args.Player.SendMessage("══════════════════════════════════", Microsoft.Xna.Framework.Color.Yellow);
                args.Player.SendInfoMessage("用法: /选择星宿 <星宿名称>");
                return;
            }

            string signName = string.Join(" ", args.Parameters);
            var selectedSign = XiuXianConfig.Instance.StarSigns.FirstOrDefault(s => s.Name.Equals(signName, StringComparison.OrdinalIgnoreCase));

            if (selectedSign == null)
            {
                args.Player.SendErrorMessage("无效的星宿名称，请重新选择");
                return;
            }

            data.StarSign = selectedSign.Name;
            args.Player.SendMessage($"★★★ 星宿觉醒: {selectedSign.Name} ★★★", selectedSign.Color);
            args.Player.SendMessage($"职业: {selectedSign.TerrariaClass}", Microsoft.Xna.Framework.Color.LightGreen);
            args.Player.SendMessage($"特性: {selectedSign.Description}", Microsoft.Xna.Framework.Color.Yellow);
            args.Player.SendSuccessMessage("你的修仙之路正式开始！输入 /状态 查看完整信息");

            ApplyStarSignInitialBuffs(args.Player, selectedSign);

            UpdateChatUI(args.Player, data);
            UpdateTopUI(args.Player, data);
        }

        private void ApplyStarSignInitialBuffs(TSPlayer player, StarSignInfo starSign)
        {
            try
            {
                if (starSign.InitialBuffs != null)
                {
                    foreach (var buff in starSign.InitialBuffs)
                    {
                        player.SetBuff(buff.BuffID, buff.Duration * 60);
                        player.SendSuccessMessage($"获得星宿Buff: {TShock.Utils.GetBuffName(buff.BuffID)} ({buff.Duration}秒)");
                    }
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"应用星宿初始Buff失败: {ex.Message}");
            }
        }

        private void ViewConstellation(CommandArgs args)
        {
            var data = XiuXianData.GetPlayer(args.Player.Name);
            args.Player.SendInfoMessage($"命座等级: {data.ConstellationLevel}/7");
            args.Player.SendInfoMessage($"命座点数: {data.ConstellationPoints}");
            args.Player.SendMessage("命座效果:", Microsoft.Xna.Framework.Color.Cyan);

            var constellationEffects = XiuXianConfig.Instance.GetConstellationEffects(data.StarSign, data.ConstellationLevel);

            for (int i = 0; i < data.ConstellationLevel; i++)
            {
                if (i < constellationEffects.Count)
                {
                    string effect = constellationEffects[data.ConstellationLevel - 1];
                    args.Player.SendMessage($"★ {effect}", Microsoft.Xna.Framework.Color.LightBlue);
                }
            }

            if (data.ConstellationLevel < 7)
            {
                var nextEffects = XiuXianConfig.Instance.GetConstellationEffects(data.StarSign, data.ConstellationLevel + 1);
                if (nextEffects.Count > data.ConstellationLevel)
                {
                    int pointsRequired = (data.ConstellationLevel + 1) * 10;
                    args.Player.SendMessage($"下一级需要: {pointsRequired}点命座点数", Microsoft.Xna.Framework.Color.Yellow);
                    args.Player.SendMessage($"下一级效果: {nextEffects[data.ConstellationLevel]}", Microsoft.Xna.Framework.Color.LightGreen);
                }
            }

            ApplyConstellationEffects(args.Player, data);
        }

        private void ProcessRebirth(TSPlayer player, XiuXianData data)
        {
            bool isFreeRebirth = data.LifeYears <= 1;
            
            if (!isFreeRebirth && data.Realm != "凡人")
            {
                player.SendErrorMessage("需散尽修为才可转世重修！");
                return;
            }
            
            if (!isFreeRebirth && data.LifeYears < XiuXianConfig.Instance.RebirthCost)
            {
                player.SendErrorMessage($"转生需要{XiuXianConfig.Instance.RebirthCost}年寿元！");
                return;
            }
            
            data.RebirthCount++;
            
            if (isFreeRebirth)
            {
                data.LifeYears = XiuXianConfig.Instance.InitialLifeYears;
                player.SendSuccessMessage($"★ 寿元耗尽，转世成功！获得新生寿元: {data.LifeYears:F1}年 ★");
            }
            else
            {
                data.LifeYears = 60 + (data.RebirthCount * 20);
                player.SendSuccessMessage($"★ 转世成功！获得新生寿元: {data.LifeYears:F1}年 ★");
            }
            
            data.ConstellationLevel = 0;
            data.ConstellationPoints = 0;
            data.KilledBosses.Clear();
            data.LifeDepletionWarned = false;
            data.LifeDepletionTime = DateTime.MinValue;
            data.IsGhostState = false;
            data.GhostBattleNpcIndex = -1;

            UpdateChatUI(player, data);
            UpdateTopUI(player, data);
        }

        private void Cultivate(object sender, ElapsedEventArgs e)
        {
            foreach (var player in TShock.Players.Where(p => p != null && p.Active && p.IsLoggedIn))
            {
                try
                {
                    var data = XiuXianData.GetPlayer(player.Name);
                    var realmInfo = XiuXianConfig.Instance.GetRealmInfoForStarSign(data.Realm, data.StarSign);

                    if (data.StarSign == "未选择") continue;

                    var starSign = XiuXianConfig.Instance.StarSigns.FirstOrDefault(s => s.Name == data.StarSign);
                    if (starSign == null) continue;

                    float multiplier = data.InHolyLand ? 2.0f : 1.0f;
                    float signBonus = starSign.CultivationBonus;
                    float constellationBonus = 1.0f + (data.ConstellationLevel * 0.05f);

                    data.CultivationProgress += (int)(0.5f * multiplier * constellationBonus * (1 + signBonus));

                    float lifeReduction = 0.001f;
                    data.LifeYears = Math.Max(1, data.LifeYears - lifeReduction);

                    if (data.LifeYears <= 1 && !player.Group.HasPermission("shouyuan.admin"))
                    {
                        if (data.LifeDepletionTime == DateTime.MinValue)
                        {
                            data.LifeDepletionTime = DateTime.Now;
                            player.SendWarningMessage($"★★★ 寿元已耗尽！{XiuXianConfig.Instance.LifeDepletionCountdown}秒后将被踢出服务器 ★★★");
                        }
                        continue;
                    }

                    if (DateTime.Now.Minute == 0 && DateTime.Now.Second < 30)
                    {
                        player.SendInfoMessage("灵潮涌动！修炼速度大幅提升");
                        data.CultivationProgress += 30;
                    }

                    UpdateChatUI(player, data);
                    UpdateTopUI(player, data);
                }
                catch (Exception ex)
                {
                    TShock.Log.Error($"{player.Name}修炼失败: {ex.Message}");
                }
            }
        }

        private void CultivationCommand(CommandArgs args)
        {
            if (args.Parameters.Count == 0)
            {
                args.Player.SendInfoMessage("修仙指令: /修仙 [状态|修炼|突破|转生|星宿|命座|事件]");
                return;
            }

            string subcmd = args.Parameters[0].ToLower();
            var data = XiuXianData.GetPlayer(args.Player.Name);

            if (data.StarSign == "未选择" && subcmd != "选择星宿")
            {
                args.Player.SendErrorMessage("请先使用 /选择星宿 确定你的修仙流派");
                return;
            }

            var realmInfo = XiuXianConfig.Instance.GetRealmInfoForStarSign(data.Realm, data.StarSign);
            var starSign = XiuXianConfig.Instance.StarSigns.FirstOrDefault(s => s.Name == data.StarSign);

            switch (subcmd)
            {
                case "状态":
                    ShowFullStatus(args.Player, data);
                    break;
                case "修炼":
                    ProcessMeditation(args.Player, data, starSign);
                    break;
                case "突破":
                    ProcessBreakthrough(args.Player, data, realmInfo, starSign);
                    break;
                case "转生":
                    ProcessRebirth(args.Player, data);
                    break;
                case "星宿":
                    ChooseStarSign(args);
                    break;
                case "命座":
                    ViewConstellation(args);
                    break;
                case "事件":
                    CheckEventsCommand(args);
                    break;
                default:
                    args.Player.SendErrorMessage("未知指令，可用: 状态, 修炼, 突破, 转生, 星宿, 命座, 事件");
                    break;
            }
        }

        private void ProcessMeditation(TSPlayer player, XiuXianData data, StarSignInfo starSign)
        {
            if (data.LifeYears < 0.1f)
            {
                player.SendErrorMessage("寿元不足无法修炼！");
                return;
            }

            data.LifeYears -= 0.1f;
            int baseGain = new Random().Next(5, 15);
            int gain = (int)(baseGain * (1 + starSign.CultivationBonus));
            gain = (int)(gain * (1.0f + data.ConstellationLevel * 0.05f));
            float lifeGain = gain * 0.1f;
            data.CultivationProgress += gain;
            data.LifeYears += lifeGain;

            player.SendSuccessMessage($"修炼成功！修为+{gain}%，寿元+{lifeGain:F1}年");
            player.SendMessage($"星宿「{starSign.Name}」加成: +{(starSign.CultivationBonus * 100):F0}%", starSign.Color);

            if (data.LifeYears <= 1 && !player.Group.HasPermission("shouyuan.admin"))
            {
                if (data.LifeDepletionTime == DateTime.MinValue)
                {
                    data.LifeDepletionTime = DateTime.Now;
                    player.SendWarningMessage($"★★★ 寿元已耗尽！{XiuXianConfig.Instance.LifeDepletionCountdown}秒后将被踢出服务器 ★★★");
                }
                return;
            }

            UpdateChatUI(player, data);
            UpdateTopUI(player, data);
        }

        private void ProcessBreakthrough(TSPlayer player, XiuXianData data, RealmInfo realm, StarSignInfo starSign)
        {
            var next = XiuXianConfig.Instance.GetNextRealmForStarSign(realm, data.StarSign);
            if (next == null)
            {
                player.SendErrorMessage("已至大道巅峰，无法突破");
                return;
            }

            var unmetConditions = GetUnmetBreakthroughConditions(next, data);
            if (unmetConditions.Count > 0)
            {
                player.SendErrorMessage($"突破条件未满足！请完成以下要求:");
                foreach (var condition in unmetConditions)
                {
                    player.SendErrorMessage($"- 击败 {condition}");
                }
                return;
            }

            if (data.CultivationProgress < next.BreakthroughReq)
            {
                player.SendErrorMessage($"突破{next.Name}需要{next.BreakthroughReq}%修为");
                return;
            }

            double adjustedRate = next.SuccessRate;
            if (starSign.TerrariaClass == "战士" || starSign.TerrariaClass == "射手")
                adjustedRate *= 1.1;

            if (new Random().NextDouble() < adjustedRate)
            {
                data.Realm = next.Name;
                data.CultivationProgress = 0;
                data.LifeYears += next.LifeBonus;

                string currentRealmDisplay = realm.Name + "境（" + data.StarSign + "）";
                string nextRealmDisplay = next.Name + "境（" + data.StarSign + "）";
                player.SendMessage($"★★★★ {currentRealmDisplay}→{nextRealmDisplay} ★★★★", Microsoft.Xna.Framework.Color.Gold);
                player.SendMessage($"星宿「{starSign.Name}」护佑突破！", starSign.Color);
                player.SendSuccessMessage($"寿元增加{next.LifeBonus}年! 当前: {data.LifeYears:F1}年");

                if (next.RewardGoods != null && next.RewardGoods.Length > 0)
                {
                    player.SendSuccessMessage($"获得{next.RewardGoods.Length}种突破奖励!");
                    foreach (var item in next.RewardGoods)
                    {
                        int stack = IsMaterialOrOre(item.NetID) ? (int)Math.Ceiling(item.Stack / 2.0) : item.Stack;
                        
                        player.GiveItem(item.NetID, stack, item.Prefix);
                        player.SendSuccessMessage($"获得突破奖励: {TShock.Utils.GetItemById(item.NetID).Name} x{stack}");
                    }
                }
                else
                {
                    player.SendInfoMessage("本次突破未获得物品奖励");
                }

                if (next.RewardBuffs != null && next.RewardBuffs.Length > 0)
                {
                    foreach (var buff in next.RewardBuffs)
                    {
                        player.SetBuff(buff.BuffID, buff.Duration * 60);
                        player.SendSuccessMessage($"获得境界Buff: {TShock.Utils.GetBuffName(buff.BuffID)} ({buff.Duration}秒)");
                    }
                }

                if (XiuXianConfig.Instance.BroadcastBreakthrough)
                {
                    string realmDisplay = next.Name + "境（" + data.StarSign + "）";
                    TSPlayer.All.SendMessage($"{player.Name}突破{realmDisplay}，天地异象！", Microsoft.Xna.Framework.Color.Yellow);
                }
            }
            else
            {
                int damage = new Random().Next(10, 30);
                data.LifeYears = Math.Max(1, data.LifeYears - damage);
                player.SendErrorMessage($"★★ 天劫降临！损失{damage}年寿元 ★★");
                player.SendMessage($"星宿「{starSign.Name}」为你抵挡部分天劫", starSign.Color);

                if (data.LifeYears <= 1 && !player.Group.HasPermission("shouyuan.admin"))
                {
                    if (data.LifeDepletionTime == DateTime.MinValue)
                    {
                        data.LifeDepletionTime = DateTime.Now;
                        player.SendWarningMessage($"★★★ 寿元已耗尽！{XiuXianConfig.Instance.LifeDepletionCountdown}秒后将被踢出服务器 ★★★");
                    }
                    return;
                }
            }

            UpdateChatUI(player, data);
            UpdateTopUI(player, data);
        }

        private bool IsMaterialOrOre(int itemId)
        {
            int[] materialAndOreIds = {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                21, 22, 23, 24, 25, 26, 27, 28, 29, 30,
                31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
                41, 42, 43, 44, 45, 46, 47, 48, 49, 50,
                
                75, 76, 77, 78, 79, 80, 81, 82, 83, 84,
                85, 86, 87, 88, 89, 90, 91, 92, 93, 94,
                95, 96, 97, 98, 99, 100, 101, 102, 103, 104,
                105, 106, 107, 108, 109, 110, 111, 112, 113, 114,
                115, 116, 117, 118, 119, 120,
                
                121, 122, 123, 124, 125, 126, 127, 128, 129, 130,
                131, 132, 133, 134, 135, 136, 137, 138, 139, 140,
                141, 142, 143, 144, 145, 146, 147, 148, 149, 150,
            };
            
            return materialAndOreIds.Contains(itemId);
        }

        private List<string> GetUnmetBreakthroughConditions(RealmInfo nextRealm, XiuXianData data)
        {
            var unmet = new List<string>();
            if (nextRealm.ProgressLimits == null || nextRealm.ProgressLimits.Count == 0)
                return unmet;

            foreach (var bossName in nextRealm.ProgressLimits)
            {
                string normalizedBossName = NormalizeBossName(bossName);
                bool hasKilled = false;
                
                foreach (var killedBoss in data.KilledBosses)
                {
                    if (NormalizeBossName(killedBoss) == normalizedBossName)
                    {
                        hasKilled = true;
                        break;
                    }
                }
                
                if (!hasKilled)
                {
                    unmet.Add(bossName);
                }
            }

            return unmet;
        }

        private void AddRealmCondition(CommandArgs args)
        {
            if (args.Parameters.Count < 2)
            {
                args.Player.SendInfoMessage("用法: /添加境界条件 <境界名称> <Boss名称>");
                args.Player.SendInfoMessage("示例: /添加境界条件 筑基 \"克苏鲁之眼\"");
                return;
            }

            string realmName = args.Parameters[0];
            string bossName = string.Join(" ", args.Parameters.Skip(1));

            var realm = XiuXianConfig.Instance.Realms.FirstOrDefault(r => r.Name.Equals(realmName, StringComparison.OrdinalIgnoreCase));
            if (realm == null)
            {
                args.Player.SendErrorMessage($"找不到境界: {realmName}");
                return;
            }

            if (realm.ProgressLimits == null)
                realm.ProgressLimits = new List<string>();

            realm.ProgressLimits.Add(bossName);
            XiuXianConfig.Save(ConfigPath);

            args.Player.SendSuccessMessage($"已为境界 {realm.Name} 添加进度条件: 击败 {bossName}");
        }

        private void AddRealmReward(CommandArgs args)
        {
            if (args.Parameters.Count < 4)
            {
                args.Player.SendInfoMessage("用法: /添加境界奖励 <境界名称> <物品ID> <数量> [前缀]");
                args.Player.SendInfoMessage("示例: /添加境界奖励 筑基 123 5 0");
                return;
            }

            string realmName = args.Parameters[0];
            if (!int.TryParse(args.Parameters[1], out int itemId) ||
                !int.TryParse(args.Parameters[2], out int stack) ||
                !byte.TryParse(args.Parameters[3], out byte prefix))
            {
                args.Player.SendErrorMessage("无效的参数格式");
                return;
            }

            var realm = XiuXianConfig.Instance.Realms.FirstOrDefault(r => r.Name.Equals(realmName, StringComparison.OrdinalIgnoreCase));
            if (realm == null)
            {
                args.Player.SendErrorMessage($"找不到境界: {realmName}");
                return;
            }

            var newItem = new Item
            {
                NetID = itemId,
                Stack = stack,
                Prefix = prefix
            };

            var newRewards = realm.RewardGoods.ToList();
            newRewards.Add(newItem);
            realm.RewardGoods = newRewards.ToArray();

            XiuXianConfig.Save(ConfigPath);

            var itemName = TShock.Utils.GetItemById(itemId)?.Name ?? $"物品ID:{itemId}";
            args.Player.SendSuccessMessage($"已为境界 {realm.Name} 添加奖励: {itemName} x{stack} (前缀:{prefix})");
        }

        private void AddRealmBuff(CommandArgs args)
        {
            if (args.Parameters.Count < 3)
            {
                args.Player.SendInfoMessage("用法: /添加境界buff <境界名称> <BuffID> <持续时间(秒)>");
                args.Player.SendInfoMessage("示例: /添加境界buff 筑基 1 300");
                return;
            }

            string realmName = args.Parameters[0];
            if (!int.TryParse(args.Parameters[1], out int buffId) ||
                !int.TryParse(args.Parameters[2], out int duration))
            {
                args.Player.SendErrorMessage("无效的参数格式");
                return;
            }

            var realm = XiuXianConfig.Instance.Realms.FirstOrDefault(r => r.Name.Equals(realmName, StringComparison.OrdinalIgnoreCase));
            if (realm == null)
            {
                args.Player.SendErrorMessage($"找不到境界: {realmName}");
                return;
            }

            if (buffId < 1 || buffId > Terraria.ID.BuffID.Count)
            {
                args.Player.SendErrorMessage($"无效的BuffID，范围应为1-{Terraria.ID.BuffID.Count}");
                return;
            }

            var newBuff = new RealmBuff
            {
                BuffID = buffId,
                Duration = duration
            };

            var newBuffs = realm.RewardBuffs.ToList();
            newBuffs.Add(newBuff);
            realm.RewardBuffs = newBuffs.ToArray();

            XiuXianConfig.Save(ConfigPath);

            var buffName = TShock.Utils.GetBuffName(buffId);
            args.Player.SendSuccessMessage($"已为境界 {realm.Name} 添加Buff奖励: {buffName} ({duration}秒)");
        }

        private void CRC(CommandArgs args)
        {
            try
            {
                RC(args.Player);
                args.Player.SendSuccessMessage("修仙配置重读完毕。");
            }
            catch (Exception ex)
            {
                args.Player.SendErrorMessage($"重读修仙配置文件失败: {ex.Message}");
                TShock.Log.Error($"重读修仙配置文件失败: {ex.Message}");
            }
        }
        
        private void RC(TSPlayer player = null)
        {
            try
            {
                string currentConfigPath = ConfigPath;
                int currentRefreshInterval = XiuXianConfig.Instance.TopUIRefreshInterval;
                
                if (!File.Exists(currentConfigPath))
                {
                    player?.SendErrorMessage($"修仙配置文件不存在: {currentConfigPath}");
                    TShock.Log.Error($"修仙配置文件不存在: {currentConfigPath}");
                    return;
                }
                
                string jsonContent = File.ReadAllText(currentConfigPath);
                
                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    player?.SendErrorMessage("修仙配置文件为空，请检查配置文件内容");
                    TShock.Log.Error("修仙配置文件为空，请检查配置文件内容");
                    return;
                }
                
                var newConfig = JsonConvert.DeserializeObject<XiuXianConfig>(jsonContent);
                
                if (newConfig == null)
                {
                    player?.SendErrorMessage("修仙配置文件格式错误，无法解析");
                    TShock.Log.Error("修仙配置文件格式错误，无法解析");
                    return;
                }
                
                if (newConfig.Realms == null || newConfig.Realms.Count == 0)
                {
                    player?.SendErrorMessage("修仙配置文件中境界数据为空，请检查配置文件");
                    TShock.Log.Error("修仙配置文件中境界数据为空，请检查配置文件");
                    return;
                }
                
                if (newConfig.StarSigns == null || newConfig.StarSigns.Count == 0)
                {
                    player?.SendErrorMessage("修仙配置文件中星宿数据为空，请检查配置文件");
                    TShock.Log.Error("修仙配置文件中星宿数据为空，请检查配置文件");
                    return;
                }
                
                XiuXianConfig.Instance = newConfig;
                
                _topUiRefreshTimer.Interval = XiuXianConfig.Instance.TopUIRefreshInterval;
                _chatUiRefreshTimer.Interval = XiuXianConfig.Instance.ChatUIRefreshInterval * 60000;
                
                TSPlayer.All.SendInfoMessage($"修仙配置文件已重读完成！");
                TSPlayer.All.SendInfoMessage($"当前服务器名称: {XiuXianConfig.Instance.ServerName}");

                foreach (var p in TShock.Players.Where(p => p != null && p.Active))
                {
                    try
                    {
                        var data = XiuXianData.GetPlayer(p.Name);
                        UpdateChatUI(p, data);
                        UpdateTopUI(p, data);
                    }
                    catch (Exception ex)
                    {
                        TShock.Log.Error($"更新玩家 {p.Name} UI失败: {ex.Message}");
                    }
                }
                
                StringBuilder infoBuilder = new StringBuilder();
                infoBuilder.AppendLine("══════════ 境界信息 ══════════");
                
                foreach (var realm in XiuXianConfig.Instance.Realms)
                {
                    infoBuilder.AppendLine($"境界: {realm.Name}");
                    
                    if (realm.ProgressLimits != null && realm.ProgressLimits.Count > 0)
                    {
                        infoBuilder.AppendLine($"  进度限制: {string.Join(", ", realm.ProgressLimits)}");
                    }
                    else
                    {
                        infoBuilder.AppendLine($"  进度限制: 无");
                    }
                    
                    if (realm.RewardGoods != null && realm.RewardGoods.Length > 0)
                    {
                        infoBuilder.AppendLine($"  物品奖励:");
                        foreach (var item in realm.RewardGoods)
                        {
                            var itemName = TShock.Utils.GetItemById(item.NetID)?.Name ?? $"物品ID:{item.NetID}";
                            infoBuilder.AppendLine($"    - {itemName} x{item.Stack}");
                        }
                    }
                    else
                    {
                        infoBuilder.AppendLine($"  物品奖励: 无");
                    }
                    
                    if (realm.RewardBuffs != null && realm.RewardBuffs.Length > 0)
                    {
                        infoBuilder.AppendLine($"  Buff奖励:");
                        foreach (var buff in realm.RewardBuffs)
                        {
                            var buffName = TShock.Utils.GetBuffName(buff.BuffID);
                            infoBuilder.AppendLine($"    - {buffName} ({buff.Duration}秒)");
                        }
                    }
                    else
                    {
                        infoBuilder.AppendLine($"  Buff奖励: 无");
                    }
                    
                    infoBuilder.AppendLine();
                }
                
                infoBuilder.AppendLine("══════════════════════════════");
                
                TShock.Log.Info($"修仙配置文件重读完成，包含 {XiuXianConfig.Instance.Realms.Count} 个境界的进度限制和突破奖励");
                
                if (player != null)
                {
                    var lines = infoBuilder.ToString().Split('\n');
                    var pageSize = 10;
                    var pageCount = (int)Math.Ceiling((double)lines.Length / pageSize);
                    
                    for (int i = 0; i < pageCount; i++)
                    {
                        var pageLines = lines.Skip(i * pageSize).Take(pageSize);
                        player.SendMessage(string.Join("\n", pageLines), Color.Cyan);
                    }
                }
            }
            catch (JsonException jsonEx)
            {
                player?.SendErrorMessage($"修仙配置文件JSON格式错误: {jsonEx.Message}");
                TShock.Log.Error($"修仙配置文件JSON格式错误: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                player?.SendErrorMessage($"重读修仙配置文件失败: {ex.Message}");
                TShock.Log.Error($"重读修仙配置文件失败: {ex.Message}");
            }
        }
        
        private void ResetCultivation(CommandArgs args)
        {
            var data = XiuXianData.GetPlayer(args.Player.Name);
            
            if (data.Realm == "凡人")
            {
                args.Player.SendErrorMessage("你已经是凡人境界，无需散尽修为！");
                return;
            }
            
            if (args.Parameters.Count == 0 || args.Parameters[0].ToLower() != "确认")
            {
                args.Player.SendWarningMessage($"警告：散尽修为将重置你的境界到凡人，所有修炼进度清零！");
                args.Player.SendWarningMessage($"当前境界: {data.Realm}境，修炼进度: {data.CultivationProgress}%");
                args.Player.SendInfoMessage($"如果你确定要散尽修为，请输入: /散尽修为 确认");
                return;
            }
            
            data.Realm = "凡人";
            data.CultivationProgress = 0;
            data.ConstellationLevel = 0;
            data.ConstellationPoints = 0;
            data.IsGhostState = false;
            data.GhostBattleNpcIndex = -1;
            
            args.Player.SendSuccessMessage("★★★ 已散尽修为，重返凡人！★★★");
            args.Player.SendInfoMessage($"当前境界: {data.Realm}境，寿元: {data.LifeYears:F1}年");
            
            UpdateChatUI(args.Player, data);
            UpdateTopUI(args.Player, data);
        }
        
        private void ResetAllPlayersCultivation(CommandArgs args)
        {
            if (args.Parameters.Count == 0 || args.Parameters[0].ToLower() != "确认")
            {
                args.Player.SendWarningMessage($"警告：仙道重开将重置所有玩家的境界到凡人！");
                args.Player.SendWarningMessage($"这将影响 {XiuXianData.Players.Count} 名玩家");
                args.Player.SendInfoMessage($"如果你确定要执行仙道重开，请输入: /仙道重开 确认");
                return;
            }
            
            int resetCount = 0;
            foreach (var playerData in XiuXianData.Players.Values)
            {
                if (playerData.Realm != "凡人")
                {
                    playerData.Realm = "凡人";
                    playerData.CultivationProgress = 0;
                    playerData.ConstellationLevel = 0;
                    playerData.ConstellationPoints = 0;
                    playerData.IsGhostState = false;
                    playerData.GhostBattleNpcIndex = -1;
                    resetCount++;
                }
            }
            
            XiuXianData.Save(DataPath);
            
            args.Player.SendSuccessMessage($"★★★ 仙道重开完成！共重置 {resetCount} 名玩家的境界 ★★★");
            
            TSPlayer.All.SendInfoMessage($"管理员 {args.Player.Name} 已执行仙道重开，所有玩家境界重置为凡人");
            
            foreach (var player in TShock.Players.Where(p => p != null && p.Active))
            {
                var data = XiuXianData.GetPlayer(player.Name);
                UpdateChatUI(player, data);
                UpdateTopUI(player, data);
            }
        }
        #endregion

        #region 配置与数据
        public class Item
        {
            [JsonProperty("物品ID")]
            public int NetID { get; set; }

            [JsonProperty("数量")]
            public int Stack { get; set; }

            [JsonProperty("前缀")]
            public byte Prefix { get; set; } = 0;
        }

        public class RealmBuff
        {
            [JsonProperty("BuffID")]
            public int BuffID { get; set; }

            [JsonProperty("持续时间")]
            public int Duration { get; set; }
        }

        public class RealmInfo
        {
            public string Name { get; set; } = "凡人";
            public int Level { get; set; } = 0;
            public int LifeBonus { get; set; } = 0;
            public int BreakthroughReq { get; set; } = 0;
            public double SuccessRate { get; set; } = 1.0;

            [JsonProperty("进度限制")]
            public List<string> ProgressLimits { get; set; } = new List<string>();

            [JsonProperty("突破奖励")]
            public Item[] RewardGoods { get; set; } = Array.Empty<Item>();

            [JsonProperty("突破Buff奖励")]
            public RealmBuff[] RewardBuffs { get; set; } = Array.Empty<RealmBuff>();
        }

        public class StarSignInfo
        {
            public string Name { get; set; }
            public string TerrariaClass { get; set; }
            public string Description { get; set; }
            public string ShortDesc { get; set; }
            public float CultivationBonus { get; set; } = 0f;
            public float LifeBonus { get; set; } = 0f;
            public float DamageBonus { get; set; } = 0f;
            public float DefenseBonus { get; set; } = 0f;
            public Microsoft.Xna.Framework.Color Color { get; set; }

            [JsonProperty("图标物品ID", DefaultValueHandling = DefaultValueHandling.Populate)]
            public int Icon { get; set; } = 0;

            [JsonProperty("初始Buff")]
            public RealmBuff[] InitialBuffs { get; set; } = Array.Empty<RealmBuff>();

            public string Hex3()
            {
                return $"{Color.R:X2}{Color.G:X2}{Color.B:X2}";
            }
        }

        public class XiuXianConfig
        {
            public static XiuXianConfig Instance;

            public string ServerName { get; set; } = "泰拉修仙传";
            
            [JsonProperty("初始寿元")]
            public int InitialLifeYears { get; set; } = 100;
            
            public int RebirthCost { get; set; } = 200;
            public bool BroadcastBreakthrough { get; set; } = true;
            
            [JsonProperty("星宿境界体系")]
            public Dictionary<string, List<RealmInfo>> StarSignRealms { get; set; } = new Dictionary<string, List<RealmInfo>>();
            
            public List<StarSignInfo> StarSigns { get; set; } = new List<StarSignInfo>();
            
            [JsonProperty("星宿命座效果")]
            public Dictionary<string, List<string>> StarSignConstellations { get; set; } = new Dictionary<string, List<string>>();
            
            [JsonProperty("命座Buff效果")]
            public Dictionary<string, List<RealmBuff[]>> ConstellationBuffs { get; set; } = new Dictionary<string, List<RealmBuff[]>>();

            public List<RealmInfo> Realms { get; set; } = new List<RealmInfo>();

            public int ChatUIOffsetX { get; set; } = 2;
            public int ChatUIOffsetY { get; set; } = 1;
            public int TopUIOffsetX { get; set; } = -120;
            public int TopUIOffsetY { get; set; } = 2;

            [JsonProperty("顶部UI刷新间隔")]
            public int TopUIRefreshInterval { get; set; } = 10;

            [JsonProperty("聊天UI刷新间隔")]
            public int ChatUIRefreshInterval { get; set; } = 900000;

            [JsonProperty("寿元耗尽倒计时")]
            public int LifeDepletionCountdown { get; set; } = 60;
            
            [JsonProperty("反魂消耗寿元")]
            public int SoulReturnCost { get; set; } = 50;

            static XiuXianConfig()
            {
            }

            public XiuXianConfig()
            {
                InitializeStaticData();
            }

            private void InitializeStaticData()
            {
                StarSigns = new List<StarSignInfo>
                {
                    new StarSignInfo
                    {
                        Name = "紫微帝星",
                        TerrariaClass = "战士",
                        Description = "帝王之相，统御四方，全面提升修炼效率",
                        ShortDesc = "近战物理职业",
                        CultivationBonus = 0.15f,
                        LifeBonus = 0.2f,
                        DamageBonus = 0.25f,
                        DefenseBonus = 0.3f,
                        Color = new Microsoft.Xna.Framework.Color(255, 215, 0),
                        Icon = 497,
                        InitialBuffs = new RealmBuff[] {
                            new RealmBuff { BuffID = 1, Duration = 300 },
                            new RealmBuff { BuffID = 3, Duration = 300 }
                        }
                    },
                    new StarSignInfo
                    {
                        Name = "破军杀星", 
                        TerrariaClass = "射手",
                        Description = "主杀伐征战，攻击力冠绝天下",
                        ShortDesc = "远程物理职业",
                        CultivationBonus = 0.1f,
                        LifeBonus = 0.1f,
                        DamageBonus = 0.3f,
                        DefenseBonus = 0.1f,
                        Color = new Microsoft.Xna.Framework.Color(220, 20, 60),
                        Icon = 498,
                        InitialBuffs = new RealmBuff[] {
                            new RealmBuff { BuffID = 2, Duration = 300 },
                            new RealmBuff { BuffID = 4, Duration = 300 }
                        }
                    },
                    new StarSignInfo
                    {
                        Name = "天机玄星",
                        TerrariaClass = "法师",
                        Description = "洞悉天机，擅长辅助与阵法",
                        ShortDesc = "魔法职业",
                        CultivationBonus = 0.2f,
                        LifeBonus = 0.05f,
                        DamageBonus = 0.35f,
                        DefenseBonus = 0.05f,
                        Color = new Microsoft.Xna.Framework.Color(138, 43, 226),
                        Icon = 499,
                        InitialBuffs = new RealmBuff[] {
                            new RealmBuff { BuffID = 6, Duration = 300 },
                            new RealmBuff { BuffID = 7, Duration = 300 }
                        }
                    },
                    new StarSignInfo
                    {
                        Name = "武曲战星",
                        TerrariaClass = "召唤师",
                        Description = "战神护体，铜墙铁壁般的防御",
                        ShortDesc = "召唤职业",
                        CultivationBonus = 0.25f,
                        LifeBonus = 0.15f,
                        DamageBonus = 0.2f,
                        DefenseBonus = 0.15f,
                        Color = new Microsoft.Xna.Framework.Color(0, 191, 255),
                        Icon = 500,
                        InitialBuffs = new RealmBuff[] {
                            new RealmBuff { BuffID = 8, Duration = 300 },
                            new RealmBuff { BuffID = 9, Duration = 300 }
                        }
                    },
                    new StarSignInfo
                    {
                        Name = "七杀凶星",
                        TerrariaClass = "盗贼", 
                        Description = "以杀证道，越战越勇",
                        ShortDesc = "敏捷职业",
                        CultivationBonus = 0.18f,
                        LifeBonus = 0.15f,
                        DamageBonus = 0.25f,
                        DefenseBonus = 0.1f,
                        Color = new Microsoft.Xna.Framework.Color(178, 34, 34),
                        Icon = 501,
                        InitialBuffs = new RealmBuff[] {
                            new RealmBuff { BuffID = 2, Duration = 300 },
                            new RealmBuff { BuffID = 10, Duration = 300 }
                        }
                    },
                    new StarSignInfo
                    {
                        Name = "太阴玄星",
                        TerrariaClass = "牧师",
                        Description = "月华之力，治疗与恢复",
                        ShortDesc = "治疗辅助职业",
                        CultivationBonus = 0.15f,
                        LifeBonus = 0.25f,
                        DamageBonus = 0.1f,
                        DefenseBonus = 0.2f,
                        Color = new Microsoft.Xna.Framework.Color(173, 216, 230),
                        Icon = 502,
                        InitialBuffs = new RealmBuff[] {
                            new RealmBuff { BuffID = 1, Duration = 300 },
                            new RealmBuff { BuffID = 5, Duration = 300 }
                        }
                    }
                };

                StarSignRealms = new Dictionary<string, List<RealmInfo>>();
                foreach (var starSign in StarSigns)
                {
                    var realms = new List<RealmInfo>
                    {
                        new RealmInfo { 
                            Name = "凡人", Level = 0, LifeBonus = 10, BreakthroughReq = 50, SuccessRate = 0.5,
                            ProgressLimits = new List<string>(),
                            RewardGoods = new Item[] {
                                new Item { NetID = 1, Stack = 50 }, // 铜镐
                                new Item { NetID = 2, Stack = 50 }  // 铜斧
                            },
                            RewardBuffs = new RealmBuff[] {
                                new RealmBuff { BuffID = 1, Duration = 300 } // 生命回复
                            }
                        },
                        new RealmInfo { 
                            Name = "搬血", Level = 1, LifeBonus = 50, BreakthroughReq = 100, SuccessRate = 0.95,
                            ProgressLimits = new List<string> { "史莱姆王" },
                            RewardGoods = new Item[] {
                                new Item { NetID = 29, Stack = 5 }, // 生命水晶
                                new Item { NetID = 75, Stack = 100 } // 陨石锭
                            },
                            RewardBuffs = new RealmBuff[] {
                                new RealmBuff { BuffID = 2, Duration = 600 }, // 耐力
                                new RealmBuff { BuffID = 3, Duration = 600 }  // 铁皮
                            }
                        },
                        new RealmInfo { 
                            Name = "洞天", Level = 2, LifeBonus = 100, BreakthroughReq = 150, SuccessRate = 0.9,
                            ProgressLimits = new List<string> { "克苏鲁之眼", "世界吞噬怪", "克苏鲁之脑" },
                            RewardGoods = new Item[] {
                                new Item { NetID = 88, Stack = 200 }, // 魔金锭
                                new Item { NetID = 117, Stack = 100 } // 暗影鳞片
                            },
                            RewardBuffs = new RealmBuff[] {
                                new RealmBuff { BuffID = 4, Duration = 600 }, // 伤害
                                new RealmBuff { BuffID = 5, Duration = 600 }  // 恢复
                            }
                        },
                        new RealmInfo { 
                            Name = "化灵", Level = 3, LifeBonus = 200, BreakthroughReq = 200, SuccessRate = 0.85,
                            ProgressLimits = new List<string> { "蜂王", "骷髅王" },
                            RewardGoods = new Item[] {
                                new Item { NetID = 210, Stack = 50 }, // 暗影之魂
                                new Item { NetID = 211, Stack = 50 }  // 光明之魂
                            },
                            RewardBuffs = new RealmBuff[] {
                                new RealmBuff { BuffID = 6, Duration = 600 }, // 魔力回复
                                new RealmBuff { BuffID = 7, Duration = 600 }  // 魔力提升
                            }
                        },
                        new RealmInfo { 
                            Name = "铭纹", Level = 4, LifeBonus = 400, BreakthroughReq = 300, SuccessRate = 0.8,
                            ProgressLimits = new List<string> { "独眼巨鹿", "血肉墙" },
                            RewardGoods = new Item[] {
                                new Item { NetID = 1225, Stack = 100 }, // 神圣锭
                                new Item { NetID = 549, Stack = 50 }    // 力量之魂
                            },
                            RewardBuffs = new RealmBuff[] {
                                new RealmBuff { BuffID = 8, Duration = 600 },  // 召唤
                                new RealmBuff { BuffID = 9, Duration = 600 },  // 荆棘
                                new RealmBuff { BuffID = 10, Duration = 600 }  // 黑斑
                            }
                        },
                        new RealmInfo { 
                            Name = "列阵", Level = 5, LifeBonus = 800, BreakthroughReq = 400, SuccessRate = 0.75,
                            ProgressLimits = new List<string> { "毁灭者", "机械骷髅王", "双子魔眼" },
                            RewardGoods = new Item[] {
                                new Item { NetID = 1225, Stack = 150 }, // 神圣锭
                                new Item { NetID = 549, Stack = 75 },   // 力量之魂
                                new Item { NetID = 550, Stack = 75 },   // 视域之魂
                                new Item { NetID = 551, Stack = 75 }    // 恐惧之魂
                            },
                            RewardBuffs = new RealmBuff[] {
                                new RealmBuff { BuffID = 11, Duration = 600 }, // 敏捷
                                new RealmBuff { BuffID = 12, Duration = 600 }, // 怒气
                                new RealmBuff { BuffID = 13, Duration = 600 }  // 心醉
                            }
                        },
                        new RealmInfo { 
                            Name = "尊者", Level = 6, LifeBonus = 1600, BreakthroughReq = 500, SuccessRate = 0.7,
                            ProgressLimits = new List<string> { "世纪之花", "石巨人" },
                            RewardGoods = new Item[] {
                                new Item { NetID = 1006, Stack = 100 }, // 叶绿锭
                                new Item { NetID = 1329, Stack = 50 },  // 甲虫莎草纸
                                new Item { NetID = 1291, Stack = 1 }    // 甲虫壳
                            },
                            RewardBuffs = new RealmBuff[] {
                                new RealmBuff { BuffID = 14, Duration = 600 }, // 隐身
                                new RealmBuff { BuffID = 15, Duration = 600 }, // 夜猫子
                                new RealmBuff { BuffID = 16, Duration = 600 }  // 狩猎
                            }
                        },
                        new RealmInfo { 
                            Name = "神火", Level = 7, LifeBonus = 3200, BreakthroughReq = 600, SuccessRate = 0.65,
                            ProgressLimits = new List<string> { "猪龙鱼公爵", "光之女皇" },
                            RewardGoods = new Item[] {
                                new Item { NetID = 3380, Stack = 100 }, // 夜明锭
                                new Item { NetID = 3460, Stack = 50 },  // 灵气
                                new Item { NetID = 3384, Stack = 1 }    // 天界符
                            },
                            RewardBuffs = new RealmBuff[] {
                                new RealmBuff { BuffID = 17, Duration = 600 }, // 建造
                                new RealmBuff { BuffID = 18, Duration = 600 }, // 采矿
                                new RealmBuff { BuffID = 19, Duration = 600 }  // 镇静
                            }
                        },
                        new RealmInfo { 
                            Name = "真一", Level = 8, LifeBonus = 6400, BreakthroughReq = 700, SuccessRate = 0.6,
                            ProgressLimits = new List<string> { "拜月教邪教徒", "月亮领主" },
                            RewardGoods = new Item[] {
                                new Item { NetID = 3467, Stack = 100 }, // 夜明锭
                                new Item { NetID = 3459, Stack = 50 },  // 月亮碎片
                                new Item { NetID = 3456, Stack = 50 },  // 星星碎片
                                new Item { NetID = 3457, Stack = 50 },  // 星系碎片
                                new Item { NetID = 3458, Stack = 50 }   // 太阳碎片
                            },
                            RewardBuffs = new RealmBuff[] {
                                new RealmBuff { BuffID = 20, Duration = 600 },  // 爱情
                                new RealmBuff { BuffID = 21, Duration = 600 },  // 生命力
                                new RealmBuff { BuffID = 22, Duration = 600 },  // 耐力
                                new RealmBuff { BuffID = 23, Duration = 600 }   // 怒气
                            }
                        },
                        new RealmInfo { 
                            Name = "圣祭", Level = 9, LifeBonus = 12800, BreakthroughReq = 800, SuccessRate = 0.55,
                            ProgressLimits = new List<string> { "火星飞碟" },
                            RewardGoods = new Item[] {
                                new Item { NetID = 3380, Stack = 200 }, // 夜明锭
                                new Item { NetID = 3459, Stack = 100 }, // 月亮碎片
                                new Item { NetID = 3460, Stack = 100 }  // 灵气
                            },
                            RewardBuffs = new RealmBuff[] {
                                new RealmBuff { BuffID = 24, Duration = 600 }, // 召唤
                                new RealmBuff { BuffID = 25, Duration = 600 }, // 荆棘
                                new RealmBuff { BuffID = 26, Duration = 600 }  // 蜂蜜
                            }
                        },
                        new RealmInfo { 
                            Name = "天神", Level = 10, LifeBonus = 25600, BreakthroughReq = 900, SuccessRate = 0.5,
                            ProgressLimits = new List<string> { "日耀柱", "星云柱", "星尘柱", "星座柱" },
                            RewardGoods = new Item[] {
                                new Item { NetID = 2798, Stack = 100 }, // 火星管道镀层
                                new Item { NetID = 2866, Stack = 1 },   // 宇宙车钥匙
                                new Item { NetID = 2797, Stack = 1 }    // 反重力钩
                            },
                            RewardBuffs = new RealmBuff[] {
                                new RealmBuff { BuffID = 27, Duration = 600 }, // 箭术
                                new RealmBuff { BuffID = 28, Duration = 600 }, // 弹药储备
                                new RealmBuff { BuffID = 29, Duration = 600 }  // 魔能
                            }
                        },
                        new RealmInfo { 
                            Name = "虚道", Level = 11, LifeBonus = 51200, BreakthroughReq = 1000, SuccessRate = 0.45,
                            ProgressLimits = new List<string> { "黑暗魔法师", "食人魔", "双足翼龙", "荷兰飞盗船" },
                            RewardGoods = new Item[] {
                                new Item { NetID = 3380, Stack = 300 }, // 夜明锭
                                new Item { NetID = 3460, Stack = 150 }, // 灵气
                                new Item { NetID = 3459, Stack = 150 }  // 月亮碎片
                            },
                            RewardBuffs = new RealmBuff[] {
                                new RealmBuff { BuffID = 30, Duration = 600 }, // 战斗
                                new RealmBuff { BuffID = 31, Duration = 600 }, // 生命力
                                new RealmBuff { BuffID = 32, Duration = 600 }  // 耐力
                            }
                        },
                        new RealmInfo { 
                            Name = "斩我", Level = 12, LifeBonus = 102400, BreakthroughReq = 1100, SuccessRate = 0.4,
                            ProgressLimits = new List<string> { "哀木", "南瓜王", "常绿尖叫怪", "圣诞坦克", "冰雪女王" },
                            RewardGoods = new Item[] {
                                new Item { NetID = 3827, Stack = 100 }, // 护卫奖章
                                new Item { NetID = 3549, Stack = 1 },   // 永恒之矛
                                new Item { NetID = 3870, Stack = 1 }    // 双足翼龙诅咒
                            },
                            RewardBuffs = new RealmBuff[] {
                                new RealmBuff { BuffID = 33, Duration = 600 }, // 召唤
                                new RealmBuff { BuffID = 34, Duration = 600 }, // 怒气
                                new RealmBuff { BuffID = 35, Duration = 600 }  // 心醉
                            }
                        },
                        new RealmInfo { 
                            Name = "遁一", Level = 13, LifeBonus = 204800, BreakthroughReq = 1200, SuccessRate = 0.35,
                            ProgressLimits = new List<string> { "史莱姆皇后" },
                            RewardGoods = new Item[] {
                                new Item { NetID = 4061, Stack = 100 }, // 明胶女式鞍
                                new Item { NetID = 4062, Stack = 1 },   // 粘鞍
                                new Item { NetID = 4063, Stack = 1 }    // 皇家凝胶
                            },
                            RewardBuffs = new RealmBuff[] {
                                new RealmBuff { BuffID = 36, Duration = 600 }, // 敏捷
                                new RealmBuff { BuffID = 37, Duration = 600 }, // 黑斑
                                new RealmBuff { BuffID = 38, Duration = 600 }  // 寂静
                            }
                        },
                        new RealmInfo { 
                            Name = "至尊", Level = 14, LifeBonus = 409600, BreakthroughReq = 1300, SuccessRate = 0.3,
                            ProgressLimits = new List<string> { "机械美杜莎" },
                            RewardGoods = new Item[] {
                                new Item { NetID = 4956, Stack = 100 }, // 晶光刃
                                new Item { NetID = 4957, Stack = 1 },   // 耀斑盔甲
                                new Item { NetID = 4958, Stack = 1 }    // 日耀喷发剑
                            },
                            RewardBuffs = new RealmBuff[] {
                                new RealmBuff { BuffID = 39, Duration = 600 }, // 生命力
                                new RealmBuff { BuffID = 40, Duration = 600 }, // 耐力
                                new RealmBuff { BuffID = 41, Duration = 600 }  // 怒气
                            }
                        },
                        new RealmInfo { 
                            Name = "真仙", Level = 15, LifeBonus = 819200, BreakthroughReq = 1400, SuccessRate = 0.25,
                            ProgressLimits = new List<string> { "所有事件Boss" },
                            RewardGoods = new Item[] {
                                new Item { NetID = 3380, Stack = 500 }, // 夜明锭
                                new Item { NetID = 3460, Stack = 250 }, // 灵气
                                new Item { NetID = 3459, Stack = 250 }  // 月亮碎片
                            },
                            RewardBuffs = new RealmBuff[] {
                                new RealmBuff { BuffID = 42, Duration = 600 }, // 建造
                                new RealmBuff { BuffID = 43, Duration = 600 }, // 采矿
                                new RealmBuff { BuffID = 44, Duration = 600 }  // 镇静
                            }
                        },
                        new RealmInfo { 
                            Name = "仙王", Level = 16, LifeBonus = 1638400, BreakthroughReq = 1500, SuccessRate = 0.2,
                            ProgressLimits = new List<string> { "所有困难模式Boss" },
                            RewardGoods = new Item[] {
                                new Item { NetID = 3380, Stack = 750 }, // 夜明锭
                                new Item { NetID = 3460, Stack = 375 }, // 灵气
                                new Item { NetID = 3459, Stack = 375 }  // 月亮碎片
                            },
                            RewardBuffs = new RealmBuff[] {
                                new RealmBuff { BuffID = 45, Duration = 600 }, // 召唤
                                new RealmBuff { BuffID = 46, Duration = 600 }, // 荆棘
                                new RealmBuff { BuffID = 47, Duration = 600 }  // 蜂蜜
                            }
                        },
                        new RealmInfo { 
                            Name = "仙帝", Level = 17, LifeBonus = 3276800, BreakthroughReq = 1600, SuccessRate = 0.15,
                            ProgressLimits = new List<string> { "所有Boss" },
                            RewardGoods = new Item[] {
                                new Item { NetID = 3380, Stack = 1000 }, // 夜明锭
                                new Item { NetID = 3460, Stack = 500 },  // 灵气
                                new Item { NetID = 3459, Stack = 500 }   // 月亮碎片
                            },
                            RewardBuffs = new RealmBuff[] {
                                new RealmBuff { BuffID = 48, Duration = 600 }, // 箭术
                                new RealmBuff { BuffID = 49, Duration = 600 }, // 弹药储备
                                new RealmBuff { BuffID = 50, Duration = 600 }  // 魔能
                            }
                        },
                        new RealmInfo { 
                            Name = "天道", Level = 18, LifeBonus = 6553600, BreakthroughReq = 1700, SuccessRate = 0.1,
                            ProgressLimits = new List<string> { "全部Boss和事件" },
                            RewardGoods = new Item[] {
                                new Item { NetID = 3380, Stack = 1500 }, // 夜明锭
                                new Item { NetID = 3460, Stack = 750 },  // 灵气
                                new Item { NetID = 3459, Stack = 750 }   // 月亮碎片
                            },
                            RewardBuffs = new RealmBuff[] {
                                new RealmBuff { BuffID = 51, Duration = 600 }, // 全属性提升
                                new RealmBuff { BuffID = 52, Duration = 600 }, // 无限飞行
                                new RealmBuff { BuffID = 53, Duration = 600 }  // 无敌状态
                            }
                        }
                    };

                    StarSignRealms[starSign.Name] = realms;
                }

                Realms = StarSignRealms.Values.FirstOrDefault() ?? new List<RealmInfo>();

                InitializeConstellations();
            }

            private void InitializeConstellations()
            {
                StarSignConstellations = new Dictionary<string, List<string>>();
                ConstellationBuffs = new Dictionary<string, List<RealmBuff[]>>();

                foreach (var starSign in StarSigns)
                {
                    var constellations = new List<string>();
                    var constellationBuffs = new List<RealmBuff[]>();

                    if (starSign.TerrariaClass == "战士")
                    {
                        constellations.AddRange(new List<string> {
                            "天枢：近战伤害+15%，生命回复速度提升",
                            "天璇：暴击率+10%，暴击伤害+25%",
                            "天玑：防御力+10，受到伤害减少15%",
                            "天权：移动速度+20%，近战攻击速度+15%",
                            "玉衡：生命上限+25%，自动回复生命值",
                            "开阳：攻击有几率触发吸血效果",
                            "摇光：全属性+20%，获得霸体状态"
                        });

                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 4, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 5, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 3, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 2, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 1, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 114, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { 
                            new RealmBuff { BuffID = 4, Duration = 3600 },
                            new RealmBuff { BuffID = 3, Duration = 3600 },
                            new RealmBuff { BuffID = 2, Duration = 3600 }
                        });
                    }
                    else if (starSign.TerrariaClass == "射手")
                    {
                        constellations.AddRange(new List<string> {
                            "天枢：远程伤害+15%，弹药消耗减少20%",
                            "天璇：暴击率+12%，远程暴击伤害+30%",
                            "天玑：移动速度+25%，闪避几率+15%",
                            "天权：箭矢穿透+2，射击速度+20%",
                            "玉衡：特殊弹药效果持续时间+50%",
                            "开阳：有几率不消耗弹药",
                            "摇光：全属性+15%，获得无限弹药状态"
                        });

                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 4, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 2, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 11, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 12, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 13, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 14, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { 
                            new RealmBuff { BuffID = 4, Duration = 3600 },
                            new RealmBuff { BuffID = 2, Duration = 3600 },
                            new RealmBuff { BuffID = 11, Duration = 3600 }
                        });
                    }
                    else if (starSign.TerrariaClass == "法师")
                    {
                        constellations.AddRange(new List<string> {
                            "天枢：魔法伤害+15%，魔力消耗减少20%",
                            "天璇：魔力回复速度+50%，魔法暴击率+15%",
                            "天玑：法术穿透+2，魔法效果范围+25%",
                            "天权：魔力上限+100，魔法攻击速度+20%",
                            "玉衡：有几率不消耗魔力",
                            "开阳：魔法攻击附加元素效果",
                            "摇光：全属性+15%，获得无限魔力状态"
                        });

                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 6, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 7, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 8, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 9, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 10, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 114, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { 
                            new RealmBuff { BuffID = 6, Duration = 3600 },
                            new RealmBuff { BuffID = 7, Duration = 3600 },
                            new RealmBuff { BuffID = 8, Duration = 3600 }
                        });
                    }
                    else if (starSign.TerrariaClass == "召唤师")
                    {
                        constellations.AddRange(new List<string> {
                            "天枢：召唤伤害+15%，召唤数量+1",
                            "天璇：召唤物暴击率+10%，召唤物生命+50%",
                            "天玑：召唤物移动速度+25%，攻击速度+20%",
                            "天权：可同时存在召唤物数量+2",
                            "玉衡：召唤物附加特殊效果",
                            "开阳：召唤物有几率触发连锁攻击",
                            "摇光：全属性+15%，召唤物获得无敌状态"
                        });

                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 8, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 9, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 10, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 11, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 12, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { new RealmBuff { BuffID = 13, Duration = 3600 } });
                        constellationBuffs.Add(new RealmBuff[] { 
                            new RealmBuff { BuffID = 8, Duration = 3600 },
                            new RealmBuff { BuffID = 9, Duration = 3600 },
                            new RealmBuff { BuffID = 10, Duration = 3600 }
                        });
                    }

                    StarSignConstellations[starSign.Name] = constellations;
                    ConstellationBuffs[starSign.Name] = constellationBuffs;
                }
            }

            public RealmInfo GetRealmInfoForStarSign(string name, string starSign)
            {
                if (StarSignRealms.ContainsKey(starSign))
                {
                    return StarSignRealms[starSign].FirstOrDefault(r => r.Name == name) ?? StarSignRealms[starSign][0];
                }
                return Realms.FirstOrDefault(r => r.Name == name) ?? Realms[0];
            }

            public RealmInfo GetNextRealmForStarSign(RealmInfo current, string starSign)
            {
                if (StarSignRealms.ContainsKey(starSign))
                {
                    return StarSignRealms[starSign].FirstOrDefault(r => r.Level == current.Level + 1);
                }
                return Realms.FirstOrDefault(r => r.Level == current.Level + 1);
            }

            public List<string> GetConstellationEffects(string starSign, int level)
            {
                if (StarSignConstellations.ContainsKey(starSign) && level > 0)
                {
                    var effects = StarSignConstellations[starSign];
                    return effects.Take(level).ToList();
                }
                return new List<string>();
            }

            public List<RealmBuff> GetConstellationBuffs(string starSign, int level)
            {
                var buffs = new List<RealmBuff>();
                if (ConstellationBuffs.ContainsKey(starSign) && level > 0)
                {
                    var starSignBuffs = ConstellationBuffs[starSign];
                    for (int i = 0; i < Math.Min(level, starSignBuffs.Count); i++)
                    {
                        buffs.AddRange(starSignBuffs[i]);
                    }
                }
                return buffs;
            }

            public RealmInfo GetRealmInfo(string name) => GetRealmInfoForStarSign(name, "");
            public RealmInfo GetNextRealm(RealmInfo current) => GetNextRealmForStarSign(current, "");

            public static void Load(string path)
            {
                try
                {
                    if (File.Exists(path))
                    {
                        string json = File.ReadAllText(path);
                        Instance = JsonConvert.DeserializeObject<XiuXianConfig>(json);

                        // 设置默认初始寿元如果配置中没有
                        if (Instance.InitialLifeYears <= 0)
                        {
                            Instance.InitialLifeYears = 100;
                        }

                        foreach (var realms in Instance.StarSignRealms.Values)
                        {
                            foreach (var realm in realms)
                            {
                                if (realm.ProgressLimits == null)
                                    realm.ProgressLimits = new List<string>();

                                if (realm.RewardGoods == null)
                                    realm.RewardGoods = Array.Empty<Item>();

                                if (realm.RewardBuffs == null)
                                    realm.RewardBuffs = Array.Empty<RealmBuff>();
                            }
                        }

                        foreach (var starSign in Instance.StarSigns)
                        {
                            if (starSign.Icon == 0)
                            {
                                starSign.Icon = starSign.TerrariaClass switch
                                {
                                    "战士" => 497,
                                    "射手" => 498,
                                    "法师" => 499,
                                    "召唤师" => 500,
                                    "盗贼" => 501,
                                    "牧师" => 502,
                                    _ => 65
                                };
                            }
                        }

                        if (Instance.TopUIRefreshInterval < 1 || Instance.TopUIRefreshInterval > 3600000)
                        {
                            Instance.TopUIRefreshInterval = 10;
                        }

                        if (Instance.ChatUIRefreshInterval < 1 || Instance.ChatUIRefreshInterval > 3600000)
                        {
                            Instance.ChatUIRefreshInterval = 900000;
                        }

                        if (Instance.LifeDepletionCountdown < 5 || Instance.LifeDepletionCountdown > 300)
                        {
                            Instance.LifeDepletionCountdown = 20;
                        }

                        TShock.Log.Info($"修仙配置已加载，服务器名称: {Instance.ServerName}");
                        TShock.Log.Info($"已加载 {Instance.StarSigns.Count} 个星宿职业系统");
                        TShock.Log.Info($"初始寿元: {Instance.InitialLifeYears}年");
                    }
                    else
                    {
                        Instance = new XiuXianConfig();
                        File.WriteAllText(path, JsonConvert.SerializeObject(Instance, Formatting.Indented));
                    }
                }
                catch (Exception ex)
                {
                    TShock.Log.Error($"加载修仙配置失败: {ex.Message}");
                    Instance = new XiuXianConfig();
                }
            }

            public static void Save(string path)
            {
                try
                {
                    File.WriteAllText(path, JsonConvert.SerializeObject(Instance, Formatting.Indented));
                    TShock.Log.Info($"修仙配置已保存，服务器名称: {Instance.ServerName}");
                }
                catch (Exception ex)
                {
                    TShock.Log.Error($"保存修仙配置失败: {ex.Message}");
                }
            }
        }

        public class XiuXianData
        {
            public static Dictionary<string, XiuXianData> Players = new Dictionary<string, XiuXianData>();

            public string Name { get; set; }
            public string Realm { get; set; } = "凡人";
            public int CultivationProgress { get; set; } = 0;
            public float LifeYears { get; set; } = 80;
            public int RebirthCount { get; set; } = 0;
            public bool InHolyLand { get; set; } = false;
            public string StarSign { get; set; } = "未选择";
            public int ConstellationLevel { get; set; } = 0;
            public int ConstellationPoints { get; set; } = 0;
            public string DharmaName { get; set; } = "";
            public string NameColor { get; set; } = "FF69B4";
            public bool LifeDepletionWarned { get; set; } = false;
            public DateTime LifeDepletionTime { get; set; } = DateTime.MinValue;
            public int TotalOnlineMinutes { get; set; } = 0;
            public bool IsGhostState { get; set; } = false;
            public int GhostBattleNpcIndex { get; set; } = -1;

            [JsonProperty("击杀Boss")]
            public HashSet<string> KilledBosses { get; set; } = new HashSet<string>();

            public static XiuXianData GetPlayer(string name)
            {
                if (!Players.TryGetValue(name, out XiuXianData data))
                {
                    data = new XiuXianData { Name = name };
                    Players[name] = data;
                }
                return data;
            }

            public static void Load(string path)
            {
                try
                {
                    if (File.Exists(path))
                    {
                        Players = JsonConvert.DeserializeObject<Dictionary<string, XiuXianData>>(File.ReadAllText(path))
                                 ?? new Dictionary<string, XiuXianData>();

                        foreach (var data in Players.Values)
                        {
                            if (data.LifeDepletionTime == default)
                                data.LifeDepletionTime = DateTime.MinValue;

                            if (data.KilledBosses == null)
                                data.KilledBosses = new HashSet<string>();
                                
                            // 初始化新增字段
                            if (data.TotalOnlineMinutes == 0)
                                data.TotalOnlineMinutes = 0;
                                
                            if (!data.IsGhostState)
                                data.IsGhostState = false;
                                
                            if (data.GhostBattleNpcIndex == 0)
                                data.GhostBattleNpcIndex = -1;
                        }

                        TShock.Log.Info($"修仙数据已加载，{Players.Count}名修士");
                    }
                }
                catch (Exception ex)
                {
                    TShock.Log.Error($"加载修仙数据失败: {ex.Message}");
                }
            }

            public static void Save(string path)
            {
                try
                {
                    File.WriteAllText(path, JsonConvert.SerializeObject(Players, Formatting.Indented));
                    TShock.Log.Info($"修仙数据已保存，{Players.Count}名修士");
                }
                catch (Exception ex)
                {
                    TShock.Log.Error($"保存修仙数据失败: {ex.Message}");
                }
            }

            public static void SavePlayer(string name)
            {
                try
                {
                    if (Players.ContainsKey(name))
                        Save(DataPath);
                }
                catch (Exception ex)
                {
                    TShock.Log.Error($"保存玩家 {name} 数据失败: {ex.Message}");
                }
            }
        }
        #endregion

        #region StatusManager 实现
        private class StatusManager
        {
            private class PlayerStatus
            {
                public string Key { get; set; }
                public string Text { get; set; }
            }

            private readonly Dictionary<TSPlayer, List<PlayerStatus>> _playerStatuses = new Dictionary<TSPlayer, List<PlayerStatus>>();

            public void AddOrUpdateText(TSPlayer player, string key, string text)
            {
                if (!_playerStatuses.TryGetValue(player, out var statuses))
                {
                    statuses = new List<PlayerStatus>();
                    _playerStatuses[player] = statuses;
                }

                var existing = statuses.FirstOrDefault(s => s.Key == key);
                if (existing != null)
                {
                    existing.Text = text;
                }
                else
                {
                    statuses.Add(new PlayerStatus { Key = key, Text = text });
                }

                UpdatePlayerStatus(player);
            }

            public void RemoveText(TSPlayer player, string key)
            {
                if (_playerStatuses.TryGetValue(player, out var statuses))
                {
                    var item = statuses.FirstOrDefault(s => s.Key == key);
                    if (item != null)
                    {
                        statuses.Remove(item);
                        UpdatePlayerStatus(player);
                    }
                }
            }

            public void RemoveText(TSPlayer player)
            {
                if (_playerStatuses.ContainsKey(player))
                {
                    _playerStatuses.Remove(player);
                    player.SendData(PacketTypes.Status, "", 0, 0x1f);
                }
            }

            private void UpdatePlayerStatus(TSPlayer player)
            {
                if (!_playerStatuses.TryGetValue(player, out var statuses) || !statuses.Any())
                    return;

                var combined = new StringBuilder();
                foreach (var status in statuses)
                {
                    combined.AppendLine(status.Text);
                }

                player.SendData(PacketTypes.Status, combined.ToString(), 0, 0x1f);
            }
        }
        #endregion
    }
    
    public static class StringExtensions
    {
        public static string Color(this string text, Color color)
        {
            return $"[c/{color.R:X2}{color.G:X2}{color.B:X2}:{text}]";
        }
    }
}