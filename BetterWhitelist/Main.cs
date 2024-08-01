using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Terraria;
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
            ServerApi.Hooks.ServerLeave.Register(this,OnLeave);
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
                args.Player.SendErrorMessage(_translation.language["未知指令提示文本"]);
                return;
            }

            string command = args.Parameters[0];

            switch (command.ToLowerInvariant())
            {
                case "help":
                    args.Player.SendInfoMessage("-------[BetterWhitelist]-------");
                    args.Player.SendInfoMessage(_translation.language["全部的指令帮助提示"]);
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
                        args.Player.SendErrorMessage(Main._translation.language["禁用失败提示"]);
                    }
                    else
                    {
                        Main._config.Disabled = true;
                        args.Player.SendSuccessMessage(Main._translation.language["禁用成功提示"]);
                        File.WriteAllText(Main.config_path, JsonConvert.SerializeObject(Main._config, Formatting.Indented));
                    }
                    break;

                case "true":
                    if (!Main._config.Disabled)
                    {
                        args.Player.SendErrorMessage(Main._translation.language["启用失败提示"]);
                    }
                    else
                    {
                        Main._config.Disabled = false;
                        args.Player.SendSuccessMessage(Main._translation.language["启用成功提示"]);

                        if (Main.players.Count > 0)
                        {
                            if (Main._config.WhitePlayers.Count > 0)
                            {
                                foreach (TSPlayer tsplayer in Main.players.Values.Where(player => !Main._config.WhitePlayers.Contains(player.Name)))
                                {
                                    tsplayer.Disconnect(Main._translation.language["连接时不在白名单提示"]);
                                }
                            }
                            else
                            {
                                foreach (TSPlayer tsplayer in Main.players.Values)
                                {
                                    tsplayer.Disconnect(Main._translation.language["连接时不在白名单提示"]);
                                }
                            }
                        }
                        File.WriteAllText(Main.config_path, JsonConvert.SerializeObject(Main._config, Formatting.Indented));
                    }
                    break;

                case "add":
                    if (Main._config.Disabled)
                    {
                        args.Player.SendErrorMessage(Main._translation.language["未启用插件提示"]);
                    }
                    else
                    {
                        string playerNameToAdd = args.Parameters.ElementAtOrDefault(1);

                        if (playerNameToAdd != null && !Main._config.WhitePlayers.Contains(playerNameToAdd))
                        {
                            Main._config.WhitePlayers.Add(playerNameToAdd);
                            args.Player.SendSuccessMessage(Main._translation.language["添加成功提示"]);
                            File.WriteAllText(Main.config_path, JsonConvert.SerializeObject(Main._config, Formatting.Indented));
                        }
                        else
                        {
                            args.Player.SendSuccessMessage(Main._translation.language["添加失败提示"]);
                        }
                    }
                    break;

                case "reload":
                    Main._config = JsonConvert.DeserializeObject<BConfig>(File.ReadAllText(Main.config_path));
                    Main._translation = JsonConvert.DeserializeObject<Translation>(File.ReadAllText(Main.translation_path));
                    args.Player.SendSuccessMessage(Main._translation.language["重载成功提示"]);
                    break;

                case "del":
                    if (Main._config.Disabled)
                    {
                        args.Player.SendErrorMessage(Main._translation.language["未启用插件提示"]);
                    }
                    else
                    {
                        string playerNameToDelete = args.Parameters.ElementAtOrDefault(1);

                        if (playerNameToDelete != null && Main._config.WhitePlayers.Contains(playerNameToDelete))
                        {
                            Main._config.WhitePlayers.Remove(playerNameToDelete);
                            args.Player.SendSuccessMessage(Main._translation.language["删除成功提示"]);
                            File.WriteAllText(Main.config_path, JsonConvert.SerializeObject(Main._config, Formatting.Indented));

                            if (Main.players.ContainsKey(playerNameToDelete))
                            {
                                Main.players[playerNameToDelete].Disconnect(Main._translation.language["删除白名单后断开玩家连接提示"]);
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
                tsplayer.Disconnect(Main._translation.language["连接时不在白名单提示"]);
            }
            else if (Main._config.Disabled)
            {
                TShock.Log.ConsoleInfo(Main._translation.language["未启用插件提示"]);
            }
        }

        private void Load()
		{
			Main._config = BConfig.Load(Main.config_path);
			Main._translation = Translation.Load(Main.translation_path);
			File.WriteAllText(Main.config_path, JsonConvert.SerializeObject(Main._config, Formatting.Indented));
			File.WriteAllText(Main.translation_path, JsonConvert.SerializeObject(Main._translation, Formatting.Indented));
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

		public static string translation_path = Path.Combine(Main.bwldir, "language.json");

		public static BConfig _config;

		public static Translation _translation;

		public static Dictionary<string, TSPlayer> players = new Dictionary<string, TSPlayer>();
	}
}
