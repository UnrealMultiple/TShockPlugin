using EssentialsPlus.Extensions;
using Microsoft.Xna.Framework;
using System.Text;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using TShockAPI;

namespace EssentialsPlus;

public static class Commands
{
    public static async void Find(CommandArgs e)
    {
        var regex = new Regex(@"^\w+ -(?<switch>\w+) (?<search>.+?) ?(?<page>\d*)$");
        var match = regex.Match(e.Message);
        if (!match.Success)
        {
            e.Player.SendErrorMessage(GetString("格式错误！正确的语法是：{0}find <-参数> <名称...> [页数]"),
                TShock.Config.Settings.CommandSpecifier);
            e.Player.SendSuccessMessage(GetString("有效的 {0}find 参数："), TShock.Config.Settings.CommandSpecifier);
            e.Player.SendInfoMessage(GetString("-command: 查找命令."));
            e.Player.SendInfoMessage(GetString("-item: 查找物品."));
            e.Player.SendInfoMessage(GetString("-npc: 查找NPC."));
            e.Player.SendInfoMessage(GetString("-tile: 查找方块."));
            e.Player.SendInfoMessage(GetString("-wall: 查找墙壁."));
            return;
        }

        var page = 1;
        if (!string.IsNullOrWhiteSpace(match.Groups["page"].Value) &&
            (!int.TryParse(match.Groups["page"].Value, out page) || page <= 0))
        {
            e.Player.SendErrorMessage(GetString("无效的页数 '{0}'！"), match.Groups["page"].Value);
            return;
        }

        switch (match.Groups["switch"].Value.ToLowerInvariant())
        {
            #region Command

            case "command":
            case "c":
            case "命令":
            {
                var commands = new List<string>();

                await Task.Run(() =>
                {
                    foreach (
                        var command in
                            TShockAPI.Commands.ChatCommands.FindAll(c => c.Names.Any(s => s.ContainsInsensitive(match.Groups[2].Value))))
                    {
                        commands.Add(string.Format(GetString("{0} (权限: {1})"), command.Name, command.Permissions.FirstOrDefault()));
                    }
                });

                PaginationTools.SendPage(e.Player, page, commands,
                    new PaginationTools.Settings
                    {
                        HeaderFormat = GetString("找到的命令 ({0}/{1}):"),
                        FooterFormat = string.Format(GetString("输入 /find -command {0} {{0}} 查看更多"), match.Groups[2].Value),
                        NothingToDisplayString = GetString("未找到命令。")
                    });
                return;
            }

            #endregion

            #region Item

            case "item":
            case "i":
            case "物品":
            {
                var items = new List<string>();

                await Task.Run(() =>
                {
                    for (var i = -48; i < 0; i++)
                    {
                        var item = new Item();
                        item.netDefaults(i);
                        if (item.HoverName.ContainsInsensitive(match.Groups[2].Value))
                        {
                            items.Add(string.Format("{0} (ID: {1})", item.HoverName, i));
                        }
                    }
                    for (var i = 0; i < ItemID.Count; i++)
                    {
                        if (Lang.GetItemNameValue(i).ContainsInsensitive(match.Groups[2].Value))
                        {
                            items.Add(string.Format("{0} (ID: {1})", Lang.GetItemNameValue(i), i));
                        }
                    }
                });

                PaginationTools.SendPage(e.Player, page, items,
                    new PaginationTools.Settings
                    {
                        HeaderFormat = GetString("找到的物品 ({0}/{1}):"),
                        FooterFormat = string.Format(GetString("输入 /find -item {0} {{0}} 查看更多"), match.Groups[2].Value),
                        NothingToDisplayString = GetString("未找到物品。")
                    });
                return;
            }


            #endregion

            #region NPC

            case "npc":
            case "n":
            case "NPC":
            {
                var npcs = new List<string>();

                await Task.Run(() =>
                {
                    for (var i = -65; i < 0; i++)
                    {
                        var npc = new NPC();
                        npc.SetDefaults(i);
                        if (npc.FullName.ContainsInsensitive(match.Groups[2].Value))
                        {
                            npcs.Add(string.Format("{0} (ID: {1})", npc.FullName, i));
                        }
                    }
                    for (var i = 0; i < NPCID.Count; i++)
                    {
                        if (Lang.GetNPCNameValue(i).ContainsInsensitive(match.Groups[2].Value))
                        {
                            npcs.Add(string.Format("{0} (ID: {1})", Lang.GetNPCNameValue(i), i));
                        }
                    }
                });

                PaginationTools.SendPage(e.Player, page, npcs,
                    new PaginationTools.Settings
                    {
                        HeaderFormat = GetString("找到的NPC ({0}/{1}):"),
                        FooterFormat = string.Format(GetString("输入 /find -npc {0} {{0}} 查看更多"), match.Groups[2].Value),
                        NothingToDisplayString = GetString("未找到NPC。"),
                    });
                return;
            }


            #endregion

            #region Tile

            case "tile":
            case "t":
            case "方块":
            {
                var tiles = new List<string>();

                await Task.Run(() =>
                {
                    foreach (var fi in typeof(TileID).GetFields())
                    {
                        var sb = new StringBuilder();
                        for (var i = 0; i < fi.Name.Length; i++)
                        {
                            if (char.IsUpper(fi.Name[i]) && i > 0)
                            {
                                sb.Append(' ').Append(fi.Name[i]);
                            }
                            else
                            {
                                sb.Append(fi.Name[i]);
                            }
                        }

                        var name = sb.ToString();
                        if (name.ContainsInsensitive(match.Groups[2].Value))
                        {
                            tiles.Add(string.Format("{0} (ID: {1})", name, fi.GetValue(null)));
                        }
                    }
                });

                PaginationTools.SendPage(e.Player, page, tiles,
                    new PaginationTools.Settings
                    {
                        HeaderFormat = GetString("找到的方块 ({0}/{1}):"),
                        FooterFormat = string.Format(GetString("输入 /find -tile {0} {{0}} 查看更多"), match.Groups[2].Value),
                        NothingToDisplayString = GetString("未找到方块。"),
                    });
                return;
            }

            #endregion

            #region Wall

            case "wall":
            case "w":
            case "墙壁":
            {
                var walls = new List<string>();

                await Task.Run(() =>
                {
                    foreach (var fi in typeof(WallID).GetFields())
                    {
                        var sb = new StringBuilder();
                        for (var i = 0; i < fi.Name.Length; i++)
                        {
                            if (char.IsUpper(fi.Name[i]) && i > 0)
                            {
                                sb.Append(' ').Append(fi.Name[i]);
                            }
                            else
                            {
                                sb.Append(fi.Name[i]);
                            }
                        }

                        var name = sb.ToString();
                        if (name.ContainsInsensitive(match.Groups[2].Value))
                        {
                            walls.Add(string.Format("{0} (ID: {1})", name, fi.GetValue(null)));
                        }
                    }
                });

                PaginationTools.SendPage(e.Player, page, walls,
                    new PaginationTools.Settings
                    {
                        HeaderFormat = GetString("找到的墙壁 ({0}/{1}):"),
                        FooterFormat = string.Format(GetString("输入 /find -wall {0} {{0}} 查看更多"), match.Groups[2].Value),
                        NothingToDisplayString = GetString("未找到墙壁。"),
                    });
                return;
            }

            #endregion

            default:
            {
                e.Player.SendSuccessMessage(GetString("有效的 {0}find 参数:"), TShock.Config.Settings.CommandSpecifier);
                e.Player.SendInfoMessage(GetString("-command: 查找命令."));
                e.Player.SendInfoMessage(GetString("-item: 查找物品."));
                e.Player.SendInfoMessage(GetString("-npc: 查找NPC."));
                e.Player.SendInfoMessage(GetString("-tile: 查找方块."));
                e.Player.SendInfoMessage(GetString("-wall: 查找墙壁."));
                return;
            }
        }
    }

