using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;
using Terraria;
using TShockAPI.Hooks;
using TShockAPI;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;
using System.Threading;
using System.IO;
using SurvivalCrisis.Effects;

namespace SurvivalCrisis
{
    using GItems = Nets.NetGroundItemCollection;
    using TeleportPylonInfo = Terraria.GameContent.TeleportPylonInfo;
    using TeleportPylonType = Terraria.GameContent.TeleportPylonType;
    using PSystem = Terraria.GameContent.TeleportPylonsSystem;
    [ApiVersion(2, 1)]
    public partial class SurvivalCrisis : TerrariaPlugin
    {
        #region Fields
        /// <summary>
        /// 聊天半径(按方格)
        /// </summary>
        private const int ChatRadius = 50;
        public const int EventTime1 = SunsetToMidnight;
        public const int EventTime2 = SunsetToMidnight + 7 * 60 * 60;
        public const int EventTime3 = SunsetToMidnight + 15 * 60 * 60;
        public const int AvalancheTime = NightTime + DawnToNoon;
        public const int NightTime = (int)(4.5 + 12 - 7.5) * 60 * 60;
        public const int DawnToNoon = (int)(12 - 4.5) * 60 * 60;
        public const int SunsetToMidnight = (int)(12 - 7.5) * 60 * 60;
        public const int DayTime = (int)(19.5 - 4.5) * 60 * 60;
        public const int PeaceTime = 60 * 60 * 3;
        public const int BossComingTime = NightTime + DayTime + FPTime + 5;
        public const int FBPTime = 3 * 60 * 60;
        public const int FPTime = 2 * 60 * 60;
        public const int TotalTime = BossComingTime + NightTime;
        public const string IslandsPath = @"SurvivalCrisis\Islands";
        public const string SpheresPath = @"SurvivalCrisis\Spheres";
        private const string SavePath = @"SurvivalCrisis\CrisisDatas.json";
        private const string ConfigPath = @"SurvivalCrisis\CrisisConfig.json";
        private int[] FinalBossTypes;
        private int? gameStartDelay;
        private int timer;
        private Task<bool> generateMap;
        private CancellationTokenSource gMapToken;
        #endregion
        #region Properties
        public static Random Rand
        {
            get;
            private set;
        }
        public static SurvivalCrisis Instance
        {
            get;
            private set;
        }

        public DataManager DataBase { get; }
        public GamePlayer[] Players { get; }
        public GItems GroundItems { get; }
        public CrisisConfig Config
        {
            get;
            private set;
        }

        public Dictionary<int, string> Titles { get; }
        public Dictionary<int, string> Prefixs { get; }
        public int PrefixIDMin { get; }
        public int PrefixIDMax { get; }
        public int TitleIDMin { get; }
        public int TitleIDMax { get; }

        //[Obsolete("事件没做完")]
        // 事件没机会做完了
        public SpecialEvent[] Events
        {
            get;
            private set;
        }
        public Queue<Effect> Effects
        {
            get;
            private set;
        }
        public bool IsFinalBattleTime
        {
            get;
            private set;
        }
        public bool IsInGame
        {
            get;
            private set;
        }
        public int GameTime
        {
            get;
            private set;
        }
        public int FinalBossType
        {
            get;
            private set;
        }
        public int? FinalBossIndex
        {
            get;
            private set;
        }
        public NPC FinalBoss
        {
            get
            {
                return Main.npc[(int)FinalBossIndex];
            }
        }
        public int? FinalBossLifeMax
        {
            get;
            private set;
        }
        public int? FinalBossLifeRest
        {
            get;
            private set;
        }
        public int SurvivorCounts
        {
            get;
            private set;
        }
        public GamePlayer[] Participants
        {
            get;
            private set;
        }

        public int ChestUnlocked
		{
            get;
            set;
		}
        public bool EnabledGlobalChat
        {
            get;
            set;
        }
        public bool IsLongLongDark
        {
            get;
            set;
        }
        public bool IsMagnetStorm
        {
            get;
            set;
        }
        public bool IsFarce
        {
            get;
            set;
        }
        public int TraitorEMPTime
        {
            get;
            private set;
        }
        public SurvivorTask CurrentTask
        {
            get;
            private set;
        }
        public List<Point> MazePylons 
        { 
            get;
            set;
        }
        public Shop TraitorShop { get; }
        public bool[] ChestOpened { get; }

