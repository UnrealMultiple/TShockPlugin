using Microsoft.Xna.Framework;
using MiniGamesAPI;
using System.ComponentModel;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Configuration;
using TShockAPI.Models.PlayerUpdate;
using static TShockAPI.GetDataHandlers;

namespace MainPlugin
{
    [ApiVersion(2, 1)]
    public class MainPlugin : TerrariaPlugin
    {
        public override string Name => "BuildMaster";

        public override Version Version => new Version(1, 0, 2);

        public override string Author => "豆沙 羽学，肝帝熙恩适配";

        public override string Description => "A minigame that is named BuildMaster";

        public MainPlugin(Main game)
            : base(game)
        {
        }
        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("bm.user", BM, "bm", "建筑大师"));
            Commands.ChatCommands.Add(new Command("bm.admin", BMA, "bma", "建筑大师管理"));
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnJoin);
            ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
            ServerApi.Hooks.ServerChat.Register(this, OnChat);
            GetDataHandlers.TogglePvp += OnTogglePVP;
            GetDataHandlers.TileEdit += OnTileEdit;
            GetDataHandlers.PlayerUpdate += OnPlayerUpdate;
            GetDataHandlers.PlayerTeam += OnTeam;
            GetDataHandlers.LiquidSet += OnSetLiquid;
            GetDataHandlers.PlayerSlot += OnPlayerSlot;
            ConfigUtils.LoadConfig();
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == BM || x.CommandDelegate == BMA);
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnJoin);
                ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
                ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
                GetDataHandlers.TogglePvp -= OnTogglePVP;
                GetDataHandlers.TileEdit -= OnTileEdit;
                GetDataHandlers.PlayerUpdate -= OnPlayerUpdate;
                GetDataHandlers.PlayerTeam -= OnTeam;
                GetDataHandlers.LiquidSet -= OnSetLiquid;
                GetDataHandlers.PlayerSlot -= OnPlayerSlot;
            }

            base.Dispose(disposing);
        }

        private void OnChat(ServerChatEventArgs args)
        {
            BuildPlayer playerByName = ConfigUtils.GetPlayerByName(TShock.Players[args.Who].Name);
            BuildRoom roomByID = ConfigUtils.GetRoomByID(playerByName.CurrentRoomID);
            if (!args.Text.StartsWith(((ConfigFile<TShockSettings>)(object)TShock.Config).Settings.CommandSilentSpecifier) && !args.Text.StartsWith(((ConfigFile<TShockSettings>)(object)TShock.Config).Settings.CommandSpecifier) && playerByName != null && roomByID != null)
            {
                roomByID.Broadcast("[房内聊天]" + playerByName.Name + ":" + args.Text, Color.DodgerBlue);
                ((HandledEventArgs)(object)args).Handled = true;
            }
        }

        private void OnTeam(object sender, PlayerTeamEventArgs args)
        {
            BuildPlayer playerByName = ConfigUtils.GetPlayerByName(args.Player.Name);
            BuildRoom buildRoom = null;
            if (playerByName != null)
            {
                buildRoom = ConfigUtils.GetRoomByID(playerByName.CurrentRoomID);
                if (buildRoom != null && buildRoom.Status != 0)
                {
                    playerByName.SetTeam(0);
                    playerByName.SendInfoMessage("当前状态不能切换队伍");
                    ((HandledEventArgs)(object)args).Handled = true;
                }
            }
        }

        private void OnPlayerSlot(object sender, PlayerSlotEventArgs args)
        {
            BuildPlayer playerByName = ConfigUtils.GetPlayerByName(args.Player.Name);
            _ = args.Player;
            BuildRoom buildRoom = null;
            if (playerByName != null)
            {
                buildRoom = ConfigUtils.GetRoomByID(playerByName.CurrentRoomID);
                if (buildRoom != null && (int)buildRoom.Status == 1)
                {
                    ConfigUtils.evaluatePack.RestoreCharacter((MiniPlayer)(object)playerByName);
                }
            }
        }

        private void OnTogglePVP(object sender, TogglePvpEventArgs args)
        {
            BuildPlayer playerByName = ConfigUtils.GetPlayerByName(args.Player.Name);
            BuildRoom buildRoom = null;
            if (playerByName != null)
            {
                buildRoom = ConfigUtils.GetRoomByID(playerByName.CurrentRoomID);
                if (buildRoom != null && buildRoom.Status != 0)
                {
                    playerByName.SetPVP(false);
                    playerByName.SendInfoMessage("当前状态不能开PVP");
                    ((HandledEventArgs)(object)args).Handled = true;
                }
            }
        }

        private void OnSetLiquid(object sender, LiquidSetEventArgs args)
        {
            BuildPlayer playerByName = ConfigUtils.GetPlayerByName(args.Player.Name);
            BuildRoom buildRoom = null;
            if (playerByName == null)
            {
                return;
            }
            buildRoom = ConfigUtils.GetRoomByID(playerByName.CurrentRoomID);
            if (buildRoom != null && playerByName.CurrentRegion != null && (int)buildRoom.Status == 2)
            {
                Rectangle area = playerByName.CurrentRegion.Area;

                // 去掉 ref 关键字和类型转换，直接使用 area.Contains(args.TileX, args.TileY)
                if (!area.Contains(args.TileX, args.TileY))
                {
                    NetMessage.sendWater(args.TileX, args.TileY);
                    ((HandledEventArgs)(object)args).Handled = true;
                    playerByName.SendInfoMessage("你不能在别人的区域内恶意倒液体");
                }
            }
            if (buildRoom != null && (int)buildRoom.Status == 1)
            {
                NetMessage.sendWater(args.TileX, args.TileY);
                ((HandledEventArgs)(object)args).Handled = true;
                playerByName.SendInfoMessage("评选阶段不允许倒液体");
            }
        }

        private void OnTileEdit(object sender, TileEditEventArgs args)
        {
            BuildPlayer playerByName = ConfigUtils.GetPlayerByName(args.Player.Name);
            BuildRoom buildRoom = null;
            if (playerByName == null)
            {
                return;
            }
            buildRoom = ConfigUtils.GetRoomByID(playerByName.CurrentRoomID);
            if (buildRoom != null && playerByName.CurrentRegion != null && (int)buildRoom.Status == 2)
            {
                if (args.X == playerByName.CurrentRegion.TopLeft.X || args.Y == playerByName.CurrentRegion.TopLeft.Y || args.X == playerByName.CurrentRegion.BottomRight.X || args.Y == playerByName.CurrentRegion.BottomRight.Y)
                {
                    TSPlayer.All.SendTileRect((short)args.X, (short)args.Y, 1, 1, 0);
                    playerByName.SendInfoMessage("不可以破坏边框");
                    ((HandledEventArgs)(object)args).Handled = true;
                }
                Rectangle area = playerByName.CurrentRegion.Area;
                if (!area.Contains(args.X, args.Y))
                {
                    TSPlayer.All.SendTileRect((short)args.X, (short)args.Y, 1, 1, 0);
                    playerByName.SendInfoMessage("这不是你区域内的物块哦");
                    ((HandledEventArgs)(object)args).Handled = true;
                }
            }
            if (buildRoom != null && (int)buildRoom.Status == 1)
            {
                ((HandledEventArgs)(object)args).Handled = true;
                TSPlayer.All.SendTileRect((short)args.X, (short)args.Y, 3, 3, 0);
                playerByName.SendInfoMessage("评选阶段不允许建造");
            }
        }

        private void OnPlayerUpdate(object sender, PlayerUpdateEventArgs args)
        {
            BuildPlayer playerByName = ConfigUtils.GetPlayerByName(args.Player.Name);
            TSPlayer player = args.Player;
            BuildRoom buildRoom = null;
            if (playerByName == null)
            {
                return;
            }
            if (buildRoom is not null && (int)buildRoom.Status == 2 && playerByName.CurrentRegion is not null)
            {
                var tileX = args.Player.TileX;
                var tileY = args.Player.TileY;

                var checkPoints = new[] { new Point(tileX, tileY), new Point(tileX, tileY + 1), new Point(tileX, tileY + 2) };

                foreach (var point in checkPoints)
                {
                    if (!playerByName.CurrentRegion.Area.Contains(point))
                    {
                        if (!playerByName.Locked)
                        {
                            playerByName.Teleport(playerByName.CurrentRegion.Center);
                            playerByName.SendInfoMessage("不可擅自离开建筑区域");
                            break;
                        }
                    }
                }
                if (buildRoom == null || (int)buildRoom.Status != 1)
                {
                    return;
                }
                ControlSet control = args.Control;
                if (control.IsUsingItem && args.Player.TPlayer.HeldItem.netID == 75)
                {
                    if (!playerByName.Marked)
                    {
                        int num = player.TPlayer.selectedItem + 1;
                        BuildPlayer buildPlayer = buildRoom.Players[buildRoom.PlayerIndex];

                        if (buildPlayer.Name == playerByName.Name)
                        {
                            playerByName.SendInfoMessage("你不能给自己评分");
                            return;
                        }

                        if (playerByName.GiveMarks == 0)
                        {
                            playerByName.SendInfoMessage($"已给 {buildPlayer.Name} 的建筑评分:{num}分");
                        }
                        else
                        {
                            playerByName.SendInfoMessage($"已给 {buildPlayer.Name} 的建筑更改评分:{num}分");
                        }

                        playerByName.GiveMarks = num;
                    }
                    else
                    {
                        playerByName.SendInfoMessage("你已经评分过了");
                    }
                }
                else if (ConfigUtils.config.BanItem.Contains(args.Player.TPlayer.HeldItem.netID))
                {
                    playerByName.SetBuff(156, 600, false);
                }
            }
        }


        private void BMA(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendInfoMessage("请输入 /bma help 查看帮助");
                return;
            }
            StringBuilder stringBuilder = new StringBuilder();
            BuildRoom buildRoom = null;
            int result;
            int result2;
            switch (args.Parameters[0])
            {
                case "help":
                    stringBuilder.AppendLine("/bma list 列出所有房间");
                    stringBuilder.AppendLine("/bma create [房间名] 创建房间");
                    stringBuilder.AppendLine("/bma remove [房间ID] 移除指定房间");
                    stringBuilder.AppendLine("/bma start [房间ID] 开启指定房间");
                    stringBuilder.AppendLine("/bma stop [房间ID] 关闭指定房间");
                    stringBuilder.AppendLine("/bma smp [房间ID] [人数] 设置房间最大玩家数");
                    stringBuilder.AppendLine("/bma sdp [房间ID] [人数] 设置房间最小玩家数");
                    stringBuilder.AppendLine("/bma swt [房间ID] [时间] 设置等待时间(单位：秒)");
                    stringBuilder.AppendLine("/bma sgt [房间ID] [时间] 设置游戏时间(单位：秒)");
                    stringBuilder.AppendLine("/bma sst [房间ID] [时间] 设置评分时间(单位：秒)");
                    stringBuilder.AppendLine("/bma sp [1/2] 选取点1/2");
                    stringBuilder.AppendLine("/bma sr [房间ID] 设置房间的游戏区域");
                    stringBuilder.AppendLine("/bma addt [房间ID] [主题名] 添加主题");
                    stringBuilder.AppendLine("/bma sh [房间ID] [高] 设置小区域高");
                    stringBuilder.AppendLine("/bma sw [房间ID] [宽] 设置小区域宽");
                    stringBuilder.AppendLine("/bma sg [房间ID] [间隔] 设置小区域间隔");
                    stringBuilder.AppendLine("/bma dp [玩家名字] 设置基础建造背包");
                    stringBuilder.AppendLine("/bma ep 设置评分套装");
                    stringBuilder.AppendLine("/bma reload 重载配置文件非房间文件");
                    args.Player.SendMessage(stringBuilder.ToString(), Color.DarkTurquoise);
                    break;
                case "list":
                    foreach (BuildRoom room in ConfigUtils.rooms)
                    {
                        stringBuilder.AppendLine($"[{room.ID}][{room.Name}][{room.GetPlayerCount()}/{room.MaxPlayer}][{room.Status}]");
                    }
                    args.Player.SendMessage(stringBuilder.ToString(), Color.DarkTurquoise);
                    break;
                case "dp":
                    {
                        if (args.Parameters.Count != 2)
                        {
                            args.Player.SendInfoMessage("正确指令 /bma dp [玩家名]");
                            break;
                        }
                        List<TSPlayer> list = TSPlayer.FindByNameOrID(args.Parameters[1]);
                        TSPlayer val = null;
                        if (list.Count != 0)
                        {
                            val = list[0];
                        }
                        ConfigUtils.defaultPack.CopyFromPlayer(val);
                        ConfigUtils.UpdatePack();
                        args.Player.SendInfoMessage("已设置基础套装");
                        break;
                    }
                case "ep":
                    {
                        ConfigUtils.evaluatePack = ConfigUtils.defaultPack.GetCopyNoItems("评分套", 3);
                        for (int i = 0; i < 10; i++)
                        {
                            ConfigUtils.evaluatePack.Items.Add(new MiniItem(i, 0, 75, i + 1));
                        }
                        ConfigUtils.UpdatePack();
                        args.Player.SendInfoMessage("已设置评分套装");
                        break;
                    }
                case "create":
                    if (args.Parameters.Count != 2)
                    {
                        args.Player.SendInfoMessage("正确指令 /bma create [房间名]");
                        break;
                    }
                    buildRoom = new BuildRoom(args.Parameters[1], ConfigUtils.rooms.Count + 1);
                    ConfigUtils.rooms.Add(buildRoom);
                    ConfigUtils.AddRoom(buildRoom);
                    args.Player.SendInfoMessage($"成功创建房间(id:{buildRoom.ID})");
                    break;
                case "remove":
                    if (args.Parameters.Count != 2)
                    {
                        args.Player.SendInfoMessage("正确指令 /bma remove [房间ID]");
                    }
                    else if (int.TryParse(args.Parameters[1], out result))
                    {
                        buildRoom = ConfigUtils.GetRoomByID(result);
                        if (buildRoom != null)
                        {
                            buildRoom.Dispose();
                            ConfigUtils.rooms.Remove(buildRoom);
                            ConfigUtils.RemoveRoom(buildRoom.ID);
                            args.Player.SendInfoMessage("成功移除房间");
                        }
                        else
                        {
                            args.Player.SendInfoMessage("房间不存在");
                        }
                    }
                    else
                    {
                        args.Player.SendInfoMessage("请输入正确的数字");
                    }
                    break;
                case "start":
                    if (args.Parameters.Count != 2)
                    {
                        args.Player.SendInfoMessage("正确指令 /bma start [房间ID]");
                    }
                    else if (int.TryParse(args.Parameters[1], out result))
                    {
                        buildRoom = ConfigUtils.GetRoomByID(result);
                        if (buildRoom != null)
                        {
                            buildRoom.Restore();
                            buildRoom.Start();
                            args.Player.SendInfoMessage($"成功开启房间(id:{buildRoom.ID})");
                            ConfigUtils.UpdateRooms(result);
                        }
                        else
                        {
                            args.Player.SendInfoMessage("房间不存在");
                        }
                    }
                    else
                    {
                        args.Player.SendInfoMessage("请输入正确的数字");
                    }
                    break;
                case "stop":
                    if (args.Parameters.Count != 2)
                    {
                        args.Player.SendInfoMessage("正确指令 /bma stop [房间ID]");
                    }
                    else if (int.TryParse(args.Parameters[1], out result))
                    {
                        buildRoom = ConfigUtils.GetRoomByID(result);
                        if (buildRoom != null)
                        {
                            buildRoom.Stop();
                            buildRoom.Dispose();
                            args.Player.SendInfoMessage($"成功强制关闭房间(id:{buildRoom.ID})");
                            ConfigUtils.UpdateRooms(result);
                        }
                        else
                        {
                            args.Player.SendInfoMessage("房间不存在");
                        }
                    }
                    else
                    {
                        args.Player.SendInfoMessage("请输入正确的数字");
                    }
                    break;
                case "smp":
                    if (args.Parameters.Count != 3)
                    {
                        args.Player.SendInfoMessage("正确指令 /bma smp [房间ID] [人数]");
                    }
                    else if (int.TryParse(args.Parameters[1], out result) && int.TryParse(args.Parameters[2], out result2))
                    {
                        buildRoom = ConfigUtils.GetRoomByID(result);
                        if (buildRoom != null)
                        {
                            buildRoom.MaxPlayer = result2;
                            args.Player.SendInfoMessage($"成功设置房间(id:{buildRoom.ID}) 的最大玩家数为{result2}");
                            ConfigUtils.UpdateRooms(result);
                        }
                        else
                        {
                            args.Player.SendInfoMessage("房间不存在");
                        }
                    }
                    else
                    {
                        args.Player.SendInfoMessage("请输入正确的数字");
                    }
                    break;
                case "sdp":
                    if (args.Parameters.Count != 3)
                    {
                        args.Player.SendInfoMessage("正确指令 /bma sdp [房间ID] [人数]");
                    }
                    else if (int.TryParse(args.Parameters[1], out result) && int.TryParse(args.Parameters[2], out result2))
                    {
                        buildRoom = ConfigUtils.GetRoomByID(result);
                        if (buildRoom != null)
                        {
                            buildRoom.MinPlayer = result2;
                            args.Player.SendInfoMessage($"成功设置房间(id:{buildRoom.ID}) 的最小玩家数为{result2}");
                            ConfigUtils.UpdateRooms(result);
                        }
                        else
                        {
                            args.Player.SendInfoMessage("房间不存在");
                        }
                    }
                    else
                    {
                        args.Player.SendInfoMessage("请输入正确的数字");
                    }
                    break;
                case "sp":
                    if (args.Parameters.Count != 2)
                    {
                        args.Player.SendInfoMessage("正确指令 /bma sp [1/2]");
                        break;
                    }
                    if (args.Parameters[1] == "1")
                    {
                        args.Player.AwaitingTempPoint = 1;
                        args.Player.SendInfoMessage("请选择点1");
                    }
                    if (args.Parameters[1] == "2")
                    {
                        args.Player.AwaitingTempPoint = 2;
                        args.Player.SendInfoMessage("请选择点2");
                    }
                    break;
                case "swt":
                    if (args.Parameters.Count != 3)
                    {
                        args.Player.SendInfoMessage("正确指令 /bma smp [房间ID] [人数]");
                    }
                    else if (int.TryParse(args.Parameters[1], out result) && int.TryParse(args.Parameters[2], out result2))
                    {
                        buildRoom = ConfigUtils.GetRoomByID(result);
                        if (buildRoom != null)
                        {
                            buildRoom.WaitingTime = result2;
                            args.Player.SendInfoMessage($"成功设置房间(id:{buildRoom.ID}) 的等待时间为{result2}秒");
                            ConfigUtils.UpdateRooms(result);
                        }
                        else
                        {
                            args.Player.SendInfoMessage("房间不存在");
                        }
                    }
                    else
                    {
                        args.Player.SendInfoMessage("请输入正确的数字");
                    }
                    break;
                case "sgt":
                    if (args.Parameters.Count != 3)
                    {
                        args.Player.SendInfoMessage("正确指令 /bma smp [房间ID] [人数]");
                    }
                    else if (int.TryParse(args.Parameters[1], out result) && int.TryParse(args.Parameters[2], out result2))
                    {
                        buildRoom = ConfigUtils.GetRoomByID(result);
                        if (buildRoom != null)
                        {
                            buildRoom.GamingTime = result2;
                            args.Player.SendInfoMessage($"成功设置房间(id:{buildRoom.ID}) 的游戏时间为{result2}秒");
                            ConfigUtils.UpdateRooms(result);
                        }
                        else
                        {
                            args.Player.SendInfoMessage("房间不存在");
                        }
                    }
                    else
                    {
                        args.Player.SendInfoMessage("请输入正确的数字");
                    }
                    break;
                case "sst":
                    if (args.Parameters.Count != 3)
                    {
                        args.Player.SendInfoMessage("正确指令 /bma smp [房间ID] [人数]");
                    }
                    else if (int.TryParse(args.Parameters[1], out result) && int.TryParse(args.Parameters[2], out result2))
                    {
                        buildRoom = ConfigUtils.GetRoomByID(result);
                        if (buildRoom != null)
                        {
                            buildRoom.SeletingTime = result2;
                            args.Player.SendInfoMessage($"成功设置房间(id:{buildRoom.ID}) 的评分时间为{result2}秒");
                            ConfigUtils.UpdateRooms(result);
                        }
                        else
                        {
                            args.Player.SendInfoMessage("房间不存在");
                        }
                    }
                    else
                    {
                        args.Player.SendInfoMessage("请输入正确的数字");
                    }
                    break;
                case "addt":
                    if (args.Parameters.Count != 3)
                    {
                        args.Player.SendInfoMessage("正确指令 /bma addt [房间ID] [主题名]");
                    }
                    else if (int.TryParse(args.Parameters[1], out result))
                    {
                        buildRoom = ConfigUtils.GetRoomByID(result);
                        string text = args.Parameters[2];
                        if (buildRoom != null)
                        {
                            buildRoom.Topics.Add(text, 0);
                            args.Player.SendInfoMessage($"成功添加房间(id:{buildRoom.ID}) 的一个主题 {text}");
                            ConfigUtils.UpdateRooms(result);
                        }
                        else
                        {
                            args.Player.SendInfoMessage("房间不存在");
                        }
                    }
                    else
                    {
                        args.Player.SendInfoMessage("请输入正确的数字");
                    }
                    break;
                case "sr":
                    if (args.Parameters.Count != 2)
                    {
                        args.Player.SendInfoMessage("正确指令 /bma sr [房间ID]");
                    }
                    else if (args.Player.TempPoints[0] == Point.Zero || args.Player.TempPoints[1] == Point.Zero)
                    {
                        args.Player.SendInfoMessage("点未选取完毕");
                    }
                    else if (int.TryParse(args.Parameters[1], out result))
                    {
                        buildRoom = ConfigUtils.GetRoomByID(result);
                        if (buildRoom != null)
                        {
                            buildRoom.TL = args.Player.TempPoints[0];
                            buildRoom.BR = args.Player.TempPoints[1];
                            args.Player.TempPoints[0] = Point.Zero;
                            args.Player.TempPoints[1] = Point.Zero;
                            args.Player.SendInfoMessage($"成功添加房间(id:{buildRoom.ID}) 的游戏区域");
                            ConfigUtils.UpdateRooms(result);
                        }
                        else
                        {
                            args.Player.SendInfoMessage("房间不存在");
                        }
                    }
                    else
                    {
                        args.Player.SendInfoMessage("请输入正确的数字");
                    }
                    break;
                case "swp":
                    if (args.Parameters.Count != 2)
                    {
                        args.Player.SendInfoMessage("正确指令 /bma swp [房间ID]");
                    }
                    else if (int.TryParse(args.Parameters[1], out result))
                    {
                        buildRoom = ConfigUtils.GetRoomByID(result);
                        if (buildRoom != null)
                        {
                            buildRoom.WaitingPoint = new Point(args.Player.TileX, args.Player.TileY - 3);
                            args.Player.SendInfoMessage($"成功设置房间(id:{buildRoom.ID}) 的等待点");
                            ConfigUtils.UpdateRooms(result);
                        }
                        else
                        {
                            args.Player.SendInfoMessage("房间不存在");
                        }
                    }
                    else
                    {
                        args.Player.SendInfoMessage("请输入正确的数字");
                    }
                    break;
                case "sw":
                    if (args.Parameters.Count != 3)
                    {
                        args.Player.SendInfoMessage("正确指令 /bma sw [房间ID] [宽]");
                    }
                    else if (int.TryParse(args.Parameters[1], out result) && int.TryParse(args.Parameters[2], out result2))
                    {
                        buildRoom = ConfigUtils.GetRoomByID(result);
                        if (buildRoom != null)
                        {
                            buildRoom.PerWidth = result2;
                            args.Player.SendInfoMessage($"成功设置房间(id:{buildRoom.ID}) 的区域宽度为{result2}");
                            ConfigUtils.UpdateRooms(result);
                        }
                        else
                        {
                            args.Player.SendInfoMessage("房间不存在");
                        }
                    }
                    else
                    {
                        args.Player.SendInfoMessage("请输入正确的数字");
                    }
                    break;
                case "sh":
                    if (args.Parameters.Count != 3)
                    {
                        args.Player.SendInfoMessage("正确指令 /bma sh [房间ID] [高]");
                    }
                    else if (int.TryParse(args.Parameters[1], out result) && int.TryParse(args.Parameters[2], out result2))
                    {
                        buildRoom = ConfigUtils.GetRoomByID(result);
                        if (buildRoom != null)
                        {
                            buildRoom.PerHeight = result2;
                            args.Player.SendInfoMessage($"成功设置房间(id:{buildRoom.ID}) 的区域高度为{result2}");
                            ConfigUtils.UpdateRooms(result);
                        }
                        else
                        {
                            args.Player.SendInfoMessage("房间不存在");
                        }
                    }
                    else
                    {
                        args.Player.SendInfoMessage("请输入正确的数字");
                    }
                    break;
                case "sg":
                    if (args.Parameters.Count != 3)
                    {
                        args.Player.SendInfoMessage("正确指令 /bma sg [房间ID] [间隔]");
                    }
                    else if (int.TryParse(args.Parameters[1], out result) && int.TryParse(args.Parameters[2], out result2))
                    {
                        buildRoom = ConfigUtils.GetRoomByID(result);
                        if (buildRoom != null)
                        {
                            buildRoom.Gap = result2;
                            args.Player.SendInfoMessage($"成功设置房间(id:{buildRoom.ID}) 的区域间隔为{result2}");
                            ConfigUtils.UpdateRooms(result);
                        }
                        else
                        {
                            args.Player.SendInfoMessage("房间不存在");
                        }
                    }
                    else
                    {
                        args.Player.SendInfoMessage("请输入正确的数字");
                    }
                    break;
                case "reload":
                    ConfigUtils.ReloadConfig();
                    args.Player.SendInfoMessage("插件重载成功");
                    break;
                default:
                    args.Player.SendInfoMessage("输入/bma help 查看帮助");
                    break;
            }
        }

        private void BM(CommandArgs args)
        {
            BuildPlayer playerByName = ConfigUtils.GetPlayerByName(args.Player.Name);
            if (playerByName == null)
            {
                args.Player.SendInfoMessage("[BuildMaster] 玩家数据出错,请尝试重新进入服务器");
                return;
            }
            if (args.Parameters.Count < 1)
            {
                playerByName.SendInfoMessage("请输入/bm help 查看指令帮助");
                return;
            }
            StringBuilder stringBuilder = new StringBuilder();
            switch (args.Parameters[0])
            {
                case "help":
                    stringBuilder.AppendLine("/bm list 查看房间列表");
                    stringBuilder.AppendLine("/bm join [房间ID] 加入房间");
                    stringBuilder.AppendLine("/bm leave 离开房间");
                    stringBuilder.AppendLine("/bm ready 准备/未准备");
                    stringBuilder.AppendLine("/bm vote [主题] 投票主题");
                    playerByName.SendSuccessMessage(stringBuilder.ToString());
                    break;
                case "list":
                    foreach (BuildRoom room in ConfigUtils.rooms)
                    {
                        stringBuilder.AppendLine($"[{room.ID}][{room.Name}][{room.GetPlayerCount()}/{room.MaxPlayer}][{room.Status}]");
                    }
                    playerByName.SendSuccessMessage(stringBuilder.ToString());
                    break;
                case "join":
                    {
                        int result;
                        if (args.Parameters.Count != 2)
                        {
                            playerByName.SendInfoMessage("正确指令 /bm join [房间号]");
                        }
                        else if (int.TryParse(args.Parameters[1], out result))
                        {
                            BuildRoom roomByID = ConfigUtils.GetRoomByID(result);
                            if (roomByID != null)
                            {
                                playerByName.Join(roomByID);
                            }
                            else
                            {
                                playerByName.SendInfoMessage("房间不存在");
                            }
                        }
                        else
                        {
                            playerByName.SendInfoMessage("请输入正确的数字");
                        }
                        break;
                    }
                case "leave":
                    playerByName.Leave();
                    break;
                case "ready":
                    {
                        BuildRoom roomByID = ConfigUtils.GetRoomByID(playerByName.CurrentRoomID);
                        if (roomByID != null)
                        {
                            playerByName.Ready();
                            roomByID.Broadcast("玩家 " + playerByName.Name + " " + (playerByName.IsReady ? "已准备" : "未准备"), Color.OrangeRed);
                        }
                        else
                        {
                            playerByName.SendInfoMessage("你不在房间中");
                        }
                        break;
                    }
                case "vote":
                    {
                        if (args.Parameters.Count != 2)
                        {
                            playerByName.SendInfoMessage("正确指令/bm vote 主题名");
                            break;
                        }
                        string text = args.Parameters[1];
                        BuildRoom roomByID = ConfigUtils.GetRoomByID(playerByName.CurrentRoomID);
                        if (roomByID != null)
                        {
                            if (roomByID.Status == 0)
                            {
                                if (roomByID.Topics.ContainsKey(text))
                                {
                                    if (string.IsNullOrEmpty(playerByName.SelectedTopic))
                                    {
                                        roomByID.Topics[text]++;
                                        roomByID.Broadcast("玩家 " + playerByName.Name + " 投票主题 " + text, Color.DarkTurquoise);
                                    }
                                    else
                                    {
                                        roomByID.Topics[playerByName.SelectedTopic]--;
                                        roomByID.Broadcast("玩家 " + playerByName.Name + " 更改投票主题为 " + text, Color.DarkTurquoise);
                                        roomByID.Topics[text]++;
                                    }
                                    playerByName.SelectedTopic = text;
                                }
                                else
                                {
                                    playerByName.SendInfoMessage("该房间不存在此主题");
                                }
                            }
                            else
                            {
                                playerByName.SendInfoMessage("当前房间状态不允许投票选主题");
                            }
                        }
                        else
                        {
                            playerByName.SendInfoMessage("不在房间中");
                        }
                        break;
                    }
                case "tl":
                    {
                        BuildRoom roomByID = ConfigUtils.GetRoomByID(playerByName.CurrentRoomID);
                        if (roomByID != null)
                        {
                            stringBuilder.AppendLine("此房支持的主题");
                            foreach (string key in roomByID.Topics.Keys)
                            {
                                stringBuilder.AppendLine("[" + key + "]");
                            }
                            playerByName.SendSuccessMessage(stringBuilder.ToString());
                        }
                        else
                        {
                            playerByName.SendInfoMessage("不在房间中");
                        }
                        break;
                    }
                default:
                    args.Player.SendInfoMessage("输入/bm help 查看帮助");
                    break;
            }
        }

        private void OnLeave(LeaveEventArgs args)
        {
            {
                TSPlayer tsplr = TShock.Players[args.Who];
                BuildPlayer buildPlayer = ConfigUtils.players.Find((BuildPlayer p) => p.Name == tsplr.Name);
                if (buildPlayer != null)
                {
                    BuildRoom roomByID = ConfigUtils.GetRoomByID(buildPlayer.CurrentRoomID);
                    if (roomByID != null)
                    {
                        roomByID.Players.Remove(buildPlayer);
                        roomByID.Broadcast("玩家 " + buildPlayer.Name + " 强制退出了房间", Color.Crimson);
                    }
                    buildPlayer.CurrentRoomID = 0;
                    buildPlayer.IsReady = false;
                    buildPlayer.Marked = false;
                    buildPlayer.GiveMarks = 0;
                    buildPlayer.AquiredMarks = 0;
                    buildPlayer.SelectedTopic = "";
                    if (buildPlayer.BackUp != null)
                    {
                        buildPlayer.BackUp.RestoreCharacter((MiniPlayer)(object)buildPlayer);
                    }
                    buildPlayer.CurrentRegion = null;
                    buildPlayer.Player = null;
                    buildPlayer.BackUp = null;
                    buildPlayer.Status = 0;
                    buildPlayer.Locked = false;
                }
            }
        }

        private void OnJoin(GreetPlayerEventArgs args)
        {
            TSPlayer tsplr = TShock.Players[args.Who];
            BuildPlayer buildPlayer = ConfigUtils.players.Find((BuildPlayer p) => p.Name == tsplr.Name);
            if (buildPlayer == null)
            {
                buildPlayer = new BuildPlayer(ConfigUtils.players.Count + 1, tsplr.Name, tsplr);
                ConfigUtils.players.Add(buildPlayer);
            }
            else
            {
                buildPlayer.Player = tsplr;
            }
        }
    }
}