    public static System.Timers.Timer FreezeTimer = new System.Timers.Timer(1000);

    public static void FreezeTime(CommandArgs e)
    {
        if (FreezeTimer.Enabled)
        {
            FreezeTimer.Stop();
            TSPlayer.All.SendInfoMessage(GetString("{0} 解除了时间冻结。"), e.Player.Name);
        }
        else
        {
            var dayTime = Main.dayTime;
            var time = Main.time;

            FreezeTimer.Dispose();
            FreezeTimer = new System.Timers.Timer(1000);
            FreezeTimer.Elapsed += (o, ee) =>
            {
                Main.dayTime = dayTime;
                Main.time = time;
                TSPlayer.All.SendData(PacketTypes.TimeSet);
            };
            FreezeTimer.Start();
            TSPlayer.All.SendInfoMessage(GetString("{0} 冻结了时间。"), e.Player.Name);
        }
    }

    public static void DeleteHome(CommandArgs e)
    {
        if (e.Parameters.Count > 1)
        {
            e.Player.SendErrorMessage(GetString("语法错误！正确的语法是：{0}delhome <家的名称>"), TShock.Config.Settings.CommandSpecifier);
            return;
        }

        var homeName = e.Parameters.Count == 1 ? e.Parameters[0] : "home";
        var home = EssentialsPlus.Homes.GetHome(e.Player, homeName);
        if (home != null)
        {
            if (EssentialsPlus.Homes.DeleteHome(e.Player, homeName))
            {
                e.Player.SendSuccessMessage(GetString("成功删除您的家 '{0}'。"), homeName);
            }
            else
            {
                e.Player.SendErrorMessage(GetString("无法删除家，请检查日志获取更多详细信息。"));
            }
        }
        else
        {
            e.Player.SendErrorMessage(GetString("无效的家 '{0}'！"), homeName);
        }
    }

