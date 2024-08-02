using Newtonsoft.Json;
using TerrariaApi.Server;
using TShockAPI;

namespace BetterWhitelist
{
    [ApiVersion(2, 1)]
    public class Main : TerrariaPlugin
    {
        public Main(Terraria.Main game) : base(game)
        {
            Order = 9999;
        }

        public override string Name => "BetterWhitelist";

        public override Version Version => new Version(2, 3);

        public override string Author => "豆沙，肝帝熙恩修改";

        public override string Description => "通过检查玩家姓名的玩家白名单";
        public override void Initialize()
        {
            string path = Path.Combine(TShock.SavePath, "BetterWhitelist");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            this.Load();

            Commands.ChatCommands.Add(new Command("bwl.use", new CommandDelegate(this.bwl), new string[] { "bwl" }));
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
            ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
        }


        private void OnLeave(LeaveEventArgs args)
        {
            TSPlayer tsplayer = new TSPlayer(args.Who);
            players.Remove(tsplayer.Name);
        }
        private void bwl(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage("用法: 输入 /bwl help 显示帮助信息.");
                return;
            }

            string command = args.Parameters[0];

            switch (command.ToLowerInvariant())
            {
                case "help":
                    args.Player.SendInfoMessage("-------[BetterWhitelist]-------");
                    args.Player.SendInfoMessage("/bwl help, 显示帮助信息\n/bwl add {name}, 添加玩家名到白名单中\n/bwl del {name}, 将玩家移出白名单\n/bwl list, 显示白名单上的全部玩家\n/bwl true, 启用插件\n/bwl false, 关闭插件\n/bwl reload, 重载插件");
                    break;

                case "list":
                    foreach (string msg in _config.WhitePlayers)
                    {
                        args.Player.SendInfoMessage(msg);
                    }
                    break;

                case "false":
                    if (Main._config.Disabled)
                    {
                        args.Player.SendErrorMessage("禁用失败 ! 插件已是关闭状态");
                    }
                    else
                    {
                        Main._config.Disabled = true;
                        args.Player.SendSuccessMessage("禁用成功!");
                        File.WriteAllText(Main.config_path, JsonConvert.SerializeObject(Main._config, Formatting.Indented));
                    }
                    break;

                case "true":
                    if (!Main._config.Disabled)
                    {
                        args.Player.SendErrorMessage("启用失败 ! 插件已是打开状态");
                    }
                    else
                    {
                        Main._config.Disabled = false;
                        args.Player.SendSuccessMessage("启用成功!");

                        if (Main.players.Count > 0)
                        {
                            if (Main._config.WhitePlayers.Count > 0)
                            {
                                foreach (TSPlayer tsplayer in Main.players.Values.Where(player => !Main._config.WhitePlayers.Contains(player.Name)))
                                {
                                    tsplayer.Disconnect(_config.NotInWhiteList);
                                }
                            }
                            else
                            {
                                foreach (TSPlayer tsplayer in Main.players.Values)
                                {
                                    tsplayer.Disconnect(_config.NotInWhiteList);
                                }
                            }
                        }
                        File.WriteAllText(Main.config_path, JsonConvert.SerializeObject(Main._config, Formatting.Indented));
                    }
                    break;

                case "add":
                    if (Main._config.Disabled)
                    {
                        args.Player.SendErrorMessage("插件开关已被禁用，请检查配置文件!");
                    }
                    else
                    {
                        string playerNameToAdd = args.Parameters.ElementAtOrDefault(1);

                        if (playerNameToAdd != null && !Main._config.WhitePlayers.Contains(playerNameToAdd))
                        {
                            Main._config.WhitePlayers.Add(playerNameToAdd);
                            args.Player.SendSuccessMessage("添加成功!");
                            File.WriteAllText(Main.config_path, JsonConvert.SerializeObject(Main._config, Formatting.Indented));
                        }
                        else
                        {
                            args.Player.SendSuccessMessage("添加失败! 该玩家已经在白名单中");
                        }
                    }
                    break;

                case "reload":
                    Main._config = JsonConvert.DeserializeObject<BConfig>(File.ReadAllText(Main.config_path));
                    args.Player.SendSuccessMessage("重载成功!");
                    break;

                case "del":
                    if (Main._config.Disabled)
                    {
                        args.Player.SendErrorMessage("插件开关已被禁用，请检查配置文件!");
                    }
                    else
                    {
                        string playerNameToDelete = args.Parameters.ElementAtOrDefault(1);

                        if (playerNameToDelete != null && Main._config.WhitePlayers.Contains(playerNameToDelete))
                        {
                            Main._config.WhitePlayers.Remove(playerNameToDelete);
                            args.Player.SendSuccessMessage("删除成功!");
                            File.WriteAllText(Main.config_path, JsonConvert.SerializeObject(Main._config, Formatting.Indented));

                            if (Main.players.ContainsKey(playerNameToDelete))
                            {
                                Main.players[playerNameToDelete].Disconnect("你已经不在白名单中！");
                            }
                        }
                    }
                    break;
            }
        }


        private void OnJoin(JoinEventArgs args)
        {
            TSPlayer tsplayer = new TSPlayer(args.Who);
            Main.players.Add(tsplayer.Name, tsplayer);

            if (!Main._config.Disabled && !Main._config.WhitePlayers.Contains(tsplayer.Name))
            {
                tsplayer.Disconnect(_config.NotInWhiteList);
            }
            else if (Main._config.Disabled)
            {
                TShock.Log.ConsoleInfo("插件开关已被禁用，请检查配置文件!");
            }
        }

        private void Load()
        {
            Main._config = BConfig.Load(Main.config_path);
            File.WriteAllText(Main.config_path, JsonConvert.SerializeObject(Main._config, Formatting.Indented));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
                ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
            }
            base.Dispose(disposing);
        }

        public static string bwldir = Path.Combine(TShock.SavePath, "BetterWhitelist");

        public static string config_path = Path.Combine(Main.bwldir, "config.json");

        public static BConfig _config;

        public static Dictionary<string, TSPlayer> players = new Dictionary<string, TSPlayer>();
    }
}