        #endregion
        #region Plugin Infos
        public override Version Version => new (1,0,0);
        public override string Author => "TOFOUT";
        public override string Name => nameof(SurvivalCrisis);
        public override string Description => "a survival game";
        #endregion
        #region Initialize & Dispose
        public SurvivalCrisis(Main game) : base(game)
        {
            if (!Directory.Exists("SurvivalCrisis"))
            {
                Directory.CreateDirectory("SurvivalCrisis");
            }

            Instance = this;
            DataBase = new DataManager(SavePath);
            Players = new GamePlayer[Main.maxPlayers];
            GroundItems = new GItems();
            TraitorShop = new Shop(PlayerIdentity.Traitor);
            ChestOpened = new bool[Main.maxChests];

            // ba55d3 淡紫色(186,85,211)

            string lightPurple = "ba55d3";
            string lightBlue = "87ceeb";
            string golden = "ffd700";
            string lightRed = "f08080";

            Prefixs = new Dictionary<int, string>
            {
                [-8] = $"[c/{lightPurple}:腰缠万贯的]",
                [-7] = $"[c/{lightRed}:蛮不讲理的]",
                [-6] = $"[c/{golden}:深明大义的]",
                [-5] = $"[c/{lightBlue}:心如止水的]",
                [-4] = $"[c/{lightPurple}:实事求是的]",
                [-3] = $"[c/{lightRed}:嗜杀成性的]",
                [-2] = $"[c/{golden}:明察秋毫的]",
                [-1] = $"[c/{lightBlue}:沉默寡言的]",
                [0] = string.Empty,
                [1] = "英勇的",
                [2] = "无畏的",
                [3] = "冷静的",
                [4] = "淡定的"
            };
            Titles = new Dictionary<int, string>
            {
                [0] = string.Empty,
                [1] = "宝藏猎人",
                [2] = "幕后黑手",
                [3] = "吟游诗人",
                [4] = "受害者"
            };

            PrefixIDMin = -8;
            PrefixIDMax = 0;
            TitleIDMin = 0;
            TitleIDMax = 4;
        }
        public override void Initialize()
        {
            AddCommands();
            InitializeVars();
            AttachHooks();
        }
        protected override void Dispose(bool disposing)
        {
            if (gMapToken != null)
            {
                gMapToken.Cancel();
                gMapToken = null;
            }
            RemoveCommands();
            RemoveHooks();
            DataBase.Save();
            base.Dispose(disposing);
        }
        private void InitializeVars()
        {
            Rand = new Random();
            Effects = new Queue<Effect>();
            FinalBossTypes = new int[]
            {
                NPCID.SkeletronHead,
                NPCID.SkeletronPrime,
                NPCID.TheDestroyer,
                NPCID.Retinazer,
                NPCID.Spazmatism
            };
            #region LoadConfig
            if (!File.Exists(ConfigPath))
            {
                Config = new CrisisConfig();
                Config.Save(ConfigPath);
            }
            else
            {
                Config = CrisisConfig.LoadFile(ConfigPath);
            }
            #endregion
            #region LoadRegions
            Regions.Initialize(Config);
            #endregion
            #region LoadEvents
            //var types = typeof(SpecialEvent).Assembly.GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(SpecialEvent))).ToArray();
            //Events = new SpecialEvent[types.Length];
            //for (int i = 0; i < Events.Length; i++)
            //{
            //	Events[i] = (SpecialEvent)Activator.CreateInstance(types[i]);
            //}
            #endregion
        }
        private void AddCommands()
        {
            Commands.ChatCommands.Add(new Command("sc.hotreload", HotReloadCmd, "hotreload"));
            Commands.ChatCommands.Add(new Command("sc.debug", DebugCommand, "scd"));
            Commands.ChatCommands.Add(new Command("sc.player", PlayerCommand, "sc"));
        }
        private void AttachHooks()
        {
            #region ServerApi
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
            ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
            ServerApi.Hooks.NpcStrike.Register(this, OnNpcStrike);
            ServerApi.Hooks.NpcKilled.Register(this, OnNPCKilled);
            ServerApi.Hooks.NetGetData.Register(this, OnGetData, 0);
            #endregion
            #region TShock
            PlayerHooks.PlayerPostLogin += OnLogin;
            PlayerHooks.PlayerLogout += OnLogout;
            PlayerHooks.PlayerChat += OnChat;
            #endregion
            #region GetdataHandlers
            GetDataHandlers.KillMe += OnKillMe;
            GetDataHandlers.PlayerDamage += OnPlayerDamage;
            GetDataHandlers.ChestOpen += OnChestOpen;
            GetDataHandlers.PlayerTeam += OnChangeTeam;
            GetDataHandlers.TogglePvp += OnTogglePvp;
            GetDataHandlers.TileEdit += OnTileEdit;
            GetDataHandlers.PlaceObject += OnPlaceObject;
            GetDataHandlers.ReadNetModule += OnReadNetModule;
            GetDataHandlers.PlayerSlot += OnPlayerSlot;
            GetDataHandlers.Teleport += OnTeleport;
            GetDataHandlers.PlayerSpawn += OnPlayerSpawn;
            #endregion
        }
        private void RemoveCommands()
        {
            Commands.ChatCommands.RemoveAll(cmd => cmd.HasAlias("hotreload"));
            Commands.ChatCommands.RemoveAll(cmd => cmd.HasAlias("scd"));
            Commands.ChatCommands.RemoveAll(cmd => cmd.HasAlias("sc"));
        }
        private void RemoveHooks()
        {
            #region ServerApi
            ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
            ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
            ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
            ServerApi.Hooks.NpcStrike.Deregister(this, OnNpcStrike);
            ServerApi.Hooks.NpcKilled.Deregister(this, OnNPCKilled);
            ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
            #endregion
            #region TShock
            PlayerHooks.PlayerPostLogin -= OnLogin;
            PlayerHooks.PlayerLogout -= OnLogout;
            PlayerHooks.PlayerChat -= OnChat;
            #endregion
            #region GetdataHandlers
            GetDataHandlers.KillMe -= OnKillMe;
            GetDataHandlers.PlayerDamage -= OnPlayerDamage;
            GetDataHandlers.ChestOpen -= OnChestOpen;
            GetDataHandlers.PlayerTeam -= OnChangeTeam;
            GetDataHandlers.TogglePvp -= OnTogglePvp;
            GetDataHandlers.TileEdit -= OnTileEdit;
            GetDataHandlers.PlaceObject -= OnPlaceObject;
            GetDataHandlers.ReadNetModule -= OnReadNetModule;
            GetDataHandlers.PlayerSlot -= OnPlayerSlot;
            GetDataHandlers.Teleport -= OnTeleport;
            GetDataHandlers.PlayerSpawn -= OnPlayerSpawn;
            #endregion
        }
        #endregion
        #region Hooks
        #region OnGetData
        private void OnGetData(GetDataEventArgs args)
        {
            if (!IsInGame)
            {
                return;
            }
            switch (args.MsgID)
            {
                case PacketTypes.Teleport:
                    {

                        using var stream = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length);
                        using var reader = new BinaryReader(stream);
                        BitsByte bitsByte6 = reader.ReadByte();
                        int index = reader.ReadInt16();
                        index = args.Msg.whoAmI;
                        Vector2 vector = reader.ReadVector2();
                        int num28 = 0;
                        num28 = reader.ReadByte();
                        int num29 = 0;
                        if (bitsByte6[0])
                        {
                            num29++;
                        }
                        if (bitsByte6[1])
                        {
                            num29 += 2;
                        }
                        bool flag3 = false;
                        if (bitsByte6[2])
                        {
                            flag3 = true;
                        }
                        int num30 = 0;
                        if (bitsByte6[3])
                        {
                            num30 = reader.ReadInt32();
                        }
                        if (flag3)
                        {
                            // vector = Main.player[index].position;
                        }
                        switch (num29)
                        {
                            case 2:
                                {
                                    // Main.player[index].Teleport(vector, num28, num30);
                                    int tI = -1;
                                    float dis = 9999f;
                                    for (int i = 0; i < 255; i++)
                                    {
                                        if (Main.player[i].active && i != index)
                                        {
                                            Vector2 dist = Main.player[i].position - vector;
                                            if (dist.Length() < dis)
                                            {
                                                dis = dist.Length();
                                                tI = i;
                                            }
                                        }
                                    }
                                    if (tI >= 0)
                                    {
                                        var player = Players[index];
                                        if (player.CanVote)
                                        {
                                            var target = Players[tI];
                                            if (target != null)
                                            {
                                                target.VotedCount++;
#if DEBUG
                                                BCToAll($"{player.Index}号投了{target.Index}号一票", Color.HotPink);
#endif
                                                BCToAll($"{player.Index}号已投票", Color.HotPink);player.TSPlayer.SendInfoMessage($"你票了{target.Index}号");
                                            }
                                            player.CanVote = false;
                                            player.TeleportTo(player.TPlayer.position);
                                            args.Handled = true;
                                        }
                                        // ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Game.HasTeleportedTo", Main.player[whoAmI].name, Main.player[tI].name), new Color(250, 250, 0));
                                    }
                                    break;
                                }
                        }
                    }
                    break;
                case PacketTypes.SpawnBossorInvasion:
                    {
                        args.Handled = true;
                        using var stream = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length);
                        using var reader = new BinaryReader(stream);
                        int index = reader.ReadInt16();
                        int type = reader.ReadInt16();
                        if (type < 0)
                        {
                            if (type > -5)
                            {
                                type *= -1;
                            }
                            if (IsInGame)
                            {
                                if (type == InvasionID.GoblinArmy)
                                {
                                    Players[index].AddPerformanceScore(8, "干扰通讯");
                                    GameOperations.AddEffect(new ChatInterference(60 * 60 * 3));
                                }
                            }
                        }
                        break;
                    }
                case PacketTypes.ChestUnlock:
                    {
                        args.Handled = true;
                        using var stream = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length);
                        using var reader = new BinaryReader(stream);
                        int type = reader.ReadByte();
                        int x = reader.ReadInt16();
                        int y = reader.ReadInt16();
                        if (type == 1)
                        {
                            var player = Players[args.Msg.whoAmI];
                            Chest.Unlock(x, y);
                            if (ChestUnlocked == 0)
                            {
                                player.AddPerformanceScore(25, "解锁7级箱");
                            }
                            else
							{
                                var chest = Main.chest[Chest.FindChest(x, y)];
                                foreach(var item in chest.item)
								{
                                    item.TurnToAir();
								}
                                chest.item[0].SetDefaults(ItemID.PocketMirror);
                                player.SendText("这份财富只独属于一人，它已被人捷足先登...", Color.DarkGreen);
                                player.SendText("你在箱底翻到了一面镜子", Color.Green);
                                player.AddPerformanceScore(10, "残羹剩饭");
                            }
                            ChestUnlocked++;
                            NetMessage.TrySendData(52, -1, args.Msg.whoAmI, null, 0, type, x, y);
                            NetMessage.SendTileSquare(-1, x, y, 2);
                        }
                        if (type == 2)
                        {
                            NetMessage.TrySendData(52, -1, args.Msg.whoAmI, null, 0, type, x, y);
                            NetMessage.SendTileSquare(-1, x, y, 2);
                        }
                        break;
                    }
                case PacketTypes.PlayerHp:
                    {
                        using var stream = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length);
                        using var reader = new BinaryReader(stream);
                        int index = reader.ReadByte();
                        if (Players[index].Identity == PlayerIdentity.Watcher)
                        {
                            return;
                        }
                        if (index != Main.myPlayer || Main.ServerSideCharacter)
                        {
                            index = args.Msg.whoAmI;
                            var player = Players[index];
                            var statLife = reader.ReadInt16();
                            var statLifeMax = reader.ReadInt16();
                            if (statLifeMax > player.LifeMax)
                            {
                                if (player.LifeMax < 400)
                                {
                                    player.AddPerformanceScore((statLifeMax - player.LifeMax) * 6 / 20, "提高生命上限");
                                }
                                else
                                {
                                    player.AddPerformanceScore((statLifeMax - player.LifeMax) * 7 / 5, "提高生命上限");
                                }
                            }
                        }
                        break;
                    }
                case PacketTypes.PlayerMana:
                    {
                        using var stream = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length);
                        using var reader = new BinaryReader(stream);
                        int index = reader.ReadByte();
                        if (Main.netMode == 2)
                        {
                            index = args.Msg.whoAmI;
                        }

                        var player = Players[index];
                        int statMana = reader.ReadInt16();
                        int statManaMax = reader.ReadInt16();
                        if (statManaMax > player.ManaMax)
                        {
                            player.AddPerformanceScore((statManaMax - player.ManaMax) * 10 / 20, "提高魔力上限");
                        }
                        break;
                    }
                case PacketTypes.ItemOwner:
                    {
                        if (Players[args.Msg.whoAmI].Identity == PlayerIdentity.Watcher)
                        {
                            args.Handled = true;
                        }
                        break;
                    }
            }
        }
        #endregion
        #region OnReadNetModule
        private void OnReadNetModule(object sender, GetDataHandlers.ReadNetModuleEventArgs args)
        {
            if (args.ModuleType == GetDataHandlers.NetModuleType.TeleportPylon)
            {
                args.Handled = true;
                return;
#if false
                args.Data.Position = 2;
                var reader = new BinaryReader(args.Data);
                var eventType = reader.ReadByte();
                switch (eventType)
                {
                    case 0:
                    case 1:
                        break;
                    case 2:
                        {
                            var info = default(TeleportPylonInfo);
                            var playerIndex = args.Player.Index;
                            info.PositionInTiles = new Point16(reader.ReadInt16(), reader.ReadInt16());
                            info.TypeOfPylon = (TeleportPylonType)reader.ReadByte();
                            {

                                Player player = Main.player[playerIndex];
                                if (PSystem.IsPlayerNearAPylon(player))
                                {
                                    Vector2 newPos = info.PositionInTiles.ToWorldCoordinates() - new Vector2(0f, player.HeightOffsetBoost);
                                    int num2 = 9;
                                    int typeOfPylon = (int)info.TypeOfPylon;
                                    int number = 0;
                                    player.Teleport(newPos, num2, typeOfPylon);
                                    player.velocity = Vector2.Zero;
                                    if (Main.netMode == 2)
                                    {
                                        RemoteClient.CheckSection(player.whoAmI, player.position);
                                        NetMessage.SendData(65, -1, -1, null, 0, player.whoAmI, newPos.X, newPos.Y, num2, number, typeOfPylon);
                                    }
                                }
                                else
                                {
                                    var key = "Net.CannotTeleportToPylonBecausePlayerIsNotNearAPylon";
                                    Terraria.Chat.ChatHelper.SendChatMessageToClient(Terraria.Localization.NetworkText.FromKey(key), new Color(255, 240, 20), playerIndex);
                                }
                            }
                            break;
                        }
                }
                args.Handled = true;
#endif
            }
        }
        #endregion
        #region OnChestOpen
        private void OnChestOpen(object sender, GetDataHandlers.ChestOpenEventArgs args)
        {
            var player = Players[args.Player.Index];
            if (IsInGame)
            {
                if (player.Identity == PlayerIdentity.Watcher)
                {
                    args.Handled = true;
                }
                else
                {
                    var chestIndex = Chest.FindChest(args.X, args.Y);
                    if (0 <= chestIndex && chestIndex < ChestOpened.Length && !ChestOpened[chestIndex])
                    {
                        ChestOpened[chestIndex] = true;
                        player.ChestsOpened++;
                        if (Main.chest[chestIndex].item.Count(item => item.type == ItemID.GoldenKey) > 0)
                        {
                            player.AddPerformanceScore(20, "发现金钥匙");
                            player.GoldenKeyFound++;
                        }
                    }
                }
            }
        }
        #endregion
        #region OnChat
        private void OnChat(PlayerChatEventArgs args)
        {
            if (!IsInGame)
            {
                return;
            }
            var player = Players[args.Player.Index];
            if (player.Identity == PlayerIdentity.Watcher)
            {
                var grouP = player.TSPlayer.Group;
                BCToWatcher("[旁观者频道]" + args.TShockFormattedText, new Color(grouP.R, grouP.G, grouP.B));
                TSPlayer.Server.SendConsoleMessage(args.TShockFormattedText, grouP.R, grouP.G, grouP.B);
                args.Handled = true;
                return;
            }
            else
            {
                player.ChatCount++;
                args.TShockFormattedText = args.TShockFormattedText.Replace(args.Player.Name, $"{player.Prefix}{player.Title}{args.Player.Index}号");
            }
            if (IsFinalBattleTime)
            {
                var prefix = player.Identity switch
                {
                    PlayerIdentity.Survivor => "[生存者]",
                    PlayerIdentity.Traitor => "[背板者]",
                    _ => ""
                };
                args.TShockFormattedText = prefix + args.TShockFormattedText;
                return;
            }
            var group = args.Player.Group;
            var color = new Color(group.R, group.G, group.B);
            if (player.Identity == PlayerIdentity.Watcher)
            {
                BCToWatcher("[旁观者频道]" + args.TShockFormattedText, color);
            }
            else if (EnabledGlobalChat && TraitorEMPTime == 0 && player.HeldItem.type == ItemID.WeatherRadio)
            {
                BCToAll("[全图广播]" + args.TShockFormattedText, color);
            }
            else if (player.Equipped(ItemID.SpectreGoggles))
            {
                BCToTraitor("[背板者频道]" + args.TShockFormattedText, color);
            }
            else
            {
                if (Regions.Hall.InRange(player))
                {
                    BCToHall("[大厅]" + args.TShockFormattedText, color);
                    //BCToWatcher("[大厅]" + args.TShockFormattedText, color);
                }
                else
                {
                    string prefix = player.Name + "=>";
                    foreach (var ply in Players)
                    {
                        if (ply?.TSPlayer == null || ply.Identity == PlayerIdentity.Watcher)
                        {
                            continue;
                        }
                        if (Vector2.Distance(ply.TPlayer.Center, player.TPlayer.Center) < ChatRadius * 16)
                        {
                            prefix += ply.Name + "、";
                            ply.SendText(args.TShockFormattedText, color);
                        }
                    }
                    BCToWatcher(prefix + ":" + args.TShockFormattedText, color);
                }
            }
            TSPlayer.Server.SendConsoleMessage(args.TShockFormattedText, group.R, group.G, group.B);
            args.Handled = true;
        }
        #endregion
        #region Pvp/Team
        private void OnChangeTeam(object sender, GetDataHandlers.PlayerTeamEventArgs args)
        {
            var player = TShock.Players[args.PlayerId];
            if (IsInGame)
            {
                if (GameTime > BossComingTime)
                {

                }
                else if (player.TPlayer.hostile || TShock.Players.Count(p => p?.Team == args.Team) >= 2|| GameTime > BossComingTime - FBPTime)
                {
                    args.Handled = true;
                    player.SetTeam(player.Team);
                }
            }
        }
        private void OnTogglePvp(object sender, GetDataHandlers.TogglePvpEventArgs args)
        {
            args.Handled = true;
            var player = Players[args.PlayerId];
            player.Pvp = player.TPlayer.hostile;
        }
        #endregion
        #region OnKillMe
        private void OnKillMe(object sender, GetDataHandlers.KillMeEventArgs args)
		{
			args.Handled = true;
            if (Players[args.PlayerId].Identity != PlayerIdentity.Watcher)
            {
                Players[args.PlayerId].OnKill(args);
            }
        }
        #endregion
        #region OnPlayerDamage
        private void OnPlayerDamage(object sender, GetDataHandlers.PlayerDamageEventArgs args)
        {
            if (args.PVP)
            {
                var attacker = Players[args.PlayerDeathReason._sourcePlayerIndex];
                var player = Players[args.ID];
                for (int i = 0; i < attacker.TPlayer.armor.Length; i++)
                {
                    if (attacker.TPlayer.armor[i].type == ItemID.PocketMirror)
                    {
                        var identity = player.Party == PlayerIdentity.Traitor ? "[c/ff0000:背叛者]" : "[c/008000:生存者]";
                        attacker.SendText($"{player.Index}号的身份是{identity}", Color.Aqua);
                        attacker.TPlayer.armor[i].TurnToAir();
                        attacker.SendData(PacketTypes.PlayerSlot, "", attacker.Index, i + NetItem.ArmorIndex.Item1);
                        attacker.AddPerformanceScore(8, "镜子验人");
                        if (attacker.Party == PlayerIdentity.Survivor && player.Party == PlayerIdentity.Traitor)
                        {
                            if (!attacker.HasPrefix(-6))
                            {
                                attacker.AddPrefix(-6);
                            }
                            attacker.AddPerformanceScore(30, "发现背叛者");
                        }
                        break;
                    }
                }
            }
        }
        #endregion
        #region OnHurt
        private void OnHurt(object sender, GetDataHandlers.PlayerDamageEventArgs args)
        {
            Players[args.Player.Index].OnHurt(args);
        }
        #endregion
        #region OnUpdate
        private void OnUpdate(object args)
        {
            if (Main.invasionType != 0)
            {
                Main.invasionType = 0;
            }
#if DEBUG
            const int leastPlayerCount = 1;
#else
			const int leastPlayerCount = 4;
#endif
            timer++;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].aiStyle != 7)
                {
                    if (Regions.WaitingZone.InRange(Main.npc[i]) || Regions.Lobby.InRange(Main.npc[i]))
                    {
                        Main.npc[i].type = 0;
                        Main.npc[i].active = false;
                        TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", i);
                    }
                }
            }
            #region PlayerUpdate
            for (int i = 0; i < Main.myPlayer; i++)
            {
                if (Players[i] != null && TShock.Players[i] == null)
                {
                    Players[i] = null;
                    continue;
                }
                if (Players[i] == null)
                {
                    continue;
                }
                Players[i].Update();
            }
            #endregion
            #region Waiting
            if (gameStartDelay == null)
            {
                var count = Players.Count(player => player?.IsGuest == false && Regions.WaitingZone.InRange(player));
                if (count >= leastPlayerCount)
                {
                    gameStartDelay = 30 * 60;
                }
                else
                {
                    if (count > 0 && timer % 150 == 0)
                    {
                        var pos = new Vector2((Regions.WaitingZone.Left + Regions.WaitingZone.Right) / 2f, Regions.WaitingZone.CenterY) * 16;
                        Utils.SendCombatText(Texts.NotEnoughPlayers, Color.Pink, pos);
                    }
                }
            }
            else if (gameStartDelay > 0)
            {
                gameStartDelay--;
                if (gameStartDelay % 90 == 0)
                {
                    if (Players.Count(player => player?.IsGuest == false && Regions.WaitingZone.InRange(player)) < leastPlayerCount)
                    {
                        gameStartDelay = null;
                        if (gMapToken != null)
                        {
                            gMapToken.Cancel();
                            gMapToken = null;
                            generateMap = null;
                        }
                        Utils.SendCombatText(Texts.NotEnoughPlayers, Color.Pink, Regions.WaitingZone.Center);
                        return;
                    }
                }
                if (gameStartDelay % 180 == 0)
                {
                    Utils.SendCombatText($"{gameStartDelay / 60}", Color.Pink, Regions.WaitingZone.Center);
                }
                if (gameStartDelay == 15 * 60)
                {
                    generateMap = GenerateMapAsync();
                }
                else if (gameStartDelay == 0)
                {
                    if (generateMap.IsCompleted)
                    {
                        if (generateMap.Status == TaskStatus.Faulted)
                        {
                            DebugMessage(generateMap.Exception.ToString());
                        }
                        GameOperations.GameStart();
                    }
                    else
                    {
                        gameStartDelay += 10 * 60;
                        BCToAll(Texts.NotCompleteMap, Color.Blue);
                    }
                }
            }
            else if (IsInGame)
            {
                GameOperations.GameUpdate();
            }
            #endregion
        }
        #endregion
        #region Join/Leave/Login/Logout
        private void OnLogout(PlayerLogoutEventArgs args)
        {
            Players[args.Player.Index] = GamePlayer.Guest(args.Player.Index);
        }

        private void OnLogin(PlayerPostLoginEventArgs args)
        {
            Players[args.Player.Index] = DataBase.GetPlayer(args.Player.Index);
        }

        private void OnJoin(JoinEventArgs args)
        {
            Players[args.Who] ??= GamePlayer.Guest(args.Who);
        }

        private void OnLeave(LeaveEventArgs args)
        {
            Players[args.Who] = null;
        }
        #endregion
        #region OnNPCKilled
        private void OnNPCKilled(NpcKilledEventArgs args)
        {
            if (IsFinalBattleTime && args.npc == FinalBoss && args.npc.type == FinalBossType)
            {
                GameOperations.GameEnd(PlayerIdentity.Survivor);
            }
            else if (IsInGame)
            {
                var pos = args.npc.position;
                var rBox = args.npc.Size;
                void DropItem(int type, int stack = 1, int prefix = -1, double probability = 1)
                {
                    if (Rand.NextDouble() < probability)
                    {
                        Item.NewItem(new EntitySource_DebugCommand(), pos, rBox, type, stack, false, prefix);
                    }
                }
                switch (args.npc.type)
                {
                    case NPCID.HellArmoredBonesMace:
                    case NPCID.HellArmoredBonesSpikeShield:
                    case NPCID.HellArmoredBonesSword:
                    case NPCID.HellArmoredBones:
                        {
                            var a = Rand.NextDouble();
                            if (a < 0.7)
                            {
                                int[] moltenArmors = { ItemID.MoltenBreastplate, ItemID.MoltenHelmet, ItemID.MoltenGreaves };
                                Rand.Shuffle(moltenArmors);
                                DropItem(ItemID.ShadowKey);
                                DropItem(moltenArmors[0]);
                                DropItem(moltenArmors[1]);
                            }
                            else if (a < 1)
                            {
                                DropItem(ItemID.GoldenKey);
                            }
                            DropItem(ItemID.InfernoFork);
                        }
                        break;
                    case NPCID.Mimic:
                        {
                            var a = Rand.NextDouble();
                            if (a < 0.4)
                            {
                                DropItem(ItemID.DD2BallistraTowerT1Popper, Rand.Next(20, 50));
                            }
                            else if (a < 0.7)
                            {
                                DropItem(ItemID.DD2BallistraTowerT2Popper, Rand.Next(15, 40));
                            }
                            else if (a < 0.9)
                            {
                                DropItem(ItemID.DD2BallistraTowerT3Popper, Rand.Next(10, 30));
                            }
                            else if (a < 1)
                            {
                                DropItem(ItemID.RainbowCrystalStaff, Rand.Next(5, 20));
                            }
                        }
                        break;
                    case NPCID.AngryBones:
                        DropItem(ItemID.GoldenKey, probability: 0.02);
                        break;
                    case NPCID.BoneSerpentHead:
                        DropItem(ItemID.BoneWhip, probability: 0.04);
                        DropItem(ItemID.MoltenFury, probability: 0.12);
                        break;
                    case NPCID.Skeleton:
                        DropItem(ItemID.Bone, Rand.Next(20, 35));
                        DropItem(ItemID.BeamSword, probability: 0.08);
                        {
                            var a = Rand.NextDouble();
                            if (a < 0.4)
                            {
                                DropItem(ItemID.PalladiumOre, Rand.Next(20, 50));
                            }
                            else if (a < 0.7)
                            {
                                DropItem(ItemID.OrichalcumOre, Rand.Next(15, 40));
                            }
                            else if (a < 0.9)
                            {
                                DropItem(ItemID.AdamantiteOre, Rand.Next(10, 30));
                            }
                            else if (a < 1)
                            {
                                DropItem(ItemID.ChlorophyteOre, Rand.Next(5, 20));
                            }
                        }
                        break;
                    case NPCID.DiggerHead:
                        {
                            var a = Rand.NextDouble();
                            if (a < 0.3)
                            {

                            }
                            else if (a < 0.5)
                            {
                                DropItem(ItemID.PalladiumPickaxe);
                            }
                            else if (a < 0.7)
                            {
                                DropItem(ItemID.TitaniumPickaxe);
                            }
                            else if (a < 0.9)
                            {
                                DropItem(ItemID.Drax);
                            }
                            else if (a < 1)
                            {
                                DropItem(ItemID.StardustPickaxe);
                            }
                        }
                        break;
                    case NPCID.ChaosElemental:
                        DropItem(ItemID.RodofDiscord, probability: 0.08);
                        DropItem(ItemID.RainbowRod, probability: 0.08);
                        break;
                    case NPCID.EnchantedSword:
                        {
                            var a = Rand.NextDouble();
                            if (a < 0.4)
                            {
                                DropItem(ItemID.EnchantedSword);
                            }
                            else if (a < 0.7)
                            {
                                DropItem(ItemID.Arkhalis);
                            }
                            else if (a < 0.95)
                            {
                                DropItem(ItemID.Excalibur);
                            }
                            else if (a < 1)
                            {
                                DropItem(ItemID.TrueExcalibur);
                            }
                        }
                        break;
                    case NPCID.CursedHammer:
                        {
                            DropItem(ItemID.Hammush, probability: 0.01);
                            var a = Rand.NextDouble();
                            if (a < 0.4)
                            {
                                DropItem(ItemID.TheBreaker);
                            }
                            else if (a < 0.7)
                            {
                                DropItem(ItemID.Pwnhammer);
                            }
                            else if (a < 0.95)
                            {
                                DropItem(ItemID.NightsEdge);
                            }
                            else if (a < 1)
                            {
                                DropItem(ItemID.TrueNightsEdge);
                            }
                        }
                        break;
                    case NPCID.IlluminantBat:
                        DropItem(ItemID.CrystalBullet, Rand.Next(40, 80));
                        DropItem(ItemID.Chik, probability: 0.25);
                        DropItem(ItemID.CrystalSerpent, probability: 0.50);
                        break;
                    case NPCID.FireImp:
                        DropItem(ItemID.Hellstone, Rand.Next(10, 20));
                        DropItem(ItemID.Flamarang, probability: 0.05);
                        DropItem(ItemID.ImpStaff, probability: 0.05);
                        break;
                    case NPCID.Hellbat:
                        DropItem(ItemID.HellstoneBar, Rand.Next(1, 2), probability: 0.50);
                        DropItem(ItemID.Obsidian, Rand.Next(6, 8), probability: 0.50);
                        break;
                    case NPCID.Lavabat:
                        DropItem(ItemID.HellstoneBar, Rand.Next(4, 10));
                        DropItem(ItemID.HellwingBow, probability: 0.25);
                        break;
                    case NPCID.SkeletonSniper:
                        DropItem(ItemID.HighVelocityBullet, Rand.Next(30, 40));
                        DropItem(ItemID.NanoBullet, Rand.Next(30, 40));
                        DropItem(ItemID.ChlorophyteBullet, Rand.Next(30, 40));
                        DropItem(ItemID.RifleScope, probability: 0.5);
                        DropItem(ItemID.SniperRifle, prefix: PrefixID.Broken, probability: 0.25);
                        break;
                    case NPCID.Medusa:
                        DropItem(ItemID.PocketMirror, probability: 0.33);
                        break;
                    case NPCID.SkeletonArcher:
                        DropItem(ItemID.BoneArrow, Rand.Next(40, 60));
                        DropItem(ItemID.EndlessQuiver, probability: 0.4);
                        DropItem(ItemID.MagicQuiver, probability: 0.4);
                        DropItem(ItemID.MoltenQuiver, probability: 0.4);
                        DropItem(ItemID.StalkersQuiver, probability: 0.4);
                        break;
                    case NPCID.Harpy:
                        DropItem(ItemID.HarpyWings, probability: 0.1);
                        DropItem(ItemID.Meteorite, Rand.Next(2, 3));
                        DropItem(ItemID.MeteorStaff, probability: 0.05);
                        break;
                    case NPCID.CursedSkull:
                        DropItem(ItemID.GhostarsWings, probability: 0.1);
                        DropItem(ItemID.BookofSkulls, probability: 0.1);
                        break;
                }
            }
        }
        #endregion
        #region OnNpcStrike
        private void OnNpcStrike(NpcStrikeEventArgs args)
        {
            var player = Players[args.Player.whoAmI];
            if (player == null || player.IsGuest)
            {
                return;
            }
            player.OnStrikeNpc(args);
            if (args.Npc.whoAmI == FinalBossIndex)
            {
                FinalBossLifeRest -= (int)Main.CalculateDamageNPCsTake(args.Damage, args.Npc.defense);
                if (FinalBossLifeRest < 0)
                {
                    FinalBossLifeRest = 0;
                }
            }
        }
        #endregion
        #region OnTileEdit
        private void OnTileEdit(object sender, GetDataHandlers.TileEditEventArgs args)
        {
            if (!Regions.GamingZone.InRange(args.X, args.Y) || Regions.Maze.InRange(args.X, args.Y))
            {
                //if (!args.Player.HasPermission("scd.editworld"))
                {
                    args.Handled = true;
                    if (Regions.Maze.InRange(args.X, args.Y) && (!Main.tile[args.X, args.Y].active() || Main.tile[args.X, args.Y].type == TileID.Torches))
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                if (Main.tile[args.X + i, args.Y + j].type == TileID.TeleportationPylon)
                                {
                                    args.Handled = true;
                                    new TileSection(args.X, args.Y, 4, 4).UpdateToPlayer(args.Player.Index);
                                    return;
                                }
                            }
                        }
                        args.Handled = false;
                    }
                    if (args.Handled)
                    {
                        new TileSection(args.X, args.Y, 4, 4).UpdateToPlayer(args.Player.Index);
                    }
                    if (Main.tile[args.X, args.Y].type == TileID.TeleportationPylon)
                    {
                        var pylon = Rand.Next(MazePylons);
                        new TileSection(pylon.X, pylon.Y, 1, 1).UpdateToPlayer(args.Player.Index);
                        args.Player.Teleport(16 * pylon.X, 16 * pylon.Y);
                        args.Handled = true;
                    }
                }
            }
            if (!Regions.Spheres.InRange(args.X, args.Y))
            {
                var tile = Main.tile[args.X, args.Y];
                if (tile.active())
                {
                    switch (tile.type)
                    {
                        case TileID.Titanium:
                        case TileID.Adamantite:
                        case TileID.Crimtane:
                        case TileID.Demonite:
                            //tile.active(false); 之前qi觉得能挖不好，现在想了想，算了，挖呗，不会怎么样的
                            break;
                    }
                }
            }
            if(IsInGame && Players[args.Player.Index].Identity == PlayerIdentity.Watcher)
			{
                args.Handled = true;
                new TileSection(args.X, args.Y, 1, 1).UpdateToPlayer(args.Player.Index);
            }
        }
        #endregion
        #region OnPlaceObject
        private void OnPlaceObject(object sender, GetDataHandlers.PlaceObjectEventArgs args)
        {
            if (!Regions.GamingZone.InRange(args.X, args.Y) || Regions.Maze.InRange(args.X, args.Y))
            {
                args.Handled = true;
                new TileSection(args.X, args.Y, 4, 4).UpdateToPlayer(args.Player.Index);
            }
        }
        #endregion
        #region OnPlayerSlot
        private void OnPlayerSlot(object sender, GetDataHandlers.PlayerSlotEventArgs args)
        {
            if (IsInGame)
            {
                var player = Players[args.PlayerId];
                if (player?.TSPlayer != null && player.Identity != PlayerIdentity.Watcher)
                {
                    if (NetItem.PiggyIndex.Item1 <= args.Slot && args.Slot < NetItem.PiggyIndex.Item2)
                    {
                        player.TPlayer.bank.item[args.Slot - NetItem.PiggyIndex.Item1].netDefaults(args.Type);
                        player.TPlayer.bank.item[args.Slot - NetItem.PiggyIndex.Item1].stack = args.Stack;
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, player.Index, null, args.Slot);
                        if (player.Equipped(ItemID.DiscountCard))
                        {
                            TraitorShop.TryBuyFromPiggy(player);
                        }
                        else
                        {
                            CurrentTask.CheckPiggy(player);
                        }
                        args.Handled = true;
                    }
                }
            }
        }
        #endregion
        #region OnTeleport
        private void OnTeleport(object sender, GetDataHandlers.TeleportEventArgs args)
        {

        }
        #endregion
        #region OnPlayerSpawn
        private void OnPlayerSpawn(object sender, GetDataHandlers.SpawnEventArgs args)
        {
            if (args.SpawnContext == PlayerSpawnContext.RecallFromItem)
            {
                var player = Players[args.PlayerId];
                if (player.Identity != PlayerIdentity.Watcher)
                {
                    if (player.WarpingCountDown == 0)
                    {
                        var home = new Vector2(Regions.Hall.Center.X, Regions.Hall.Y);
                        player.AddEffect(new PlayerWarp(player, 10 * 60, home));
                    }
                    else
					{
                        player.SendText("请等待上一次跃迁", Color.Yellow);
					}
                    args.Handled = true;
                    player.TeleportTo(player.TPlayer.Center);
                }
            }
        }
        #endregion
        #endregion
        #region GenerateMap
        private async Task<bool> GenerateMapAsync()
        {
            gMapToken = new CancellationTokenSource();
            BCToAll(Texts.GeneratingMap, Color.Purple);
            return await Task.Run(GenerateMap, gMapToken.Token);
        }
        private bool GenerateMap()
        {
            Regions.GamingZone.TurnToAir();
            Main.PylonSystem.Pylons.Clear();
            var islands = new MapGenerating.IslandsGenerator();
            var surface = new MapGenerating.SurfaceGenerator();
            var cave = new MapGenerating.CaveGenerator();
            var spheres = new MapGenerating.SpheresGenerator();
            var maze = new MapGenerating.MazeGenerator();
            var caveEx = new MapGenerating.CaveExGenerator();
            var hell = new MapGenerating.HellGenerator();
            islands.Generate();
            surface.Generate();
            cave.Generate();
            spheres.Generate();
            maze.Generate();
            caveEx.Generate();
            hell.Generate();
            Regions.GamingZone.UpdateToPlayer();
            BCToAll(Texts.GeneratedMap, Color.Purple);
            return true;
        }
        #endregion
        #region Commands
        #region HotReload
        public void PostHotReload()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (TShock.Players[i] != null)
                {
                    if (TShock.Players[i].IsLoggedIn)
                    {
                        Players[i] = DataBase.GetPlayer(i);
                    }
                    else
                    {
                        Players[i] = GamePlayer.Guest(i);
                    }
                }
            }
        }
        private void HotReloadCmd(CommandArgs args)
        {
            Dispose();
            #region FindContainer
            PluginContainer Container = null;
            foreach (var container in ServerApi.Plugins)
            {
                if (container.Plugin == this)
                {
                    Container = container;
                    break;
                }
            }
            #endregion
            #region Load New
            byte[] pdb = null;
            var path = Path.Combine(ServerApi.PluginsPath, "SurvivalCrisis.dll");
            if (File.Exists("SurvivalCrisis.pdb"))
            {
                pdb = File.ReadAllBytes("SurvivalCrisis.pdb");
            }
            var newPlugin = System.Reflection.Assembly.Load(File.ReadAllBytes(path), pdb);
            var pluginClass = newPlugin.GetType(typeof(SurvivalCrisis).FullName);
            var instance = Activator.CreateInstance(pluginClass, new[] { Main.instance });
            #endregion
            #region Replace
            Container
                .GetType()
                .GetProperty(nameof(Container.Plugin))
                .SetValue(Container, instance);
            #endregion
            #region Initialize
            Container.Initialize();
            pluginClass
                .GetMethod(nameof(PostHotReload))
                .Invoke(instance, Array.Empty<object>());
            #endregion
        }
        #endregion
        #region Debug
        private void DebugCommand(CommandArgs args)
        {
            string option;
            if (args.Parameters.Count < 1)
            {
                option = "help";
            }
            else
            {
                option = args.Parameters[0];
            }
            switch (option)
            {
                case "help":
                    break;
                case "pstates":
                    for (int i = 0; i < Players.Length; i++)
                    {
                        if (Players[i] != null)
                        {
                            args.Player.SendInfoMessage($"({i}){Players[i].Name}: isGuest: {Players[i].IsGuest}, identity: {Players[i].Identity}");
                        }
                    }
                    break;
                case "regions":
                    Regions.ShowTo(args.Player);
                    break;
                case "cfg":
                    Config.ShowTo(args.Player);
                    break;
                case "ss":
                    {
                        var point = new Point(args.Player.TileX, args.Player.TileY);
                        if (args.Parameters.Count >= 3 && int.TryParse(args.Parameters[1], out int width) && int.TryParse(args.Parameters[2], out int height))
                        {
                            var block = MapGenerating.IslandsGenerator.TileBlockData.FromMap(new TileSection(point.X, point.Y, width, height));
                            block.Save();
                        }
                        else
                        {
                            var block = MapGenerating.IslandsGenerator.TileBlockData.BFSBlock(point);
                            block.Save();
                        }
                    }
                    break;
                case "island":
                    {
                        MapGenerating.IslandsGenerator.TileBlockData island = null;
                        var files = new DirectoryInfo(IslandsPath).GetFiles();
                        if (args.Parameters.Count > 1)
                        {
                            var file = files.FirstOrDefault(f => f.Name.StartsWith(args.Parameters[1], StringComparison.OrdinalIgnoreCase));
                            if (file != null)
                            {
                                island = MapGenerating.IslandsGenerator.TileBlockData.FromFile(file.FullName);
                            }
                        }
                        if (island != null)
                        {
                            var section = new TileSection(args.Player.TileX, args.Player.TileY, island.Width, island.Height);
                            island.AffixTo(section);
                            section.UpdateToPlayer();
                        }
                        else
                        {
                            foreach (var file in files)
                            {
                                args.Player.SendInfoMessage(file.Name);
                            }
                        }
                    }
                    break;
                case "sphere":
                    {
                        MapGenerating.IslandsGenerator.TileBlockData sphere = null;
                        var files = new DirectoryInfo(SpheresPath).GetFiles();
                        if (args.Parameters.Count > 1)
                        {
                            var file = files.FirstOrDefault(f => f.Name.StartsWith(args.Parameters[1], StringComparison.OrdinalIgnoreCase));
                            if (file != null)
                            {
                                sphere = MapGenerating.IslandsGenerator.TileBlockData.FromFile(file.FullName);
                            }
                        }
                        if (sphere != null)
                        {
                            var section = new TileSection(args.Player.TileX, args.Player.TileY, sphere.Width, sphere.Height);
                            sphere.AffixTo(section);
                            section.UpdateToPlayer();
                        }
                        else
                        {
                            foreach (var file in files)
                            {
                                args.Player.SendInfoMessage(file.Name);
                            }
                        }
                    }
                    break;
                case "sigma":
                    {
                        int x = args.Player.TileX;
                        List<int> heights = new List<int>(50);
                        for (int i = x; i < x + 50; i++)
                        {
                            for (int j = args.Player.TileY - 50; j < Main.maxTilesY; j++)
                            {
                                if (Main.tile[i, j].active())
                                {
                                    heights.Add(j);
                                    break;
                                }
                            }
                        }
                        var average = heights.Average();
                        var sigma2 = heights.Average(value => (value - average) * (value - average));
                        args.Player.SendMessage($"average: {average} σ²: {sigma2}", Color.Blue);
                    }
                    break;
            }
        }
        #endregion
        #region PCommand
        private void PlayerCommand(CommandArgs args)
        {
            string option;
            if (args.Parameters.Count > 0)
            {
                option = args.Parameters[0];
            }
            else
            {
                option = "help";
            }
            switch (option)
            {
                case "pch":
                    if (!IsInGame)
                    {
                        args.Player.SendErrorMessage("只能在游戏中使用这个命令");
                    }
                    else if (Players[args.Player.Index].Identity != PlayerIdentity.Traitor)
                    {
                        args.Player.SendErrorMessage("只有背叛者可以使用这个命令");
                    }
                    else
                    {
                        var msg = args.Message.Substring(args.Message.LastIndexOf("ss pchat ") + "ss pchat".Length);
                        BCToTraitor($"[背叛者频道]{args.Player.Index}号: {msg}", Color.White);
                    }
                    break;
                case "task":
                    if (!IsInGame)
                    {
                        args.Player.SendErrorMessage("你只能在游戏中使用该命令");
                    }
                    else if (CurrentTask == null)
                    {
                        args.Player.SendInfoMessage("当前无生存者任务");
                    }
                    else
                    {
                        args.Player.SendMessage(CurrentTask.Text, Color.White);
                        args.Player.SendMessage(CurrentTask.CurrentProcess(), Color.BlueViolet);
                    }
                    break;
                case "cps":
                    if (!IsInGame)
                    {
                        args.Player.SendErrorMessage("你只能在游戏中使用该命令");
                    }
                    else if (CurrentTask == null)
                    {
                        args.Player.SendInfoMessage("当前无生存者任务");
                    }
                    else
                    {
                        CurrentTask.CheckPiggy(Players[args.Player.Index]);
                    }
                    break;
                case "st":
                    if (!IsInGame)
                    {
                        args.Player.SendErrorMessage("只能在游戏中使用这个命令");
                    }
                    else if (Players[args.Player.Index].Identity != PlayerIdentity.Traitor)
                    {
                        args.Player.SendErrorMessage("只有背叛者可以使用这个命令");
                    }
                    else
                    {
                        if (args.Parameters.Count >= 2 && int.TryParse(args.Parameters[1], out int index))
                        {
                            TraitorShop.TryBuy(Players[args.Player.Index], index);
                        }
                        else
                        {
                            TraitorShop.DisplayTo(Players[args.Player.Index]);
                        }
                    }
                    break;
                case "rank":
					{
                        var traitorRank = DataBase.GetTraitorRank();
                        var survivorRank = DataBase.GetSurvivorRank();
                        args.Player.SendInfoMessage($"生存者排行:");
                        for (int i = 0; i < 10 && i < survivorRank.Count; i++)
                        {
                            args.Player.SendInfoMessage($"第{i + 1}名: {survivorRank[i].Name}({survivorRank[i].SurvivorDatas.TotalScore})");
                        }
                        args.Player.SendInfoMessage($"背叛者排行:");
                        for (int i = 0; i < 10 && i < traitorRank.Count; i++)
                        {
                            args.Player.SendInfoMessage($"第{i + 1}名: {traitorRank[i].Name}({traitorRank[i].TraitorDatas.TotalScore})");
                        }
                    }
                    break;
                case "np":
					{
						if (Players[args.Player.Index].Data.UnlockedPrefixs.Count == 0)
						{
							args.Player.SendInfoMessage("尚未解锁任何前缀");
							return;
						}
						Players[args.Player.Index].ToNextPrefixID();
                        break;
					}
                case "nt":
					{
						if (Players[args.Player.Index].Data.UnlockedTitles.Count == 0)
						{
							args.Player.SendInfoMessage("尚未解锁任何称号");
							return;
						}
						Players[args.Player.Index].ToNextTitleID();
                        break;
                    }
                case "rp":
					{
                        Players[args.Player.Index].BuyRandomPrefix(200);
                        break;
					}
                case "rt":
                    {
                        Players[args.Player.Index].BuyRandomTitle(200);
                        break;
					}
                case "score":
                    {
                        var player = Players[args.Player.Index];
                        var traitorRank = DataBase.GetTraitorRank();
                        var survivorRank = DataBase.GetSurvivorRank();

                        player.SendText($"——————生存者信息——————", Color.LightBlue);
                        player.SendText($"当前排名: {survivorRank.IndexOf(player.Data) + 1}", Color.LightSkyBlue);
                        player.SendText($"总积分: {player.Data.SurvivorDatas.TotalScore}", Color.LightSkyBlue);
                        player.SendText($"游戏次数: {player.Data.SurvivorDatas.GameCounts, 4}      胜利次数: {player.Data.SurvivorDatas.WinCounts, 4}", Color.LightSkyBlue);
                        player.SendText($"死亡次数: {player.Data.SurvivorDatas.KilledCount}", Color.LightSkyBlue);

                        player.SendText($"——————背叛者信息——————", Color.DarkRed);
                        player.SendText($"当前排名: {survivorRank.IndexOf(player.Data) + 1}", Color.OrangeRed);
                        player.SendText($"总积分: {player.Data.TraitorDatas.TotalScore}", Color.OrangeRed);
                        player.SendText($"游戏次数: {player.Data.TraitorDatas.GameCounts,4}      胜利次数: {player.Data.TraitorDatas.WinCounts,4}", Color.OrangeRed);
                        player.SendText($"击杀次数: {player.Data.TraitorDatas.KillingCount}", Color.OrangeRed);

                        player.SendText($"当前可消费积分: {player.Data.Coins}", Color.Yellow);
                    }
                    break;
                case "stat":
                    args.Player.SendMessage(Players[args.Player.Index].LastStatusMessage ?? string.Empty, Color.White);
                    break;
                default:
                    args.Player.SendInfoMessage("task    查看生存者任务");
                    args.Player.SendInfoMessage("cps     提交生存者任务物品");
                    args.Player.SendInfoMessage("cpt     提交背叛者任务物品");
                    args.Player.SendInfoMessage("st      背叛者商店");
                    args.Player.SendInfoMessage("pch     背叛者间交流");
                    args.Player.SendInfoMessage("rank    查看排行");
                    args.Player.SendInfoMessage("score   查看积分");
                    args.Player.SendInfoMessage("nt      切换称号");
                    args.Player.SendInfoMessage("np      切换前缀");
                    args.Player.SendInfoMessage("rt      随机称号");
                    args.Player.SendInfoMessage("rp      随机前缀");
                    args.Player.SendInfoMessage("stat    获取pc端右侧提示");
                    break;
            }
        }
        #endregion
        #endregion
        #region BroadCasts
        public static void DebugMessage(string message)
        {
#if DEBUG
            TSPlayer.All.SendMessage(message, Color.Blue);
            TSPlayer.Server.SendConsoleMessage(message, 0, 0, 255);
#endif
        }
        public void BCToHall(string message, Color color)
        {
            foreach (var player in Players)
            {
                if (player != null && Regions.Hall.InRange(player))
                {
                    player.SendText(message, color);
                }
            }
        }
        public void BCToAll(string message, Color color)
        {
            TSPlayer.All.SendMessage(message, color);
        }
        public void BCToWatcher(string message, Color color)
        {
            BCTo(PlayerIdentity.Watcher, message, color);
        }
        public void BCToTraitor(string message, Color color)
        {
            BCTo(PlayerIdentity.Traitor, message, color);
        }
        public void BCTo(PlayerIdentity identity, string message, Color color)
        {
            foreach (var player in Players)
            {
                if (player?.Identity == identity)
                {
                    player.SendText(message, color);
                }
            }
        }
        #endregion
    }
}