    public static void MyHome(CommandArgs e)
    {
        if (e.Parameters.Count > 1)
        {
            e.Player.SendErrorMessage(GetString("语法错误！正确的语法是：{0}myhome <家的名称>"), TShock.Config.Settings.CommandSpecifier);
            return;
        }

        if (Regex.Match(e.Message, @"^\w+ -l(?:ist)?$").Success)
        {
            var homes = EssentialsPlus.Homes.GetAllAsync(e.Player);
            e.Player.SendInfoMessage(homes.Count == 0 ? GetString("您没有设置家。") : GetString("家的列表: {0}"), string.Join(", ", homes.Select(h => h.Name)));
        }
        else
        {
            var homeName = e.Parameters.Count == 1 ? e.Parameters[0] : "home";
            var home = EssentialsPlus.Homes.GetHome(e.Player, homeName);
            if (home != null)
            {
                e.Player.Teleport(home.X, home.Y);
                e.Player.SendSuccessMessage(GetString("传送您到您的家 '{0}'。"), homeName);
            }
            else
            {
                e.Player.SendErrorMessage(GetString("无效的家 '{0}'！"), homeName);
            }
        }
    }

    public static void SetHome(CommandArgs e)
    {
        if (e.Parameters.Count > 1)
        {
            e.Player.SendErrorMessage(GetString("语法错误！正确的语法是：{0}sethome <家的名称>"), TShock.Config.Settings.CommandSpecifier);
            return;
        }

        var homeName = e.Parameters.Count == 1 ? e.Parameters[0] : "home";
        if (EssentialsPlus.Homes.GetHome(e.Player, homeName) != null)
        {
            if (EssentialsPlus.Homes.UpdateHome(e.Player, homeName, e.Player.X, e.Player.Y))
            {
                e.Player.SendSuccessMessage(GetString("更新了您的家 '{0}'。"), homeName);
            }
            else
            {
                e.Player.SendErrorMessage(GetString("无法更新家，请检查日志获取更多详细信息。"));
            }
            return;
        }

        if (EssentialsPlus.Homes.GetAllAsync(e.Player).Count >= e.Player.Group.GetDynamicPermission(Permissions.HomeSet))
        {
            e.Player.SendErrorMessage(GetString("您已达到家的设置上限！"));
            return;
        }

        if (EssentialsPlus.Homes.AddHome(e.Player, homeName, e.Player.X, e.Player.Y))
        {
            e.Player.SendSuccessMessage(GetString("设置了您的家 '{0}'。"), homeName);
        }
        else
        {
            e.Player.SendErrorMessage(GetString("无法设置家，请检查日志获取更多详细信息。"));
        }
    }


    public static async void KickAll(CommandArgs e)
    {
        var regex = new Regex(@"^\w+(?: -(\w+))* ?(.*)$");
        var match = regex.Match(e.Message);
        var noSave = false;
        foreach (Capture capture in match.Groups[1].Captures)
        {
            switch (capture.Value.ToLowerInvariant())
            {
                case "nosave":
                    noSave = true;
                    continue;
                default:
                    e.Player.SendSuccessMessage(GetString("有效的 {0}kickall 参数:"), TShock.Config.Settings.CommandSpecifier);
                    e.Player.SendInfoMessage(GetString("-nosave: 踢出时不保存 SSC 数据."));
                    return;
            }
        }

        var kickLevel = e.Player.Group.GetDynamicPermission(Permissions.KickAll);
        var reason = string.IsNullOrWhiteSpace(match.Groups[2].Value) ? GetString("没有原因。") : match.Groups[2].Value;
        await Task.WhenAll(TShock.Players.Where(p => p != null && p.Group.GetDynamicPermission(Permissions.KickAll) < kickLevel).Select(p => Task.Run(() =>
        {
            if (!noSave && p.IsLoggedIn)
            {
                p.SaveServerCharacter();
            }
            p.Disconnect(GetString("踢出原因: ") + reason);
        })));
        e.Player.SendSuccessMessage(GetString("成功踢出所有人。原因: '{0}'。"), reason);
    }


    public static async void RepeatLast(CommandArgs e)
    {
        var lastCommand = e.Player.GetPlayerInfo().LastCommand;
        if (string.IsNullOrWhiteSpace(lastCommand))
        {
            e.Player.SendErrorMessage(GetString("您没有上次的命令！"));
            return;
        }

        e.Player.SendSuccessMessage(GetString("重复执行上次的命令 '{0}{1}'！"), TShock.Config.Settings.CommandSpecifier, lastCommand);
        await Task.Run(() => TShockAPI.Commands.HandleCommand(e.Player, TShock.Config.Settings.CommandSpecifier + lastCommand));
    }

