using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using Newtonsoft.Json;
using Terraria;
using Terraria.GameContent.Creative;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace ServerTools
{
    [ApiVersion(2, 1)]
    public partial class Plugin : TerrariaPlugin
    {
        public override string Author => "少司命";// 插件作者

        public override string Description => "服务器工具";// 插件说明

        public override string Name => "ServerTools";// 插件名字

        public override Version Version => new(1, 0, 1, 0);// 插件版本

        private static Config Config = new();

        private readonly string PATH = Path.Combine(TShock.SavePath, "ServerTools.json");

        private DateTime LastCommandUseTime = DateTime.Now;

        private long TimerCount = 0;

        private readonly Dictionary<string, DateTime> PlayerDeath = new();

        public event Action<EventArgs>? Timer;

        public static Hook CmdHook;

        public Plugin(Main game) : base(game)
        {

        }

        public override void Initialize()
        {

            LoadConfig();
            #region 钩子
            ServerApi.Hooks.GamePostInitialize.Register(this, PostInitialize);
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreetPlayer);
            ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
            ServerApi.Hooks.NetGetData.Register(this, GetData);
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreet);
            ServerApi.Hooks.ServerLeave.Register(this, _OnLeave);
            #endregion

            #region 指令
            Commands.ChatCommands.Add(new Command(Permissions.clear, Clear, "clp"));
            Commands.ChatCommands.Add(new Command("servertool.query.exit", Exit, "退出"));
            Commands.ChatCommands.Add(new Command("servertool.query.wall", WallQ, "查花苞","scp"));
            Commands.ChatCommands.Add(new Command("servertool.query.wall", RWall, "移除花苞","rcp"));
            Commands.ChatCommands.Add(new Command("servertool.user.kick", SelfKick, "自踢"));
            Commands.ChatCommands.Add(new Command("servertool.user.kill", SelfKill, "自杀"));
            Commands.ChatCommands.Add(new Command("servertool.user.ghost", Ghost, "ghost"));
            Commands.ChatCommands.Add(new Command("servertool.set.journey", JourneyDiff, "旅途难度"));
            Commands.ChatCommands.Add(new Command("servertool.user.dead", DeathRank, "死亡排行"));
            Commands.ChatCommands.Add(new("servertool.user.online", Online, "在线排行"));
            #endregion
            #region TShcok 钩子
            GetDataHandlers.NewProjectile.Register(NewProj);
            GetDataHandlers.ItemDrop.Register(OnItemDrop);
            GetDataHandlers.KillMe.Register(KillMe);
            GetDataHandlers.PlayerSpawn.Register(OnPlayerSpawn);
            GetDataHandlers.PlayerUpdate.Register(OnUpdate);
            GeneralHooks.ReloadEvent += (_) => LoadConfig();
            #endregion
            CmdHook = new Hook(typeof(Commands).GetMethod(nameof(Commands.HandleCommand)), CommandHook);
            new Hook(typeof(Commands).GetMethod("ViewAccountInfo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static), ViewAccountInfo);

            #region RestAPI
            TShock.RestApi.Register("/deathrank", DeadRank);
            TShock.RestApi.Register("/onlineDuration", Queryduration);
            #endregion
            Timer += OnUpdatePlayerOnline;
            HandleCommandLine(Environment.GetCommandLineArgs());
        }

        private void OnUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
        {
            if (!Config.KeepArmor || e.Player.HasPermission("servertool.armor.white"))
                return;
            var ArmorGroup = e.Player.TPlayer.armor
                .GroupBy(x => x.netID)
                .Where(x => x.Count() > 1)
                .Select(x => x.First());
            foreach (var keepArmor in ArmorGroup)
            {
                e.Player.SetBuff(156, 180, true);
                TShock.Utils.Broadcast($"玩家 [{e.Player.Name}] 因多饰品被冻结3秒，请清理多饰品装备[i:{keepArmor.netID}]", Color.DarkRed);
            }
        }

        private static void ViewAccountInfo(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage("语法错误，正确语法: {0}accountinfo <username>.", Commands.Specifier);
                return;
            }

            string username = String.Join(" ", args.Parameters);
            if (!string.IsNullOrWhiteSpace(username))
            {
                var account = TShock.UserAccounts.GetUserAccountByName(username);
                if (account != null)
                {
                    DateTime LastSeen;
                    string Timezone = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours.ToString("+#;-#");

                    if (DateTime.TryParse(account.LastAccessed, out LastSeen))
                    {
                        LastSeen = DateTime.Parse(account.LastAccessed).ToLocalTime();
                        args.Player.SendSuccessMessage("{0} 最后的登录时间为 {1} {2} UTC{3}.", account.Name, LastSeen.ToShortDateString(),
                            LastSeen.ToShortTimeString(), Timezone);
                    }

                    if (args.Player.Group.HasPermission(Permissions.advaccountinfo))
                    {
                        List<string> KnownIps = JsonConvert.DeserializeObject<List<string>>(account.KnownIps?.ToString() ?? string.Empty);
                        string ip = KnownIps?[KnownIps.Count - 1] ?? "N/A";
                        DateTime Registered = DateTime.Parse(account.Registered).ToLocalTime();
                        args.Player.SendSuccessMessage("{0} 账户ID为 {1}.", account.Name, account.ID);
                        args.Player.SendSuccessMessage("{0} 权限组为 {1}.", account.Name, account.Group);
                        args.Player.SendSuccessMessage("{0} 最后登录使用的IP为 {1}.", account.Name, ip);
                        args.Player.SendSuccessMessage("{0} 注册时间为 {1} {2} UTC{3}.", account.Name, Registered.ToShortDateString(), Registered.ToShortTimeString(), Timezone);
                    }
                }
                else
                    args.Player.SendErrorMessage("用户 {0} 不存在.", username);
            }
            else args.Player.SendErrorMessage("语法错误，正确语法: {0}accountinfo <username>.", Commands.Specifier);
        }

        private void Exit(CommandArgs args)
        {
            args.Player.SendData(PacketTypes.ChestOpen, "", args.Player.Index, -1);
        }

        public static bool CommandHook(TSPlayer ply, string cmd)
        {
            CmdHook.Undo();
            if (ply.GetType() == typeof(TSRestPlayer))
            {
                ply.Account = new()
                {
                    Name = ply.Name,
                    Group = ply.Group.Name,
                    ID = ply.Index
                };
            }
            var status = Commands.HandleCommand(ply, cmd);
            CmdHook.Apply();
            return status;
        }

        private void OnPlayerSpawn(object? sender, GetDataHandlers.SpawnEventArgs e)
        {
            if (e.Player != null)
            {
                Deads.Remove(e.Player);
            }
        }

        private void OnItemDrop(object? sender, GetDataHandlers.ItemDropEventArgs e)
        {
            if (e.Player != null && e.Player.Dead && Config.ClearDrop)
            {
                Main.item[e.ID].active = false;
                e.Handled = true;
                NetMessage.SendData(21, -1, -1, null, e.ID, 0, 0);
            }
        }





        private void OnGreetPlayer(GreetPlayerEventArgs args)
        {
            if (Config.PreventsDeathStateJoin && Main.player[args.Who].dead)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(1000);
                    TShock.Players[args.Who].SendSuccessMessage("请在单人模式中结束死亡状态重新进入服务器!");
                    int count = 0;
                    while (count < 3)
                    {
                        TShock.Players[args.Who].SendSuccessMessage($"你将在{3 - count}秒后被踢出!");
                        await Task.Delay(1000);
                        count++;
                    }
                    TShock.Players[args.Who].Disconnect("请在单人模式中结束死亡状态重新进入服务器!");
                });
            }
        }

        private void OnUpdate(EventArgs e)
        {
            TimerCount++;
            if (TimerCount % 60 == 0)
            {
                Timer?.Invoke(e);
                if (Config.DeadTimer)
                {
                    foreach (var ply in Deads)
                    {
                        if (ply != null && ply.Active && ply.Dead && ply.RespawnTimer > 0)
                        {
                            ply.SendInfoMessage(Config.DeadFormat, ply.RespawnTimer);
                        }
                    }
                }

            }
        }

        private void OnLeave(LeaveEventArgs args)
        {
            var ply = TShock.Players[args.Who];
            if (ply == null)
                return;
            Deads.Remove(ply);
            if (ply.Dead)
                PlayerDeath[ply.Name] = DateTime.Now.AddSeconds(ply.RespawnTimer);
        }

        private void NewProj(object? sender, GetDataHandlers.NewProjectileEventArgs e)
        {
            if (Main.projectile[e.Index].sentry)
            {
                if (Main.projectile.Where(x => x.owner == e.Owner).Count() > Config.sentryLimit)
                {
                    Main.projectile[e.Index].active = false;
                    e.Handled = true;
                    return;
                }
            }
            if (Main.projectile[e.Index].bobber && Config.MultipleFishingRodsAreProhibited && Config.ForbiddenBuoys.FindAll(f => f == e.Type).Count > 0)
            {
                var bobber = Main.projectile.Where(f => f != null && f.owner == e.Owner && f.active && f.type == e.Type);
                if (bobber.Count() > 2)
                {
                    e.Player.SendErrorMessage("你因多鱼线被石化3秒钟!");
                    e.Player.SetBuff(156, 180, true);
                }
            }
        }



        private void HandleCommandLine(string[] param)
        {
            Dictionary<string, string> args = Terraria.Utils.ParseArguements(param);
            foreach (var key in args)
            {
                switch (key.Key.ToLower())
                {
                    case "-seed":
                        Main.AutogenSeedName = key.Value;
                        break;
                }
            }
        }
        public void LoadConfig()
        {
            if (File.Exists(PATH))
            {
                try
                {
                    Config = Config.Read(PATH);
                }
                catch (Exception e)
                {

                    TShock.Log.ConsoleError("ServerTools.json配置读取错误:{0}", e.ToString());
                }
            }
            else
            {
                Config.ForbiddenBuoys = new List<short>() { 360, 361, 362, 363, 364, 365, 366, 381, 382, 760, 775, 986, 987, 988, 989, 990, 991, 992, 993 };
            }
            Config.Write(PATH);
        }

        public bool SetJourneyDiff(string diffName)
        {
            float diff;
            switch (diffName.ToLower())
            {
                case "master":
                    diff = 1f;
                    break;
                case "journey":
                    diff = 0f;
                    break;
                case "normal":
                    diff = 0.33f;
                    break;
                case "expert":
                    diff = 0.66f;
                    break;
                default:
                    return false;
            }
            var power = CreativePowerManager.Instance.GetPower<CreativePowers.DifficultySliderPower>();
            power._sliderCurrentValueCache = diff;
            power.UpdateInfoFromSliderValueCache();
            power.OnPlayerJoining(0);
            return true;
        }



        private void PostInitialize(EventArgs args)
        {
            //设置世界模式
            if (Config.SetWorldMode)
            {
                if (Config.WorldMode > 1 && Config.WorldMode < 4)
                {
                    Main.GameMode = Config.WorldMode;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo);
                }
            }
            //旅途难度
            if (Main._currentGameModeInfo.IsJourneyMode && Config.SetJourneyDifficult)
            {
                SetJourneyDiff(Config.JourneyMode);
            }
        }


        private void GetData(GetDataEventArgs args)
        {
            var ply = TShock.Players[args.Msg.whoAmI];
            if (args.Handled || ply == null) return;

            if (Config.PickUpMoney && args.MsgID == PacketTypes.SyncExtraValue)
            {
                using BinaryReader reader = new(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length));
                var npcid = reader.ReadInt16();
                var money = reader.ReadInt32();
                var x = reader.ReadSingle();
                var y = reader.ReadSingle();
                ply.SendData(PacketTypes.SyncExtraValue, "", npcid, 0, x, y);
                args.Handled = true;
                return;
            }

            if (Config.KeepOpenChest && args.MsgID == PacketTypes.ChestOpen)
            {

                using BinaryReader binaryReader5 = new(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length));
                short ChestId = binaryReader5.ReadInt16();
                if (ChestId != -1 && ply.ActiveChest != -1)
                {
                    ply.ActiveChest = -1;
                    ply.SendData(PacketTypes.ChestOpen, "", -1);
                    ply.SendErrorMessage("禁止双箱!");
                    args.Handled = true;
                }
            }

            if (args.MsgID == PacketTypes.ChestGetContents && Config.KeepOpenChest)
            {
                if (ply.ActiveChest != -1)
                {
                    ply.ActiveChest = -1;
                    ply.SendData(PacketTypes.ChestOpen, "", -1);
                    ply.SendErrorMessage("禁止双箱!");
                    args.Handled = true;
                }
            }
        }


        private void OnInitialize(EventArgs args)
        {
            //执行命令
            if (TShock.UserAccounts.GetUserAccounts().Count == 0 && Config.ResetExecCommands.Length > 0)
            {
                for (int i = 0; i < Config.ResetExecCommands.Length; i++)
                {
                    Commands.HandleCommand(TSPlayer.Server, Config.ResetExecCommands[i]);
                }
            }
        }

        private void OnJoin(JoinEventArgs args)
        {
            if (args.Handled)
            {
                return;
            }

            if (Config.OnlySoftCoresAreAllowed)
            {
                if (TShock.Players[args.Who].Difficulty != 0)
                {
                    TShock.Players[args.Who].Disconnect("仅允许软核进入!");
                }
            }
            if (Config.BlockUnregisteredEntry)
            {
                //阻止未注册进入
                if (args.Who == -1 || TShock.Players[args.Who] == null || TShock.UserAccounts.GetUserAccountsByName(TShock.Players[args.Who].Name).Count == 0)
                {
                    TShock.Players[args.Who].Disconnect(Config.BlockEntryStatement);
                }
            }
            if (Config.DeathLast && TShock.Players[args.Who] != null)
            {
                if (PlayerDeath.TryGetValue(TShock.Players[args.Who].Name, out var time))
                {
                    var respawn = time - DateTime.Now;
                    if (respawn.TotalSeconds > 0)
                    {
                        TShock.Players[args.Who].Disconnect($"退出服务器时处于死亡状态！\n请等待死亡结束，还有{respawn.TotalSeconds}秒结束！");
                    }
                }
            }
        }
    }
}