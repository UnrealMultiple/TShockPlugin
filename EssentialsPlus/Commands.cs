using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using EssentialsPlus.Db;
using EssentialsPlus.Extensions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using TShockAPI;
using TShockAPI.DB;

namespace EssentialsPlus
{
	public static class Commands
	{
        public static void Find(CommandArgs e)
        {
            if (e.Parameters.Count < 1)
            {
                e.Player.SendErrorMessage("无效语法!正确语法: {0}find <类> <内容...> [页码]",
                    TShock.Config.Settings.CommandSpecifier);
                e.Player.SendSuccessMessage("可用的 {0}find 类型:", TShock.Config.Settings.CommandSpecifier);
                e.Player.SendInfoMessage("-指令(command): 查找指令及权限.");
                e.Player.SendInfoMessage("-物品(item): 查找物品ID.");
                e.Player.SendInfoMessage("-npc: 查找 NPC.");
                e.Player.SendInfoMessage("-图格(tile): 查找图格ID.");
                e.Player.SendInfoMessage("-墙(wall): 查找墙ID.");
                return;
            }
            else
            {
                switch (e.Parameters[0])
                {
                    case "物品":
                    case "i":
                    case "item":
                        if (e.Parameters.Count < 2)
                        {
                            e.Player.SendErrorMessage("无效语法!正确语法: {0}find <类> <内容...> [页码]",
                    TShock.Config.Settings.CommandSpecifier);
                            e.Player.SendInfoMessage("-物品(item): 查找物品ID.");
                            return;
                        }
                        int page;
                        bool fl3 = PaginationTools.TryParsePageNumber(e.Parameters, 2, e.Player, out page);
                        if (!fl3)
                        {
                            e.Player.SendErrorMessage("无效页码!");
                            return;
                        }
                        var items = new List<string>();
                        for (int i = -48; i < 0; i++)
                        {
                            var item = new Item();
                            item.netDefaults(i);
                            if (item.HoverName.ContainsInsensitive(e.Parameters[1]))
                            {
                                items.Add(String.Format("{0} (ID: {1})", item.HoverName, i));
                            }
                        }
                        for (int i = 0; i < ItemID.Count; i++)
                        {
                            if (Lang.GetItemNameValue(i).ContainsInsensitive(e.Parameters[1]))
                            {
                                items.Add(String.Format("{0} (ID: {1})", Lang.GetItemNameValue(i), i));
                            }
                        }
                        PaginationTools.SendPage(e.Player, page, items,
                            new PaginationTools.Settings
                            {
                                HeaderFormat = "找到物品 ({0}/{1}):",
                                FooterFormat = String.Format("使用 /find(查) item(物品) {0} {{0}} 获取更多", e.Parameters[1]),
                                NothingToDisplayString = "没有物品被找到."
                            });
                        return;
                    case "指令":
                    case "c":
                    case "command":
                        {
                            var commands = new List<string>();


                            foreach (
                            Command command in
                                    TShockAPI.Commands.ChatCommands.FindAll(c => c.Names.Any(s => s.ContainsInsensitive(e.Parameters[1]))))
                            {
                                commands.Add(String.Format("{0} (权限: {1})", command.Name, command.Permissions.FirstOrDefault()));
                            }
                            int page2;
                            bool fl = PaginationTools.TryParsePageNumber(e.Parameters, 2, e.Player, out page2);
                            if (!fl)
                            {
                                e.Player.SendErrorMessage("无效页码!");
                                return;
                            }
                            PaginationTools.SendPage(e.Player, page2, commands,
                                new PaginationTools.Settings
                                {
                                    HeaderFormat = "找到指令 ({0}/{1}):",
                                    FooterFormat = String.Format("使用 /find(查) 指令(command) {0} {{0}} 获取更多", e.Parameters[1]),
                                    NothingToDisplayString = "没有指令被找到."
                                });
                            return;
                        }
                    case "npc":
                        var npcs = new List<string>();
                        int page3;
                        bool fl2 = PaginationTools.TryParsePageNumber(e.Parameters, 2, e.Player, out page3);
                        if (!fl2)
                        {
                            e.Player.SendErrorMessage("无效页码!");
                            return;
                        }
                        for (int i = -65; i < 0; i++)
                        {
                            var npc = new NPC();
                            npc.SetDefaults(i);
                            if (npc.FullName.ContainsInsensitive(e.Parameters[1]))
                            {
                                npcs.Add(String.Format("{0} (ID: {1})", npc.FullName, i));
                            }
                        }
                        for (int i = 0; i < NPCID.Count; i++)
                        {
                            if (Lang.GetNPCNameValue(i).ContainsInsensitive(e.Parameters[1]))
                            {
                                npcs.Add(String.Format("{0} (ID: {1})", Lang.GetNPCNameValue(i), i));
                            }
                        }

                        PaginationTools.SendPage(e.Player, page3, npcs,
                            new PaginationTools.Settings
                            {
                                HeaderFormat = "找到的NPC ({0}/{1}):",
                                FooterFormat = String.Format("使用 /find(查) npc {0} {{0}} 获取更多", e.Parameters[1]),
                                NothingToDisplayString = "没有NPC被找到.",
                            });
                        return;
                    case "图格":
                    case "tile":
                        var tiles = new List<string>();
                        int page4;
                        bool fl4 = PaginationTools.TryParsePageNumber(e.Parameters, 2, e.Player, out page4);
                        if (!fl4)
                        {
                            e.Player.SendErrorMessage("无效页码!");
                            return;
                        }
                        foreach (FieldInfo fi in typeof(TileID).GetFields())
                        {
                            var sb = new StringBuilder();
                            for (int i = 0; i < fi.Name.Length; i++)
                            {
                                if (Char.IsUpper(fi.Name[i]) && i > 0)
                                {
                                    sb.Append(" ").Append(fi.Name[i]);
                                }
                                else
                                {
                                    sb.Append(fi.Name[i]);
                                }
                            }

                            string name = sb.ToString();
                            if (name.ContainsInsensitive(e.Parameters[1]))
                            {
                                tiles.Add(String.Format("{0} (ID: {1})", name, fi.GetValue(null)));
                            }
                        }

                        PaginationTools.SendPage(e.Player, page4, tiles,
                            new PaginationTools.Settings
                            {
                                HeaderFormat = "查找到的图格 ({0}/{1}):",
                                FooterFormat = String.Format("使用 /find(查) tile(图格) {0} {{0}} 获取更多", e.Parameters[1]),
                                NothingToDisplayString = "没有图格被找到.",
                            });
                        return;
                    case "墙":
                    case "wall":
                        var walls = new List<string>();
                        int page5;
                        bool fl5 = PaginationTools.TryParsePageNumber(e.Parameters, 2, e.Player, out page5);
                        if (!fl5)
                        {
                            e.Player.SendErrorMessage("无效页码!");
                            return;
                        }
                        foreach (FieldInfo fi in typeof(WallID).GetFields())
                        {
                            var sb = new StringBuilder();
                            for (int i = 0; i < fi.Name.Length; i++)
                            {
                                if (Char.IsUpper(fi.Name[i]) && i > 0)
                                {
                                    sb.Append(" ").Append(fi.Name[i]);
                                }
                                else
                                {
                                    sb.Append(fi.Name[i]);
                                }
                            }

                            string name = sb.ToString();
                            if (name.ContainsInsensitive(e.Parameters[1]))
                            {
                                walls.Add(String.Format("{0} (ID: {1})", name, fi.GetValue(null)));
                            }
                        }

                        PaginationTools.SendPage(e.Player, page5, walls,
                            new PaginationTools.Settings
                            {
                                HeaderFormat = "找到的墙ID ({0}/{1}):",
                                FooterFormat = String.Format("Type /find(查) wall(墙) {0} {{0}} 获取更多", e.Parameters[1]),
                                NothingToDisplayString = "没有墙被找到.",
                            });
                        return;
                    default:
                        e.Player.SendErrorMessage("无效语法!正确语法: {0}find <类> <内容...> [页码]",
                    TShock.Config.Settings.CommandSpecifier);
                        e.Player.SendSuccessMessage("可用的 {0}find 类型:", TShock.Config.Settings.CommandSpecifier);
                        e.Player.SendInfoMessage("-指令(command): 查找指令及权限.");
                        e.Player.SendInfoMessage("-物品(item): 查找物品ID.");
                        e.Player.SendInfoMessage("-npc: 查找 NPC.");
                        e.Player.SendInfoMessage("-图格(tile): 查找图格ID.");
                        e.Player.SendInfoMessage("-墙(wall): 查找墙ID.");
                        break;
                }
            }
        }