    public static async void More(CommandArgs e)
    {
        await Task.Run(() =>
        {
            if (e.Parameters.Count > 0 && e.Parameters[0].ToLower() == "all")
            {
                var full = true;
                foreach (var item in e.TPlayer.inventory)
                {
                    if (item == null || item.stack == 0 || item.Name.ToLower().Contains("coin"))
                    {
                        continue;
                    }

                    var amtToAdd = item.maxStack - item.stack;
                    // 检查物品是否有词条（前缀），如果有词条则跳过
                    if (amtToAdd > 0 && item.prefix == 0)
                    {
                        full = false;
                        e.Player.GiveItem(item.type, amtToAdd);
                    }
                }
                if (!full)
                {
                    e.Player.SendSuccessMessage(GetString("填满了您的物品栏。"));
                }
                else
                {
                    e.Player.SendErrorMessage(GetString("您的物品栏已经满了。"));
                }
            }
            else if (e.Parameters.Count > 0 && e.Parameters[0].ToLower() == "fall")
            {
                var full = true;
                foreach (var item in e.TPlayer.inventory)
                {
                    if (item == null || item.stack == 0 || item.Name.ToLower().Contains("coin"))
                    {
                        continue;
                    }

                    var amtToAdd = item.maxStack - item.stack;
                    // fall模式不检查词条，所有物品都补充
                    if (amtToAdd > 0)
                    {
                        full = false;
                        e.Player.GiveItem(item.type, amtToAdd, item.prefix);
                    }
                }
                if (!full)
                {
                    e.Player.SendSuccessMessage(GetString("填满了您的物品栏（包括带词条的物品）。"));
                }
                else
                {
                    e.Player.SendErrorMessage(GetString("您的物品栏已经满了。"));
                }
            }
            else
            {
                var item = e.Player.TPlayer.inventory[e.TPlayer.selectedItem];            
                var amtToAdd = item.maxStack - item.stack;
                if (amtToAdd == 0)
                {
                    e.Player.SendErrorMessage(GetString("您的{0}已经满了。"), item.Name);
                }
                else if (amtToAdd > 0)
                {
                    // 使用GiveItem并带上prefix参数，保留词条
                    e.Player.GiveItem(item.type, amtToAdd, item.prefix);
                    e.Player.SendSuccessMessage(GetString("增加了您的{0}的堆叠数量。"), item.Name);
                }
            }
        });
    }

