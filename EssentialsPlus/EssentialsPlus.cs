using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EssentialsPlus.Db;
using EssentialsPlus.Extensions;
using MySql.Data.MySqlClient;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace EssentialsPlus
{
	[ApiVersion(2, 1)]
	public class EssentialsPlus : TerrariaPlugin
	{
		public static Config Config { get; private set; }
		public static IDbConnection Db { get; private set; }
		public static HomeManager Homes { get; private set; }
		public static MuteManager Mutes { get; private set; }

		public override string Author
		{
			get { return "Cjx适配"; }
		}

		public override string Description
		{
			get { return "拓展管理插件"; }
		}

		public override string Name
		{
			get { return "EssentialsPlus"; }
		}

		public override Version Version
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version; }
		}

		public EssentialsPlus(Main game)
			: base(game)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				GeneralHooks.ReloadEvent -= OnReload;
				PlayerHooks.PlayerCommand -= OnPlayerCommand;

				ServerApi.Hooks.GamePostInitialize.Deregister(this, OnPostInitialize);
				ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
				ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
			}
			base.Dispose(disposing);
		}

		public override void Initialize()
		{
            GeneralHooks.ReloadEvent += OnReload;
			PlayerHooks.PlayerCommand += OnPlayerCommand;
			OnInitialize();
            ServerApi.Hooks.GamePostInitialize.Register(this, OnPostInitialize);
			ServerApi.Hooks.NetGetData.Register(this, OnGetData);
			ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
        }

        private async void OnReload(ReloadEventArgs e)
		{
            string path = Path.Combine(TShock.SavePath, "essentials.json");
			Config = Config.Read(path);
			if (!File.Exists(path))
			{
				Config.Write(path);
			}
			await Homes.ReloadAsync();
			e.Player.SendSuccessMessage("[EssentialsPlus] 重读配置成功!");
		}

		private List<string> teleportCommands = new List<string>
		{
			"tp", "tppos", "tpnpc", "warp", "spawn", "home"
		};

		private void OnPlayerCommand(PlayerCommandEventArgs e)
		{
            if (e.Handled || e.Player == null)
			{
				return;
			}

			Command command = e.CommandList.FirstOrDefault();
			if (command == null || (command.Permissions.Any() && !command.Permissions.Any(s => e.Player.Group.HasPermission(s))))
			{
				return;
			}

			if (e.Player.TPlayer.hostile &&
				command.Names.Select(s => s.ToLowerInvariant())
					.Intersect(Config.DisabledCommandsInPvp.Select(s => s.ToLowerInvariant()))
					.Any())
			{
				e.Player.SendErrorMessage("此命令在PVP模式下禁用!");
				e.Handled = true;
				return;
			}

			if (e.Player.Group.HasPermission(Permissions.LastCommand) && command.CommandDelegate != Commands.RepeatLast)
			{
				e.Player.GetPlayerInfo().LastCommand = e.CommandText;
			}

			if (teleportCommands.Contains(e.CommandName) && e.Player.Group.HasPermission(Permissions.TpBack))
			{
				e.Player.GetPlayerInfo().PushBackHistory(e.Player.TPlayer.position);
			}
		}

		private void OnInitialize()
		{
            #region Config

            string path = Path.Combine(TShock.SavePath, "essentials.json");
			Config = Config.Read(path);
			if (!File.Exists(path))
			{
				Config.Write(path);
			}

			#endregion

			#region Database

			if (TShock.Config.Settings.StorageType.Equals("mysql", StringComparison.OrdinalIgnoreCase))
			{
				if (string.IsNullOrWhiteSpace(Config.MySqlHost) ||
					string.IsNullOrWhiteSpace(Config.MySqlDbName))
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("[Essentials+] MYSQL已经被启用但是配置文件并没有完成配置.");
					Console.WriteLine("[Essentials+] 请完成配置文件中MYSQL数据库的配置.");
					Console.WriteLine("[Essentials+] 此插件已被禁用...");
					Console.ResetColor();

					GeneralHooks.ReloadEvent -= OnReload;
					PlayerHooks.PlayerCommand -= OnPlayerCommand;

					ServerApi.Hooks.GamePostInitialize.Deregister(this, OnPostInitialize);
					ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
					ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);

					return;
				}

				string[] host = Config.MySqlHost.Split(':');
				Db = new MySqlConnection
				{
					ConnectionString = String.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4};",
						host[0],
						host.Length == 1 ? "3306" : host[1],
						Config.MySqlDbName,
						Config.MySqlUsername,
						Config.MySqlPassword)
				};
			}
			else if (TShock.Config.Settings.StorageType.Equals("sqlite", StringComparison.OrdinalIgnoreCase))
			{
				Db = TShock.DB;
			}
			else
			{
				throw new InvalidOperationException("不存在的数据库类型!");
			}

			Mutes = new MuteManager(Db);

			#endregion

			#region Commands

			//Allows overriding of already created commands.
			Action<Command> Add = c =>
			{
				//Finds any commands with names and aliases that match the new command and removes them.
				TShockAPI.Commands.ChatCommands.RemoveAll(c2 => c2.Names.Exists(s2 => c.Names.Contains(s2)));
				//Then adds the new command.
				TShockAPI.Commands.ChatCommands.Add(c);
			};

			Add(new Command(Permissions.Find, Commands.Find, "find","查")
			{
				HelpText = "查找信息指令."
			});

            Add(new Command(Permissions.FreezeTime, Commands.FreezeTime, "freezetime")
			{
				HelpText = "锁定时间."
			});

			Add(new Command(Permissions.HomeDelete, Commands.DeleteHome, "delhome")
			{
				AllowServer = false,
				HelpText = "删除你的一个家园传送点."
			});
			Add(new Command(Permissions.HomeSet, Commands.SetHome, "sethome")
			{
				AllowServer = false,
				HelpText = "Sets you a home point."
			});
			Add(new Command(Permissions.HomeTp, Commands.MyHome, "myhome")
			{
				AllowServer = false,
				HelpText = "Teleports you to one of your home points."
			});

			Add(new Command(Permissions.KickAll, Commands.KickAll, "kickall")
			{
				HelpText = "Kicks everyone on the server."
			});

			Add(new Command(Permissions.LastCommand, Commands.RepeatLast, "=")
			{
				HelpText = "Allows you to repeat your last command."
			});

			Add(new Command(Permissions.More, Commands.More, "more")
			{
				AllowServer = false,
				HelpText = "Maximizes item stack of held item."
			});

			//This will override TShock's 'mute' command
			Add(new Command(Permissions.Mute, Commands.Mute, "mute")
			{
				HelpText = "管理禁言."
			});

			Add(new Command(Permissions.PvP, Commands.PvP, "pvp")
			{
				AllowServer = false,
				HelpText = "Toggles your PvP status."
			});

			Add(new Command(Permissions.Ruler, Commands.Ruler, "ruler")
			{
				AllowServer = false,
				HelpText = "Allows you to measure the distances between two blocks."
			});

			Add(new Command(Permissions.Send, Commands.Send, "send")
			{
				HelpText = "Broadcasts a message in a custom color."
			});

			Add(new Command(Permissions.Sudo, Commands.Sudo, "sudo")
			{
				HelpText = "Allows you to execute a command as another user."
			});

			Add(new Command(Permissions.TimeCmd, Commands.TimeCmd, "timecmd")
			{
				HelpText = "Executes a command after a given time interval."
			});

			Add(new Command(Permissions.TpBack, Commands.Back, "back", "b")
			{
				AllowServer = false,
				HelpText = "Teleports you back to your previous position after dying or teleporting."
			});
			Add(new Command(Permissions.TpDown, Commands.Down, "down")
			{
				AllowServer = false,
				HelpText = "Teleports you down through a layer of blocks."
			});
			Add(new Command(Permissions.TpLeft, Commands.Left, "left")
			{
				AllowServer = false,
				HelpText = "Teleports you left through a layer of blocks."
			});
			Add(new Command(Permissions.TpRight, Commands.Right, "right")
			{
				AllowServer = false,
				HelpText = "Teleports you right through a layer of blocks."
			});
			Add(new Command(Permissions.TpUp, Commands.Up, "up")
			{
				AllowServer = false,
				HelpText = "Teleports you up through a layer of blocks."
			});

			#endregion
		}
        private void OnPostInitialize(EventArgs args)
		{
            Homes = new HomeManager(Db);
		}

		private async void OnJoin(JoinEventArgs e)
		{
			if (e.Handled)
			{
				return;
			}

			TSPlayer player = TShock.Players[e.Who];
			if (player == null)
			{
				return;
			}

			DateTime muteExpiration = await Mutes.GetExpirationAsync(player);

			if (DateTime.UtcNow < muteExpiration)
			{
				player.mute = true;
				try
				{
					await Task.Delay(muteExpiration - DateTime.UtcNow, player.GetPlayerInfo().MuteToken);
					player.mute = false;
					player.SendInfoMessage("你已被禁言.");
				}
				catch (TaskCanceledException)
				{
				}
			}
		}

		private void OnGetData(GetDataEventArgs e)
		{
            if (e.Handled)
			{
				return;
			}

			TSPlayer tsplayer = TShock.Players[e.Msg.whoAmI];
			if (tsplayer == null)
			{
				return;
			}

			switch (e.MsgID)
			{
				#region Packet 118 - PlayerDeathV2

				case PacketTypes.PlayerDeathV2:
					if (tsplayer.Group.HasPermission(Permissions.TpBack))
					{
						tsplayer.GetPlayerInfo().PushBackHistory(tsplayer.TPlayer.position);
					}
					return;

				case PacketTypes.Teleport:
					{
						if (tsplayer.Group.HasPermission(Permissions.TpBack))
						{
							using (MemoryStream ms = new MemoryStream(e.Msg.readBuffer, e.Index, e.Length))
							{
								BitsByte flags = (byte)ms.ReadByte();

								int type = 0;
								if (flags[1])
								{
									type = 2;
								}

								if (type == 0 && tsplayer.Group.HasPermission(TShockAPI.Permissions.rod))
								{
									tsplayer.GetPlayerInfo().PushBackHistory(tsplayer.TPlayer.position);
								}
								else if (type == 2 && tsplayer.Group.HasPermission(TShockAPI.Permissions.wormhole))
								{
									tsplayer.GetPlayerInfo().PushBackHistory(tsplayer.TPlayer.position);
								}
							}
						}
					}
					return;

					#endregion
			}
		}
	}
}