        private static System.Timers.Timer FreezeTimer = new System.Timers.Timer(1000);

		public static void FreezeTime(CommandArgs e)
		{
			if (FreezeTimer.Enabled)
			{
				FreezeTimer.Stop();
				TSPlayer.All.SendInfoMessage("{0} unfroze the time.", e.Player.Name);
			}
			else
			{
				bool dayTime = Main.dayTime;
				double time = Main.time;

				FreezeTimer.Dispose();
				FreezeTimer = new System.Timers.Timer(1000);
				FreezeTimer.Elapsed += (o, ee) =>
				{
					Main.dayTime = dayTime;
					Main.time = time;
					TSPlayer.All.SendData(PacketTypes.TimeSet);
				};
				FreezeTimer.Start();
				TSPlayer.All.SendInfoMessage("{0} froze the time.", e.Player.Name);
			}
		}

		public static async void DeleteHome(CommandArgs e)
		{
			if (e.Parameters.Count > 1)
			{
				e.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}delhome <home name>", TShock.Config.Settings.CommandSpecifier);
				return;
			}

			string homeName = e.Parameters.Count == 1 ? e.Parameters[0] : "home";
			Home home = await EssentialsPlus.Homes.GetAsync(e.Player, homeName);
			if (home != null)
			{
				if (await EssentialsPlus.Homes.DeleteAsync(e.Player, homeName))
				{
					e.Player.SendSuccessMessage("Deleted your home '{0}'.", homeName);
				}
				else
				{
					e.Player.SendErrorMessage("Could not delete home, check logs for more details.");
				}
			}
			else
			{
				e.Player.SendErrorMessage("Invalid home '{0}'!", homeName);
			}
		}