    public static async void Mute(CommandArgs e)
    {
        var subCmd = e.Parameters.FirstOrDefault() ?? "help";
        switch (subCmd.ToLowerInvariant())
        {
            #region 添加禁言

            case "add":
            {
                var regex = new Regex(@"^\w+ \w+ (?:""(.+?)""|([^\s]+?))(?: (.+))?$");
                var match = regex.Match(e.Message);
                if (!match.Success)
                {
                    e.Player.SendErrorMessage(GetString("无效的语法！正确的语法是：/mute add <名称> [时间]"));
                    return;
                }

                var seconds = int.MaxValue / 1000;
                if (!string.IsNullOrWhiteSpace(match.Groups[3].Value) &&
                    (!TShock.Utils.TryParseTime(match.Groups[3].Value, out seconds) || seconds <= 0 ||
                     seconds > int.MaxValue / 1000))
                {
                    e.Player.SendErrorMessage(GetString("无效的时间 '{0}'！"), match.Groups[3].Value);
                    return;
                }

                var playerName = string.IsNullOrWhiteSpace(match.Groups[2].Value)
                    ? match.Groups[1].Value
                    : match.Groups[2].Value;
                var players = TShock.Players.FindPlayers(playerName);
                if (players.Count == 0)
                {
                    var user = TShock.UserAccounts.GetUserAccountByName(playerName);
                    if (user == null)
                    {
                        e.Player.SendErrorMessage(GetString("无效的玩家或账户 '{0}'！"), playerName);
                    }
                    else
                    {
                        if (TShock.Groups.GetGroupByName(user.Group).GetDynamicPermission(Permissions.Mute) >=
                            e.Player.Group.GetDynamicPermission(Permissions.Mute))
                        {
                            e.Player.SendErrorMessage(GetString("您不能禁言 {0}！"), user.Name);
                            return;
                        }

                        if (EssentialsPlus.Mutes.AddMute(user, DateTime.UtcNow.AddSeconds(seconds)))
                        {
                            TSPlayer.All.SendInfoMessage(GetString("{0} 禁言了 {1}。"), e.Player.Name, user.Name);
                        }
                        else
                        {
                            e.Player.SendErrorMessage(GetString("无法禁言，请查看日志获取更多信息。"));
                        }
                    }
                }
                else if (players.Count > 1)
                {
                    e.Player.SendErrorMessage(GetString("匹配到多个玩家：{0}"), string.Join(", ", players.Select(p => p.Name)));
                }
                else
                {
                    if (players[0].Group.GetDynamicPermission(Permissions.Mute) >=
                        e.Player.Group.GetDynamicPermission(Permissions.Mute))
                    {
                        e.Player.SendErrorMessage(GetString("您不能禁言 {0}！"), players[0].Name);
                        return;
                    }

                    if (EssentialsPlus.Mutes.AddMute(players[0], DateTime.UtcNow.AddSeconds(seconds)))
                    {
                        TSPlayer.All.SendInfoMessage(GetString("{0} 禁言了 {1}。"), e.Player.Name, players[0].Name);

                        players[0].mute = true;
                        try
                        {
                            await Task.Delay(TimeSpan.FromSeconds(seconds), players[0].GetPlayerInfo().MuteToken);
                            players[0].mute = false;
                            players[0].SendInfoMessage(GetString("您已解除禁言。"));
                        }
                        catch (TaskCanceledException)
                        {
                        }
                    }
                    else
                    {
                        e.Player.SendErrorMessage(GetString("无法禁言，请查看日志获取更多信息。"));
                    }
                }
            }
            return;

            #endregion

            #region 解除禁言

            case "del":
            case "delete":
            {
                var regex = new Regex(@"^\w+ \w+ (?:""(.+?)""|([^\s]*?))$");
                var match = regex.Match(e.Message);
                if (!match.Success)
                {
                    e.Player.SendErrorMessage(GetString("无效的语法！正确的语法是：/mute del <名称>"));
                    return;
                }

                var playerName = string.IsNullOrWhiteSpace(match.Groups[2].Value)
                    ? match.Groups[1].Value
                    : match.Groups[2].Value;
                var players = TShock.Players.FindPlayers(playerName);
                if (players.Count == 0)
                {
                    var user = TShock.UserAccounts.GetUserAccountByName(playerName);
                    if (user == null)
                    {
                        e.Player.SendErrorMessage(GetString("无效的玩家或账户 '{0}'！"), playerName);
                    }
                    else
                    {
                        if (EssentialsPlus.Mutes.DeleteMute(user))
                        {
                            TSPlayer.All.SendInfoMessage(GetString("{0} 解除了 {1} 的禁言。"), e.Player.Name, user.Name);
                        }
                        else
                        {
                            e.Player.SendErrorMessage(GetString("无法解除禁言，请查看日志获取更多信息。"));
                        }
                    }
                }
                else if (players.Count > 1)
                {
                    e.Player.SendErrorMessage(GetString("匹配到多个玩家：{0}"), string.Join(", ", players.Select(p => p.Name)));
                }
                else
                {
                    if (EssentialsPlus.Mutes.DeleteMute(players[0]))
                    {
                        players[0].mute = false;
                        TSPlayer.All.SendInfoMessage(GetString("{0} 解除了 {1} 的禁言。"), e.Player.Name, players[0].Name);
                    }
                    else
                    {
                        e.Player.SendErrorMessage(GetString("无法解除禁言，请查看日志获取更多信息。"));
                    }
                }
            }
            return;

            #endregion

            #region 帮助

            default:
                e.Player.SendSuccessMessage(GetString("禁言子命令:"));
                e.Player.SendInfoMessage(GetString("add <名称> [时间] - 禁言玩家或账户。"));
                e.Player.SendInfoMessage(GetString("del <名称> - 解除禁言玩家或账户。"));
                return;

                #endregion
        }
    }

    public static void TpAllow(CommandArgs e)
    {
        try
        {
            if (EssentialsPlus.TpAllows.ToggleTpAllow(e.Player))
            {
                var isEnabled = EssentialsPlus.TpAllows.IsTpAllowed(e.Player);
                var status = isEnabled ? GetString("允许") : GetString("禁止");
                e.Player.SendSuccessMessage(GetString("您的传送状态已切换为：{0}"), status);
            }
            else
            {
                e.Player.SendErrorMessage(GetString("无法切换传送状态，请查看日志获取更多信息。"));
            }
        }
        catch (Exception ex)
        {
            TShock.Log.Error($"TpAllow command error for {e.Player.Name}: {ex}");
            e.Player.SendErrorMessage(GetString("命令执行失败，请稍后重试。"));
        }
    }

    public static void PvP(CommandArgs e)
    {
        e.TPlayer.hostile = !e.TPlayer.hostile;
        var hostile = Language.GetTextValue(e.TPlayer.hostile ? "LegacyMultiplayer.11" : "LegacyMultiplayer.12", e.Player.Name);
        TSPlayer.All.SendData(PacketTypes.TogglePvp, "", e.Player.Index);
        TSPlayer.All.SendMessage(hostile, Main.teamColor[e.Player.Team]);
    }

    public static void Ruler(CommandArgs e)
    {
        if (e.Parameters.Count == 0)
        {
            if (e.Player.TempPoints.Any(p => p == Point.Zero))
            {
                e.Player.SendErrorMessage(GetString("尺规未设置！"));
                return;
            }

            var p1 = e.Player.TempPoints[0];
            var p2 = e.Player.TempPoints[1];
            var x = Math.Abs(p1.X - p2.X);
            var y = Math.Abs(p1.Y - p2.Y);
            var cartesian = Math.Sqrt((x * x) + (y * y));
            e.Player.SendInfoMessage(GetString("距离：X轴：{0}，Y轴：{1}，直角距离：{2:N3}"), x, y, cartesian);
        }
        else if (e.Parameters.Count == 1)
        {
            if (e.Parameters[0] == "1")
            {
                e.Player.AwaitingTempPoint = 1;
                e.Player.SendInfoMessage(GetString("修改一个方块以设置第一个尺规点。"));
            }
            else if (e.Parameters[0] == "2")
            {
                e.Player.AwaitingTempPoint = 2;
                e.Player.SendInfoMessage(GetString("修改一个方块以设置第二个尺规点。"));
            }
            else
            {
                e.Player.SendErrorMessage(GetString("无效的语法！正确的语法是：{0}ruler [1/2]"), TShock.Config.Settings.CommandSpecifier);
            }
        }
        else
        {
            e.Player.SendErrorMessage(GetString("无效的语法！正确的语法是：{0}ruler [1/2]"), TShock.Config.Settings.CommandSpecifier);
        }
    }


    public static void Send(CommandArgs e)
    {
        var regex = new Regex(@"^\w+(?: (\d+),(\d+),(\d+))? (.+)$");
        var match = regex.Match(e.Message);
        if (!match.Success)
        {
            e.Player.SendErrorMessage(GetString("无效的语法！正确的语法是：{0}send [r,g,b] <文本...>"), TShock.Config.Settings.CommandSpecifier);
            return;
        }

        var r = e.Player.Group.R;
        var g = e.Player.Group.G;
        var b = e.Player.Group.B;
        if (!string.IsNullOrWhiteSpace(match.Groups[1].Value) && !string.IsNullOrWhiteSpace(match.Groups[2].Value) && !string.IsNullOrWhiteSpace(match.Groups[3].Value) &&
            (!byte.TryParse(match.Groups[1].Value, out r) || !byte.TryParse(match.Groups[2].Value, out g) || !byte.TryParse(match.Groups[3].Value, out b)))
        {
            e.Player.SendErrorMessage(GetString("无效的颜色！"));
            return;
        }
        TSPlayer.All.SendMessage(match.Groups[4].Value, new Color(r, g, b));
    }