		public static async void MyHome(CommandArgs e)
		{
			if (e.Parameters.Count > 1)
			{
				e.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}myhome <home name>", TShock.Config.Settings.CommandSpecifier);
				return;
			}

			if (Regex.Match(e.Message, @"^\w+ -l(?:ist)?$").Success)
			{
				List<Home> homes = await EssentialsPlus.Homes.GetAllAsync(e.Player);
				e.Player.SendInfoMessage(homes.Count == 0 ? "You have no homes set." : "List of homes: {0}", string.Join(", ", homes.Select(h => h.Name)));
			}
			else
			{
				string homeName = e.Parameters.Count == 1 ? e.Parameters[0] : "home";
				Home home = await EssentialsPlus.Homes.GetAsync(e.Player, homeName);
				if (home != null)
				{
					e.Player.Teleport(home.X, home.Y);
					e.Player.SendSuccessMessage("Teleported you to your home '{0}'.", homeName);
				}
				else
				{
					e.Player.SendErrorMessage("Invalid home '{0}'!", homeName);
				}
			}
		}
		public static async void SetHome(CommandArgs e)
		{
			if (e.Parameters.Count > 1)
			{
				e.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}sethome <home name>", TShock.Config.Settings.CommandSpecifier);
				return;
			}

			string homeName = e.Parameters.Count == 1 ? e.Parameters[0] : "home";
			if (await EssentialsPlus.Homes.GetAsync(e.Player, homeName) != null)
			{
				if (await EssentialsPlus.Homes.UpdateAsync(e.Player, homeName, e.Player.X, e.Player.Y))
				{
					e.Player.SendSuccessMessage("Updated your home '{0}'.", homeName);
				}
				else
				{
					e.Player.SendErrorMessage("Could not update home, check logs for more details.");
				}
				return;
			}

			if ((await EssentialsPlus.Homes.GetAllAsync(e.Player)).Count >= e.Player.Group.GetDynamicPermission(Permissions.HomeSet))
			{
				e.Player.SendErrorMessage("You have reached your home limit!");
				return;
			}

			if (await EssentialsPlus.Homes.AddAsync(e.Player, homeName, e.Player.X, e.Player.Y))
			{
				e.Player.SendSuccessMessage("Set your home '{0}'.", homeName);
			}
			else
			{
				e.Player.SendErrorMessage("Could not set home, check logs for more details.");
			}
		}

		public static async void KickAll(CommandArgs e)
		{
			var regex = new Regex(@"^\w+(?: -(\w+))* ?(.*)$");
			Match match = regex.Match(e.Message);
			bool noSave = false;
			foreach (Capture capture in match.Groups[1].Captures)
			{
				switch (capture.Value.ToLowerInvariant())
				{
					case "nosave":
						noSave = true;
						continue;
					default:
						e.Player.SendSuccessMessage("Valid {0}kickall switches:", TShock.Config.Settings.CommandSpecifier);
						e.Player.SendInfoMessage("-nosave: Kicks without saving SSC data.");
						return;
				}
			}

			int kickLevel = e.Player.Group.GetDynamicPermission(Permissions.KickAll);
			string reason = String.IsNullOrWhiteSpace(match.Groups[2].Value) ? "No reason." : match.Groups[2].Value;
			await Task.WhenAll(TShock.Players.Where(p => p != null && p.Group.GetDynamicPermission(Permissions.KickAll) < kickLevel).Select(p => Task.Run(() =>
			{
				if (!noSave && p.IsLoggedIn)
				{
					p.SaveServerCharacter();
				}
				p.Disconnect("Kicked: " + reason);
			})));
			e.Player.SendSuccessMessage("Kicked everyone for '{0}'.", reason);
		}

		public static async void RepeatLast(CommandArgs e)
		{
			string lastCommand = e.Player.GetPlayerInfo().LastCommand;
			if (String.IsNullOrWhiteSpace(lastCommand))
			{
				e.Player.SendErrorMessage("You don't have a last command!");
				return;
			}

			e.Player.SendSuccessMessage("Repeated last command '{0}{1}'!", TShock.Config.Settings.CommandSpecifier, lastCommand);
			await Task.Run(() => TShockAPI.Commands.HandleCommand(e.Player, TShock.Config.Settings.CommandSpecifier + lastCommand));
		}

		public static async void More(CommandArgs e)
		{
			await Task.Run(() =>
			{
				if (e.Parameters.Count > 0 && e.Parameters[0].ToLower() == "all")
				{
					bool full = true;
					foreach (Item item in e.TPlayer.inventory)
					{
						if (item == null || item.stack == 0) continue;
						int amtToAdd = item.maxStack - item.stack;
						if (amtToAdd > 0 && item.stack > 0 && !item.Name.ToLower().Contains("coin"))
						{
							full = false;
							e.Player.GiveItem(item.type, amtToAdd);
						}
					}
					if (!full)
						e.Player.SendSuccessMessage("Filled all your items.");
					else
						e.Player.SendErrorMessage("Your inventory is already full.");
				}
				else
				{
					Item item = e.Player.TPlayer.inventory[e.TPlayer.selectedItem];
					int amtToAdd = item.maxStack - item.stack;
					if (amtToAdd == 0)
						e.Player.SendErrorMessage("Your {0} is already full.", item.Name);
					else if (amtToAdd > 0 && item.stack > 0)
						e.Player.GiveItem(item.type, amtToAdd);
						e.Player.SendSuccessMessage("Filled up your {0}.", item.Name);
				}
			});
		}

		public static async void Mute(CommandArgs e)
		{
			string subCmd = e.Parameters.FirstOrDefault() ?? "help";
			switch (subCmd.ToLowerInvariant())
			{
				#region Add

				case "add":
					{
						var regex = new Regex(@"^\w+ \w+ (?:""(.+?)""|([^\s]+?))(?: (.+))?$");
						Match match = regex.Match(e.Message);
						if (!match.Success)
						{
							e.Player.SendErrorMessage("Invalid syntax! Proper syntax: /mute add <name> [time]");
							return;
						}

						int seconds = Int32.MaxValue / 1000;
						if (!String.IsNullOrWhiteSpace(match.Groups[3].Value) &&
							(!TShock.Utils.TryParseTime(match.Groups[3].Value, out seconds) || seconds <= 0 ||
							 seconds > Int32.MaxValue / 1000))
						{
							e.Player.SendErrorMessage("Invalid time '{0}'!", match.Groups[3].Value);
							return;
						}

						string playerName = String.IsNullOrWhiteSpace(match.Groups[2].Value)
							? match.Groups[1].Value
							: match.Groups[2].Value;
						List<TSPlayer> players = TShock.Players.FindPlayers(playerName);
						if (players.Count == 0)
						{
							UserAccount user = TShock.UserAccounts.GetUserAccountByName(playerName);
							if (user == null)
								e.Player.SendErrorMessage("Invalid player or account '{0}'!", playerName);
							else
							{
								if (TShock.Groups.GetGroupByName(user.Group).GetDynamicPermission(Permissions.Mute) >=
									e.Player.Group.GetDynamicPermission(Permissions.Mute))
								{
									e.Player.SendErrorMessage("You can't mute {0}!", user.Name);
									return;
								}

								if (await EssentialsPlus.Mutes.AddAsync(user, DateTime.UtcNow.AddSeconds(seconds)))
								{
									TSPlayer.All.SendInfoMessage("{0} muted {1}.", e.Player.Name, user.Name);
								}
								else
								{
									e.Player.SendErrorMessage("Could not mute, check logs for details.");
								}
							}
						}
						else if (players.Count > 1)
						{
							e.Player.SendErrorMessage("More than one player matched: {0}", String.Join(", ", players.Select(p => p.Name)));
						}
						else
						{
							if (players[0].Group.GetDynamicPermission(Permissions.Mute) >=
								e.Player.Group.GetDynamicPermission(Permissions.Mute))
							{
								e.Player.SendErrorMessage("You can't mute {0}!", players[0].Name);
								return;
							}

							if (await EssentialsPlus.Mutes.AddAsync(players[0], DateTime.UtcNow.AddSeconds(seconds)))
							{
								TSPlayer.All.SendInfoMessage("{0} muted {1}.", e.Player.Name, players[0].Name);

								players[0].mute = true;
								try
								{
									await Task.Delay(TimeSpan.FromSeconds(seconds), players[0].GetPlayerInfo().MuteToken);
									players[0].mute = false;
									players[0].SendInfoMessage("You have been unmuted.");
								}
								catch (TaskCanceledException)
								{
								}
							}
							else
								e.Player.SendErrorMessage("Could not mute, check logs for details.");
						}
					}
					return;

				#endregion

				#region Delete

				case "del":
				case "delete":
					{
						var regex = new Regex(@"^\w+ \w+ (?:""(.+?)""|([^\s]*?))$");
						Match match = regex.Match(e.Message);
						if (!match.Success)
						{
							e.Player.SendErrorMessage("Invalid syntax! Proper syntax: /mute del <name>");
							return;
						}

						string playerName = String.IsNullOrWhiteSpace(match.Groups[2].Value)
							? match.Groups[1].Value
							: match.Groups[2].Value;
						List<TSPlayer> players = TShock.Players.FindPlayers(playerName);
						if (players.Count == 0)
						{
							UserAccount user = TShock.UserAccounts.GetUserAccountByName(playerName);
							if (user == null)
								e.Player.SendErrorMessage("Invalid player or account '{0}'!", playerName);
							else
							{
								if (await EssentialsPlus.Mutes.DeleteAsync(user))
									TSPlayer.All.SendInfoMessage("{0} unmuted {1}.", e.Player.Name, user.Name);
								else
									e.Player.SendErrorMessage("Could not unmute, check logs for details.");
							}
						}
						else if (players.Count > 1)
							e.Player.SendErrorMessage("More than one player matched: {0}", String.Join(", ", players.Select(p => p.Name)));
						else
						{
							if (await EssentialsPlus.Mutes.DeleteAsync(players[0]))
							{
								players[0].mute = false;
								TSPlayer.All.SendInfoMessage("{0} unmuted {1}.", e.Player.Name, players[0].Name);
							}
							else
								e.Player.SendErrorMessage("Could not unmute, check logs for details.");
						}
					}
					return;

				#endregion

				#region Help

				default:
					e.Player.SendSuccessMessage("Mute Sub-Commands:");
					e.Player.SendInfoMessage("add <name> [time] - Mutes a player or account.");
					e.Player.SendInfoMessage("del <name> - Unmutes a player or account.");
					return;

				#endregion
			}
		}

		public static void PvP(CommandArgs e)
		{
			e.TPlayer.hostile = !e.TPlayer.hostile;
			string hostile = Language.GetTextValue(e.TPlayer.hostile ? "LegacyMultiplayer.11" : "LegacyMultiplayer.12", e.Player.Name);
			TSPlayer.All.SendData(PacketTypes.TogglePvp, "", e.Player.Index);
			TSPlayer.All.SendMessage(hostile, Main.teamColor[e.Player.Team]);
		}

		public static void Ruler(CommandArgs e)
		{
			if (e.Parameters.Count == 0)
			{
				if (e.Player.TempPoints.Any(p => p == Point.Zero))
				{
					e.Player.SendErrorMessage("Ruler points are not set up!");
					return;
				}

				Point p1 = e.Player.TempPoints[0];
				Point p2 = e.Player.TempPoints[1];
				int x = Math.Abs(p1.X - p2.X);
				int y = Math.Abs(p1.Y - p2.Y);
				double cartesian = Math.Sqrt(x * x + y * y);
				e.Player.SendInfoMessage("Distances: X: {0}, Y: {1}, Cartesian: {2:N3}", x, y, cartesian);
			}
			else if (e.Parameters.Count == 1)
			{
				if (e.Parameters[0] == "1")
				{
					e.Player.AwaitingTempPoint = 1;
					e.Player.SendInfoMessage("Modify a block to set the first ruler point.");
				}
				else if (e.Parameters[0] == "2")
				{
					e.Player.AwaitingTempPoint = 2;
					e.Player.SendInfoMessage("Modify a block to set the second ruler point.");
				}
				else
					e.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}ruler [1/2]", TShock.Config.Settings.CommandSpecifier);
			}
			else
				e.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}ruler [1/2]", TShock.Config.Settings.CommandSpecifier);
		}

		public static void Send(CommandArgs e)
		{
			var regex = new Regex(@"^\w+(?: (\d+),(\d+),(\d+))? (.+)$");
			Match match = regex.Match(e.Message);
			if (!match.Success)
			{
				e.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}send [r,g,b] <text...>", TShock.Config.Settings.CommandSpecifier);
				return;
			}

			byte r = e.Player.Group.R;
			byte g = e.Player.Group.G;
			byte b = e.Player.Group.B;
			if (!String.IsNullOrWhiteSpace(match.Groups[1].Value) && !String.IsNullOrWhiteSpace(match.Groups[2].Value) && !String.IsNullOrWhiteSpace(match.Groups[3].Value) &&
				(!byte.TryParse(match.Groups[1].Value, out r) || !byte.TryParse(match.Groups[2].Value, out g) || !byte.TryParse(match.Groups[3].Value, out b)))
			{
				e.Player.SendErrorMessage("Invalid color!");
				return;
			}
			TSPlayer.All.SendMessage(match.Groups[4].Value, new Color(r, g, b));
		}

		public static async void Sudo(CommandArgs e)
		{
			var regex = new Regex(String.Format(@"^\w+(?: -(\w+))* (?:""(.+?)""|([^\s]*?)) (?:{0})?(.+)$", TShock.Config.Settings.CommandSpecifier));
			Match match = regex.Match(e.Message);
			if (!match.Success)
			{
				e.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}sudo [-switches...] <player> <command...>", TShock.Config.Settings.CommandSpecifier);
				e.Player.SendSuccessMessage("Valid {0}sudo switches:", TShock.Config.Settings.CommandSpecifier);
				e.Player.SendInfoMessage("-f, -force: Force sudo, ignoring permissions.");
				return;
			}

			bool force = false;
			foreach (Capture capture in match.Groups[1].Captures)
			{
				switch (capture.Value.ToLowerInvariant())
				{
					case "f":
					case "force":
						if (!e.Player.Group.HasPermission(Permissions.SudoForce))
						{
							e.Player.SendErrorMessage("You do not have access to the switch '-{0}'!", capture.Value);
							return;
						}
						force = true;
						continue;
					default:
						e.Player.SendSuccessMessage("Valid {0}sudo switches:", TShock.Config.Settings.CommandSpecifier);
						e.Player.SendInfoMessage("-f, -force: Force sudo, ignoring permissions.");
						return;
				}
			}

			string playerName = String.IsNullOrWhiteSpace(match.Groups[3].Value) ? match.Groups[2].Value : match.Groups[3].Value;
			string command = match.Groups[4].Value;

			List<TSPlayer> players = TShock.Players.FindPlayers(playerName);
			if (players.Count == 0)
				e.Player.SendErrorMessage("Invalid player '{0}'!", playerName);
			else if (players.Count > 1)
				e.Player.SendErrorMessage("More than one player matched: {0}", String.Join(", ", players.Select(p => p.Name)));
			else
			{
				if ((e.Player.Group.GetDynamicPermission(Permissions.Sudo) <= players[0].Group.GetDynamicPermission(Permissions.Sudo))
					&& !e.Player.Group.HasPermission(Permissions.SudoSuper))
				{
					e.Player.SendErrorMessage("You cannot force {0} to execute {1}{2}!", players[0].Name, TShock.Config.Settings.CommandSpecifier, command);
					return;
				}

				e.Player.SendSuccessMessage("Forced {0} to execute {1}{2}.", players[0].Name, TShock.Config.Settings.CommandSpecifier, command);
				if (!e.Player.Group.HasPermission(Permissions.SudoInvisible))
					players[0].SendInfoMessage("{0} forced you to execute {1}{2}.", e.Player.Name, TShock.Config.Settings.CommandSpecifier, command);

				var fakePlayer = new TSPlayer(players[0].Index)
				{
					AwaitingName = players[0].AwaitingName,
					AwaitingNameParameters = players[0].AwaitingNameParameters,
					AwaitingTempPoint = players[0].AwaitingTempPoint,
					Group = force ? new SuperAdminGroup() : players[0].Group,
					TempPoints = players[0].TempPoints
				};
				await Task.Run(() => TShockAPI.Commands.HandleCommand(fakePlayer, TShock.Config.Settings.CommandSpecifier + command));

				players[0].AwaitingName = fakePlayer.AwaitingName;
				players[0].AwaitingNameParameters = fakePlayer.AwaitingNameParameters;
				players[0].AwaitingTempPoint = fakePlayer.AwaitingTempPoint;
				players[0].TempPoints = fakePlayer.TempPoints;
			}
		}

		public static async void TimeCmd(CommandArgs e)
		{
			var regex = new Regex(String.Format(@"^\w+(?: -(\w+))* (\w+) (?:{0})?(.+)$", TShock.Config.Settings.CommandSpecifier));
			Match match = regex.Match(e.Message);
			if (!match.Success)
			{
				e.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}timecmd [-switches...] <time> <command...>", TShock.Config.Settings.CommandSpecifier);
				e.Player.SendSuccessMessage("Valid {0}timecmd switches:", TShock.Config.Settings.CommandSpecifier);
				e.Player.SendInfoMessage("-r, -repeat: Repeats the time command indefinitely.");
				return;
			}

			bool repeat = false;
			foreach (Capture capture in match.Groups[1].Captures)
			{
				switch (capture.Value.ToLowerInvariant())
				{
					case "r":
					case "repeat":
						repeat = true;
						break;
					default:
						e.Player.SendSuccessMessage("Valid {0}timecmd switches:", TShock.Config.Settings.CommandSpecifier);
						e.Player.SendInfoMessage("-r, -repeat: Repeats the time command indefinitely.");
						return;
				}
			}

			if (!TShock.Utils.TryParseTime(match.Groups[2].Value, out int seconds) || seconds <= 0 || seconds > Int32.MaxValue / 1000)
			{
				e.Player.SendErrorMessage("Invalid time '{0}'!", match.Groups[2].Value);
				return;
			}

			if (repeat)
				e.Player.SendSuccessMessage("Queued command '{0}{1}' indefinitely. Use /cancel to cancel!", TShock.Config.Settings.CommandSpecifier, match.Groups[3].Value);
			else
				e.Player.SendSuccessMessage("Queued command '{0}{1}'. Use /cancel to cancel!", TShock.Config.Settings.CommandSpecifier, match.Groups[3].Value);
			e.Player.AddResponse("cancel", o =>
			{
				e.Player.GetPlayerInfo().CancelTimeCmd();
				e.Player.SendSuccessMessage("Cancelled all time commands!");
			});

			CancellationToken token = e.Player.GetPlayerInfo().TimeCmdToken;
			try
			{
				await Task.Run(async () =>
				{
					do
					{
						await Task.Delay(TimeSpan.FromSeconds(seconds), token);
						TShockAPI.Commands.HandleCommand(e.Player, TShock.Config.Settings.CommandSpecifier + match.Groups[3].Value);
					}
					while (repeat);
				}, token);
			}
			catch (TaskCanceledException)
			{
			}
		}

		public static void Back(CommandArgs e)
		{
			if (e.Parameters.Count > 1)
			{
				e.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}back [steps]", TShock.Config.Settings.CommandSpecifier);
				return;
			}

			int steps = 1;
			if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out steps) || steps <= 0))
			{
				e.Player.SendErrorMessage("Invalid number of steps '{0}'!", e.Parameters[0]);
				return;
			}

			PlayerInfo info = e.Player.GetPlayerInfo();
			if (info.BackHistoryCount == 0)
			{
				e.Player.SendErrorMessage("Could not teleport back!");
				return;
			}

			steps = Math.Min(steps, info.BackHistoryCount);
			e.Player.SendSuccessMessage("Teleported back {0} step{1}.", steps, steps == 1 ? "" : "s");
			Vector2 vector = info.PopBackHistory(steps);
			e.Player.Teleport(vector.X, vector.Y);
		}
		public static async void Down(CommandArgs e)
		{
			if (e.Parameters.Count > 1)
			{
				e.Player.SendErrorMessage("Invalid syntax! Correct syntax: {0}down [levels]", TShock.Config.Settings.CommandSpecifier);
				return;
			}

			int levels = 1;
			if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out levels) || levels <= 0))
			{
				e.Player.SendErrorMessage("Invalid number of levels '{0}'!", levels);
				return;
			}

			int currentLevel = 0;
			bool empty = false;
			int x = Math.Max(0, Math.Min(e.Player.TileX, Main.maxTilesX - 2));
			int y = Math.Max(0, Math.Min(e.Player.TileY + 3, Main.maxTilesY - 3));

			await Task.Run(() =>
			{
				for (int j = y; currentLevel < levels && j < Main.maxTilesY - 2; j++)
				{
					if (Main.tile[x, j].IsEmpty() && Main.tile[x + 1, j].IsEmpty() &&
						Main.tile[x, j + 1].IsEmpty() && Main.tile[x + 1, j + 1].IsEmpty() &&
						Main.tile[x, j + 2].IsEmpty() && Main.tile[x + 1, j + 2].IsEmpty())
					{
						empty = true;
					}
					else if (empty)
					{
						empty = false;
						currentLevel++;
						y = j;
					}
				}
			});

			if (currentLevel == 0)
				e.Player.SendErrorMessage("Could not teleport down!");
			else
			{
				if (e.Player.Group.HasPermission(Permissions.TpBack))
					e.Player.GetPlayerInfo().PushBackHistory(e.TPlayer.position);
				e.Player.Teleport(16 * x, 16 * y - 10);
				e.Player.SendSuccessMessage("Teleported down {0} level{1}.", currentLevel, currentLevel == 1 ? "" : "s");
			}
		}
		public static async void Left(CommandArgs e)
		{
			if (e.Parameters.Count > 1)
			{
				e.Player.SendErrorMessage("Invalid syntax! Correct syntax: {0}left [levels]", TShock.Config.Settings.CommandSpecifier);
				return;
			}

			int levels = 1;
			if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out levels) || levels <= 0))
			{
				e.Player.SendErrorMessage("Invalid number of levels '{0}'!", levels);
				return;
			}

			int currentLevel = 0;
			bool solid = false;
			int x = Math.Max(0, Math.Min(e.Player.TileX, Main.maxTilesX - 2));
			int y = Math.Max(0, Math.Min(e.Player.TileY, Main.maxTilesY - 3));

			await Task.Run(() =>
			{
				for (int i = x; currentLevel < levels && i >= 0; i--)
				{
					if (Main.tile[i, y].IsEmpty() && Main.tile[i + 1, y].IsEmpty() &&
						Main.tile[i, y + 1].IsEmpty() && Main.tile[i + 1, y + 1].IsEmpty() &&
						Main.tile[i, y + 2].IsEmpty() && Main.tile[i + 1, y + 2].IsEmpty())
					{
						if (solid)
						{
							solid = false;
							currentLevel++;
							x = i;
						}
					}
					else
						solid = true;
				}
			});

			if (currentLevel == 0)
				e.Player.SendErrorMessage("Could not teleport left!");
			else
			{
				if (e.Player.Group.HasPermission(Permissions.TpBack))
					e.Player.GetPlayerInfo().PushBackHistory(e.TPlayer.position);
				e.Player.Teleport(16 * x + 12, 16 * y);
				e.Player.SendSuccessMessage("Teleported left {0} level{1}.", currentLevel, currentLevel == 1 ? "" : "s");
			}
		}
		public static async void Right(CommandArgs e)
		{
			if (e.Parameters.Count > 1)
			{
				e.Player.SendErrorMessage("Invalid syntax! Correct syntax: {0}right [levels]", TShock.Config.Settings.CommandSpecifier);
				return;
			}

			int levels = 1;
			if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out levels) || levels <= 0))
			{
				e.Player.SendErrorMessage("Invalid number of levels '{0}'!", levels);
				return;
			}

			int currentLevel = 0;
			bool solid = false;
			int x = Math.Max(0, Math.Min(e.Player.TileX + 1, Main.maxTilesX - 2));
			int y = Math.Max(0, Math.Min(e.Player.TileY, Main.maxTilesY - 3));

			await Task.Run(() =>
			{
				for (int i = x; currentLevel < levels && i < Main.maxTilesX - 1; i++)
				{
					if (Main.tile[i, y].IsEmpty() && Main.tile[i + 1, y].IsEmpty() &&
						Main.tile[i, y + 1].IsEmpty() && Main.tile[i + 1, y + 1].IsEmpty() &&
						Main.tile[i, y + 2].IsEmpty() && Main.tile[i + 1, y + 2].IsEmpty())
					{
						if (solid)
						{
							solid = false;
							currentLevel++;
							x = i;
						}
					}
					else
						solid = true;
				}
			});

			if (currentLevel == 0)
				e.Player.SendErrorMessage("Could not teleport right!");
			else
			{
				if (e.Player.Group.HasPermission(Permissions.TpBack))
					e.Player.GetPlayerInfo().PushBackHistory(e.TPlayer.position);
				e.Player.Teleport(16 * x, 16 * y);
				e.Player.SendSuccessMessage("Teleported right {0} level{1}.", currentLevel, currentLevel == 1 ? "" : "s");
			}
		}
		public static async void Up(CommandArgs e)
		{
			if (e.Parameters.Count > 1)
			{
				e.Player.SendErrorMessage("Invalid syntax! Correct syntax: {0}up [levels]", TShock.Config.Settings.CommandSpecifier);
				return;
			}

			int levels = 1;
			if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out levels) || levels <= 0))
			{
				e.Player.SendErrorMessage("Invalid number of levels '{0}'!", levels);
				return;
			}

			int currentLevel = 0;
			bool solid = false;
			int x = Math.Max(0, Math.Min(e.Player.TileX, Main.maxTilesX - 2));
			int y = Math.Max(0, Math.Min(e.Player.TileY, Main.maxTilesY - 3));

			await Task.Run(() =>
			{
				for (int j = y; currentLevel < levels && j >= 0; j--)
				{
					if (Main.tile[x, j].IsEmpty() && Main.tile[x + 1, j].IsEmpty() &&
						Main.tile[x, j + 1].IsEmpty() && Main.tile[x + 1, j + 1].IsEmpty() &&
						Main.tile[x, j + 2].IsEmpty() && Main.tile[x + 1, j + 2].IsEmpty())
					{
						if (solid)
						{
							solid = false;
							currentLevel++;
							y = j;
						}
					}
					else
						solid = true;
				}
			});

			if (currentLevel == 0)
				e.Player.SendErrorMessage("Could not teleport up!");
			else
			{
				if (e.Player.Group.HasPermission(Permissions.TpBack))
					e.Player.GetPlayerInfo().PushBackHistory(e.TPlayer.position);
				e.Player.Teleport(16 * x, 16 * y + 6);
				e.Player.SendSuccessMessage("Teleported up {0} level{1}.", currentLevel, currentLevel == 1 ? "" : "s");
			}
		}
	}
}