    public static async void Sudo(CommandArgs e)
    {
        var regex = new Regex(string.Format(@"^\w+(?: -(\w+))* (?:""(.+?)""|([^\s]*?)) (?:{0})?(.+)$", TShock.Config.Settings.CommandSpecifier));
        var match = regex.Match(e.Message);
        if (!match.Success)
        {
            e.Player.SendErrorMessage(GetString("无效的语法！正确的语法是：{0}sudo [-switches...] <玩家> <命令...>"), TShock.Config.Settings.CommandSpecifier);
            e.Player.SendSuccessMessage(GetString("有效的 {0}sudo 参数："), TShock.Config.Settings.CommandSpecifier);
            e.Player.SendInfoMessage(GetString("-f, -force: 强制sudo，忽略权限限制。"));
            return;
        }

        var force = false;
        foreach (Capture capture in match.Groups[1].Captures)
        {
            switch (capture.Value.ToLowerInvariant())
            {
                case "f":
                case "force":
                    if (!e.Player.Group.HasPermission(Permissions.SudoForce))
                    {
                        e.Player.SendErrorMessage(GetString("您没有访问 '-{0}' 参数的权限！"), capture.Value);
                        return;
                    }
                    force = true;
                    continue;
                default:
                    e.Player.SendSuccessMessage(GetString("有效的 {0}sudo 参数："), TShock.Config.Settings.CommandSpecifier);
                    e.Player.SendInfoMessage(GetString("-f, -force: 强制sudo，忽略权限限制。"));
                    return;
            }
        }

        var playerName = string.IsNullOrWhiteSpace(match.Groups[3].Value) ? match.Groups[2].Value : match.Groups[3].Value;
        var command = match.Groups[4].Value;

        var players = TShock.Players.FindPlayers(playerName);
        if (players.Count == 0)
        {
            e.Player.SendErrorMessage(GetString("无效的玩家 '{0}'！"), playerName);
        }
        else if (players.Count > 1)
        {
            e.Player.SendErrorMessage(GetString("匹配到多个玩家：{0}"), string.Join(", ", players.Select(p => p.Name)));
        }
        else
        {
            if ((e.Player.Group.GetDynamicPermission(Permissions.Sudo) <= players[0].Group.GetDynamicPermission(Permissions.Sudo))
                && !e.Player.Group.HasPermission(Permissions.SudoSuper))
            {
                e.Player.SendErrorMessage(GetString("您无法强制 {0} 执行 {1}{2}！"), players[0].Name, TShock.Config.Settings.CommandSpecifier, command);
                return;
            }

            e.Player.SendSuccessMessage(GetString("强制 {0} 执行 {1}{2}。"), players[0].Name, TShock.Config.Settings.CommandSpecifier, command);
            if (!e.Player.Group.HasPermission(Permissions.SudoInvisible))
            {
                players[0].SendInfoMessage(GetString("{0} 强制您执行 {1}{2}。"), e.Player.Name, TShock.Config.Settings.CommandSpecifier, command);
            }

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
        var regex = new Regex(string.Format(@"^\w+(?: -(\w+))* (\w+) (?:{0})?(.+)$", TShock.Config.Settings.CommandSpecifier));
        var match = regex.Match(e.Message);
        if (!match.Success)
        {
            e.Player.SendErrorMessage(GetString("无效的语法！正确的语法是：{0}timecmd [-switches...] <时间> <命令...>"), TShock.Config.Settings.CommandSpecifier);
            e.Player.SendSuccessMessage(GetString("有效的 {0}timecmd 参数:"), TShock.Config.Settings.CommandSpecifier);
            e.Player.SendInfoMessage(GetString("-r, -repeat: 重复执行时间命令。"));
            return;
        }

        var repeat = false;
        foreach (Capture capture in match.Groups[1].Captures)
        {
            switch (capture.Value.ToLowerInvariant())
            {
                case "r":
                case "repeat":
                    repeat = true;
                    break;
                default:
                    e.Player.SendSuccessMessage(GetString("有效的 {0}timecmd 参数:"), TShock.Config.Settings.CommandSpecifier);
                    e.Player.SendInfoMessage(GetString("-r, -repeat: 重复执行时间命令。"));
                    return;
            }
        }

        if (!TShock.Utils.TryParseTime(match.Groups[2].Value, out int seconds) || seconds <= 0 || seconds > int.MaxValue / 1000)
        {
            e.Player.SendErrorMessage(GetString("无效的时间 '{0}'!"), match.Groups[2].Value);
            return;
        }

        if (repeat)
        {
            e.Player.SendSuccessMessage(GetString("已排队执行命令 '{0}{1}'。使用 /cancel 取消！"), TShock.Config.Settings.CommandSpecifier, match.Groups[3].Value);
        }
        else
        {
            e.Player.SendSuccessMessage(GetString("已排队执行命令 '{0}{1}'。使用 /cancel 取消！"), TShock.Config.Settings.CommandSpecifier, match.Groups[3].Value);
        }

        e.Player.AddResponse("cancel", o =>
        {
            e.Player.GetPlayerInfo().CancelTimeCmd();
            e.Player.SendSuccessMessage(GetString("取消所有时间命令！"));
        });

        var token = e.Player.GetPlayerInfo().TimeCmdToken;
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
            e.Player.SendErrorMessage(GetString("无效的语法！正确的语法是：{0}eback [步数]"), TShock.Config.Settings.CommandSpecifier);
            return;
        }

        var steps = 1;
        if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out steps) || steps <= 0))
        {
            e.Player.SendErrorMessage(GetString("无效的步数 '{0}'！"), e.Parameters[0]);
            return;
        }

        var info = e.Player.GetPlayerInfo();
        if (info.BackHistoryCount == 0)
        {
            e.Player.SendErrorMessage(GetString("无法传送回上一个位置！"));
            return;
        }

        steps = Math.Min(steps, info.BackHistoryCount);
        e.Player.SendSuccessMessage(GetString("传送回 {0} 步."), steps);
        var vector = info.PopBackHistory(steps);
        e.Player.Teleport(vector.X, vector.Y);
    }

    public static async void Down(CommandArgs e)
    {
        if (e.Parameters.Count > 1)
        {
            e.Player.SendErrorMessage(GetString("无效的语法！正确的语法是：{0}down [层数]"), TShock.Config.Settings.CommandSpecifier);
            return;
        }

        var levels = 1;
        if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out levels) || levels <= 0))
        {
            e.Player.SendErrorMessage(GetString("无效的层数 '{0}'！"), levels);
            return;
        }

        var currentLevel = 0;
        var empty = false;
        var x = Math.Max(0, Math.Min(e.Player.TileX, Main.maxTilesX - 2));
        var y = Math.Max(0, Math.Min(e.Player.TileY + 3, Main.maxTilesY - 3));

        await Task.Run(() =>
        {
            for (var j = y; currentLevel < levels && j < Main.maxTilesY - 2; j++)
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
        {
            e.Player.SendErrorMessage(GetString("无法传送下去！"));
        }
        else
        {
            if (e.Player.Group.HasPermission(Permissions.TpBack))
            {
                e.Player.GetPlayerInfo().PushBackHistory(e.TPlayer.position);
            }

            e.Player.Teleport(16 * x, (16 * y) - 10);
            e.Player.SendSuccessMessage(GetString("传送下 {0} 层."), currentLevel);
        }
    }

    public static async void Left(CommandArgs e)
    {
        if (e.Parameters.Count > 1)
        {
            e.Player.SendErrorMessage(GetString("无效的语法！正确的语法是：{0}left [层数]"), TShock.Config.Settings.CommandSpecifier);
            return;
        }

        var levels = 1;
        if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out levels) || levels <= 0))
        {
            e.Player.SendErrorMessage(GetString("无效的层数 '{0}'！"), levels);
            return;
        }

        var currentLevel = 0;
        var solid = false;
        var x = Math.Max(0, Math.Min(e.Player.TileX, Main.maxTilesX - 2));
        var y = Math.Max(0, Math.Min(e.Player.TileY, Main.maxTilesY - 3));

        await Task.Run(() =>
        {
            for (var i = x; currentLevel < levels && i >= 0; i--)
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
                {
                    solid = true;
                }
            }
        });

        if (currentLevel == 0)
        {
            e.Player.SendErrorMessage(GetString("无法传送左边！"));
        }
        else
        {
            if (e.Player.Group.HasPermission(Permissions.TpBack))
            {
                e.Player.GetPlayerInfo().PushBackHistory(e.TPlayer.position);
            }

            e.Player.Teleport((16 * x) + 12, 16 * y);
            e.Player.SendSuccessMessage(GetString("传送左 {0} 层."), currentLevel);
        }
    }

    public static async void Right(CommandArgs e)
    {
        if (e.Parameters.Count > 1)
        {
            e.Player.SendErrorMessage(GetString("无效的语法！正确的语法是：{0}right [层数]"), TShock.Config.Settings.CommandSpecifier);
            return;
        }

        var levels = 1;
        if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out levels) || levels <= 0))
        {
            e.Player.SendErrorMessage(GetString("无效的层数 '{0}'！"), levels);
            return;
        }

        var currentLevel = 0;
        var solid = false;
        var x = Math.Max(0, Math.Min(e.Player.TileX + 1, Main.maxTilesX - 2));
        var y = Math.Max(0, Math.Min(e.Player.TileY, Main.maxTilesY - 3));

        await Task.Run(() =>
        {
            for (var i = x; currentLevel < levels && i < Main.maxTilesX - 1; i++)
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
                {
                    solid = true;
                }
            }
        });

        if (currentLevel == 0)
        {
            e.Player.SendErrorMessage(GetString("无法传送右边！"));
        }
        else
        {
            if (e.Player.Group.HasPermission(Permissions.TpBack))
            {
                e.Player.GetPlayerInfo().PushBackHistory(e.TPlayer.position);
            }

            e.Player.Teleport(16 * x, 16 * y);
            e.Player.SendSuccessMessage(GetString("传送右 {0} 层."), currentLevel);
        }
    }

    public static async void Up(CommandArgs e)
    {
        if (e.Parameters.Count > 1)
        {
            e.Player.SendErrorMessage(GetString("无效的语法！正确的语法是：{0}up [层数]"), TShock.Config.Settings.CommandSpecifier);
            return;
        }

        var levels = 1;
        if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out levels) || levels <= 0))
        {
            e.Player.SendErrorMessage(GetString("无效的层数 '{0}'！"), levels);
            return;
        }

        var currentLevel = 0;
        var solid = false;
        var x = Math.Max(0, Math.Min(e.Player.TileX, Main.maxTilesX - 2));
        var y = Math.Max(0, Math.Min(e.Player.TileY, Main.maxTilesY - 3));

        await Task.Run(() =>
        {
            for (var j = y; currentLevel < levels && j >= 0; j--)
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
                {
                    solid = true;
                }
            }
        });

        if (currentLevel == 0)
        {
            e.Player.SendErrorMessage(GetString("无法传送上面！"));
        }
        else
        {
            if (e.Player.Group.HasPermission(Permissions.TpBack))
            {
                e.Player.GetPlayerInfo().PushBackHistory(e.TPlayer.position);
            }

            e.Player.Teleport(16 * x, (16 * y) + 6);
            e.Player.SendSuccessMessage(GetString("传送上 {0} 层."), currentLevel);
        }
    }

}