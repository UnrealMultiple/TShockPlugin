using Microsoft.Xna.Framework;
using System.Globalization;
using System.IO.Compression;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;
// ReSharper disable InconsistentNaming

namespace ZHIPlayerManager;

public partial class ZHIPM
{
    /// <summary>
    ///     帮助指令方法指令
    /// </summary>
    /// <param name="args"></param>
    private void Help(CommandArgs args)
    {
        if (args.Parameters.Count != 0)
        {
            args.Player.SendInfoMessage(GetString("输入 /zhelp  来查看指令帮助"));
        }
        else
        {
            args.Player.SendMessage(GetString("输入 /zsave 来备份自己的人物存档\n") +
                                    GetString("输入 /zsaveauto <minute> 来每隔 minute 分钟自动备份自己的人物存档，当 minute为0时关闭该功能\n") +
                                    GetString("输入 /zvisa <num> 来查看自己的人物备份\n") +
                                    GetString("输入 /zvisa name <num> 来查看该玩家的第几个人物备份\n") +
                                    GetString("输入 /zhide kill 来取消 kill + 1 的显示，再次使用启用显示\n") +
                                    GetString("输入 /zhide point 来取消 + 1 $ 的显示，再次使用启用显示\n") +
                                    GetString("输入 /zback <name> 来读取该玩家的人物存档\n") +
                                    GetString("输入 /zback <name> <num> 来读取该玩家的第几个人物存档\n") +
                                    GetString("输入 /zclone <name1> <name2> 将玩家1的人物数据复制给玩家2\n") +
                                    GetString("输入 /zclone <name> 将该玩家的人物数据复制给自己\n") +
                                    GetString("输入 /zmodify help 查看修改玩家数据的指令帮助\n") +
                                    GetString("输入 /vi <name> 来查看该玩家的库存\n") +
                                    GetString("输入 /vid <name> 来查看该玩家的库存，不分类\n") +
                                    GetString("输入 /vs <name> 来查看该玩家的状态\n") +
                                    GetString("输入 /vs me 来查看自己的状态\n") +
                                    GetString("输入 /zfre <name> 来冻结该玩家\n") +
                                    GetString("输入 /zunfre <name> 来解冻该玩家\n") +
                                    GetString("输入 /zunfre all 来解冻所有玩家\n") +
                                    GetString("输入 /zsort help 来查看排序系列指令帮助\n") +
                                    GetString("输入 /zout <name> 来导出该玩家的人物存档\n") +
                                    GetString("输入 /zout all 来导出所有人物的存档并压缩打包\n") +
                                    GetString("输入 /zreset help 来查看zreset系列指令帮助\n") +
                                    GetString("输入 /zban add <name> <reason> 来封禁无论是否在线的玩家，reason 可不填\n") +
                                    GetString("输入 /zban add uuid <uuid> <reason> 来封禁uuid\n") +
                                    GetString("输入 /zban add ip <ip> <reason> 来封禁ip\n") +
                                    GetString("输入 /zclear useless 来清理世界的掉落物品，非城镇或BossNPC，和无用射弹\n") +
                                    GetString("输入 /zclear buff <name> 来清理该玩家的所有Buff\n") +
                                    GetString("输入 /zclear buff all 来清理所有玩家所有Buff\n") +
                                    GetString("输入 /zbpos 来返回上次死亡地点\n"),
                TextColor()
            );
        }
    }


    /// <summary>
    ///     回档指令方法指令
    /// </summary>
    /// <param name="args"></param>
    private void MySSCBack(CommandArgs args)
    {
        if (args.Parameters.Count == 1)
        {
            this.MySscBack2(args, 1);
            return;
        }

        if (args.Parameters.Count == 2)
        {
            if (!int.TryParse(args.Parameters[1], out var num))
            {
                args.Player.SendInfoMessage(GetString("输入 /zback <name>  来读取该玩家的最新人物存档\n") +
                                            GetString("输入 /zback <name> <num>  来读取该玩家的第几个人物存档"));
                return;
            }

            if (num < 1 || num > config.MaxBackupsPerPlayer)
            {
                args.Player.SendInfoMessage(GetString($"玩家最多有 {config.MaxBackupsPerPlayer} 个备份存档，范围 1 ~ {config.MaxBackupsPerPlayer}，请重新输入"));
                return;
            }

            this.MySscBack2(args, num);
        }
        else
        {
            args.Player.SendInfoMessage(GetString("输入 /zback <name>  来读取该玩家的最新人物存档\n") +
                                        GetString("输入 /zback <name> <num>  来读取该玩家的第几个人物存档"));
        }
    }


    /// <summary>
    ///     保存指令方法指令
    /// </summary>
    /// <param name="args"></param>
    private void MySSCSave(CommandArgs args)
    {
        if (args.Parameters.Count != 0)
        {
            args.Player.SendInfoMessage(GetString("输入 /zsave  来备份自己的人物存档"));
            return;
        }

        if (!args.Player.RealPlayer)
        {
            args.Player.SendInfoMessage(GetString("对象不正确，请检查您的状态，您是否为游戏内玩家？"));
            return;
        }

        if (ZPDataBase.AddZPlayerDB(args.Player))
        {
            var extraData = edPlayers.Find(x => x.Name == args.Player.Name);
            if (extraData != null)
            {
                ZPExtraDB.WriteExtraDB(extraData);
            }

            args.Player.SendMessage(GetString("您的备份保存成功！"), new Color(0, 255, 0));
        }
        else
        {
            args.Player.SendMessage(GetString("您的备份保存失败！请尝试重进游戏重试"), new Color(255, 0, 0));
        }
    }


    /// <summary>
    ///     自动备份指令
    /// </summary>
    /// <param name="args"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void MySSCSaveAuto(CommandArgs args)
    {
        if (!config.EnablePlayerAutoBackup)
        {
            args.Player.SendMessage(GetString("自动备份被禁用，请联系管理员询问详情"), new Color(255, 0, 0));
            return;
        }

        if (args.Parameters.Count != 1)
        {
            args.Player.SendInfoMessage(GetString("输入 /zsaveauto [minute]  来每隔 minute 分钟自动备份自己的人物存档，当 minute 为 0 时关闭该功能"));
            return;
        }

        if (!args.Player.RealPlayer)
        {
            args.Player.SendInfoMessage(GetString("对象不正确，请检查您的状态，您是否为游戏内玩家？"));
            return;
        }

        if (int.TryParse(args.Parameters[0], out var num))
        {
            if (num < 0)
            {
                args.Player.SendInfoMessage(GetString("数字不合理"));
                return;
            }

            var ex = edPlayers.Find(x => x.Name == args.Player.Name);
            if (ex == null)
            {
                args.Player.SendInfoMessage(GetString("修改失败，请重进服务器重试"));
                return;
            }

            ex.backuptime = num;
            args.Player.SendMessage(num != 0
                ? GetString($"修改成功，你的存档将每隔{num}分钟自动备份一次，请注意存档覆盖情况，这可能会覆盖你手动备份的部分")
                : GetString("修改成功，你的自动备份已关"), new Color(0, 255, 0));
        }
        else
        {
            args.Player.SendInfoMessage(GetString("输入 /zsaveauto [minute]  来每隔 minute 分钟自动备份自己的人物存档，当 minute 为 0 时关闭该功能"));
        }
    }


    /// <summary>
    ///     查看我的存档方法指令
    /// </summary>
    /// <param name="args"></param>
    private void ViewMySSCSave(CommandArgs args)
    {
        //查询本人
        if (args.Parameters.Count == 0 || (args.Parameters.Count == 1 && int.TryParse(args.Parameters[0], out _)))
        {
            if (!args.Player.RealPlayer)
            {
                args.Player.SendInfoMessage(GetString("对象不正确，请检查您的状态，您是否为游戏内玩家？"));
                return;
            }

            int slot;
            if (args.Parameters.Count == 0)
            {
                slot = 1;
            }
            else
            {
                var num = int.Parse(args.Parameters[0]);
                if (num < 1 || num > config.MaxBackupsPerPlayer)
                {
                    args.Player.SendInfoMessage(GetString($"玩家最多有 {config.MaxBackupsPerPlayer} 个备份存档，范围 1 ~ {config.MaxBackupsPerPlayer}，请重新输入"));
                    return;
                }

                slot = num;
            }

            var playerData = ZPDataBase.ReadZPlayerDB(args.Player, args.Player.Account.ID, slot);
            if (!playerData.exists)
            {
                args.Player.SendInfoMessage(GetString("您还未备份"));
            }
            else
            {
                var items = new Item[NetItem.MaxInventory];
                for (var i = 0; i < NetItem.MaxInventory; i++)
                {
                    items[i] = TShock.Utils.GetItemById(playerData.inventory[i].NetId);
                    items[i].stack = playerData.inventory[i].Stack;
                    items[i].prefix = playerData.inventory[i].PrefixId;
                }

                var text = GetItemsString(items, NetItem.MaxInventory);
                text = FormatArrangement(text, 30, " ");
                var str = GetString($"您的备份[{args.Player.Account.ID} - {slot}]的内容为：\n{text}");
                args.Player.SendInfoMessage(str);
            }
        }

        //查询他人
        else if (args.Parameters.Count == 1 || (args.Parameters.Count == 2 && int.TryParse(args.Parameters[1], out _)))
        {
            int slot;
            if (args.Parameters.Count == 1)
            {
                slot = 1;
            }
            else
            {
                var num = int.Parse(args.Parameters[1]);
                if (num < 1 || num > config.MaxBackupsPerPlayer)
                {
                    args.Player.SendInfoMessage(GetString($"玩家最多有 {config.MaxBackupsPerPlayer} 个备份存档，范围 1 ~ {config.MaxBackupsPerPlayer}，请重新输入"));
                    return;
                }

                slot = num;
            }

            int ID;
            string playerfullname;
            var list = this.BestFindPlayerByNameOrIndex(args.Parameters[0]);
            if (list.Count == 0)
            {
                args.Player.SendInfoMessage(this.playerOfflineWarning);
                var users = TShock.UserAccounts.GetUserAccountsByName(args.Parameters[0], true);
                if (users.Count == 1 || (users.Count > 1 && users.Exists(x => x.Name == args.Parameters[0])))
                {
                    if (users.Count == 1)
                    {
                        ID = users[0].ID;
                        playerfullname = users[0].Name;
                    }
                    else
                    {
                        var temp = users.Find(x => x.Name == args.Parameters[0])!;
                        ID = temp.ID;
                        playerfullname = temp.Name;
                    }
                }
                else if (users.Count == 0)
                {
                    args.Player.SendInfoMessage(this.nonExistPlayerWarning);
                    return;
                }
                else
                {
                    args.Player.SendInfoMessage(this.MultipleMatchPlayerWarning);
                    return;
                }
            }
            else
            {
                ID = list[0].Account.ID;
                playerfullname = list[0].Name;
            }

            var playerData = ZPDataBase.ReadZPlayerDB(new TSPlayer(-1), ID, slot);
            if (!playerData.exists)
            {
                args.Player.SendInfoMessage(GetString("该玩家还未备份"));
            }
            else
            {
                var items = new Item[NetItem.MaxInventory];
                for (var i = 0; i < NetItem.MaxInventory; i++)
                {
                    items[i] = TShock.Utils.GetItemById(playerData.inventory[i].NetId);
                    items[i].stack = playerData.inventory[i].Stack;
                    items[i].prefix = playerData.inventory[i].PrefixId;
                }

                string text;
                if (args.Player.RealPlayer)
                {
                    text = GetItemsString(items, NetItem.MaxInventory);
                    text = FormatArrangement(text, 30, " ");
                }
                else
                {
                    text = GetItemsString(items, NetItem.MaxInventory, 1);
                }

                var str = GetString($"玩家[{playerfullname}]备份 [{ID} - {slot}] 的内容为：\n") + text;
                args.Player.SendInfoMessage(str);
            }
        }

        else
        {
            args.Player.SendInfoMessage(GetString("输入 /zvisa [num] 来查看自己的第几个人物备份\n") +
                                        GetString("输入 /zvisa name [num] 来查看该玩家的第几个人物备份"));
        }
    }


    /// <summary>
    ///     克隆另一个人的数据的方法指令
    /// </summary>
    /// <param name="args"></param>
    private void SSCClone(CommandArgs args)
    {
        if (args.Parameters.Count == 0 || args.Parameters.Count > 2)
        {
            args.Player.SendInfoMessage(GetString("输入 /zclone [name1] [name2]  将玩家1的人物数据复制给玩家2\n") +
                                        GetString("输入 /zclone [name]  将该玩家的人物数据复制给自己"));
            return;
        }

        if (args.Parameters.Count == 1)
        {
            if (args.Parameters[0] == args.Player.Name)
            {
                args.Player.SendMessage(GetString("克隆失败！请不要克隆自己"), new Color(255, 0, 0));
                return;
            }

            if (!args.Player.RealPlayer)
            {
                args.Player.SendInfoMessage(GetString("对象不正确，请检查您的状态，您是否为游戏内玩家？"));
                return;
            }

            var list = this.BestFindPlayerByNameOrIndex(args.Parameters[0]);
            //找不到人，查离线
            if (list.Count == 0)
            {
                args.Player.SendInfoMessage(this.playerOfflineWarning);
                var user = TShock.UserAccounts.GetUserAccountByName(args.Parameters[0]);
                var users = TShock.UserAccounts.GetUserAccountsByName(args.Parameters[0], true);
                if (user == null)
                {
                    if (users.Count == 0)
                    {
                        args.Player.SendInfoMessage(this.nonExistPlayerWarning);
                        return;
                    }

                    if (users.Count > 1)
                    {
                        args.Player.SendInfoMessage(this.MultipleMatchPlayerWarning);
                        return;
                    }

                    user = users[0];
                }

                var playerData = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), user.ID);
                if (this.UpdatePlayerAll(args.Player, playerData))
                {
                    args.Player.SendMessage(GetString($"克隆成功！您已将玩家[{user.Name}]的数据克隆到你身上"), new Color(0, 255, 0));
                }
                else
                {
                    args.Player.SendMessage(GetString("克隆失败！未在原数据库中查到该玩家，请检查输入是否正确，该玩家是否避免SSC检测，再重新输入"), new Color(255, 0, 0));
                }
            }
            //人太多，舍弃
            else if (list.Count > 1)
            {
                args.Player.SendInfoMessage(this.MultipleMatchPlayerWarning);
                return;
            }
            //一个在线
            else
            {
                var playerData = list[0].PlayerData;
                playerData.CopyCharacter(list[0]);
                playerData.exists = true;
                if (this.UpdatePlayerAll(args.Player, playerData))
                {
                    args.Player.SendMessage(GetString($"克隆成功！您已将玩家[{list[0].Name}]的数据克隆到你身上"), new Color(0, 255, 0));
                }
                else
                {
                    args.Player.SendMessage(GetString("克隆失败！未在原数据库中查到该玩家，请检查输入是否正确，该玩家是否避免SSC检测，再重新输入"), new Color(255, 0, 0));
                }
            }
        }

        if (args.Parameters.Count == 2)
        {
            var player1 = this.BestFindPlayerByNameOrIndex(args.Parameters[0]);
            var player2 = this.BestFindPlayerByNameOrIndex(args.Parameters[1]);
            if (player1.Count > 1 || player2.Count > 1)
            {
                args.Player.SendInfoMessage(this.MultipleMatchPlayerWarning);
                return;
            }

            switch (player1.Count)
            {
                //都在线的情况
                case 1 when player2.Count == 1:
                {
                    if (player1[0].Name == player2[0].Name)
                    {
                        args.Player.SendInfoMessage(GetString("请不要对同一个人进行克隆"));
                        return;
                    }

                    player1[0].PlayerData.CopyCharacter(player1[0]);
                    player1[0].PlayerData.exists = true;
                    if (this.UpdatePlayerAll(player2[0], player1[0].PlayerData))
                    {
                        if (args.Player.Account.ID != player2[0].Account.ID)
                        {
                            args.Player.SendMessage(GetString($"克隆成功！您已将玩家 [{player1[0].Name}] 的数据克隆到 [{player2[0].Name}] 身上"), new Color(0, 255, 0));
                        }
                        else
                        {
                            player2[0].SendMessage(GetString($"克隆成功！您已将玩家 [{player1[0].Name}] 的数据克隆到你身上"), new Color(0, 255, 0));
                        }
                    }
                    else
                    {
                        args.Player.SendMessage(GetString("克隆失败！未在原数据库中查到该玩家，请检查输入是否正确，该玩家是否避免SSC检测，再重新输入"), new Color(255, 0, 0));
                    }

                    return;
                }
                //赋值者不在线，被赋值者在线的情况
                case 0 when player2.Count == 1:
                {
                    args.Player.SendInfoMessage(GetString("玩家1不在线，正在查询离线数据"));
                    var user1 = TShock.UserAccounts.GetUserAccountByName(args.Parameters[0]);
                    var user1s = TShock.UserAccounts.GetUserAccountsByName(args.Parameters[0], true);
                    if (user1 == null)
                    {
                        if (user1s.Count == 0)
                        {
                            args.Player.SendInfoMessage(GetString("玩家1不存在"));
                            return;
                        }

                        if (user1s.Count > 1)
                        {
                            args.Player.SendInfoMessage(GetString("玩家1不唯一"));
                            return;
                        }

                        user1 = user1s[0];
                    }

                    var playerData1 = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), user1.ID);
                    if (this.UpdatePlayerAll(player2[0], playerData1))
                    {
                        if (args.Player.Account.ID != player2[0].Account.ID)
                        {
                            args.Player.SendMessage(GetString($"克隆成功！您已将玩家 [{user1.Name}] 的数据克隆到玩家 [{player2[0].Name}]身上"), new Color(0, 255, 0));
                        }
                        else
                        {
                            player2[0].SendMessage(GetString($"克隆成功！已将玩家 [{user1.Name}] 的数据克隆到你身上"), new Color(0, 255, 0));
                        }
                    }
                    else
                    {
                        args.Player.SendMessage(GetString("克隆失败！未在原数据库中查到该玩家，请检查输入是否正确，该玩家是否避免SSC检测，再重新输入"), new Color(255, 0, 0));
                    }

                    return;
                }
                //赋值者在线，被赋值者不在线的情况
                case 1 when player2.Count == 0:
                {
                    args.Player.SendInfoMessage(GetString("玩家2不在线，正在查询离线数据"));
                    var user2 = TShock.UserAccounts.GetUserAccountByName(args.Parameters[1]);
                    var user2s = TShock.UserAccounts.GetUserAccountsByName(args.Parameters[1], true);
                    if (user2 == null)
                    {
                        if (user2s.Count == 0)
                        {
                            args.Player.SendInfoMessage(GetString("玩家2不存在"));
                            return;
                        }

                        if (user2s.Count > 1)
                        {
                            args.Player.SendInfoMessage(GetString("玩家2不唯一"));
                            return;
                        }

                        user2 = user2s[0];
                    }

                    var playerData1 = player1[0].PlayerData;
                    playerData1.exists = true;
                    if (this.UpdateTshockDbCharacter(user2.ID, playerData1))
                    {
                        args.Player.SendMessage(GetString($"克隆成功！您已将玩家 [{player1[0].Name}] 的数据克隆到玩家 [{user2.Name}] 身上"), new Color(0, 255, 0));
                    }
                    else
                    {
                        args.Player.SendMessage(GetString("克隆失败！未在原数据库中查到该玩家，请检查输入是否正确，该玩家是否避免SSC检测，再重新输入"), new Color(255, 0, 0));
                    }

                    return;
                }
                //都不在线
                case 0 when player2.Count == 0:
                {
                    args.Player.SendInfoMessage(GetString("玩家都不在线，正在查询离线数据"));
                    var user1 = TShock.UserAccounts.GetUserAccountByName(args.Parameters[0]);
                    var user1s = TShock.UserAccounts.GetUserAccountsByName(args.Parameters[0], true);
                    var user2 = TShock.UserAccounts.GetUserAccountByName(args.Parameters[1]);
                    var user2s = TShock.UserAccounts.GetUserAccountsByName(args.Parameters[1], true);
                    if (user1 == null)
                    {
                        if (user1s.Count == 0)
                        {
                            args.Player.SendInfoMessage(GetString("玩家1不存在"));
                            return;
                        }

                        if (user1s.Count > 1)
                        {
                            args.Player.SendInfoMessage(GetString("玩家1不唯一"));
                            return;
                        }

                        user1 = user1s[0];
                    }

                    if (user2 == null)
                    {
                        if (user2s.Count == 0)
                        {
                            args.Player.SendInfoMessage(GetString("玩家2不存在"));
                            return;
                        }

                        if (user2s.Count > 1)
                        {
                            args.Player.SendInfoMessage(GetString("玩家2不唯一"));
                            return;
                        }

                        user2 = user2s[0];
                    }

                    var playerData = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), user1.ID);
                    if (this.UpdateTshockDbCharacter(user2.ID, playerData))
                    {
                        args.Player.SendMessage(GetString($"克隆成功！您已将玩家 [{user1.Name}] 的数据克隆到玩家 [{user2.Name}] 身上"), new Color(0, 255, 0));
                    }
                    else
                    {
                        args.Player.SendMessage(GetString("克隆失败！未在原数据库中查到该玩家，请检查输入是否正确，该玩家是否避免SSC检测，再重新输入"), new Color(255, 0, 0));
                    }

                    break;
                }
            }
        }
    }


    /// <summary>
    ///     修改人物数据方法指令
    /// </summary>
    /// <param name="args"></param>
    private void SSCModify(CommandArgs args)
    {
        if (!TShock.ServerSideCharacterConfig.Settings.Enabled)
        {
            args.Player.SendErrorMessage(GetString("服务器没有开启ServerSideCharacter,无法使用该命令!"));
            return;
        }
        
        if (args.Parameters.Count != 1 && args.Parameters.Count != 3)
        {
            args.Player.SendInfoMessage(GetString("输入 /zmodify help  查看修改玩家数据的指令帮助"));
            return;
        }

        switch (args.Parameters.Count)
        {
            case 1:
            {
                if (args.Parameters[0].Equals("help", StringComparison.OrdinalIgnoreCase))
                {
                    var temp = config.EnablePointTracking
                        ? GetString("\n输入 /zmodify [name] point [num]  来修改玩家点数")
                        : "";
                    args.Player.SendMessage(
                        GetString("输入 /zmodify [name] life [num] 来修改玩家的血量\n") +
                        GetString("输入 /zmodify [name] lifemax [num] 来修改玩家的血量上限\n") +
                        GetString("输入 /zmodify [name] mana [num] 来修改玩家的魔力\n") +
                        GetString("输入 /zmodify [name] manamax [num] 来修改玩家的魔力上限\n") +
                        GetString("输入 /zmodify [name] fish [num] 来修改玩家的渔夫任务数\n") +
                        GetString("输入 /zmodify [name] torch [0或1] 来关闭或开启火把神增益\n") +
                        GetString("输入 /zmodify [name] demmon [0或1] 来关闭或开启恶魔心增益\n") +
                        GetString("输入 /zmodify [name] bread [0或1] 来关闭或开启工匠面包增益\n") +
                        GetString("输入 /zmodify [name] heart [0或1] 来关闭或开启埃癸斯水晶增益\n") +
                        GetString("输入 /zmodify [name] fruit [0或1] 来关闭或开启埃癸斯果增益\n") +
                        GetString("输入 /zmodify [name] pearl [0或1] 来关闭或开启银河珍珠增益\n") +
                        GetString("输入 /zmodify [name] worm [0或1] 来关闭或开启粘性蠕虫增益\n") +
                        GetString("输入 /zmodify [name] ambrosia [0或1] 来关闭或开启珍馐增益\n") +
                        GetString("输入 /zmodify [name] cart [0或1] 来关闭或开启超级矿车增益\n") +
                        GetString($"输入 /zmodify [name] all [0或1] 来关闭或开启所有玩家增益{temp}"), TextColor());
                }
                else
                {
                    args.Player.SendInfoMessage(GetString("输入 /zmodify help  查看修改玩家数据的指令帮助"));
                }

                return;
            }
            case 3:
            {
                //对参数3先判断是不是数据，不是数字结束
                if (!int.TryParse(args.Parameters[2], out var num))
                {
                    args.Player.SendInfoMessage(GetString("格式错误！输入 /zmodify help  查看修改玩家数据的指令帮助"));
                    return;
                }
                
                //再判断能不能找到人的情况
                var players = this.BestFindPlayerByNameOrIndex(args.Parameters[0]);
                if (players.Count > 1)
                {
                    args.Player.SendInfoMessage(this.MultipleMatchPlayerWarning);
                    
                }
                
                //在线能找到
                else if (players.Count == 1)
                {
                    var subCommand = args.Parameters[1].ToLower();
                    switch (subCommand)
                    {
                        case "life":
                            players[0].TPlayer.statLife = num;
                            TSPlayer.All.SendData(PacketTypes.PlayerHp, "", players[0].Index);
                            players[0].SendMessage(GetString("您的生命值已被修改为：") + num, new Color(0, 255, 0));
                            break;
                        case "lifemax":
                            players[0].TPlayer.statLifeMax = num;
                            TSPlayer.All.SendData(PacketTypes.PlayerHp, "", players[0].Index);
                            players[0].SendMessage(GetString("您的生命上限已被修改为：") + num, new Color(0, 255, 0));
                            break;
                        case "mana":
                            players[0].TPlayer.statMana = num;
                            TSPlayer.All.SendData(PacketTypes.PlayerMana, "", players[0].Index);
                            players[0].SendMessage(GetString("您的魔力值已被修改为：") + num, new Color(0, 255, 0));
                            break;
                        case "manamax":
                            players[0].TPlayer.statManaMax = num;
                            TSPlayer.All.SendData(PacketTypes.PlayerMana, "", players[0].Index);
                            players[0].SendMessage(GetString("您的魔力上限已被修改为：") + num, new Color(0, 255, 0));
                            break;
                        case "fish":
                            players[0].TPlayer.anglerQuestsFinished = num;
                            TSPlayer.All.SendData(PacketTypes.NumberOfAnglerQuestsCompleted, "", players[0].Index);
                            players[0].SendMessage(GetString("您的渔夫任务完成数已被修改为：") + num, new Color(0, 255, 0));
                            break;
                        case "golf":
                            players[0].TPlayer.golferScoreAccumulated = num;
                            TSPlayer.All.SendData(PacketTypes.NumberOfAnglerQuestsCompleted, "", players[0].Index);
                            players[0].SendMessage(GetString("您的高尔夫球分数已被修改为：") + num, new Color(0, 255, 0));
                            break;
                        default:
                        {
                            if (config.EnablePointTracking && subCommand == "point")
                            {
                                var ex = edPlayers.Find(x => x.Name == players[0].Name);
                                if (ex != null)
                                {
                                    ex.point = num;
                                    players[0].SendMessage(GetString("您的点数已被修改为：") + num, new Color(0, 255, 0));
                                }
                                else
                                {
                                    args.Player.SendInfoMessage(GetString("不可预料的错误，请重试或让该玩家重进游戏"));
                                    return;
                                }
                            }
                            else if (num != 0 && num != 1)
                            {
                                args.Player.SendInfoMessage(GetString("格式错误！输入 /zmodify help  查看修改玩家数据的指令帮助"));
                                return;
                            }
                            else
                            {
                                switch (subCommand)
                                {
                                    case "torch":
                                        players[0].TPlayer.unlockedBiomeTorches = num != 0;
                                        TSPlayer.All.SendData(PacketTypes.PlayerInfo, "", players[0].Index);
                                        players[0].SendMessage(GetString("您的火把神增益开启状态：") + players[0].TPlayer.unlockedBiomeTorches, new Color(0, 255, 0));
                                        break;
                                    case "demmon":
                                        players[0].TPlayer.extraAccessory = num != 0;
                                        TSPlayer.All.SendData(PacketTypes.PlayerInfo, "", players[0].Index);
                                        players[0].SendMessage(GetString("您的恶魔心增益开启状态：") + players[0].TPlayer.extraAccessory, new Color(0, 255, 0));
                                        break;
                                    case "bread":
                                        players[0].TPlayer.ateArtisanBread = num != 0;
                                        TSPlayer.All.SendData(PacketTypes.PlayerInfo, "", players[0].Index);
                                        players[0].SendMessage(GetString("您的工匠面包增益开启状态：") + players[0].TPlayer.ateArtisanBread, new Color(0, 255, 0));
                                        break;
                                    case "heart":
                                        players[0].TPlayer.usedAegisCrystal = num != 0;
                                        TSPlayer.All.SendData(PacketTypes.PlayerInfo, "", players[0].Index);
                                        players[0].SendMessage(GetString("您的埃癸斯水晶增益开启状态：") + players[0].TPlayer.usedAegisCrystal, new Color(0, 255, 0));
                                        break;
                                    case "fruit":
                                        players[0].TPlayer.usedAegisFruit = num != 0;
                                        TSPlayer.All.SendData(PacketTypes.PlayerInfo, "", players[0].Index);
                                        players[0].SendMessage(GetString("您的埃癸斯果增益开启状态：") + players[0].TPlayer.usedAegisFruit, new Color(0, 255, 0));
                                        break;
                                    case "star":
                                        players[0].TPlayer.usedArcaneCrystal = num != 0;
                                        TSPlayer.All.SendData(PacketTypes.PlayerInfo, "", players[0].Index);
                                        players[0].SendMessage(GetString("您的奥术水晶增益开启状态：") + players[0].TPlayer.usedArcaneCrystal, new Color(0, 255, 0));
                                        break;
                                    case "pearl":
                                        players[0].TPlayer.usedGalaxyPearl = num != 0;
                                        TSPlayer.All.SendData(PacketTypes.PlayerInfo, "", players[0].Index);
                                        players[0].SendMessage(GetString("您的银河珍珠增益开启状态：") + players[0].TPlayer.usedGalaxyPearl, new Color(0, 255, 0));
                                        break;
                                    case "worm":
                                        players[0].TPlayer.usedGummyWorm = num != 0;
                                        TSPlayer.All.SendData(PacketTypes.PlayerInfo, "", players[0].Index);
                                        players[0].SendMessage(GetString("您的粘性蠕虫增益开启状态：") + players[0].TPlayer.usedGummyWorm, new Color(0, 255, 0));
                                        break;
                                    case "ambrosia":
                                        players[0].TPlayer.usedAmbrosia = num != 0;
                                        TSPlayer.All.SendData(PacketTypes.PlayerInfo, "", players[0].Index);
                                        players[0].SendMessage(GetString("您的珍馐增益开启状态：") + players[0].TPlayer.usedAmbrosia, new Color(0, 255, 0));
                                        break;
                                    case "cart":
                                        players[0].TPlayer.unlockedSuperCart = num != 0;
                                        TSPlayer.All.SendData(PacketTypes.PlayerInfo, "", players[0].Index);
                                        players[0].SendMessage(GetString("您的超级矿车增益开启状态：") + players[0].TPlayer.unlockedSuperCart, new Color(0, 255, 0));
                                        break;
                                    case "all":
                                        switch (num)
                                        {
                                            case 1:
                                                players[0].TPlayer.unlockedBiomeTorches = true;
                                                players[0].TPlayer.extraAccessory = true;
                                                players[0].TPlayer.ateArtisanBread = true;
                                                players[0].TPlayer.usedAegisCrystal = true;
                                                players[0].TPlayer.usedAegisFruit = true;
                                                players[0].TPlayer.usedArcaneCrystal = true;
                                                players[0].TPlayer.usedGalaxyPearl = true;
                                                players[0].TPlayer.usedGummyWorm = true;
                                                players[0].TPlayer.usedAmbrosia = true;
                                                players[0].TPlayer.unlockedSuperCart = true;
                                                TSPlayer.All.SendData(PacketTypes.PlayerInfo, "", players[0].Index);
                                                players[0].SendMessage(GetString("您的所有永久增益均开启"), new Color(0, 255, 0));
                                                break;
                                            case 0:
                                                players[0].TPlayer.unlockedBiomeTorches = false;
                                                players[0].TPlayer.extraAccessory = false;
                                                players[0].TPlayer.ateArtisanBread = false;
                                                players[0].TPlayer.usedAegisCrystal = false;
                                                players[0].TPlayer.usedAegisFruit = false;
                                                players[0].TPlayer.usedArcaneCrystal = false;
                                                players[0].TPlayer.usedGalaxyPearl = false;
                                                players[0].TPlayer.usedGummyWorm = false;
                                                players[0].TPlayer.usedAmbrosia = false;
                                                players[0].TPlayer.unlockedSuperCart = false;
                                                TSPlayer.All.SendData(PacketTypes.PlayerInfo, "", players[0].Index);
                                                players[0].SendMessage(GetString("您的所有永久增益均关闭"), new Color(0, 255, 0));
                                                break;
                                        }

                                        break;
                                }
                            }

                            break;
                        }
                    }

                    args.Player.SendMessage(GetString("修改成功！"), new Color(0, 255, 0));
                }
                //不在线，修改离线数据
                else if (players.Count == 0)
                {
                    args.Player.SendInfoMessage(this.playerOfflineWarning);
                    var user = TShock.UserAccounts.GetUserAccountByName(args.Parameters[0]);
                    if (user == null)
                    {
                        var users = TShock.UserAccounts.GetUserAccountsByName(args.Parameters[0], true);
                        if (users.Count == 0)
                        {
                            args.Player.SendInfoMessage(this.nonExistPlayerWarning);
                            return;
                        }

                        if (users.Count > 1)
                        {
                            args.Player.SendInfoMessage(this.MultipleMatchPlayerWarning);
                            return;
                        }

                        user = users[0];
                    }

                    try
                    {
                        if (args.Parameters[1].Equals("life", StringComparison.OrdinalIgnoreCase))
                        {
                            TShock.DB.Query("UPDATE tsCharacter SET Health = @0 WHERE Account = @1;", num, user.ID);
                        }
                        else if (args.Parameters[1].Equals("lifemax", StringComparison.OrdinalIgnoreCase))
                        {
                            TShock.DB.Query("UPDATE tsCharacter SET MaxHealth= @0 WHERE Account = @1;", num, user.ID);
                        }
                        else if (args.Parameters[1].Equals("mana", StringComparison.OrdinalIgnoreCase))
                        {
                            TShock.DB.Query("UPDATE tsCharacter SET Mana = @0 WHERE Account = @1;", num, user.ID);
                        }
                        else if (args.Parameters[1].Equals("manamax", StringComparison.OrdinalIgnoreCase))
                        {
                            TShock.DB.Query("UPDATE tsCharacter SET MaxMana = @0 WHERE Account = @1;", num, user.ID);
                        }
                        else if (args.Parameters[1].Equals("fish", StringComparison.OrdinalIgnoreCase))
                        {
                            TShock.DB.Query("UPDATE tsCharacter SET questsCompleted = @0 WHERE Account = @1;", num, user.ID);
                        }
                        else if (config.EnablePointTracking && args.Parameters[1].Equals("point", StringComparison.OrdinalIgnoreCase))
                        {
                            TShock.DB.Query("UPDATE Zhipm_PlayerExtra SET point = @0 WHERE Account = @1;", num, user.ID);
                        }
                        else if (num != 0 && num != 1)
                        {
                            args.Player.SendInfoMessage(GetString("格式错误！输入 /zmodify help  查看修改玩家数据的指令帮助"));
                            return;
                        }
                        else if (args.Parameters[1].Equals("torch", StringComparison.OrdinalIgnoreCase))
                        {
                            TShock.DB.Query("UPDATE tsCharacter SET unlockedBiomeTorches = @0 WHERE Account = @1;", num, user.ID);
                        }
                        else if (args.Parameters[1].Equals("demmon", StringComparison.OrdinalIgnoreCase))
                        {
                            TShock.DB.Query("UPDATE tsCharacter SET extraSlot = @0 WHERE Account = @1;", num, user.ID);
                        }
                        else if (args.Parameters[1].Equals("bread", StringComparison.OrdinalIgnoreCase))
                        {
                            TShock.DB.Query("UPDATE tsCharacter SET ateArtisanBread = @0 WHERE Account = @1;", num, user.ID);
                        }
                        else if (args.Parameters[1].Equals("crystal", StringComparison.OrdinalIgnoreCase))
                        {
                            TShock.DB.Query("UPDATE tsCharacter SET usedAegisCrystal = @0 WHERE Account = @1;", num, user.ID);
                        }
                        else if (args.Parameters[1].Equals("fruit", StringComparison.OrdinalIgnoreCase))
                        {
                            TShock.DB.Query("UPDATE tsCharacter SET usedAegisFruit = @0 WHERE Account = @1;", num, user.ID);
                        }
                        else if (args.Parameters[1].Equals("arcane", StringComparison.OrdinalIgnoreCase))
                        {
                            TShock.DB.Query("UPDATE tsCharacter SET usedArcaneCrystal = @0 WHERE Account = @1;", num, user.ID);
                        }
                        else if (args.Parameters[1].Equals("pearl", StringComparison.OrdinalIgnoreCase))
                        {
                            TShock.DB.Query("UPDATE tsCharacter SET usedGalaxyPearl = @0 WHERE Account = @1;", num, user.ID);
                        }
                        else if (args.Parameters[1].Equals("worm", StringComparison.OrdinalIgnoreCase))
                        {
                            TShock.DB.Query("UPDATE tsCharacter SET usedGummyWorm = @0 WHERE Account = @1;", num, user.ID);
                        }
                        else if (args.Parameters[1].Equals("ambrosia", StringComparison.OrdinalIgnoreCase))
                        {
                            TShock.DB.Query("UPDATE tsCharacter SET usedAmbrosia = @0 WHERE Account = @1;", num, user.ID);
                        }
                        else if (args.Parameters[1].Equals("cart", StringComparison.OrdinalIgnoreCase))
                        {
                            TShock.DB.Query("UPDATE tsCharacter SET unlockedSuperCart = @0 WHERE Account = @1;", num, user.ID);
                        }
                        else if (args.Parameters[1].Equals("all", StringComparison.OrdinalIgnoreCase))
                        {
                            TShock.DB.Query("UPDATE tsCharacter SET unlockedBiomeTorches = @1, extraSlot = @2, ateArtisanBread = @3, usedAegisCrystal = @4, usedAegisFruit = @5, usedArcaneCrystal = @6, usedGalaxyPearl = @7, usedGummyWorm = @8, usedAmbrosia = @9, unlockedSuperCart = @10 WHERE Account = @0;", user.ID, num, num, num, num, num, num, num, num, num, num);
                        }

                        args.Player.SendMessage(GetString("修改成功！"), new Color(0, 255, 0));
                    }
                    catch (Exception ex)
                    {
                        args.Player.SendMessage(GetString("修改失败！错误：") + ex, new Color(255, 0, 0));
                        TShock.Log.Error(GetString("修改失败！错误：") + ex);
                    }
                }

                break;
            }
        }
    }


    /// <summary>
    ///     重置用户备份数据库方法指令
    /// </summary>
    /// <param name="args"></param>
    private void ZResetPlayerDB(CommandArgs args)
    {
        if (args.Parameters.Count != 1)
        {
            args.Player.SendInfoMessage(GetString("输入 /zresetdb [name]  来清理该玩家的备份数据\n输入 /zresetdb all  来清理所有玩家的备份数据"));
            return;
        }

        if (args.Parameters[0].Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            if (ZPDataBase.ClearALLZPlayerDB(ZPDataBase))
            {
                if (!args.Player.RealPlayer)
                {
                    args.Player.SendMessage(GetString("所有玩家的备份数据均已重置"), broadcastColor);
                    TSPlayer.All.SendMessage(GetString("所有玩家的备份数据均已重置"), broadcastColor);
                }
                else
                {
                    TSPlayer.All.SendMessage(GetString("所有玩家的备份数据均已重置"), broadcastColor);
                }
            }
            else
            {
                args.Player.SendMessage(GetString("重置失败"), new Color(255, 0, 0));
            }
        }
        else
        {
            var list = this.BestFindPlayerByNameOrIndex(args.Parameters[0]);
            if (list.Count > 1)
            {
                args.Player.SendInfoMessage(this.MultipleMatchPlayerWarning);
                return;
            }

            if (list.Count == 1)
            {
                if (ZPDataBase.ClearZPlayerDB(list[0].Account.ID))
                {
                    args.Player.SendMessage(GetString($"已重置玩家 [ {list[0].Name} ] 的备份数据"), new Color(0, 255, 0));
                    list[0].SendMessage(GetString("您的备份数据已重置"), broadcastColor);
                }
                else
                {
                    args.Player.SendMessage(GetString("重置失败"), new Color(255, 0, 0));
                }

                return;
            }

            if (list.Count == 0)
            {
                args.Player.SendInfoMessage(this.playerOfflineWarning);
                var user = TShock.UserAccounts.GetUserAccountByName(args.Parameters[0]);
                if (user == null)
                {
                    args.Player.SendMessage(this.nonExistPlayerWarning, new Color(255, 0, 0));
                }
                else
                {
                    if (ZPDataBase.ClearZPlayerDB(user.ID))
                    {
                        args.Player.SendMessage(GetString($"已重置离线玩家 [ {user.Name} ] 的备份数据"), new Color(0, 255, 0));
                    }
                    else
                    {
                        args.Player.SendMessage(GetString("重置失败"), new Color(255, 0, 0));
                    }
                }
            }
        }
    }


    /// <summary>
    ///     重置用户额外数据库方法指令
    /// </summary>
    /// <param name="args"></param>
    private void ZResetPlayerEX(CommandArgs args)
    {
        if (args.Parameters.Count != 1)
        {
            args.Player.SendInfoMessage(GetString("输入 /zresetex [name]  来清理该玩家的额外数据\n") +
                                        GetString("输入 /zresetex all  来清理所有玩家的额外数据"));
            return;
        }

        if (args.Parameters[0].Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            if (ZPExtraDB.ClearALLZPlayerExtraDB(ZPExtraDB))
            {
                edPlayers.Clear();
                if (!args.Player.RealPlayer)
                {
                    args.Player.SendMessage(GetString("所有玩家的额外数据均已重置"), broadcastColor);
                    TSPlayer.All.SendMessage(GetString("所有玩家的额外数据均已重置"), broadcastColor);
                }
                else
                {
                    TSPlayer.All.SendMessage(GetString("所有玩家的额外数据均已重置"), broadcastColor);
                }
            }
            else
            {
                args.Player.SendMessage(GetString("重置失败"), new Color(255, 0, 0));
            }

            return;
        }

        var tSPlayers = this.BestFindPlayerByNameOrIndex(args.Parameters[0]);
        if (tSPlayers.Count > 1)
        {
            args.Player.SendInfoMessage(this.MultipleMatchPlayerWarning);
            return;
        }

        if (tSPlayers.Count == 1)
        {
            if (ZPExtraDB.ClearZPlayerExtraDB(tSPlayers[0].Account.ID))
            {
                edPlayers.RemoveAll(x => x.Name == tSPlayers[0].Name);
                args.Player.SendMessage(GetString($"已重置玩家 [ {tSPlayers[0].Name} ] 的额外数据"), new Color(0, 255, 0));
                tSPlayers[0].SendMessage(GetString("您的额外数据已重置"), broadcastColor);
            }
            else
            {
                args.Player.SendMessage(GetString("重置失败"), new Color(255, 0, 0));
            }

            return;
        }

        if (tSPlayers.Count == 0)
        {
            args.Player.SendInfoMessage(this.playerOfflineWarning);
            var user = TShock.UserAccounts.GetUserAccountByName(args.Parameters[0]);
            if (user == null)
            {
                args.Player.SendMessage(this.nonExistPlayerWarning, new Color(255, 0, 0));
            }
            else
            {
                if (ZPExtraDB.ClearZPlayerExtraDB(user.ID))
                {
                    args.Player.SendMessage(GetString($"已重置离线玩家 [ {user.Name} ] 的额外数据"), new Color(0, 255, 0));
                }
                else
                {
                    args.Player.SendMessage(GetString("重置失败"), new Color(255, 0, 0));
                }
            }
        }
    }


    /// <summary>
    ///     重置玩家的人物数据方法指令
    /// </summary>
    /// <param name="args"></param>
    private void ZResetPlayer(CommandArgs args)
    {
        if (args.Parameters.Count != 1)
        {
            args.Player.SendInfoMessage(GetString("输入 /zreset <name>  来清理该玩家的人物数据\n输入 /zreset all  来清理所有玩家的人物数据"));
            return;
        }

        if (args.Parameters[0].Equals("help", StringComparison.OrdinalIgnoreCase))
        {
            args.Player.SendInfoMessage(
                GetString("输入 /zresetdb <name> 来清理该玩家的备份数据\n") +
                GetString("输入 /zresetdb all 来清理所有玩家的备份数据\n") +
                GetString("输入 /zresetex <name> 来清理该玩家的额外数据\n") +
                GetString("输入 /zresetex all 来清理所有玩家的额外数据\n") +
                GetString("输入 /zreset <name> 来清理该玩家的人物数据\n") +
                GetString("输入 /zreset all 来清理所有玩家的人物数据\n") +
                GetString("输入 /zresetallplayers 来清理所有玩家的所有数据"), TextColor());
            return;
        }

        if (args.Parameters[0].Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                foreach (var ts in TShock.Players)
                {
                    if (ts is { IsLoggedIn: true })
                    {
                        this.ResetPlayer(ts);
                    }
                }
            }
            catch { }

            TShock.DB.Query("delete from tsCharacter");
            if (!args.Player.RealPlayer)
            {
                args.Player.SendMessage(GetString("所有玩家的人物数据均已重置"), broadcastColor);
            }

            TSPlayer.All.SendMessage(GetString("所有玩家的人物数据均已重置"), broadcastColor);

            return;
        }

        var list = this.BestFindPlayerByNameOrIndex(args.Parameters[0]);
        if (list.Count > 1)
        {
            args.Player.SendInfoMessage(this.MultipleMatchPlayerWarning);
            return;
        }

        if (list.Count == 0)
        {
            args.Player.SendInfoMessage(this.playerOfflineWarning);
            var user = TShock.UserAccounts.GetUserAccountByName(args.Parameters[0]);
            if (user == null)
            {
                args.Player.SendInfoMessage(this.nonExistPlayerWarning);
            }
            else
            {
                if (TShock.CharacterDB.RemovePlayer(user.ID))
                {
                    args.Player.SendMessage(GetString($"已重置离线玩家 [ {user.Name} ] 的数据"), new Color(0, 255, 0));
                }
                else
                {
                    args.Player.SendMessage(GetString("重置失败！未在原数据库中查到该玩家，请检查输入是否正确，该玩家是否避免SSC检测，再重新输入"), new Color(255, 0, 0));
                }
            }

            return;
        }

        if (list.Count == 1)
        {
            if (this.ResetPlayer(list[0]) | TShock.CharacterDB.RemovePlayer(list[0].Account.ID))
            {
                args.Player.SendMessage(GetString($"已重置玩家 [ {list[0].Name} ] 的数据"), new Color(0, 255, 0));
                list[0].SendMessage(GetString("您的人物数据已被重置"), broadcastColor);
            }
            else
            {
                args.Player.SendInfoMessage(GetString("重置失败"));
            }
        }
    }


    /// <summary>
    /// 重置所有用户所有数据方法指令
    /// </summary>
    /// <param name="args"></param>
    private void ZResetPlayerAll(CommandArgs args)
    {
        if (args.Parameters.Count != 0)
        {
            args.Player.SendInfoMessage(GetString($"输入 /zresetallplayers  来清理所有玩家的所有数据"));
            return;
        }

        try
        {
            foreach (var tsplayer in TShock.Players)
            {
                if (tsplayer is { IsLoggedIn: true })
                {
                    this.ResetPlayer(tsplayer);
                }
            }

            TShock.DB.Query("delete from tsCharacter");
            ZPDataBase.ClearALLZPlayerDB(ZPDataBase);
            ZPExtraDB.ClearALLZPlayerExtraDB(ZPExtraDB);
            edPlayers.Clear();
        }
        catch (Exception ex)
        {
            args.Player.SendMessage(GetString($"清理失败 ZResetPlayerAll :") + ex, new Color(255, 0, 0));
            TShock.Log.Error(GetString($"清理失败 ZResetPlayerAll :") + ex);
            return;
        }

        if (!args.Player.RealPlayer)
        {
            args.Player.SendMessage(GetString($"玩家已全部初始化"), new Color(0, 255, 0));
            TSPlayer.All.SendMessage(GetString($"所有玩家的所有数据均已全部初始化"), broadcastColor);
        }
        else
        {
            TShock.Utils.Broadcast(GetString($"所有玩家的所有数据均已全部初始化"), broadcastColor);
        }
    }


    /// <summary>
    ///     分类查阅指令
    /// </summary>
    /// <param name="args"></param>
    private void ViewInvent(CommandArgs args)
    {
        if (args.Parameters.Count != 1)
        {
            args.Player.SendInfoMessage(GetString($"输入 /vi <玩家名>  来查看该玩家的库存"));
            return;
        }

        //显示模式
        var model = args.Player.RealPlayer ? 0 : 1;
        //var model = 0;

        var name = args.Parameters[0];
        var list = this.BestFindPlayerByNameOrIndex(name);
        if (list.Count > 0)
        {
            foreach (var li in list)
            {
                var sb = new StringBuilder();
                var inventory = GetItemsString(li.TPlayer.inventory, NetItem.InventorySlots, model);

                //装备栏一堆
                var armor = GetItemsString(li.TPlayer.armor, NetItem.ArmorSlots, model);
                var armor1 = GetItemsString(li.TPlayer.Loadouts[0].Armor, NetItem.ArmorSlots, model);
                var armor2 = GetItemsString(li.TPlayer.Loadouts[1].Armor, NetItem.ArmorSlots, model);
                var armor3 = GetItemsString(li.TPlayer.Loadouts[2].Armor, NetItem.ArmorSlots, model);

                //染料一堆
                var dyestuff = GetItemsString(li.TPlayer.dye, NetItem.DyeSlots, model);
                var dyestuff1 = GetItemsString(li.TPlayer.Loadouts[0].Dye, NetItem.DyeSlots, model);
                var dyestuff2 = GetItemsString(li.TPlayer.Loadouts[1].Dye, NetItem.DyeSlots, model);
                var dyestuff3 = GetItemsString(li.TPlayer.Loadouts[2].Dye, NetItem.DyeSlots, model);

                var misc = GetItemsString(li.TPlayer.miscEquips, NetItem.MiscEquipSlots, model);
                var miscDye = GetItemsString(li.TPlayer.miscDyes, NetItem.MiscDyeSlots, model);

                var trash = "";
                if (model == 0 && !li.TPlayer.trashItem.IsAir)
                {
                    trash = $"【[i/s{li.TPlayer.trashItem.stack}:{li.TPlayer.trashItem.netID}]】 ";
                }
                else if (model == 1 && !li.TPlayer.trashItem.IsAir)
                {
                    trash = $" [{Lang.prefix[li.TPlayer.trashItem.prefix].Value}.{li.TPlayer.trashItem.Name}:{li.TPlayer.trashItem.stack}] ";
                }

                var pig = GetItemsString(li.TPlayer.bank.item, NetItem.PiggySlots, model);
                var safe = GetItemsString(li.TPlayer.bank2.item, NetItem.SafeSlots, model);
                var forge = GetItemsString(li.TPlayer.bank3.item, NetItem.ForgeSlots, model);
                var vault = GetItemsString(li.TPlayer.bank4.item, NetItem.VoidSlots, model);

                sb.AppendLine(list.Count == 1 ? GetString($"玩家 【{li.Name}】 的所有库存如下:") : GetString($"多个结果  玩家 【{li.Name}】 的所有库存如下:"));

                if (inventory.Length > 0 && inventory != "")
                {
                    sb.AppendLine(GetString($"背包:"));
                    sb.AppendLine(model == 0 ? FormatArrangement(inventory, 30, " ") : inventory);
                }

                //装备栏
                if (armor.Length > 0 && armor != "")
                {
                    sb.AppendLine(GetString($"盔甲 + 饰品 + 时装:"));
                    sb.AppendLine(GetString($"当前装备栏："));
                    sb.AppendLine(armor);
                }

                if (armor1.Length > 0 && armor1 != "")
                {
                    sb.AppendLine(GetString($"装备栏1："));
                    sb.AppendLine(armor1);
                }

                if (armor2.Length > 0 && armor2 != "")
                {
                    sb.AppendLine(GetString($"装备栏2："));
                    sb.AppendLine(armor2);
                }

                if (armor3.Length > 0 && armor3 != "")
                {
                    sb.AppendLine(GetString($"装备栏3："));
                    sb.AppendLine(armor3);
                }

                //染料
                if (dyestuff.Length > 0 && dyestuff != "")
                {
                    sb.AppendLine(GetString($"当前染料:"));
                    sb.AppendLine(dyestuff);
                }

                if (dyestuff1.Length > 0 && dyestuff1 != "")
                {
                    sb.AppendLine(GetString($"染料1:"));
                    sb.AppendLine(dyestuff1);
                }

                if (dyestuff2.Length > 0 && dyestuff2 != "")
                {
                    sb.AppendLine(GetString($"染料2:"));
                    sb.AppendLine(dyestuff2);
                }

                if (dyestuff3.Length > 0 && dyestuff3 != "")
                {
                    sb.AppendLine(GetString($"染料3:"));
                    sb.AppendLine(dyestuff3);
                }


                if (misc.Length > 0 && misc != "")
                {
                    sb.AppendLine(GetString($"宠物 + 矿车 + 坐骑 + 钩爪:"));
                    sb.AppendLine(misc);
                }

                if (miscDye.Length > 0 && miscDye != "")
                {
                    sb.AppendLine(GetString($"宠物 矿车 坐骑 钩爪 染料:"));
                    sb.AppendLine(miscDye);
                }

                if (trash != "")
                {
                    sb.AppendLine(GetString($"垃圾桶:"));
                    sb.AppendLine(trash);
                }

                if (pig.Length > 0 && pig != "")
                {
                    sb.AppendLine(GetString($"猪猪储蓄罐:"));
                    sb.AppendLine(model == 0 ? FormatArrangement(pig, 30, " ") : pig);
                }

                if (safe.Length > 0 && safe != "")
                {
                    sb.AppendLine(GetString($"保险箱:"));
                    sb.AppendLine(model == 0 ? FormatArrangement(safe, 30, " ") : safe);
                }

                if (forge.Length > 0 && forge != "")
                {
                    sb.AppendLine(GetString($"护卫熔炉:"));
                    sb.AppendLine(model == 0 ? FormatArrangement(forge, 30, " ") : forge);
                }

                if (vault.Length > 0 && vault != "")
                {
                    sb.AppendLine(GetString($"虚空金库:"));
                    sb.AppendLine(model == 0 ? FormatArrangement(vault, 30, " ") : vault);
                }

                if (sb.Length > 0 && !string.IsNullOrEmpty(sb.ToString()))
                {
                    args.Player.SendMessage(sb + "\n", TextColor());
                }
                else
                {
                    args.Player.SendInfoMessage(GetString($"玩家 【{li.Name}】 未携带任何东西"));
                }
            }
        }
        else
        {
            args.Player.SendInfoMessage(this.playerOfflineWarning);
            var users = new Dictionary<UserAccount, PlayerData>();
            var temp = TShock.UserAccounts.GetUserAccountsByName(name, true);
            if (temp.Count == 1 || (temp.Count > 1 && temp.Exists(x => x.Name == name)))
            {
                if (temp.Count == 1)
                {
                    var temp2 = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), temp[0].ID);
                    if (temp2 is { exists: true })
                    {
                        users.Add(temp[0], temp2);
                    }
                }
                else
                {
                    var u = temp.Find(x => x.Name == name)!;
                    var temp2 = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), u.ID);
                    if (temp2 is { exists: true })
                    {
                        users.Add(u, temp2);
                    }
                    else //如果未找到就说明，你找到名字完全符合的没有charater数据，那么返回之前的多个模糊查找的结果
                    {
                        foreach (var t in temp)
                        {
                            var t2 = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), t.ID);
                            if (t2.exists)
                            {
                                users.Add(t, t2);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var t in temp)
                {
                    var t2 = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), t.ID);
                    if (t2.exists)
                    {
                        users.Add(t, t2);
                    }
                }
            }

            if (users.Count == 0)
            {
                args.Player.SendInfoMessage(this.nonExistPlayerWarning);
            }
            else
            {
                foreach (var p in users)
                {
                    var offAll = GetItemsString(p.Value.inventory, p.Value.inventory.Length, model);
                    if (model == 0)
                    {
                        offAll = FormatArrangement(offAll, 30, " ");
                    }

                    offAll += "\n";
                    if (!string.IsNullOrEmpty(offAll))
                    {
                        if (users.Count > 1)
                        {
                            args.Player.SendMessage(GetString($"多个结果  玩家 【{p.Key.Name}】 的所有库存如下:\n{offAll}"), TextColor());
                        }
                        else
                        {
                            args.Player.SendMessage(GetString($"玩家 【{p.Key.Name}】 的所有库存如下:\n{offAll}"), TextColor());
                        }
                    }
                    else
                    {
                        args.Player.SendInfoMessage(GetString($"玩家 【{p.Key.Name}】 未携带任何东西\n"));
                    }
                }
            }
        }
    }


    /// <summary>
    ///     不分类查阅指令
    /// </summary>
    /// <param name="args"></param>
    private void ViewInventDisorder(CommandArgs args)
    {
        if (args.Parameters.Count != 1)
        {
            args.Player.SendInfoMessage(GetString($"输入 /vid <玩家名>  来查看该玩家的库存，不进行排列"));
            return;
        }

        var model = args.Player.RealPlayer ? 0 : 1;
        var name = args.Parameters[0];
        var list = this.BestFindPlayerByNameOrIndex(name);
        if (list.Count > 0)
        {
            foreach (var li in list)
            {
                var inventory = GetItemsString(li.TPlayer.inventory, NetItem.InventorySlots, model);

                var armor = GetItemsString(li.TPlayer.armor, NetItem.ArmorSlots, model);
                var armor1 = GetItemsString(li.TPlayer.Loadouts[0].Armor, NetItem.ArmorSlots, model);
                var armor2 = GetItemsString(li.TPlayer.Loadouts[1].Armor, NetItem.ArmorSlots, model);
                var armor3 = GetItemsString(li.TPlayer.Loadouts[2].Armor, NetItem.ArmorSlots, model);


                var dyestuff = GetItemsString(li.TPlayer.dye, NetItem.DyeSlots, model);
                var dyestuff1 = GetItemsString(li.TPlayer.Loadouts[0].Dye, NetItem.DyeSlots, model);
                var dyestuff2 = GetItemsString(li.TPlayer.Loadouts[1].Dye, NetItem.DyeSlots, model);
                var dyestuff3 = GetItemsString(li.TPlayer.Loadouts[2].Dye, NetItem.DyeSlots, model);


                var misc = GetItemsString(li.TPlayer.miscEquips, NetItem.MiscEquipSlots, model);
                var miscDye = GetItemsString(li.TPlayer.miscDyes, NetItem.MiscDyeSlots, model);

                var trash = "";
                if (model == 0 && !li.TPlayer.trashItem.IsAir)
                {
                    trash = $"【[i/s{li.TPlayer.trashItem.stack}:{li.TPlayer.trashItem.netID}]】 ";
                }
                else if (model == 1 && !li.TPlayer.trashItem.IsAir)
                {
                    trash = $"[{Lang.prefix[li.TPlayer.trashItem.prefix].Value}.{li.TPlayer.trashItem.Name}]";
                }

                var pig = GetItemsString(li.TPlayer.bank.item, NetItem.PiggySlots, model);
                var safe = GetItemsString(li.TPlayer.bank2.item, NetItem.SafeSlots, model);
                var forge = GetItemsString(li.TPlayer.bank3.item, NetItem.ForgeSlots, model);
                var vault = GetItemsString(li.TPlayer.bank4.item, NetItem.VoidSlots, model);

                var all = inventory + armor + armor1 + armor2 + armor3 + dyestuff + dyestuff1 + dyestuff2 + dyestuff3 + misc + misc + miscDye + trash + pig + safe + forge + vault;
                if (model == 0)
                {
                    all = FormatArrangement(all, 30, " ");
                }

                if (!string.IsNullOrWhiteSpace(all))
                {
                    if (list.Count == 1)
                    {
                        args.Player.SendMessage(GetString($"玩家 【{li.Name}】 的所有库存如下:\n{all}\n"), TextColor());
                    }
                    else
                    {
                        args.Player.SendMessage(GetString($"多个结果  玩家 【{li.Name}】 的所有库存如下:\n{all}\n"), TextColor());
                    }
                }
                else
                {
                    args.Player.SendInfoMessage(GetString($"玩家 【{li.Name}】未携带任何东西\n"));
                }
            }
        }
        else
        {
            args.Player.SendInfoMessage(this.playerOfflineWarning);
            var users = new Dictionary<UserAccount, PlayerData>();
            var temp = TShock.UserAccounts.GetUserAccountsByName(name, true);
            if (temp.Count == 1 || (temp.Count > 1 && temp.Exists(x => x.Name == name)))
            {
                if (temp.Count == 1)
                {
                    var temp2 = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), temp[0].ID);
                    if (temp2 is { exists: true })
                    {
                        users.Add(temp[0], temp2);
                    }
                }
                else
                {
                    var u = temp.Find(x => x.Name == name)!;
                    var temp2 = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), u.ID);
                    if (temp2 is { exists: true })
                    {
                        users.Add(u, temp2);
                    }
                    else //如果未找到就说明，你找到名字完全符合的没有charater数据，那么返回之前的多个模糊查找的结果
                    {
                        foreach (var t in temp)
                        {
                            var t2 = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), t.ID);
                            if (t2.exists)
                            {
                                users.Add(t, t2);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var t in temp)
                {
                    var t2 = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), t.ID);
                    if (t2.exists)
                    {
                        users.Add(t, t2);
                    }
                }
            }

            if (users.Count == 0)
            {
                args.Player.SendInfoMessage(this.nonExistPlayerWarning);
            }
            else
            {
                foreach (var p in users)
                {
                    var offAll = GetItemsString(p.Value.inventory, p.Value.inventory.Length, model);
                    if (model == 0)
                    {
                        offAll = FormatArrangement(offAll, 30, " ");
                    }

                    if (!string.IsNullOrWhiteSpace(offAll))
                    {
                        if (users.Count > 1)
                        {
                            args.Player.SendMessage(GetString($"多个结果 玩家 【{p.Key.Name}】 的所有库存如下:\n{offAll}\n"), TextColor());
                        }
                        else
                        {
                            args.Player.SendMessage(GetString($"玩家 【{p.Key.Name}】 的所有库存如下:\n{offAll}\n"), TextColor());
                        }
                    }
                    else
                    {
                        args.Player.SendInfoMessage(GetString($"玩家 【{p.Key.Name}】 未携带任何东西\n"));
                    }
                }
            }
        }
    }


    /// <summary>
    ///     查询玩家的状态
    /// </summary>
    /// <param name="args"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void ViewState(CommandArgs args)
    {
        if (args.Parameters.Count != 1)
        {
            args.Player.SendInfoMessage(GetString("输入 /vs <玩家名>  来查看该玩家的状态"));
            return;
        }

        var name = args.Parameters[0];
        var list = this.BestFindPlayerByNameOrIndex(name);
        if (name.Equals("me", StringComparison.OrdinalIgnoreCase) && args.Player.RealPlayer)
        {
            list.Clear();
            list.Add(args.Player);
        }

        if (args.Player.RealPlayer)
        {
            if (list.Any())
            {
                var sb = new StringBuilder();
                var ex = edPlayers.Find(x => x.Name == list[0].Name);
                sb.AppendLine(GetString($"玩家 【{list[0].Name}】 的状态如下:"));
                sb.AppendLine(GetString($"最大生命值[i:29]：{list[0].TPlayer.statLifeMax2}   当前生命值[i:58]：{list[0].TPlayer.statLife}"));
                sb.AppendLine(GetString($"最大魔力值[i:109]：{list[0].TPlayer.statManaMax2}   当前魔力值[i:184]：{list[0].TPlayer.statMana}"));
                sb.AppendLine(GetString($"完成渔夫任务数[i:3120]：") + list[0].TPlayer.anglerQuestsFinished);
                sb.AppendLine(GetString($"库存硬币数[i:855]：") + this.Cointostring(this.GetPlayerCoin(list[0].Name)));
                sb.Append(GetString($"各种buff[i:678]："));
                var flag = 0;
                foreach (var buff in list[0].TPlayer.buffType)
                {
                    if (buff != 0)
                    {
                        flag++;
                        sb.Append(Lang.GetBuffName(buff) + "  ");
                        if (flag == 12)
                        {
                            sb.AppendLine();
                        }
                    }
                }

                if (flag == 0)
                {
                    sb.Append(GetString("无"));
                }

                sb.AppendLine();

                sb.Append(GetString("各种永久增益："));
                flag = 0;
                if (list[0].TPlayer.extraAccessory)
                {
                    flag++;
                    sb.Append("[i:3335]  ");
                }

                if (list[0].TPlayer.unlockedBiomeTorches)
                {
                    flag++;
                    sb.Append("[i:5043]  ");
                }

                if (list[0].TPlayer.ateArtisanBread)
                {
                    flag++;
                    sb.Append("[i:5326]  ");
                }

                if (list[0].TPlayer.usedAegisCrystal)
                {
                    flag++;
                    sb.Append("[i:5337]  ");
                }

                if (list[0].TPlayer.usedAegisFruit)
                {
                    flag++;
                    sb.Append("[i:5338]  ");
                }

                if (list[0].TPlayer.usedArcaneCrystal)
                {
                    flag++;
                    sb.Append("[i:5339]  ");
                }

                if (list[0].TPlayer.usedGalaxyPearl)
                {
                    flag++;
                    sb.Append("[i:5340]  ");
                }

                if (list[0].TPlayer.usedGummyWorm)
                {
                    flag++;
                    sb.Append("[i:5341]  ");
                }

                if (list[0].TPlayer.usedAmbrosia)
                {
                    flag++;
                    sb.Append("[i:5342]  ");
                }

                if (list[0].TPlayer.unlockedSuperCart)
                {
                    flag++;
                    sb.Append("[i:5289]");
                }

                if (flag == 0)
                {
                    sb.Append(GetString("无"));
                }

                sb.AppendLine();
                if (ex != null)
                {
                    if (config.EnableOnlineTimeTracking)
                    {
                        sb.AppendLine(GetString("在线时长[i:3099]：") + this.Timetostring(ex.time));
                    }

                    if (config.EnableDeathCountTracking)
                    {
                        sb.AppendLine(GetString("死亡次数[i:321]：") + ex.deathCount);
                    }

                    if (config.EnableNpcKillTracking)
                    {
                        sb.AppendLine(GetString($"已击杀生物数[i:3095]：{ex.killNPCnum} 个"));
                        sb.AppendLine(GetString("已击杀Boss[i:3868]：") + DictionaryToVsString(ex.killBossID));
                        sb.AppendLine(GetString("已击杀罕见生物[i:4274]：") + DictionaryToVsString(ex.killRareNPCID));
                    }

                    if (config is { EnablePointTracking: true, EnableNpcKillTracking: true })
                    {
                        sb.AppendLine(GetString("点数[i:575]：") + ex.point);
                    }
                }

                args.Player.SendMessage(sb.ToString(), TextColor());
            }
            else
            {
                args.Player.SendInfoMessage(this.playerOfflineWarning);
                var users = new Dictionary<UserAccount, PlayerData>();
                var temp = TShock.UserAccounts.GetUserAccountsByName(name, true);
                if (temp.Count == 1 || (temp.Count > 1 && temp.Exists(x => x.Name == name)))
                {
                    if (temp.Count == 1)
                    {
                        var temp2 = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), temp[0].ID);
                        if (temp2 is { exists: true })
                        {
                            users.Add(temp[0], temp2);
                        }
                    }
                    else
                    {
                        var u = temp.Find(x => x.Name == name)!;
                        var temp2 = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), u.ID);
                        if (temp2 is { exists: true })
                        {
                            users.Add(u, temp2);
                        }
                        else
                        {
                            foreach (var t in temp)
                            {
                                var t2 = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), t.ID);
                                if (t2.exists)
                                {
                                    users.Add(t, t2);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var t in temp)
                    {
                        var t2 = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), t.ID);
                        if (t2.exists)
                        {
                            users.Add(t, t2);
                        }
                    }
                }

                if (users.Count == 0)
                {
                    args.Player.SendInfoMessage(this.nonExistPlayerWarning);
                }
                else
                {
                    foreach (var p in users)
                    {
                        var sb = new StringBuilder();
                        var ex = ZPExtraDB.ReadExtraDB(p.Key.ID);
                        if (users.Count == 1)
                        {
                            sb.AppendLine(GetString($"玩家 【{p.Key.Name}】 的状态如下:"));
                        }
                        else
                        {
                            sb.AppendLine(GetString($"多个结果  玩家 【{p.Key.Name}】 的状态如下:"));
                        }

                        sb.AppendLine(GetString($"最大生命值[i:29]：{p.Value.maxHealth}   当前生命值[i:58]：{p.Value.health}"));
                        sb.AppendLine(GetString($"最大魔力值[i:109]：{p.Value.maxMana}   当前魔力值[i:184]：{p.Value.mana}"));
                        sb.AppendLine(GetString("完成渔夫任务数[i:3120]：") + p.Value.questsCompleted);
                        sb.AppendLine(GetString("库存硬币数[i:855]：") + this.Cointostring(this.GetPlayerCoin(p.Key.Name)));
                        sb.Append(GetString("各种永久增益："));
                        var flag = 0;
                        if (p.Value.extraSlot is 1)
                        {
                            flag++;
                            sb.Append("[i:3335]  ");
                        }

                        if (p.Value.unlockedBiomeTorches == 1)
                        {
                            flag++;
                            sb.Append("[i:5043]  ");
                        }

                        if (p.Value.ateArtisanBread == 1)
                        {
                            flag++;
                            sb.Append("[i:5326]  ");
                        }

                        if (p.Value.usedAegisCrystal == 1)
                        {
                            flag++;
                            sb.Append("[i:5337]  ");
                        }

                        if (p.Value.usedAegisFruit == 1)
                        {
                            flag++;
                            sb.Append("[i:5338]  ");
                        }

                        if (p.Value.usedArcaneCrystal == 1)
                        {
                            flag++;
                            sb.Append("[i:5339]  ");
                        }

                        if (p.Value.usedGalaxyPearl == 1)
                        {
                            flag++;
                            sb.Append("[i:5340]  ");
                        }

                        if (p.Value.usedGummyWorm == 1)
                        {
                            flag++;
                            sb.Append("[i:5341]  ");
                        }

                        if (p.Value.usedAmbrosia == 1)
                        {
                            flag++;
                            sb.Append("[i:5342]  ");
                        }

                        if (p.Value.unlockedSuperCart == 1)
                        {
                            flag++;
                            sb.Append("[i:5289]");
                        }

                        if (flag == 0)
                        {
                            sb.Append(GetString("无"));
                        }

                        sb.AppendLine();
                        if (ex != null)
                        {
                            if (config.EnableOnlineTimeTracking)
                            {
                                sb.AppendLine(GetString("在线时长[i:3099]：") + this.Timetostring(ex.time));
                            }

                            if (config.EnableDeathCountTracking)
                            {
                                sb.AppendLine(GetString("死亡次数[i:321]：") + ex.deathCount);
                            }

                            if (config.EnableNpcKillTracking)
                            {
                                sb.AppendLine(GetString($"已击杀生物数[i:3095]：{ex.killNPCnum} 个"));
                                sb.AppendLine(GetString("已击杀Boss[i:3868]：") + DictionaryToVsString(ex.killBossID));
                                sb.AppendLine(GetString("已击杀罕见生物[i:4274]：") + DictionaryToVsString(ex.killRareNPCID));
                            }

                            if (config is { EnablePointTracking: true, EnableNpcKillTracking: true })
                            {
                                sb.AppendLine(GetString("点数[i:575]：") + ex.point);
                            }
                        }

                        args.Player.SendMessage(sb.ToString(), TextColor());
                    }
                }
            }
        }
        else
        {
            if (list.Any())
            {
                var sb = new StringBuilder();
                var ex = edPlayers.Find(x => x.Name == list[0].Name);
                sb.AppendLine(GetString($"玩家 【{list[0].Name}】 的状态如下:"));
                sb.AppendLine(GetString($"最大生命值：{list[0].TPlayer.statLifeMax}   当前生命值：{list[0].TPlayer.statLife}"));
                sb.AppendLine(GetString($"最大魔力值：{list[0].TPlayer.statManaMax2}   当前魔力值：{list[0].TPlayer.statMana}"));
                sb.AppendLine(GetString("完成渔夫任务数：") + list[0].TPlayer.anglerQuestsFinished);
                sb.AppendLine(GetString("库存硬币数：") + this.Cointostring(this.GetPlayerCoin(list[0].Name), 1));
                sb.Append(GetString("各种buff："));
                var flag = 0;
                foreach (var buff in list[0].TPlayer.buffType)
                {
                    if (buff != 0)
                    {
                        flag++;
                        sb.Append(Lang.GetBuffName(buff) + "  ");
                    }
                }

                if (flag == 0)
                {
                    sb.Append(GetString("无"));
                }

                sb.AppendLine();

                sb.Append(GetString("各种永久增益："));
                flag = 0;
                if (list[0].TPlayer.extraAccessory)
                {
                    flag++;
                    sb.Append(Lang.GetItemNameValue(3335) + "  ");
                }

                if (list[0].TPlayer.unlockedBiomeTorches)
                {
                    flag++;
                    sb.Append(Lang.GetItemNameValue(5043) + "  ");
                }

                if (list[0].TPlayer.ateArtisanBread)
                {
                    flag++;
                    sb.Append(Lang.GetItemNameValue(5326) + "  ");
                }

                if (list[0].TPlayer.usedAegisCrystal)
                {
                    flag++;
                    sb.Append(Lang.GetItemNameValue(5337) + "  ");
                }

                if (list[0].TPlayer.usedAegisFruit)
                {
                    flag++;
                    sb.Append(Lang.GetItemNameValue(5338) + "  ");
                }

                if (list[0].TPlayer.usedArcaneCrystal)
                {
                    flag++;
                    sb.Append(Lang.GetItemNameValue(5339) + "  ");
                }

                if (list[0].TPlayer.usedGalaxyPearl)
                {
                    flag++;
                    sb.Append(Lang.GetItemNameValue(5340) + "  ");
                }

                if (list[0].TPlayer.usedGummyWorm)
                {
                    flag++;
                    sb.Append(Lang.GetItemNameValue(5341) + "  ");
                }

                if (list[0].TPlayer.usedAmbrosia)
                {
                    flag++;
                    sb.Append(Lang.GetItemNameValue(5342) + "  ");
                }

                if (list[0].TPlayer.unlockedSuperCart)
                {
                    flag++;
                    sb.Append(Lang.GetItemNameValue(5289));
                }

                if (flag == 0)
                {
                    sb.Append(GetString("无"));
                }

                sb.AppendLine();
                if (ex != null)
                {
                    if (config.EnableOnlineTimeTracking)
                    {
                        sb.AppendLine(GetString("在线时长：") + this.Timetostring(ex.time));
                    }

                    if (config.EnableDeathCountTracking)
                    {
                        sb.AppendLine(GetString("死亡次数：") + ex.deathCount);
                    }

                    if (config.EnableNpcKillTracking)
                    {
                        sb.AppendLine(GetString($"已击杀生物数：{ex.killNPCnum} 个"));
                        sb.AppendLine(GetString("已击杀Boss：") + DictionaryToVsString(ex.killBossID, false));
                        sb.AppendLine(GetString("已击杀罕见生物：") + DictionaryToVsString(ex.killRareNPCID, false));
                    }

                    if (config is { EnableNpcKillTracking: true, EnablePointTracking: true })
                    {
                        sb.AppendLine(GetString("点数：") + ex.point);
                    }
                }

                args.Player.SendMessage(sb.ToString(), TextColor());
            }
            else
            {
                args.Player.SendInfoMessage(this.playerOfflineWarning);
                var users = new Dictionary<UserAccount, PlayerData>();
                var temp = TShock.UserAccounts.GetUserAccountsByName(name, true);
                if (temp.Count == 1 || (temp.Count > 1 && temp.Exists(x => x.Name == name)))
                {
                    if (temp.Count == 1)
                    {
                        var temp2 = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), temp[0].ID);
                        if (temp2 is { exists: true })
                        {
                            users.Add(temp[0], temp2);
                        }
                    }
                    else
                    {
                        var u = temp.Find(x => x.Name == name)!;
                        var temp2 = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), u.ID);
                        if (temp2 is { exists: true })
                        {
                            users.Add(u, temp2);
                        }
                        else
                        {
                            foreach (var t in temp)
                            {
                                var t2 = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), t.ID);
                                if (t2.exists)
                                {
                                    users.Add(t, t2);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var t in temp)
                    {
                        if (t != null)
                        {
                            var t2 = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), t.ID);
                            if (t2.exists)
                            {
                                users.Add(t, t2);
                            }
                        }
                    }
                }

                if (users.Count == 0)
                {
                    args.Player.SendInfoMessage(this.nonExistPlayerWarning);
                }
                else
                {
                    foreach (var p in users)
                    {
                        var sb = new StringBuilder();
                        var ex = ZPExtraDB.ReadExtraDB(p.Key.ID);
                        sb.AppendLine(users.Count == 1 ? GetString($"玩家 【{p.Key.Name}】 的状态如下:") : GetString($"多个结果  玩家 【{p.Key.Name}】 的状态如下:"));

                        sb.AppendLine(GetString($"最大生命值：{p.Value.maxHealth}   当前生命值：{p.Value.health}"));
                        sb.AppendLine(GetString($"最大魔力值：{p.Value.maxMana}   当前魔力值：{p.Value.mana}"));
                        sb.AppendLine(GetString("完成渔夫任务数：") + p.Value.questsCompleted);
                        sb.AppendLine(GetString("库存硬币数：") + this.Cointostring(this.GetPlayerCoin(p.Key.Name), 1));
                        sb.Append(GetString("各种永久增益："));
                        var flag = 0;
                        if (p.Value.extraSlot is 1)
                        {
                            flag++;
                            sb.Append(Lang.GetItemNameValue(3335) + "  ");
                        }

                        if (p.Value.unlockedBiomeTorches == 1)
                        {
                            flag++;
                            sb.Append(Lang.GetItemNameValue(5043) + "  ");
                        }

                        if (p.Value.ateArtisanBread == 1)
                        {
                            flag++;
                            sb.Append(Lang.GetItemNameValue(5326) + "  ");
                        }

                        if (p.Value.usedAegisCrystal == 1)
                        {
                            flag++;
                            sb.Append(Lang.GetItemNameValue(5337) + "  ");
                        }

                        if (p.Value.usedAegisFruit == 1)
                        {
                            flag++;
                            sb.Append(Lang.GetItemNameValue(5338) + "  ");
                        }

                        if (p.Value.usedArcaneCrystal == 1)
                        {
                            flag++;
                            sb.Append(Lang.GetItemNameValue(5339) + "  ");
                        }

                        if (p.Value.usedGalaxyPearl == 1)
                        {
                            flag++;
                            sb.Append(Lang.GetItemNameValue(5340) + "  ");
                        }

                        if (p.Value.usedGummyWorm == 1)
                        {
                            flag++;
                            sb.Append(Lang.GetItemNameValue(5341) + "  ");
                        }

                        if (p.Value.usedAmbrosia == 1)
                        {
                            flag++;
                            sb.Append(Lang.GetItemNameValue(5342) + "  ");
                        }

                        if (p.Value.unlockedSuperCart == 1)
                        {
                            flag++;
                            sb.Append(Lang.GetItemNameValue(5289));
                        }

                        if (flag == 0)
                        {
                            sb.Append(GetString("无"));
                        }

                        sb.AppendLine();
                        if (ex != null)
                        {
                            if (config.EnableOnlineTimeTracking)
                            {
                                sb.AppendLine(GetString("在线时长：") + this.Timetostring(ex.time));
                            }

                            if (config.EnableDeathCountTracking)
                            {
                                sb.AppendLine(GetString("死亡次数：") + ex.deathCount);
                            }

                            if (config.EnableNpcKillTracking)
                            {
                                sb.AppendLine(GetString($"已击杀生物数：{ex.killNPCnum} 个"));
                                sb.AppendLine(GetString("已击杀Boss：") + DictionaryToVsString(ex.killBossID, false));
                                sb.AppendLine(GetString("已击杀罕见生物：") + DictionaryToVsString(ex.killRareNPCID, false));
                            }

                            if (config is { EnablePointTracking: true, EnableNpcKillTracking: true })
                            {
                                sb.AppendLine(GetString("点数：") + ex.point);
                            }
                        }

                        args.Player.SendMessage(sb.ToString(), TextColor());
                    }
                }
            }
        }
    }


    /// <summary>
    ///     清理
    /// </summary>
    /// <param name="args"></param>
    private void Clear(CommandArgs args)
    {
        if (args.Parameters.Count < 1 || args.Parameters.Count > 2)
        {
            args.Player.SendInfoMessage(GetString("输入 /zclear useless  来清理世界的掉落物品，非城镇或BossNPC，和无用射弹\n输入 /zclear buff <name>  来清理该玩家的所有Buff\n输入 /zclear buff all  来清理所有玩家所有Buff"));
            return;
        }

        if (args.Parameters.Count == 1 && args.Parameters[0].Equals("useless", StringComparison.OrdinalIgnoreCase))
        {
            this.clearTimer = Timer + 1200L;
            if (!args.Player.RealPlayer)
            {
                args.Player.SendMessage(GetString("服务器将在20秒后清理世界内所有无用NPC，射弹和物品"), new Color(255, 0, 0));
            }

            TSPlayer.All.SendMessage(GetString("服务器将在20秒后清理世界内所有无用NPC，射弹和物品"), new Color(255, 0, 0));
        }
        else if (args.Parameters.Count == 2 && args.Parameters[0].Equals("buff", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Parameters[1].Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var tSPlayer in TShock.Players)
                {
                    if (tSPlayer is { IsLoggedIn: true })
                    {
                        ClearAllBuffFromPlayer(tSPlayer);
                    }
                }

                args.Player.SendMessage(GetString("所有玩家的所有Buff均已消除"), new Color(0, 255, 0));
                return;
            }

            var ts = this.BestFindPlayerByNameOrIndex(args.Parameters[1]);
            if (ts.Count == 0)
            {
                args.Player.SendInfoMessage(GetString("该玩家不在线或不存在"));
            }
            else if (ts.Count > 1)
            {
                args.Player.SendInfoMessage(this.MultipleMatchPlayerWarning);
            }
            else
            {
                ClearAllBuffFromPlayer(ts[0]);
                args.Player.SendMessage(GetString($"玩家 [ {ts[0].Name} ] 的所有Buff均已消除"), new Color(0, 255, 0));
            }
        }
        else
        {
            args.Player.SendInfoMessage(
                GetString("输入 /zclear useless  来清理世界的掉落物品，非城镇或BossNPC，和无用射弹\n") +
                GetString("输入 /zclear buff <name>  来清理该玩家的所有Buff\n") +
                GetString("输入 /zclear buff all  来清理所有玩家所有Buff"));
        }
    }


    /// <summary>
    ///     游戏更新，用来实现人物额外数据如时间的同步增加，这里实现对进服人员的添加和time自增
    /// </summary>
    /// <param name="args"></param>
    private void OnGameUpdate(EventArgs args)
    {
        //自制计时器，60 Timer = 1 秒
        Timer++;
        //以秒为单位处理，降低计算机计算量
        if (Timer % 60L == 0L)
        {
            //在线时长 +1 的部分，遍历在线玩家，对time进行自增
            var players = TShock.Players;
            foreach (var tsp in players)
            {
                if (tsp is { IsLoggedIn: true })
                {
                    //如果当前玩家已存在，那么更新额外数据
                    var tsp1 = tsp;
                    var extraData = edPlayers.Find(x => x.Name == tsp1.Name);
                    if (extraData != null)
                    {
                        if (config.EnableOnlineTimeTracking)
                        {
                            extraData.time += 1L;
                            if (extraData.time % 1800L == 0L)
                            {
                                if (config.EnablePointTracking)
                                {
                                    extraData.point += 1000;
                                    this.SendText(tsp, GetString("点数奖励 + 1000"), broadcastColor, tsp.TPlayer.Center);
                                }

                                tsp.SendMessage(GetString("您已经在线了 ") + this.Timetostring(extraData.time), broadcastColor);
                                TShock.Log.Info(GetString($"玩家 {extraData.Name} 已经在线了 {this.Timetostring(extraData.time)}"));
                                NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(tsp.TPlayer.Center, 4), tsp.Index);
                                Projectile.NewProjectile(null, tsp.TPlayer.Center, -Vector2.UnitY * 4f, Main.rand.Next(415, 419), 0, 0f);
                            }
                        }
                    }
                    //否则查找是否已注册过
                    else
                    {
                        //注册过了，那么读取
                        var extraData2 = ZPExtraDB.ReadExtraDB(tsp.Account.ID);
                        if (extraData2 != null)
                        {
                            edPlayers.Add(extraData2);
                        }
                        //否则创建一个新的
                        else
                        {
                            var ex = new ExtraData(tsp.Account.ID, tsp.Name, 0L, config.DefaultAutoBackupIntervalInMinutes, 0, 0L, !config.DefaultKillFontVisibleToPlayers, !config.DefaultPointFontVisibleToPlayers);
                            ZPExtraDB.WriteExtraDB(ex);
                            edPlayers.Add(ex);
                        }
                    }
                }
            }


            //自动备份的处理部分，这里以分钟为单位 3600L = 1 分钟，默认用全局计时器进行备份的部分
            if (config.EnablePlayerAutoBackup && Timer % 3600L == 0L)
            {
                foreach (var ex in edPlayers)
                {
                    //到达备份间隔时长，备份一次
                    foreach (var ts in TShock.Players)
                    {
                        if (ts is { IsLoggedIn: true } && ts.Name == ex.Name && ex.backuptime != 0L && Timer % (3600L * ex.backuptime) == 0L)
                        {
                            ZPExtraDB.WriteExtraDB(ex);
                            ZPDataBase.AddZPlayerDB(ts);
                            ts.SendMessage(GetString("已自动备份您的人物存档，自动保存您的额外数据"), new Color(0, 255, 0));
                            TShock.Log.Info(GetString($"玩家【{ts.Name}】的人物存档和额外数据已备份和保存"));
                        }
                    }
                }
            }


            //清理世界无效数据处理
            if (Timer > this.clearTimer)
            {
                var asset = new List<int>
                {
                    NPCID.Creeper,
                    NPCID.LunarTowerNebula,
                    NPCID.LunarTowerSolar,
                    NPCID.LunarTowerStardust,
                    NPCID.LunarTowerVortex,
                    68,
                    128,
                    129,
                    130,
                    131,
                    400
                };

                foreach (var v in Main.npc)
                {
                    if (v.active && v is { boss: false, townNPC: false } && !asset.Contains(v.netID))
                    {
                        v.active = false;
                        NetMessage.SendData(23, -1, -1, null, v.whoAmI);
                    }
                }

                foreach (var v in Main.projectile)
                {
                    if (v.active)
                    {
                        v.active = false;
                        TSPlayer.All.SendData(PacketTypes.ProjectileDestroy, "", v.identity, v.owner);
                    }
                }

                foreach (var v in Main.item)
                {
                    if (v.active)
                    {
                        v.active = false;
                        TSPlayer.All.SendData(PacketTypes.ItemDrop, "", v.whoAmI);
                    }
                }

                this.clearTimer = long.MaxValue;
                TSPlayer.All.SendMessage(GetString("已清理所有射弹，物品，无用NPC"), new Color(65, 165, 238));
            }
        }

        //冻结处理
        if (frozenPlayers.Count != 0 && Timer % 2L == 0L)
        {
            foreach (var v in frozenPlayers)
            {
                TShock.Players.ForEach(x =>
                {
                    if (x is { IsLoggedIn: true } && (x.UUID.Equals(v.uuid) || x.Name.Equals(v.name) || (!string.IsNullOrEmpty(v.IPs) && !string.IsNullOrEmpty(x.IP) && ToIpStrings(v.IPs).Contains(x.IP))))
                    {
                        for (var i = 0; i < 22; i++)
                        {
                            switch (x.TPlayer.buffType[i])
                            {
                                case 149:
                                case 156:
                                case 47:
                                case 23:
                                case 31:
                                case 80:
                                case 88:
                                case 120:
                                case 145:
                                case 163:
                                case 199:
                                case 160:
                                case 197:
                                    break;
                                default:
                                    x.TPlayer.buffType[i] = 0;
                                    break;
                            }
                        }

                        x.SendData(PacketTypes.PlayerBuff, "", x.Index);
                        x.SetBuff(149, 720); //网住
                        x.SetBuff(156, 720); //石化
                        x.SetBuff(47, 300); //冰冻
                        x.SetBuff(23, 300); //诅咒
                        x.SetBuff(31, 300); //困惑
                        x.SetBuff(80, 300); //灯火管制
                        x.SetBuff(88, 300); //混沌
                        x.SetBuff(120, 300); //臭气
                        x.SetBuff(145, 300); //月食
                        x.SetBuff(163, 300); //阻塞
                        x.SetBuff(199, 300); //创意震撼
                        x.SetBuff(160, 300); //眩晕
                        x.SetBuff(197, 300); //粘液
                        if (Timer % 240L == 0)
                        {
                            x.SendInfoMessage(GetString("您已被冻结，详情请询问管理员"));
                            this.SendText(x, GetString("您已被冻结"), Color.Red, x.TPlayer.Center);
                        }

                        x.Teleport(v.pos.X, v.pos.Y);
                        if (Timer > v.clock + 90)
                        {
                            var flag = false;
                            foreach (var i in x.TPlayer.buffType)
                            {
                                if (i == 149)
                                {
                                    flag = true;
                                    break;
                                }
                            }

                            if (!flag)
                            {
                                NetMessage.SendPlayerDeath(x.Index, PlayerDeathReason.ByCustomReason(""), int.MaxValue, new Random().Next(-1, 1), false);
                                if (Timer % 240L == 0)
                                {
                                    x.SendInfoMessage(GetString("不要耍小聪明"));
                                }
                            }
                        }
                    }
                });
            }
        }
    }


    /// <summary>
    ///     对进入服务器的玩家进行一些限制
    /// </summary>
    /// <param name="args"></param>
    private void OnServerJoin(JoinEventArgs args)
    {
        if (TShock.Players[args.Who] == null)
        {
            return;
        }

        if (config.EnableSpecialNameBan)
        {
            var tsPlayer = TShock.Players[args.Who];
            if (string.IsNullOrWhiteSpace(tsPlayer.Name))
            {
                tsPlayer.Kick(GetString("请不要起空名字"), true);
            }
            else if ((tsPlayer.Name[0] >= ' ' && tsPlayer.Name[0] <= '/') || (tsPlayer.Name[0] >= ':' && tsPlayer.Name[0] <= '@') || (tsPlayer.Name[0] > '[' && tsPlayer.Name[0] <= '`') || (tsPlayer.Name[0] >= '{' && tsPlayer.Name[0] <= '~'))
            {
                tsPlayer.Kick(GetString("请不要在名字中使用特殊符号"), true);
            }
            else if (tsPlayer.Name.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                tsPlayer.Kick(GetString("你的名字含有指令关键字: all ，请更换"), true);
            }
            else if (tsPlayer.Name.Equals("time", StringComparison.OrdinalIgnoreCase))
            {
                tsPlayer.Kick(GetString("你的名字含有指令关键字: time ，请更换"), true);
            }
            else if (tsPlayer.Name.Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                tsPlayer.Kick(GetString("你的名字含有指令关键字: help ，请更换"), true);
            }
            else if (tsPlayer.Name.Equals("me", StringComparison.OrdinalIgnoreCase))
            {
                tsPlayer.Kick(GetString("你的名字含有指令关键字: me ，请更换"), true);
            }
            else if (tsPlayer.Name.Equals("uuid", StringComparison.OrdinalIgnoreCase))
            {
                tsPlayer.Kick(GetString("你的名字含有指令关键字: uuid ，请更换"), true);
            }
            else if (tsPlayer.Name.Equals("ip", StringComparison.OrdinalIgnoreCase))
            {
                tsPlayer.Kick(GetString("你的名字含有指令关键字: ip ，请更换"), true);
            }
        }
    }


    /// <summary>
    ///     对离开服务区的玩家的额外数据库，进行保存
    /// </summary>
    /// <param name="args"></param>
    private void OnServerLeave(LeaveEventArgs args)
    {
        if (TShock.Players[args.Who] == null)
        {
            return;
        }

        //清理掉这个离开服务器的玩家的额外数据内存
        foreach (var v in edPlayers)
        {
            if (v.Name == TShock.Players[args.Who].Name)
            {
                ZPExtraDB.WriteExtraDB(v);
                edPlayers.RemoveAll(x => x.Account == v.Account || x.Name == v.Name);
                break;
            }
        }

        //顺便遍历下整个edplayers，移除所有和tsplayers不同步的元素，免得越堆越多
        for (var i = 0; i < edPlayers.Count; i++)
        {
            var flag = false;
            foreach (var p in TShock.Players)
            {
                if (p is { IsLoggedIn: true } && (p.Name == edPlayers[i].Name || p.Account.ID == edPlayers[i].Account))
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                ZPExtraDB.WriteExtraDB(edPlayers[i]);
                edPlayers.RemoveAt(i);
                i--;
            }
        }
    }


    /// <summary>
    ///     对提示隐藏的指令
    /// </summary>
    /// <param name="args"></param>
    private void HideTips(CommandArgs args)
    {
        if (args.Parameters.Count != 1)
        {
            args.Player.SendInfoMessage(GetString("输入 /zhide kill  来取消 kill + 1 的显示，再次使用启用显示\n输入 /zhide point  来取消 + 1 $ 的显示，再次使用启用显示"));
            return;
        }

        if (!args.Player.RealPlayer)
        {
            args.Player.SendInfoMessage(GetString("对象不正确，请检查您的状态，您是否为游戏内玩家？"));
            return;
        }

        if (config.EnableNpcKillTracking && args.Parameters[0].Equals("kill", StringComparison.OrdinalIgnoreCase))
        {
            edPlayers.ForEach(x =>
            {
                if (x.Name == args.Player.Name)
                {
                    x.hideKillTips = !x.hideKillTips;
                    args.Player.SendMessage(GetString($"修改成功，您现在已{(x.hideKillTips ? GetString("隐藏") : GetString("启用"))}击杀数提示"), new Color(0, 255, 0));
                }
            });
        }
        else if (config.EnablePointTracking && args.Parameters[0].Equals("point", StringComparison.OrdinalIgnoreCase))
        {
            edPlayers.ForEach(x =>
            {
                if (x.Name == args.Player.Name)
                {
                    x.hidePointTips = !x.hidePointTips;
                    args.Player.SendMessage(GetString($"修改成功，您现在已{(x.hidePointTips ? GetString("隐藏") : GetString("启用"))}点数提示"), new Color(0, 255, 0));
                }
            });
        }
        else if (!config.EnableNpcKillTracking && args.Parameters[0].Equals("kill", StringComparison.OrdinalIgnoreCase))
        {
            args.Player.SendInfoMessage(GetString("未启用击杀NPC统计，该功能不可用"));
        }
        else if (!config.EnablePointTracking && args.Parameters[0].Equals("point", StringComparison.OrdinalIgnoreCase))
        {
            args.Player.SendInfoMessage(GetString("未启用点数统计，该功能不可用"));
        }
        else
        {
            args.Player.SendInfoMessage(GetString("输入 /zhide kill  来取消 kill + 1 的显示，再次使用启用显示\n输入 /zhide point  来取消 + 1 $ 的显示，再次使用启用显示"));
        }
    }


    /// <summary>
    ///     导出这个玩家的人物存档
    /// </summary>
    /// <param name="args"></param>
    private void ZhiExportPlayer(CommandArgs args)
    {
        if (args.Parameters.Count != 1)
        {
            args.Player.SendInfoMessage(GetString("输入 /zout <name>  来导出该玩家的人物存档\n") +
                                        GetString("输入 /zout all  来导出所有人物的存档"));
            return;
        }

        //对每个导出的文件夹做时间名称后缀
        this.now = "_" + this.FormatFileName(DateTime.Now.ToString(CultureInfo.CurrentCulture));
        if (args.Parameters[0].Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var players = new Dictionary<UserAccount, PlayerData>();
                using (var queryResult = TShock.DB.QueryReader("SELECT * FROM tsCharacter"))
                {
                    while (queryResult.Read())
                    {
                        var num = queryResult.Get<int>("Account");
                        var user = TShock.UserAccounts.GetUserAccountByID(num);
                        players.Add(user, TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), num));
                    }
                }

                args.Player.SendMessage(GetString("预计导出所有用户存档数目：") + players.Count, new Color(100, 233, 255));
                TShock.Log.Info(GetString("预计导出所有用户存档数目：") + players.Count);
                if (players.Count < 1)
                {
                    return;
                }

                var sb = new StringBuilder();
                var failedCount = 0;
                var worldName = this.FormatFileName(Main.worldName);
                foreach (var one in players)
                {
                    var player = this.CreateAPlayer(one.Key.Name, one.Value)!;
                    if (this.ExportPlayer(player, ZPExtraDB.getPlayerExtraDBTime(one.Key.ID)))
                    {
                        if (args.Player.RealPlayer)
                        {
                            args.Player.SendMessage(GetString($"用户 [{player.name}] 已导出，目录：tshock/Zhipm/{worldName + this.now}/{player.name}.plr"), new Color(0, 255, 0));
                        }
                        else
                        {
                            sb.AppendLine(GetString($"用户 [{player.name}] 已导出，目录：tshock/Zhipm/{worldName + this.now}/{player.name}.plr"));
                        }

                        TShock.Log.Info(GetString($"用户 [{player.name}] 已导出，目录：tshock/Zhipm/{worldName + this.now}/{player.name}.plr"));
                    }
                    else
                    {
                        if (args.Player.RealPlayer)
                        {
                            args.Player.SendInfoMessage(GetString($"用户 [{one.Key}] 因数据错误导出失败"));
                        }
                        else
                        {
                            sb.AppendLine(GetString($"用户 [{one.Key.Name}] 因数据错误导出失败"));
                        }

                        failedCount++;
                        TShock.Log.Info(GetString($"用户 [{one.Key.Name}] 因数据错误导出失败"));
                    }
                }

                sb.AppendLine(GetString($"{failedCount} 名用户因数据错误导出失败"));
                //压缩打包
                var sourcePath = $"{TShock.SavePath}/Zhipm/{worldName + this.now}";
                var destPath = $"{TShock.SavePath}/Zhipm/{worldName + this.now}.zip";
                if (!Directory.Exists(sourcePath))
                {
                    Directory.CreateDirectory(sourcePath);
                }

                ZipFile.CreateFromDirectory(sourcePath, destPath, CompressionLevel.SmallestSize, false);
                Directory.Delete(sourcePath, true);
                sb.AppendLine(GetString($"已打包为{TShock.SavePath}/Zhipm/{worldName + this.now}.zip"));
                if (!args.Player.RealPlayer)
                {
                    args.Player.SendInfoMessage(sb.ToString());
                }
                else
                {
                    args.Player.SendSuccessMessage(
                        GetString($"{failedCount} 名用户因数据残缺导出失败\n") +
                        GetString($"已全部打包为{TShock.SavePath}/Zhipm/{worldName + this.now}.zip"));
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleInfo(GetString("错误 ZhiExportPlayer ：") + ex);
            }

            return;
        }

        //只导出一人或搜到的多人
        var list = this.BestFindPlayerByNameOrIndex(args.Parameters[0]);
        if (list.Count == 0) //查不到，开始模糊搜索
        {
            args.Player.SendInfoMessage(this.playerOfflineWarning);
            var users = TShock.UserAccounts.GetUserAccountsByName(args.Parameters[0], true);
            if (users.Count == 1 || (users.Count > 1 && users.Exists(x => x.Name == args.Parameters[0])))
            {
                if (users.Count > 1)
                {
                    users[0] = users.Find(x => x.Name == args.Parameters[0]);
                }

                var playerData = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), users[0].ID);
                var player = this.CreateAPlayer(args.Parameters[0], playerData);
                if (this.ExportPlayer(player, ZPExtraDB.getPlayerExtraDBTime(users[0].ID)))
                {
                    args.Player.SendMessage(GetString($"导出成功！目录：tshock/Zhipm/{Main.worldName + this.now}/{args.Parameters[0]}.plr"), new Color(0, 255, 0));
                    TShock.Log.Info(GetString($"导出成功！目录：tshock/Zhipm/{Main.worldName + this.now}/{args.Parameters[0]}.plr"));
                }
                else
                {
                    args.Player.SendErrorMessage(GetString("导出失败，因数据错误"));
                    TShock.Log.Info(GetString("导出失败，因数据错误"));
                }
            }
            else if (users.Count == 0)
            {
                args.Player.SendInfoMessage(this.nonExistPlayerWarning);
            }
            else if (users.Count > 1)
            {
                args.Player.SendInfoMessage(this.MultipleMatchPlayerWarning);
            }
        }
        else if (list.Count > 1)
        {
            args.Player.SendInfoMessage(this.MultipleMatchPlayerWarning);
        }
        else if (this.ExportPlayer(list[0].TPlayer, ZPExtraDB.getPlayerExtraDBTime(list[0].Account.ID)))
        {
            args.Player.SendMessage(GetString($"导出成功！目录：tshock/Zhipm/{Main.worldName + this.now}/{list[0].Name}.plr"), new Color(0, 255, 0));
            TShock.Log.Info(GetString($"导出成功！目录：tshock/Zhipm/{Main.worldName + this.now}/{list[0].Name}.plr"));
        }
        else
        {
            args.Player.SendErrorMessage(GetString("导出失败，因输入错误"));
            TShock.Log.Info(GetString("导出失败，因输入错误"));
        }
    }


    /// <summary>
    ///     对玩家在线时常进行排序
    /// </summary>
    /// <param name="args"></param>
    private void ZhiSortPlayer(CommandArgs args)
    {
        if (args.Parameters.Count != 1 && args.Parameters.Count != 2)
        {
            args.Player.SendInfoMessage(GetString("输入 /zsort help  来查看排序系列指令帮助"));
            return;
        }

        //帮助指令
        if (args.Parameters[0].Equals("help", StringComparison.OrdinalIgnoreCase))
        {
            var temp1 = config.EnableOnlineTimeTracking
                ? GetString("\n输入 /zsort point [num] 来查看当前[num]个人物点数排行榜\n") +
                  GetString("输入 /zsort point 来查看人物点数排行榜前十名\n") +
                  GetString("输入 /zsort point all 来查看所有玩家点数排行榜\n")
                : "";
            var temp2 = config.EnableNpcKillTracking
                ? GetString("\n输入 /zsort death [num] 来查看当前[num]个人物死亡次数排行榜\n") +
                  GetString("输入 /zsort death 来查看人物死亡次数排行榜前十名\n") +
                  GetString("输入 /zsort death all 来查看所有玩家死亡次数排行榜\n")
                : "";
            var temp3 = config.EnablePointTracking
                ? GetString("\n输入 /zsort point [num] 来查看当前[num]个人物点数排行榜\n") +
                  GetString("输入 /zsort point 来查看人物点数排行榜前十名\n") +
                  GetString("输入 /zsort point all 来查看所有玩家点数排行榜\n")
                : "";
            var temp4 = config.EnableDeathCountTracking
                ? GetString("\n输入 /zsort death [num] 来查看当前[num]个人物死亡次数排行榜\n") +
                  GetString("输入 /zsort death 来查看人物死亡次数排行榜前十名\n") +
                  GetString("输入 /zsort death all 来查看所有玩家死亡次数排行榜\n")
                : "";
            var temp5 = config is { EnableDeathCountTracking: true, EnableOnlineTimeTracking: true }
                ? GetString("\n输入 /zsort clumsy 来查看人物手残排行榜前十名\n") +
                  GetString("输入 /zsort clumsy [num] 来查看当前[num]个人物手残排行榜\n") +
                  GetString("输入 /zsort clumsy all 来查看所有玩家手残排行榜\n")
                : "";

            args.Player.SendMessage(
                temp1 +
                GetString("输入 /zsort coin 来查看人物硬币数目排行榜前十名\n") +
                GetString("输入 /zsort coin [num] 来查看当前[num]个人物硬币数目排行榜\n") +
                GetString("输入 /zsort coin all 来查看所有玩家硬币数目排行榜\n") +
                GetString("输入 /zsort fish 来查看人物任务鱼数目排行榜前十名\n") +
                GetString("输入 /zsort fish [num] 来查看当前[num]个人物任务鱼数目排行榜\n") +
                GetString("输入 /zsort fish all 来查看所有玩家任务鱼数目排行榜\n") +
                temp4 + temp2 + temp3 + temp5, TextColor());
            return;
        }
        //时间排序

        if (config.EnableOnlineTimeTracking && args.Parameters[0].Equals("time", StringComparison.OrdinalIgnoreCase))
        {
            // time 排序前先保存
            foreach (var ex in edPlayers)
            {
                ZPExtraDB.WriteExtraDB(ex);
            }

            var list = ZPExtraDB.ListAllExtraDB(ExtraDataDate.time, false);
            if (list.Count == 0)
            {
                args.Player.SendInfoMessage(GetString("没有任何数据"));
            }
            else if (args.Parameters.Count == 1)
            {
                var num = 10;
                if (num > list.Count)
                {
                    num = list.Count;
                }

                var sb = new StringBuilder();
                for (var i = 0; i < num; i++)
                {
                    sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 在线时长 {this.Timetostring(list[i].time)}"));
                }

                args.Player.SendMessage(sb.ToString(), TextColor());
                TShock.Log.Info(sb.ToString());
            }
            else
            {
                if (int.TryParse(args.Parameters[1], out var count))
                {
                    if (count <= 0)
                    {
                        args.Player.SendInfoMessage(GetString("数字无效"));
                        return;
                    }

                    var sb = new StringBuilder();
                    if (count > list.Count)
                    {
                        sb.AppendLine(GetString($"当前最多 {list.Count} 人"));
                        count = list.Count;
                    }

                    for (var i = 0; i < count; i++)
                    {
                        sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 在线时长 {this.Timetostring(list[i].time)}"));
                    }

                    args.Player.SendMessage(sb.ToString(), TextColor());
                    TShock.Log.Info(sb.ToString());
                }
                else if (args.Parameters[1].Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i < list.Count; i++)
                    {
                        sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 在线时长 {this.Timetostring(list[i].time)}"));
                    }

                    args.Player.SendMessage(sb.ToString(), TextColor());
                    TShock.Log.Info(sb.ToString());
                }
                else
                {
                    args.Player.SendInfoMessage(
                        GetString("输入 /zsort time [num]  来查看当前[num]个人物在线时间排行榜\n") +
                        GetString("输入 /zsort time  来查看人物在线时间排行榜前十名\n输入 /zsort time all  来查看所有玩家在线时常排行榜"));
                }
            }
        }
        //钱币排序
        else if (args.Parameters[0].Equals("coin", StringComparison.OrdinalIgnoreCase))
        {
            var list = new List<UserAccount>();
            using (var queryResult = TShock.DB.QueryReader("SELECT * FROM tsCharacter"))
            {
                while (queryResult.Read())
                {
                    var num = queryResult.Get<int>("Account");
                    list.Add(TShock.UserAccounts.GetUserAccountByID(num));
                }
            }

            if (list.Count == 0)
            {
                args.Player.SendInfoMessage(GetString("没有任何数据"));
                return;
            }

            list.Sort((p1, p2) => this.GetPlayerCoin(p2.Name).CompareTo(this.GetPlayerCoin(p1.Name)));
            if (args.Parameters.Count == 1)
            {
                var num = 10;
                if (num > list.Count)
                {
                    num = list.Count;
                }

                var sb = new StringBuilder();
                for (var i = 0; i < num; i++)
                {
                    sb.AppendLine(args.Player.RealPlayer ? GetString($"第 {i + 1} 名:【{list[i].Name}】 总硬币数 {this.Cointostring(this.GetPlayerCoin(list[i].Name))}") : GetString($"第 {i + 1} 名:【{list[i].Name}】 总硬币数 {this.Cointostring(this.GetPlayerCoin(list[i].Name), 1)}"));
                }

                args.Player.SendMessage(sb.ToString(), TextColor());
                TShock.Log.Info(sb.ToString());
            }
            else
            {
                if (int.TryParse(args.Parameters[1], out var count))
                {
                    if (count <= 0)
                    {
                        args.Player.SendInfoMessage(GetString("数字无效"));
                        return;
                    }

                    var sb = new StringBuilder();
                    if (count > list.Count)
                    {
                        sb.AppendLine(GetString($"当前最多 {list.Count} 人"));
                        count = list.Count;
                    }

                    for (var i = 0; i < count; i++)
                    {
                        sb.AppendLine(args.Player.RealPlayer ? GetString($"第 {i + 1} 名:【{list[i].Name}】 总硬币数 {this.Cointostring(this.GetPlayerCoin(list[i].Name))}") : GetString($"第 {i + 1} 名:【{list[i].Name}】 总硬币数 {this.Cointostring(this.GetPlayerCoin(list[i].Name), 1)}"));
                    }

                    args.Player.SendMessage(sb.ToString(), TextColor());
                    TShock.Log.Info(sb.ToString());
                }
                else if (args.Parameters[1].Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i < list.Count; i++)
                    {
                        sb.AppendLine(args.Player.RealPlayer ? GetString($"第 {i + 1} 名:【{list[i].Name}】 总硬币数 {this.Cointostring(this.GetPlayerCoin(list[i].Name))}") : GetString($"第 {i + 1} 名:【{list[i].Name}】 总硬币数 {this.Cointostring(this.GetPlayerCoin(list[i].Name), 1)}"));
                    }

                    args.Player.SendMessage(sb.ToString(), TextColor());
                    TShock.Log.Info(sb.ToString());
                }
                else
                {
                    args.Player.SendInfoMessage(
                        GetString("输入 /zsort coin  来查看人物硬币数目排行榜前十名\n") +
                        GetString("输入 /zsort coin [num]  来查看当前[num]个人物硬币数目排行榜\n") +
                        GetString("输入 /zsort coin all  来查看所有玩家硬币数目排行榜"));
                }
            }
        }
        //钓鱼任务排序
        else if (args.Parameters[0].Equals("fish", StringComparison.OrdinalIgnoreCase))
        {
            var list = new List<UserAccount>();
            using (var queryResult = TShock.DB.QueryReader("SELECT * FROM tsCharacter ORDER BY questsCompleted DESC"))
            {
                while (queryResult.Read())
                {
                    var num = queryResult.Get<int>("Account");
                    list.Add(TShock.UserAccounts.GetUserAccountByID(num));
                }
            }

            if (list.Count == 0)
            {
                args.Player.SendInfoMessage(GetString("没有任何数据"));
                return;
            }

            if (args.Parameters.Count == 1)
            {
                var num = 10;
                if (num > list.Count)
                {
                    num = list.Count;
                }

                var sb = new StringBuilder();
                for (var i = 0; i < num; i++)
                {
                    sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 总完成任务鱼数 {TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), list[i].ID).questsCompleted}"));
                }

                args.Player.SendMessage(sb.ToString(), TextColor());
                TShock.Log.Info(sb.ToString());
            }
            else
            {
                if (int.TryParse(args.Parameters[1], out var count))
                {
                    if (count <= 0)
                    {
                        args.Player.SendInfoMessage(GetString("数字无效"));
                        return;
                    }

                    var sb = new StringBuilder();
                    if (count > list.Count)
                    {
                        sb.AppendLine(GetString($"当前最多 {list.Count} 人"));
                        count = list.Count;
                    }

                    for (var i = 0; i < count; i++)
                    {
                        sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 总完成任务鱼数 {TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), list[i].ID).questsCompleted}"));
                    }

                    args.Player.SendMessage(sb.ToString(), TextColor());
                    TShock.Log.Info(sb.ToString());
                }
                else if (args.Parameters[1].Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i < list.Count; i++)
                    {
                        sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 总完成任务鱼数 {TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), list[i].ID).questsCompleted}"));
                    }

                    args.Player.SendMessage(sb.ToString(), TextColor());
                    TShock.Log.Info(sb.ToString());
                }
                else
                {
                    args.Player.SendInfoMessage(
                        GetString("输入 /zsort fish  来查看人物任务鱼数目排行榜前十名\n") +
                        GetString("输入 /zsort fish [num]  来查看当前[num]个人物任务鱼数目排行榜\n") +
                        GetString("输入 /zsort fish all  来查看所有玩家任务鱼数目排行榜"));
                }
            }
        }
        //斩杀数排序
        else if (config.EnableNpcKillTracking && args.Parameters[0].Equals("kill", StringComparison.OrdinalIgnoreCase))
        {
            //排序前先保存
            foreach (var ex in edPlayers)
            {
                ZPExtraDB.WriteExtraDB(ex);
            }

            var list = ZPExtraDB.ListAllExtraDB(ExtraDataDate.killNPCnum, false);
            if (list.Count == 0)
            {
                args.Player.SendInfoMessage(GetString("没有任何数据"));
            }
            else if (args.Parameters.Count == 1)
            {
                var num = 10;
                if (num > list.Count)
                {
                    num = list.Count;
                }

                var sb = new StringBuilder();
                for (var i = 0; i < num; i++)
                {
                    sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 击杀生物总数 {list[i].killNPCnum} 个"));
                }

                args.Player.SendMessage(sb.ToString(), TextColor());
                TShock.Log.Info(sb.ToString());
            }
            else
            {
                if (int.TryParse(args.Parameters[1], out var count))
                {
                    if (count <= 0)
                    {
                        args.Player.SendInfoMessage(GetString("数字无效"));
                        return;
                    }

                    var sb = new StringBuilder();
                    if (count > list.Count)
                    {
                        sb.AppendLine(GetString($"当前最多 {list.Count} 人"));
                        count = list.Count;
                    }

                    for (var i = 0; i < count; i++)
                    {
                        sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 击杀生物总数 {list[i].killNPCnum} 个"));
                    }

                    args.Player.SendMessage(sb.ToString(), TextColor());
                    TShock.Log.Info(sb.ToString());
                }
                else if (args.Parameters[1].Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i < list.Count; i++)
                    {
                        sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 击杀生物总数 {list[i].killNPCnum} 个"));
                    }

                    args.Player.SendMessage(sb.ToString(), TextColor());
                    TShock.Log.Info(sb.ToString());
                }
                else
                {
                    args.Player.SendInfoMessage(
                        GetString("输入 /zsort kill [num]  来查看当前[num]个人物击杀生物数排行榜\n") +
                        GetString("输入 /zsort kill  来查看人物击杀生物数排行榜前十名\n") +
                        GetString("输入 /zsort kill all  来查看所有玩家击杀生物数排行榜"));
                }
            }
        }
        //斩杀Boss排序
        else if (config.EnableNpcKillTracking && args.Parameters[0].Equals("boss", StringComparison.OrdinalIgnoreCase))
        {
            //排序前先保存
            foreach (var ex in edPlayers)
            {
                ZPExtraDB.WriteExtraDB(ex);
            }

            var list = ZPExtraDB.ListAllExtraDB();
            if (list.Count == 0)
            {
                args.Player.SendInfoMessage(GetString("没有任何数据"));
                return;
            }

            list.Sort((p1, p2) => this.GetKillNumFromDictionary(p2.killBossID).CompareTo(this.GetKillNumFromDictionary(p1.killBossID)));
            if (args.Parameters.Count == 1)
            {
                var num = 10;
                if (num > list.Count)
                {
                    num = list.Count;
                }

                var sb = new StringBuilder();
                for (var i = 0; i < num; i++)
                {
                    sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 击杀Boss总数 {this.GetKillNumFromDictionary(list[i].killBossID)} 个"));
                }

                args.Player.SendMessage(sb.ToString(), TextColor());
                TShock.Log.Info(sb.ToString());
            }
            else
            {
                if (int.TryParse(args.Parameters[1], out var count))
                {
                    if (count <= 0)
                    {
                        args.Player.SendInfoMessage(GetString("数字无效"));
                        return;
                    }

                    var sb = new StringBuilder();
                    if (count > list.Count)
                    {
                        sb.AppendLine(GetString($"当前最多 {list.Count} 人"));
                        count = list.Count;
                    }

                    for (var i = 0; i < count; i++)
                    {
                        sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 击杀Boss总数 {this.GetKillNumFromDictionary(list[i].killBossID)} 个"));
                    }

                    args.Player.SendMessage(sb.ToString(), TextColor());
                    TShock.Log.Info(sb.ToString());
                }
                else if (args.Parameters[1].Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i < list.Count; i++)
                    {
                        sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 击杀Boss总数 {this.GetKillNumFromDictionary(list[i].killBossID)} 个"));
                    }

                    args.Player.SendMessage(sb.ToString(), TextColor());
                    TShock.Log.Info(sb.ToString());
                }
                else
                {
                    args.Player.SendInfoMessage(
                        GetString("输入 /zsort boss [num]  来查看当前[num]个人物击杀Boss总数排行榜\n") +
                        GetString("输入 /zsort boss  来查看人物击杀Boss总数排行榜前十名\n") +
                        GetString("输入 /zsort boss all  来查看所有玩家击杀Boss总数排行榜"));
                }
            }
        }
        //斩杀罕见生物排序
        else if (config.EnableNpcKillTracking && args.Parameters[0].Equals("rarenpc", StringComparison.OrdinalIgnoreCase))
        {
            //排序前先保存
            foreach (var ex in edPlayers)
            {
                ZPExtraDB.WriteExtraDB(ex);
            }

            var list = ZPExtraDB.ListAllExtraDB();
            if (list.Count == 0)
            {
                args.Player.SendInfoMessage(GetString("没有任何数据"));
                return;
            }

            list.Sort((p1, p2) => this.GetKillNumFromDictionary(p2.killRareNPCID).CompareTo(this.GetKillNumFromDictionary(p1.killRareNPCID)));
            if (args.Parameters.Count == 1)
            {
                var num = 10;
                if (num > list.Count)
                {
                    num = list.Count;
                }

                var sb = new StringBuilder();
                for (var i = 0; i < num; i++)
                {
                    sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 击杀罕见生物总数 {this.GetKillNumFromDictionary(list[i].killRareNPCID)} 个"));
                }

                args.Player.SendMessage(sb.ToString(), TextColor());
                TShock.Log.Info(sb.ToString());
            }
            else
            {
                if (int.TryParse(args.Parameters[1], out var count))
                {
                    if (count <= 0)
                    {
                        args.Player.SendInfoMessage(GetString("数字无效"));
                        return;
                    }

                    var sb = new StringBuilder();
                    if (count > list.Count)
                    {
                        sb.AppendLine(GetString($"当前最多 {list.Count} 人"));
                        count = list.Count;
                    }

                    for (var i = 0; i < count; i++)
                    {
                        sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 击杀罕见生物总数 {this.GetKillNumFromDictionary(list[i].killRareNPCID)} 个"));
                    }

                    args.Player.SendMessage(sb.ToString(), TextColor());
                    TShock.Log.Info(sb.ToString());
                }
                else if (args.Parameters[1].Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i < list.Count; i++)
                    {
                        sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 击杀罕见生物总数 {this.GetKillNumFromDictionary(list[i].killRareNPCID)} 个"));
                    }

                    args.Player.SendMessage(sb.ToString(), TextColor());
                    TShock.Log.Info(sb.ToString());
                }
                else
                {
                    args.Player.SendInfoMessage(
                        GetString("输入 /zsort rarenpc [num]  来查看当前[num]个人物击杀罕见生物总数排行榜\n") +
                        GetString("输入 /zsort rarenpc  来查看人物击杀罕见生物总数排行榜前十名\n") +
                        GetString("输入 /zsort rarenpc all  来查看所有玩家击杀罕见生物总数排行榜"));
                }
            }
        }
        //点数排行
        else if (config.EnablePointTracking && args.Parameters[0].Equals("point", StringComparison.OrdinalIgnoreCase))
        {
            //排序前先保存
            foreach (var ex in edPlayers)
            {
                ZPExtraDB.WriteExtraDB(ex);
            }

            var list = ZPExtraDB.ListAllExtraDB(ExtraDataDate.point, false);
            if (list.Count == 0)
            {
                args.Player.SendInfoMessage(GetString("没有任何数据"));
                return;
            }

            if (args.Parameters.Count == 1)
            {
                var num = 10;
                if (num > list.Count)
                {
                    num = list.Count;
                }

                var sb = new StringBuilder();
                for (var i = 0; i < num; i++)
                {
                    sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 点数 {list[i].point} "));
                }

                args.Player.SendMessage(sb.ToString(), TextColor());
                TShock.Log.Info(sb.ToString());
            }
            else
            {
                if (int.TryParse(args.Parameters[1], out var count))
                {
                    if (count <= 0)
                    {
                        args.Player.SendInfoMessage(GetString("数字无效"));
                        return;
                    }

                    var sb = new StringBuilder();
                    if (count > list.Count)
                    {
                        sb.AppendLine(GetString($"当前最多 {list.Count} 人"));
                        count = list.Count;
                    }

                    for (var i = 0; i < count; i++)
                    {
                        sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 点数 {list[i].point}"));
                    }

                    args.Player.SendMessage(sb.ToString(), TextColor());
                    TShock.Log.Info(sb.ToString());
                }
                else if (args.Parameters[1].Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i < list.Count; i++)
                    {
                        sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 点数 {list[i].point}"));
                    }

                    args.Player.SendMessage(sb.ToString(), TextColor());
                    TShock.Log.Info(sb.ToString());
                }
                else
                {
                    args.Player.SendInfoMessage(
                        GetString("输入 /zsort point [num]  来查看当前[num]个人物点数排行榜\n") +
                        GetString("输入 /zsort point  来查看人物点数排行榜前十名\n") +
                        GetString("输入 /zsort point all  来查看所有玩家点数排行榜"));
                }
            }
        }
        //死亡次数排行
        else if (config.EnableDeathCountTracking && args.Parameters[0].Equals("death", StringComparison.OrdinalIgnoreCase))
        {
            //排序前先保存
            foreach (var ex in edPlayers)
            {
                ZPExtraDB.WriteExtraDB(ex);
            }

            var list = ZPExtraDB.ListAllExtraDB(ExtraDataDate.deathCount, false);
            if (list.Count == 0)
            {
                args.Player.SendInfoMessage(GetString("没有任何数据"));
                return;
            }

            if (args.Parameters.Count == 1)
            {
                var num = 10;
                if (num > list.Count)
                {
                    num = list.Count;
                }

                var sb = new StringBuilder();
                for (var i = 0; i < num; i++)
                {
                    sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 死亡次数 {list[i].deathCount} "));
                }

                args.Player.SendMessage(sb.ToString(), TextColor());
                TShock.Log.Info(sb.ToString());
            }
            else
            {
                if (int.TryParse(args.Parameters[1], out var count))
                {
                    if (count <= 0)
                    {
                        args.Player.SendInfoMessage(GetString("数字无效"));
                        return;
                    }

                    var sb = new StringBuilder();
                    if (count > list.Count)
                    {
                        sb.AppendLine(GetString($"当前最多 {list.Count} 人"));
                        count = list.Count;
                    }

                    for (var i = 0; i < count; i++)
                    {
                        sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 死亡次数 {list[i].deathCount}"));
                    }

                    args.Player.SendMessage(sb.ToString(), TextColor());
                    TShock.Log.Info(sb.ToString());
                }
                else if (args.Parameters[1].Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i < list.Count; i++)
                    {
                        sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 死亡次数 {list[i].deathCount}"));
                    }

                    args.Player.SendMessage(sb.ToString(), TextColor());
                    TShock.Log.Info(sb.ToString());
                }
                else
                {
                    args.Player.SendInfoMessage(
                        GetString("输入 /zsort death [num]  来查看当前[num]个人物死亡次数排行榜\n") +
                        GetString("输入 /zsort death  来查看人物死亡次数排行榜前十名\n") +
                        GetString("输入 /zsort death all  来查看所有玩家死亡次数排行榜"));
                }
            }
        }
        //菜鸡榜
        else if (config is { EnableDeathCountTracking: true, EnableOnlineTimeTracking: true } && args.Parameters[0].Equals("clumsy", StringComparison.OrdinalIgnoreCase))
        {
            //排序前先保存
            foreach (var ex in edPlayers)
            {
                ZPExtraDB.WriteExtraDB(ex);
            }

            var list = ZPExtraDB.ListAllExtraDB();
            if (list.Count == 0)
            {
                args.Player.SendInfoMessage(GetString("没有任何数据"));
                return;
            }

            list.Sort((p1, p2) =>
            {
                double k1 = 0.0, k2 = 0.0;
                if (p1.time > 0L)
                {
                    k1 = p1.deathCount * 100.0 / p1.time;
                }

                if (p2.time > 0L)
                {
                    k2 = p2.deathCount * 100.0 / p2.time;
                }

                return k2.CompareTo(k1);
            });
            if (args.Parameters.Count == 1)
            {
                var num = 10;
                if (num > list.Count)
                {
                    num = list.Count;
                }

                var sb = new StringBuilder();
                for (var i = 0; i < num; i++)
                {
                    if (args.Player.RealPlayer)
                    {
                        sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 菜鸡值 {list[i].deathCount * 1000.0 / list[i].time:0.00}"));
                    }
                    else
                    {
                        sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 菜鸡值 {list[i].deathCount * 1000.0 / list[i].time:0.00}"));
                    }
                }

                args.Player.SendMessage(sb.ToString(), TextColor());
                TShock.Log.Info(sb.ToString());
            }
            else
            {
                if (int.TryParse(args.Parameters[1], out var count))
                {
                    if (count <= 0)
                    {
                        args.Player.SendInfoMessage(GetString("数字无效"));
                        return;
                    }

                    var sb = new StringBuilder();
                    if (count > list.Count)
                    {
                        sb.AppendLine(GetString($"当前最多 {list.Count} 人"));
                        count = list.Count;
                    }

                    for (var i = 0; i < count; i++)
                    {
                        if (args.Player.RealPlayer)
                        {
                            sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 菜鸡值 {list[i].deathCount * 1000.0 / list[i].time:0.00}"));
                        }
                        else
                        {
                            sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 菜鸡值 {list[i].deathCount * 1000.0 / list[i].time:0.00}"));
                        }
                    }

                    args.Player.SendMessage(sb.ToString(), TextColor());
                    TShock.Log.Info(sb.ToString());
                }
                else if (args.Parameters[1].Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i < list.Count; i++)
                    {
                        if (args.Player.RealPlayer)
                        {
                            sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 菜鸡值 {list[i].deathCount * 1000.0 / list[i].time:0.00}"));
                        }
                        else
                        {
                            sb.AppendLine(GetString($"第 {i + 1} 名:【{list[i].Name}】 菜鸡值 {list[i].deathCount * 1000.0 / list[i].time:0.00}"));
                        }
                    }

                    args.Player.SendMessage(sb.ToString(), TextColor());
                    TShock.Log.Info(sb.ToString());
                }
                else
                {
                    args.Player.SendInfoMessage(
                        GetString("输入 /zsort clumsy  来查看人物手残排行榜前十名\n") +
                        GetString("输入 /zsort clumsy [num]  来查看当前[num]个人物手残排行榜\n") +
                        GetString("输入 /zsort clumsy all  来查看所有玩家手残排行榜"));
                }
            }
        }
        else
        {
            args.Player.SendInfoMessage(GetString("输入 /zsort help  来查看排序系列指令帮助"));
        }
    }


    /// <summary>
    ///     办掉离线或在线的玩家，超级ban指令
    /// </summary>
    /// <param name="args"></param>
    private void SuperBan(CommandArgs args)
    {
        if (args.Parameters.Count < 2)
        {
            args.Player.SendInfoMessage(
                GetString("输入 /zban add <name> [reason] 来封禁无论是否在线的玩家，reason 可不填\n") +
                GetString("输入 /zban add uuid <uuid> [reason] 来封禁uuid\n") +
                GetString("输入 /zban add ip <ip> [reason] 来封禁ip\n"));
            return;
        }

        if (args.Parameters[0].Equals("add", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Parameters[1].Equals("uuid", StringComparison.OrdinalIgnoreCase))
            {
                if (args.Parameters.Count < 3 || string.IsNullOrWhiteSpace(args.Parameters[2]))
                {
                    args.Player.SendInfoMessage(GetString("参数过少"));
                    return;
                }

                var reason = args.Parameters.Count == 4 ? args.Parameters[3] : GetString("你已被管理员封禁！");
                TSPlayer? suspect = null;
                foreach (var i in TShock.Players)
                {
                    if (i is { Active: true } && i.UUID == args.Parameters[2])
                    {
                        suspect = i;
                        break;
                    }
                }

                if (suspect != null && suspect.Ban(reason, args.Player.Name))
                {
                    args.Player.SendMessage(GetString($"用户 {suspect.Name} 已被 {args.Player.Name} 封禁"), broadcastColor);
                    TShock.Log.Info(GetString($"用户 {suspect.Name} 已被 {args.Player.Name} 封禁"));
                }
                else
                {
                    TShock.Bans.InsertBan("uuid:" + args.Parameters[2], reason, args.Player.Name, DateTime.UtcNow, DateTime.MaxValue);
                    TSPlayer.All.SendMessage(GetString($"uuid: {args.Parameters[2]} 已被 {args.Player.Name} 封禁"), broadcastColor);
                    TShock.Log.Info(GetString($"uuid: {args.Parameters[2]} 已被 {args.Player.Name} 封禁"));
                }
            }
            else if (args.Parameters[1].Equals("ip", StringComparison.OrdinalIgnoreCase))
            {
                if (args.Parameters.Count < 3 || string.IsNullOrWhiteSpace(args.Parameters[2]))
                {
                    args.Player.SendInfoMessage(GetString("参数过少"));
                    return;
                }

                var reason = args.Parameters.Count == 4 ? args.Parameters[3] : GetString("你已被管理员封禁！");
                TSPlayer? suspect = null;
                foreach (var v in TShock.Players)
                {
                    if (v is { Active: true } && v.IP == args.Parameters[2])
                    {
                        suspect = v;
                        break;
                    }
                }

                if (suspect != null && suspect.Ban(reason, args.Player.Name))
                {
                    args.Player.SendMessage(GetString($"用户 {suspect.Name} 已被 {args.Player.Name} 封禁"), broadcastColor);
                    TShock.Log.Info(GetString($"用户 {suspect.Name} 已被 {args.Player.Name} 封禁"));
                }
                else
                {
                    TShock.Bans.InsertBan("ip:" + args.Parameters[2], reason, args.Player.Name, DateTime.UtcNow, DateTime.MaxValue);
                    TSPlayer.All.SendMessage(GetString($"ip: {args.Parameters[2]} 已被 {args.Player.Name} 封禁"), broadcastColor);
                    TShock.Log.Info(GetString($"ip: {args.Parameters[2]} 已被 {args.Player.Name} 封禁"));
                }
            }
            else //正常封禁玩家
            {
                var list = this.BestFindPlayerByNameOrIndex(args.Parameters[1]);
                //封禁原因，可不填
                var reason = args.Parameters.Count == 3 ? args.Parameters[2] : GetString("你已被管理员封禁！");
                if (list.Count == 1)
                {
                    if (list[0].Ban(reason, args.Player.Name))
                    {
                        args.Player.SendMessage(GetString($"用户 {list[0].Name} 已被 {args.Player.Name} 封禁"), broadcastColor);
                        TShock.Log.Info(GetString($"用户 {list[0].Name} 已被 {args.Player.Name} 封禁"));
                    }
                    else
                    {
                        //实际上这个情况永远不会发生，因为Ban方法的返回值就没返回false过
                        args.Player.SendInfoMessage(GetString($"用户 {list[0].Name} 封禁失败，可能该玩家已被封禁或所在组被禁止封禁"));
                        TShock.Log.Info(GetString($"用户 {list[0].Name} 封禁失败，可能该玩家已被封禁或所在组被禁止封禁"));
                    }
                }
                else if (list.Count > 1)
                {
                    args.Player.SendInfoMessage(this.MultipleMatchPlayerWarning);
                }
                //离线查找
                else
                {
                    args.Player.SendInfoMessage(this.playerOfflineWarning);
                    var user = TShock.UserAccounts.GetUserAccountByName(args.Parameters[1]);
                    if (user == null)
                    {
                        args.Player.SendInfoMessage(GetString("精准查找未找到，正在尝试模糊查找"));
                        var users = TShock.UserAccounts.GetUserAccountsByName(args.Parameters[1], true);
                        if (users.Count == 1)
                        {
                            user = users[0];
                        }
                        else if (users.Count > 1)
                        {
                            args.Player.SendInfoMessage(GetString("人数不唯一，为避免误封，请重新输入。若玩家名称带有空格可用英文引号将名称整个括起来"));
                            return;
                        }
                        else
                        {
                            args.Player.SendInfoMessage(GetString($"{this.nonExistPlayerWarning}。若玩家名称带有空格可用英文引号将名称整个括起来"));
                            return;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(user.Name))
                    {
                        TShock.Bans.InsertBan("acc:" + user.Name, reason, args.Player.Name, DateTime.UtcNow, DateTime.MaxValue);
                    }

                    if (!string.IsNullOrWhiteSpace(user.UUID))
                    {
                        TShock.Bans.InsertBan("uuid:" + user.UUID, reason, args.Player.Name, DateTime.UtcNow, DateTime.MaxValue);
                    }

                    if (!string.IsNullOrWhiteSpace(user.KnownIps))
                    {
                        var ips = ToIpStrings(user.KnownIps);
                        foreach (var str in ips)
                        {
                            if (!string.IsNullOrWhiteSpace(str))
                            {
                                TShock.Bans.InsertBan("ip:" + str, reason, args.Player.Name, DateTime.UtcNow, DateTime.MaxValue);
                            }
                        }
                    }

                    if (!args.Player.RealPlayer)
                    {
                        args.Player.SendMessage(GetString($"用户 {user.Name} 已被 {args.Player.Name} 封禁"), broadcastColor);
                    }

                    TSPlayer.All.SendMessage(GetString($"用户 {user.Name} 已被 {args.Player.Name} 封禁"), broadcastColor);
                    TShock.Log.Info(GetString($"用户 {user.Name} 已被 {args.Player.Name} 封禁"));
                }
            }
        }
        else
        {
            args.Player.SendInfoMessage(
                GetString("输入 /zban add [name] [reason] 来封禁无论是否在线的玩家，reason 可不填\n") +
                GetString("输入 /zban add uuid [uuid] [reason] 来封禁uuid\n") +
                GetString("输入 /zban add ip [ip] [reason] 来封禁ip\n"));
        }
    }


    /// <summary>
    ///     冻结该玩家，禁止他做出任何操作
    /// </summary>
    /// <param name="args"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void ZFreeze(CommandArgs args)
    {
        if (args.Parameters.Count != 1)
        {
            args.Player.SendInfoMessage(GetString("输入 /zfre [name]  来冻结该玩家"));
            return;
        }

        var ts = this.BestFindPlayerByNameOrIndex(args.Parameters[0]);
        if (ts.Count == 0)
        {
            args.Player.SendInfoMessage(this.playerOfflineWarning);
            var user = TShock.UserAccounts.GetUserAccountByName(args.Parameters[0]);
            if (user != null)
            {
                if (frozenPlayers.Exists(x => x.name == user.Name && x.uuid == user.UUID && x.IPs.Equals(user.KnownIps)))
                {
                    args.Player.SendMessage(GetString($"玩家 [{user.Name}] 已冻结过!"), new Color(0, 255, 0));
                }
                else
                {
                    frozenPlayers.Add(new MessPlayer(user.ID, user.Name, user.UUID, user.KnownIps, Vector2.Zero));
                    args.Player.SendMessage(GetString($"玩家 [{user.Name}] 冻结成功"), new Color(0, 255, 0));
                }
            }
            else
            {
                args.Player.SendInfoMessage(GetString("精准查找未找到，正在尝试模糊查找"));
                var users = TShock.UserAccounts.GetUserAccountsByName(args.Parameters[0], true);
                if (users.Count == 1)
                {
                    if (frozenPlayers.Exists(x => x.name == users[0].Name && x.uuid == users[0].UUID && x.IPs.Equals(users[0].KnownIps)))
                    {
                        args.Player.SendMessage(GetString($"玩家 [{users[0].Name}] 已冻结过!"), new Color(0, 255, 0));
                    }
                    else
                    {
                        frozenPlayers.Add(new MessPlayer(users[0].ID, users[0].Name, users[0].UUID, users[0].KnownIps, Vector2.Zero));
                        args.Player.SendMessage(GetString($"玩家 [{users[0].Name}] 冻结成功"), new Color(0, 255, 0));
                    }
                }
                else if (users.Count > 1)
                {
                    args.Player.SendInfoMessage(this.MultipleMatchPlayerWarning);
                }
                else
                {
                    args.Player.SendInfoMessage(this.nonExistPlayerWarning);
                }
            }
        }
        else if (ts.Count > 1)
        {
            args.Player.SendInfoMessage(this.MultipleMatchPlayerWarning);
        }
        else
        {
            if (frozenPlayers.Exists(x => x.name == ts[0].Name && x.uuid == ts[0].UUID && x.IPs.Equals(ts[0].Account.KnownIps)))
            {
                args.Player.SendMessage(GetString($"玩家 [{ts[0].Name}] 已冻结过!"), new Color(0, 255, 0));
            }
            else
            {
                ClearAllBuffFromPlayer(ts[0]);
                frozenPlayers.Add(new MessPlayer(ts[0].Account.ID, ts[0].Name, ts[0].UUID, ts[0].Account.KnownIps, ts[0].TPlayer.Center));
                args.Player.SendMessage(GetString($"玩家 [{ts[0].Name}] 冻结成功"), new Color(0, 255, 0));
            }
        }
    }


    /// <summary>
    ///     取消冻结该玩家
    /// </summary>
    /// <param name="args"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void ZUnFreeze(CommandArgs args)
    {
        if (args.Parameters.Count != 1)
        {
            args.Player.SendInfoMessage(GetString("输入 /zunfre [name]  来解冻该玩家\n输入 /zunfre all  来解冻所有玩家"));
            return;
        }

        if (args.Parameters[0].Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            frozenPlayers.ForEach(x =>
            {
                var ts = this.BestFindPlayerByNameOrIndex(x.name);
                if (ts.Count > 0 && ts[0].Name == x.name)
                {
                    ClearAllBuffFromPlayer(ts[0]);
                }
            });
            frozenPlayers.Clear();
            args.Player.SendMessage(GetString("所有玩家均已解冻"), new Color(0, 255, 0));
            return;
        }

        var ts = this.BestFindPlayerByNameOrIndex(args.Parameters[0]);
        if (ts.Count == 0)
        {
            args.Player.SendInfoMessage(this.playerOfflineWarning);
            var user = TShock.UserAccounts.GetUserAccountByName(args.Parameters[0]);
            if (user != null)
            {
                var c = frozenPlayers.RemoveAll(x => x.uuid == user.UUID || x.name == user.Name || (!string.IsNullOrEmpty(x.IPs) && !string.IsNullOrEmpty(user.KnownIps) && ToIpStrings(x.IPs).Any(y => ToIpStrings(user.KnownIps).Contains(y))) || (string.IsNullOrEmpty(x.IPs) && string.IsNullOrEmpty(user.KnownIps)));
                args.Player.SendMessage(c > 0 ? GetString($"玩家 [{user.Name}] 已解冻") : GetString($"玩家 [{user.Name}] 未被冻结！"), new Color(0, 255, 0));
            }
            else
            {
                args.Player.SendInfoMessage(GetString("精准查找未找到，正在尝试模糊查找"));
                var users = TShock.UserAccounts.GetUserAccountsByName(args.Parameters[0], true);
                if (users.Count > 1)
                {
                    args.Player.SendInfoMessage(this.MultipleMatchPlayerWarning);
                    return;
                }

                if (users.Count == 0)
                {
                    args.Player.SendInfoMessage(this.nonExistPlayerWarning);
                    return;
                }

                var c = frozenPlayers.RemoveAll(x => x.uuid == users[0].UUID || x.name == users[0].Name || (!string.IsNullOrEmpty(x.IPs) && !string.IsNullOrEmpty(users[0].KnownIps) && ToIpStrings(x.IPs).Any(y => ToIpStrings(users[0].KnownIps).Contains(y))) || (string.IsNullOrEmpty(x.IPs) && string.IsNullOrEmpty(users[0].KnownIps)));
                args.Player.SendMessage(c > 0 ? GetString($"玩家 [{users[0].Name}] 已解冻") : GetString($"玩家 [{users[0].Name}] 未被冻结"), new Color(0, 255, 0));
            }
        }
        else if (ts.Count > 1)
        {
            args.Player.SendInfoMessage(this.MultipleMatchPlayerWarning);
        }
        else
        {
            var c = frozenPlayers.RemoveAll(x => x.uuid == ts[0].UUID || x.name == ts[0].Name || (!string.IsNullOrEmpty(x.IPs) && !string.IsNullOrEmpty(ts[0].IP) && ToIpStrings(x.IPs).Any(y => ts[0].IP == y)));
            if (c > 0)
            {
                ClearAllBuffFromPlayer(ts[0]);
                args.Player.SendMessage(GetString($"玩家 [{ts[0].Name}] 已解冻"), new Color(0, 255, 0));
                ts[0].SendMessage(GetString("您已被解冻"), new Color(0, 255, 0));
            }
            else
            {
                args.Player.SendMessage(GetString($"玩家 [{ts[0].Name}] 未被冻结"), new Color(0, 255, 0));
            }
        }
    }


    /// <summary>
    ///     击中npc时进行标记
    /// </summary>
    /// <param name="args"></param>
    private void OnNpcStrike(NpcStrikeEventArgs args)
    {
        //如果 击中的玩家是空的，或npc是傀儡，或npc是飞弹，或npc是城镇npc，或者他是雕像怪，结束
        if (!config.EnableNpcKillTracking || args.Player == null || args.Npc.netID == 488 || args.Npc.lifeMax == 1 || args.Npc.townNPC || args.Npc.SpawnedFromStatue)
        {
            return;
        }

        var players = this.BestFindPlayerByNameOrIndex(args.Player.name);
        if (players.Count == 0)
        {
            return;
        }

        //这个生物是否以前被击中过
        var strike = strikeNPC.Find(x => x.index == args.Npc.whoAmI && x.id == args.Npc.netID);
        if (strike != null && strike.name != string.Empty)
        {
            //如果击中过，寻找击中他的玩家是否被记录
            if (strike.playerAndDamage.ContainsKey(players[0].Account.ID))
            {
                //已被记录，那么伤害记录加数值
                if (args.Damage <= TShock.Config.Settings.MaxDamage && args.Damage <= TShock.Config.Settings.MaxProjDamage) //不正常的伤害应舍去
                {
                    strike.playerAndDamage[players[0].Account.ID] += args.Damage;
                    strike.AllDamage += args.Damage;
                }
            }
            else //否则，创建新的 player->damage
            {
                strike.playerAndDamage.Add(players[0].Account.ID, args.Damage);
                strike.AllDamage += args.Damage;
            }
        }
        else //如果没有击中过，创建新的 npc
        {
            var snpc = new StrikeNPC { id = args.Npc.netID, index = args.Npc.whoAmI, name = args.Npc.FullName };

            if (config.EnablePointTracking)
            {
                //处理特殊生物的价值
                snpc.value = snpc.id switch
                {
                    //金色史莱姆-250
                    667 => args.Npc.value / 15,
                    //双子
                    125 or 126 or 196 => args.Npc.value / 2,
                    //飞龙-50
                    87 or 88 or 89 or 90 or 91 or 92 or -4 or 85 or 629 or 216 => args.Npc.value / 5,
                    //海盗诅咒-400
                    662 => 40000f,
                    //火星飞碟四个肢体-300
                    393 or 394 => 30000f,
                    //火星飞蝶本体-2500
                    395 => 250000f,
                    //恐惧鹦鹉螺 1000
                    618 => 100000f,
                    //圣诞坦克-500
                    346 or 621 or 622 or 623 or 580 or 581 or 508 or 509 or 60 or 59 or 62 or 66 => args.Npc.value * 2,
                    //黑暗魔法师-200
                    564 => 20000f,
                    //T3黑暗魔法师-500
                    565 => 50000f,
                    //T2食人魔-800
                    576 => 80000f,
                    //T3食人魔-1500
                    577 => 150000f,
                    //双足翼龙-6250
                    551 => 625000f,
                    //史莱姆皇后-2000
                    657 => 200000f,
                    //肉前地牢怪 + 3
                    294 or 295 or 296 or -14 or -13 or 31 or 32 or 34 or 71 => args.Npc.value + 300,
                    //猩红怪物
                    -23 or -22 or 173 or 239 or 240 or 181 or 6 or -12 or -11 or 7 or 8 or 9 => args.Npc.value + 200,
                    //毒兔兔等可量产化的生物
                    464 or 465 or 57 or 47 => 0,
                    _ => args.Npc.value
                };
                if (!Main.hardMode)
                {
                    switch (snpc.id)
                    {
                        //肉前真菌敌怪价格降低 2/3
                        case 254:
                        case 255:
                        case 257:
                        case 258:
                        case 259:
                        case 260:
                        case 261:
                        case 634:
                        case 635:
                            snpc.value /= 3;
                            break;
                    }
                }

                //天顶世界对宝箱怪的价值进行限定
                if ((Main.remixWorld || Main.zenithWorld) && (args.Npc.netID == 85 || args.Npc.netID == 629))
                {
                    snpc.value = args.Npc.value / 17;
                }
            }
            else
            {
                snpc.value = 0f;
            }

            //处理特殊生物是否为boss
            snpc.isBoss = snpc.id switch
            {
                //世界吞噬者
                13 or 14 or 15 or 325 or 327 or 564 or 565 or 576 or 577 or 551 or 344 or 345 or 346 or 517 or 422 or 493 or 507 or 68 => true,
                _ => args.Npc.boss
            };
            snpc.playerAndDamage.Add(players[0].Account.ID, args.Damage);
            snpc.AllDamage += args.Damage;
            strikeNPC.Add(snpc);
        }
    }


    /// <summary>
    ///     杀死npc时计数
    /// </summary>
    /// <param name="args"></param>
    private void OnNPCKilled(NpcKilledEventArgs args)
    {
        if (!config.EnableNpcKillTracking)
        {
            if (strikeNPC.Count > 0)
            {
                strikeNPC.Clear();
            }

            return;
        }

        //毁灭者的处理，这个npc的死亡钩子只记录他的头，所以这里没有写135和136，并且没有放在 for (int i = 0; i < strikeNPC.Count; i++) 里，因为这个boss可能没有被击中过头部
        //为什么移到外面，因为用于判断毁灭者死亡条件的头部可能一直不会被击中
        if (args.npc.netID == 134)
        {
            //遍历所有被击中过的strikeNPC，记录 135 和 136 的击中情况
            foreach (var sss in strikeNPC)
            {
                if (sss.id == 134 || sss.id == 135 || sss.id == 136)
                {
                    foreach (var ss in sss.playerAndDamage)
                    {
                        if (this.Destroyer.ContainsKey(ss.Key))
                        {
                            this.Destroyer[ss.Key] += ss.Value;
                        }
                        else
                        {
                            this.Destroyer.Add(ss.Key, ss.Value);
                        }
                    }
                }
            }

            var sum = 0;
            foreach (var des in this.Destroyer)
            {
                sum += des.Value;
            }

            foreach (var x in edPlayers)
            {
                if (this.Destroyer.TryGetValue(x.Account, out var value))
                {
                    x.killNPCnum++;

                    var point = 0;
                    if (config.EnablePointTracking)
                    {
                        point = (int) (2000f * value / sum);
                        if (point < 0 || point >= int.MaxValue)
                        {
                            TShock.Log.ConsoleError(GetString("错误的点数：") + point);
                        }

                        x.point += point is >= 0 and < int.MaxValue ? point : 1;
                    }

                    if (!x.killBossID.TryAdd(134, 1))
                    {
                        x.killBossID[134]++;
                    }

                    var temp = this.BestFindPlayerByNameOrIndex(x.Name);
                    if (temp.Count != 0)
                    {
                        if (!x.hideKillTips)
                        {
                            this.SendAllText(temp[0], "kill + 1", Color.White, Color.Gray, args.npc.Center - (Vector2.UnitY * 10));
                        }

                        NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(temp[0].TPlayer.Center, 4), temp[0].Index);
                        if (!x.hidePointTips && config.EnablePointTracking)
                        {
                            this.SendAllText(temp[0], $"+ {point} $", new Color(255, 100, 255), new Color(150, 75, 150), temp[0].TPlayer.Center);
                        }
                    }
                }
            }

            if (config.EnableBossDamageLeaderboard)
            {
                SendKillBossMessage(args.npc.FullName, this.Destroyer, sum);
            }

            this.Destroyer.Clear();
            strikeNPC.RemoveAll(x => x.id == 134 || x.id == 136 || x.id == 135 || x.id != Main.npc[x.index].netID || !Main.npc[x.index].active);
            return;
        }
        //肉山同理，肉山嘴巴

        if (args.npc.netID == 113)
        {
            //遍历所有被击中过的strikeNPC，记录 113 和 114 的击中情况
            foreach (var sss in strikeNPC)
            {
                if (sss.id == 113 || sss.id == 114)
                {
                    foreach (var ss in sss.playerAndDamage)
                    {
                        if (this.FleshWall.ContainsKey(ss.Key))
                        {
                            this.FleshWall[ss.Key] += ss.Value;
                        }
                        else
                        {
                            this.FleshWall.Add(ss.Key, ss.Value);
                        }
                    }
                }
            }

            var sum = 0;
            foreach (var fw in this.FleshWall)
            {
                sum += fw.Value;
            }

            foreach (var x in edPlayers)
            {
                if (this.FleshWall.TryGetValue(x.Account, out var value))
                {
                    x.killNPCnum++;

                    var point = 0;
                    if (config.EnablePointTracking)
                    {
                        point = (int) (2000f * value / sum);
                        if (point < 0 || point >= int.MaxValue)
                        {
                            TShock.Log.ConsoleError(GetString("错误的点数：") + point);
                        }

                        x.point += point is >= 0 and < int.MaxValue ? point : 1;
                    }

                    if (!x.killBossID.TryAdd(113, 1))
                    {
                        x.killBossID[113]++;
                    }

                    var temp = this.BestFindPlayerByNameOrIndex(x.Name);
                    if (temp.Count != 0)
                    {
                        if (!x.hideKillTips)
                        {
                            this.SendAllText(temp[0], "kill + 1", Color.White, Color.Gray, args.npc.Center - (Vector2.UnitY * 10));
                        }

                        NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(temp[0].TPlayer.Center, 4), temp[0].Index);
                        if (!x.hidePointTips && config.EnablePointTracking)
                        {
                            this.SendAllText(temp[0], $"+ {point} $", new Color(255, 100, 255), new Color(150, 75, 150), temp[0].TPlayer.Center);
                        }
                    }
                }
            }

            if (config.EnableBossDamageLeaderboard)
            {
                SendKillBossMessage(GetString("血肉墙"), this.FleshWall, sum);
            }

            this.FleshWall.Clear();
            strikeNPC.RemoveAll(x => x.id == 113 || x.id == 114 || x.id != Main.npc[x.index].netID || !Main.npc[x.index].active);
            return;
        }

        //其他生物，对被击杀的生物进行计数
        for (var i = 0; i < strikeNPC.Count; i++)
        {
            if (strikeNPC[i].index == args.npc.whoAmI && strikeNPC[i].id == args.npc.netID)
            {
                switch (strikeNPC[i].id)
                {
                    case 13: //世界吞噬者特殊处理，特殊点：可以有多个头，只有最后一个头死亡时计入击杀
                    case 14:
                    case 15:
                    {
                        var flag = true;
                        foreach (var n in Main.npc)
                        {
                            if (n.whoAmI != args.npc.whoAmI && (n.type == 13 || n.type == 14 || n.type == 15) && n.active)
                            {
                                flag = false;
                                break; //如果boss被杀死，设为true
                            }
                        }

                        foreach (var ss in strikeNPC[i].playerAndDamage)
                        {
                            if (this.Eaterworld.ContainsKey(ss.Key))
                            {
                                this.Eaterworld[ss.Key] += ss.Value;
                            }
                            else
                            {
                                this.Eaterworld.Add(ss.Key, ss.Value);
                            }
                        }

                        if (flag)
                        {
                            var sum = 0;
                            foreach (var eater in this.Eaterworld)
                            {
                                sum += eater.Value;
                            }

                            foreach (var x in edPlayers)
                            {
                                if (this.Eaterworld.TryGetValue(x.Account, out var value))
                                {
                                    x.killNPCnum++;

                                    var point = 0;
                                    if (config.EnablePointTracking)
                                    {
                                        point = (int) (1250f * value / sum);
                                        if (point < 0 || point >= int.MaxValue)
                                        {
                                            TShock.Log.ConsoleError(GetString("错误的点数：") + point);
                                        }

                                        x.point += point is >= 0 and < int.MaxValue ? point : 1;
                                    }

                                    if (!x.killBossID.TryAdd(13, 1))
                                    {
                                        x.killBossID[13]++;
                                    }

                                    var temp = this.BestFindPlayerByNameOrIndex(x.Name);
                                    if (temp.Count != 0)
                                    {
                                        if (!x.hideKillTips)
                                        {
                                            this.SendAllText(temp[0], "kill + 1", Color.White, Color.Gray, args.npc.Center - (Vector2.UnitY * 10));
                                        }

                                        NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(temp[0].TPlayer.Center, 4), temp[0].Index);
                                        if (!x.hidePointTips && config.EnablePointTracking)
                                        {
                                            this.SendAllText(temp[0], $"+ {point} $", new Color(255, 100, 255), new Color(150, 75, 150), temp[0].TPlayer.Center);
                                        }
                                    }
                                }
                            }

                            if (config.EnableBossDamageLeaderboard)
                            {
                                SendKillBossMessage(args.npc.FullName, this.Eaterworld, sum);
                            }

                            strikeNPC.RemoveAll(x => x.id == 13 || x.id == 14 || x.id == 15 || x.id != Main.npc[x.index].netID || !Main.npc[x.index].active);
                            this.Eaterworld.Clear();
                            return;
                        }
                    }
                    break;
                    case 492: //荷兰飞船的处理，特殊点：本体不可被击中，在其他炮塔全死亡后计入击杀
                    {
                        var flag = true;
                        var index = -1;
                        foreach (var n in Main.npc)
                        {
                            if (n.whoAmI != args.npc.whoAmI && n is { type: 492, active: true })
                            {
                                flag = false;
                            }

                            if (n.netID == 491)
                            {
                                index = n.whoAmI;
                            }
                        }

                        if (index >= 0)
                        {
                            var st = strikeNPC.Find(x => x.id == 491);
                            if (st == null)
                            {
                                strikeNPC.Add(new StrikeNPC(index, 491, Lang.GetNPCNameValue(491), true, strikeNPC[i].playerAndDamage, strikeNPC[i].AllDamage, 80000f));
                            }
                            else
                            {
                                foreach (var y in strikeNPC[i].playerAndDamage)
                                {
                                    if (st.playerAndDamage.ContainsKey(y.Key))
                                    {
                                        st.playerAndDamage[y.Key] += y.Value;
                                        st.AllDamage += y.Value;
                                    }
                                    else
                                    {
                                        st.playerAndDamage.Add(y.Key, y.Value);
                                        st.AllDamage += y.Value;
                                    }
                                }
                            }
                        }

                        if (flag)
                        {
                            var airship = strikeNPC.Find(x => x.id == 491);
                            if (airship == null)
                            {
                                strikeNPC.RemoveAll(x => x.id == 491 || x.id == 492 || x.id != Main.npc[x.index].netID || !Main.npc[x.index].active);
                                return;
                            }

                            foreach (var x in edPlayers)
                            {
                                if (airship.playerAndDamage.TryGetValue(x.Account, out var value))
                                {
                                    x.killNPCnum += 2;

                                    var point = 0;
                                    if (config.EnablePointTracking)
                                    {
                                        point = (int) (airship.value * value / airship.AllDamage / 100);
                                        if (point < 0 || point >= int.MaxValue)
                                        {
                                            TShock.Log.ConsoleError(GetString("错误的点数：") + point);
                                        }

                                        x.point += point is >= 0 and < int.MaxValue ? point : 1;
                                    }

                                    if (!x.killBossID.TryAdd(491, 1))
                                    {
                                        x.killBossID[491]++;
                                    }

                                    var temp = this.BestFindPlayerByNameOrIndex(x.Name);
                                    if (temp.Count != 0)
                                    {
                                        if (!x.hideKillTips)
                                        {
                                            this.SendAllText(temp[0], "kill + 1", Color.White, Color.Gray, args.npc.Center - (Vector2.UnitY * 10));
                                        }

                                        NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(temp[0].TPlayer.Center, 4), temp[0].Index);
                                        if (!x.hidePointTips && config.EnablePointTracking)
                                        {
                                            this.SendAllText(temp[0], $"+ {point} $", new Color(255, 100, 255), new Color(150, 75, 150), temp[0].TPlayer.Center);
                                        }
                                    }
                                }
                            }

                            if (config.EnableBossDamageLeaderboard)
                            {
                                SendKillBossMessage(airship.name, airship.playerAndDamage, airship.AllDamage);
                            }

                            strikeNPC.RemoveAll(x => x.id == 491 || x.id == 492 || x.id != Main.npc[x.index].netID || !Main.npc[x.index].active);
                            return;
                        }
                    }
                    break;
                    case 398: //月球领主的处理，特殊点，本体可被击中，但肢体会假死，击中肢体也应该算入本体中
                    {
                        var strikenpcs = strikeNPC.FindAll(x => x.id == 397 || x.id == 396);
                        if (strikenpcs.Count > 0)
                        {
                            foreach (var v in strikenpcs)
                            {
                                foreach (var vv in v.playerAndDamage)
                                {
                                    if (strikeNPC[i].playerAndDamage.ContainsKey(vv.Key))
                                    {
                                        strikeNPC[i].playerAndDamage[vv.Key] += vv.Value;
                                        strikeNPC[i].AllDamage += vv.Value;
                                    }
                                    else
                                    {
                                        strikeNPC[i].playerAndDamage.Add(vv.Key, vv.Value);
                                        strikeNPC[i].AllDamage += vv.Value;
                                    }
                                }
                            }
                        }

                        foreach (var x in edPlayers)
                        {
                            if (strikeNPC[i].playerAndDamage.TryGetValue(x.Account, out var value))
                            {
                                x.killNPCnum++;

                                var point = 0;
                                if (config.EnablePointTracking)
                                {
                                    point = (int) (value * 1f / strikeNPC[i].AllDamage * strikeNPC[i].value / 100);
                                    if (point < 0 || point >= int.MaxValue)
                                    {
                                        TShock.Log.ConsoleError(GetString("错误的点数：") + point);
                                    }

                                    x.point += point is >= 0 and < int.MaxValue ? point : 1;
                                }

                                if (!x.killBossID.TryAdd(398, 1))
                                {
                                    x.killBossID[398]++;
                                }

                                var temp = this.BestFindPlayerByNameOrIndex(x.Name);
                                if (temp.Count != 0)
                                {
                                    if (!x.hideKillTips)
                                    {
                                        this.SendAllText(temp[0], "kill + 1", Color.White, Color.Gray, args.npc.Center - (Vector2.UnitY * 10));
                                    }

                                    NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(temp[0].TPlayer.Center, 4), temp[0].Index);
                                    if (!x.hidePointTips && config.EnablePointTracking)
                                    {
                                        this.SendAllText(temp[0], $"+ {point} $", new Color(255, 100, 255), new Color(150, 75, 150), temp[0].TPlayer.Center);
                                    }
                                }
                            }
                        }

                        if (config.EnableBossDamageLeaderboard)
                        {
                            SendKillBossMessage(GetString("月亮领主"), strikeNPC[i].playerAndDamage, strikeNPC[i].AllDamage);
                        }

                        strikeNPC.RemoveAll(x => x.id == 398 || x.id == 397 || x.id == 396 || x.id != Main.npc[x.index].netID || !Main.npc[x.index].active);
                        return;
                    }
                    case 127: //机械骷髅王的处理，特殊点，本体可能被击中，其他肢体可能会死
                    case 128:
                    case 129:
                    case 130:
                    case 131:
                    {
                        var strike = strikeNPC.Find(x => x.id == 127);
                        if (strike == null)
                        {
                            var index = -1;
                            foreach (var n in Main.npc)
                            {
                                if (n.netID == 127)
                                {
                                    index = n.whoAmI;
                                }
                            }

                            if (index == -1) //我认为不可能发生这种情况
                            {
                                strikeNPC.RemoveAll(x => x.id == 127 || x.id == 128 || x.id == 129 || x.id == 130 || x.id == 131 || x.id != Main.npc[x.index].netID || !Main.npc[x.index].active);
                                return;
                            }

                            strike = new StrikeNPC(index, 127, Main.npc[index].FullName, true, strikeNPC[i].playerAndDamage, strikeNPC[i].AllDamage, 300000);
                            strikeNPC.Add(strike);
                        }
                        else if (strikeNPC[i].id != 127) //把肢体受伤计算加入到本体头部中
                        {
                            foreach (var v in strikeNPC[i].playerAndDamage)
                            {
                                if (strike.playerAndDamage.ContainsKey(v.Key))
                                {
                                    strike.playerAndDamage[v.Key] += v.Value;
                                    strike.AllDamage += v.Value;
                                }
                                else
                                {
                                    strike.playerAndDamage.Add(v.Key, v.Value);
                                    strike.AllDamage += v.Value;
                                }
                            }
                        }

                        if (strikeNPC[i].id == 127)
                        {
                            foreach (var x in edPlayers)
                            {
                                if (strikeNPC[i].playerAndDamage.TryGetValue(x.Account, out var value))
                                {
                                    x.killNPCnum += 1;

                                    var point = 0;
                                    if (config.EnablePointTracking)
                                    {
                                        point = (int) (value * 1.0f / strikeNPC[i].AllDamage * strikeNPC[i].value / 100);
                                        if (point < 0 || point >= int.MaxValue)
                                        {
                                            TShock.Log.ConsoleError(GetString("错误的点数：") + point);
                                        }

                                        x.point += point is >= 0 and < int.MaxValue ? point : 1;
                                    }

                                    if (!x.killBossID.TryAdd(127, 1))
                                    {
                                        x.killBossID[127]++;
                                    }

                                    var temp = this.BestFindPlayerByNameOrIndex(x.Name);
                                    if (temp.Count != 0)
                                    {
                                        if (!x.hideKillTips)
                                        {
                                            this.SendAllText(temp[0], "kill + 1", Color.White, Color.Gray, args.npc.Center - (Vector2.UnitY * 10));
                                        }

                                        NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(temp[0].TPlayer.Center, 4), temp[0].Index);
                                        if (!x.hidePointTips && config.EnablePointTracking)
                                        {
                                            this.SendAllText(temp[0], $"+ {point} $", new Color(255, 100, 255), new Color(150, 75, 150), temp[0].TPlayer.Center);
                                        }
                                    }
                                }
                            }

                            if (config.EnableBossDamageLeaderboard)
                            {
                                SendKillBossMessage(args.npc.FullName, strikeNPC[i].playerAndDamage, strikeNPC[i].AllDamage);
                            }

                            strikeNPC.RemoveAll(x => x.id == 127 || x.id == 128 || x.id == 129 || x.id == 130 || x.id == 131 || x.id != Main.npc[x.index].netID || !Main.npc[x.index].active);
                            return;
                        }
                    }
                    break;
                    case 245: //石巨人的特殊处理
                    case 246:
                    case 247:
                    case 248:
                    {
                        var strike = strikeNPC.Find(x => x.id == 245);
                        if (strike == null)
                        {
                            var index = -1;
                            foreach (var n in Main.npc)
                            {
                                if (n.netID == 245)
                                {
                                    index = n.whoAmI;
                                }
                            }

                            if (index == -1) //不可能发生这种情况
                            {
                                strikeNPC.RemoveAll(x => x.id == 245 || x.id == 246 || x.id == 247 || x.id == 248 || x.id != Main.npc[x.index].netID || !Main.npc[x.index].active);
                                return;
                            }

                            strike = new StrikeNPC(index, 245, Main.npc[index].FullName, true, strikeNPC[i].playerAndDamage, strikeNPC[i].AllDamage, 400000);
                            strikeNPC.Add(strike);
                        }
                        else if (strikeNPC[i].id != 245) //把除了本体以外的肢体的伤害计算加到本体上
                        {
                            foreach (var v in strikeNPC[i].playerAndDamage)
                            {
                                if (strike.playerAndDamage.ContainsKey(v.Key))
                                {
                                    strike.playerAndDamage[v.Key] += v.Value;
                                    strike.AllDamage += v.Value;
                                }
                                else
                                {
                                    strike.playerAndDamage.Add(v.Key, v.Value);
                                    strike.AllDamage += v.Value;
                                }
                            }
                        }

                        if (strikeNPC[i].id == 245)
                        {
                            foreach (var x in edPlayers)
                            {
                                if (strikeNPC[i].playerAndDamage.TryGetValue(x.Account, out var value))
                                {
                                    x.killNPCnum += 1;

                                    var point = 0;
                                    if (config.EnablePointTracking)
                                    {
                                        point = (int) (value * 1.0f / strikeNPC[i].AllDamage * strikeNPC[i].value / 100);
                                        if (point < 0 || point >= int.MaxValue)
                                        {
                                            TShock.Log.ConsoleError(GetString("错误的点数：") + point);
                                        }

                                        x.point += point is >= 0 and < int.MaxValue ? point : 1;
                                    }

                                    if (!x.killBossID.TryAdd(245, 1))
                                    {
                                        x.killBossID[245]++;
                                    }

                                    var temp = this.BestFindPlayerByNameOrIndex(x.Name);
                                    if (temp.Count != 0)
                                    {
                                        if (!x.hideKillTips)
                                        {
                                            this.SendAllText(temp[0], "kill + 1", Color.White, Color.Gray, args.npc.Center - (Vector2.UnitY * 10));
                                        }

                                        NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(temp[0].TPlayer.Center, 4), temp[0].Index);
                                        if (!x.hidePointTips && config.EnablePointTracking)
                                        {
                                            this.SendAllText(temp[0], $"+ {point} $", new Color(255, 100, 255), new Color(150, 75, 150), temp[0].TPlayer.Center);
                                        }
                                    }
                                }
                            }

                            if (config.EnableBossDamageLeaderboard)
                            {
                                SendKillBossMessage(args.npc.FullName, strikeNPC[i].playerAndDamage, strikeNPC[i].AllDamage);
                            }

                            strikeNPC.RemoveAll(x => x.id == 245 || x.id == 246 || x.id == 247 || x.id == 248 || x.id != Main.npc[x.index].netID || !Main.npc[x.index].active);
                            return;
                        }
                    }
                    break;
                    case 266: //克苏鲁之脑的特殊处理
                    case 267:
                    {
                        var strike = strikeNPC.Find(x => x.id == 266);
                        if (strike == null)
                        {
                            var index = -1;
                            foreach (var n in Main.npc)
                            {
                                if (n.netID == 266)
                                {
                                    index = n.whoAmI;
                                }
                            }

                            if (index == -1) //不可能发生这种情况
                            {
                                strikeNPC.RemoveAll(x => x.id == 266 || x.id == 267 || x.id != Main.npc[x.index].netID || !Main.npc[x.index].active);
                                return;
                            }

                            strike = new StrikeNPC(index, 266, Main.npc[index].FullName, true, strikeNPC[i].playerAndDamage, strikeNPC[i].AllDamage, 125000);
                            strikeNPC.Add(strike);
                        }
                        else if (strikeNPC[i].id != 266) //把除了本体以外的飞眼怪的伤害计算加到本体上
                        {
                            foreach (var v in strikeNPC[i].playerAndDamage)
                            {
                                if (strike.playerAndDamage.ContainsKey(v.Key))
                                {
                                    strike.playerAndDamage[v.Key] += v.Value;
                                    strike.AllDamage += v.Value;
                                }
                                else
                                {
                                    strike.playerAndDamage.Add(v.Key, v.Value);
                                    strike.AllDamage += v.Value;
                                }
                            }
                        }

                        if (strikeNPC[i].id == 266)
                        {
                            foreach (var x in edPlayers)
                            {
                                if (strikeNPC[i].playerAndDamage.TryGetValue(x.Account, out var value))
                                {
                                    x.killNPCnum += 1;

                                    var point = 0;
                                    if (config.EnablePointTracking)
                                    {
                                        point = (int) (value * 1.0f / strikeNPC[i].AllDamage * strikeNPC[i].value / 100);
                                        if (point < 0 || point >= int.MaxValue)
                                        {
                                            TShock.Log.ConsoleError(GetString("错误的点数：") + point);
                                        }

                                        x.point += point is >= 0 and < int.MaxValue ? point : 1;
                                    }

                                    if (!x.killBossID.TryAdd(266, 1))
                                    {
                                        x.killBossID[266]++;
                                    }

                                    var temp = this.BestFindPlayerByNameOrIndex(x.Name);
                                    if (temp.Count != 0)
                                    {
                                        if (!x.hideKillTips)
                                        {
                                            this.SendAllText(temp[0], "kill + 1", Color.White, Color.Gray, args.npc.Center - (Vector2.UnitY * 10));
                                        }

                                        NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(temp[0].TPlayer.Center, 4), temp[0].Index);
                                        if (!x.hidePointTips && config.EnablePointTracking)
                                        {
                                            this.SendAllText(temp[0], $"+ {point} $", new Color(255, 100, 255), new Color(150, 75, 150), temp[0].TPlayer.Center);
                                        }
                                    }
                                }
                            }

                            if (config.EnableBossDamageLeaderboard)
                            {
                                SendKillBossMessage(args.npc.FullName, strikeNPC[i].playerAndDamage, strikeNPC[i].AllDamage);
                            }

                            strikeNPC.RemoveAll(x => x.id == 266 || x.id == 267 || x.id != Main.npc[x.index].netID || !Main.npc[x.index].active);
                            return;
                        }
                    }
                    break;
                    default:
                    {
                        foreach (var x in edPlayers)
                        {
                            if (strikeNPC[i].playerAndDamage.TryGetValue(x.Account, out var value))
                            {
                                x.killNPCnum++;

                                var point = 0;
                                if (config.EnablePointTracking)
                                {
                                    point = (int) (value * 1.0f / strikeNPC[i].AllDamage * strikeNPC[i].value / 100);
                                    if (point == 0 && args.npc.CanBeChasedBy())
                                    {
                                        point = 1;
                                    }

                                    if (point < 0 || point >= int.MaxValue)
                                    {
                                        TShock.Log.ConsoleError(GetString("错误的点数：") + point);
                                    }

                                    x.point += point is >= 0 and < int.MaxValue ? point : 1;
                                }

                                if (strikeNPC[i].isBoss)
                                {
                                    if (!x.killBossID.TryAdd(strikeNPC[i].id, 1))
                                    {
                                        x.killBossID[strikeNPC[i].id]++;
                                    }
                                }

                                if (args.npc.rarity > 0 || (config.CreaturesTreatedAsRareForKills.Count > 0 && config.CreaturesTreatedAsRareForKills.Contains(args.npc.netID)))
                                {
                                    if (!x.killRareNPCID.TryAdd(strikeNPC[i].id, 1))
                                    {
                                        x.killRareNPCID[strikeNPC[i].id]++;
                                    }
                                }

                                var temp = this.BestFindPlayerByNameOrIndex(x.Name);

                                if (temp.Count != 0)
                                {
                                    if (args.npc.rarity > 0)
                                    {
                                        if (!x.hideKillTips)
                                        {
                                            this.SendAllText(temp[0], "rare kill + 1", new Color(0, 150, 255), new Color(0, 95, 160), args.npc.Center - (Vector2.UnitY * 10));
                                        }
                                    }
                                    else
                                    {
                                        if (!x.hideKillTips)
                                        {
                                            this.SendAllText(temp[0], "kill + 1", Color.White, Color.Gray, args.npc.Center - (Vector2.UnitY * 10));
                                        }
                                    }

                                    if (strikeNPC[i].isBoss)
                                    {
                                        NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(temp[0].TPlayer.Center, 4), temp[0].Index);
                                    }

                                    if (!x.hidePointTips && config.EnablePointTracking)
                                    {
                                        this.SendAllText(temp[0], $"+ {point} $", new Color(255, 100, 255), new Color(150, 75, 150), temp[0].TPlayer.Center);
                                    }
                                }
                            }
                        }

                        if (config.EnableBossDamageLeaderboard && (args.npc.boss || args.npc.netID == 551 || args.npc.netID == 125 || args.npc.netID == 126 || config.AdditionalCreaturesForDamageLeaderboard.Contains(args.npc.netID)))
                        {
                            SendKillBossMessage(args.npc.FullName, strikeNPC[i].playerAndDamage, strikeNPC[i].AllDamage);
                        }

                        strikeNPC.RemoveAt(i);
                        strikeNPC.RemoveAll(x => x.id != Main.npc[x.index].netID || !Main.npc[x.index].active);
                        return;
                    }
                }
            }

            //清理因为意外导致的不正确的数据
            if (i >= 0 && (strikeNPC[i].id != Main.npc[strikeNPC[i].index].netID || !Main.npc[strikeNPC[i].index].active))
            {
                strikeNPC.RemoveAt(i);
                i--;
            }
        }
    }


    /// <summary>
    ///     记录死亡事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnPlayerKilled(object? sender, GetDataHandlers.KillMeEventArgs e)
    {
        var ext = edPlayers.Find(x => x.Name == e.Player.Name);
        if (ext == null)
        {
            return;
        }

        var avoid = e.PlayerDeathReason._sourceOtherIndex == 255 || e.PlayerDeathReason._sourceOtherIndex == 254 || e.PlayerDeathReason._sourceCustomReason == GetString("代码杀");
        if (config.EnableDeathCountTracking && !avoid)
        {
            ext.deathCount++;
        }

        if (config.EnablePointTracking && !avoid)
        {
            var temp = (long) (ext.point * 0.01f * config.PointsLossMultiplierOnDeath);
            if (temp < 5)
            {
                temp = 5;
            }

            if (ext.point >= temp)
            {
                e.Player.SendInfoMessage(GetString($"您遗失了：{temp} 点数"));
                ext.point -= temp;
            }
            else
            {
                e.Player.SendInfoMessage(GetString($"您遗失了：{ext.point} 点数，已扣完"));
                ext.point = 0;
            }
        }

        if (config.AllowPlayerRespawnAtLastDeathPoint && !avoid)
        {
            ext.deadPos = e.Player.TPlayer.Center;
        }
    }


    /// <summary>
    ///     功能
    /// </summary>
    /// <param name="args"></param>
    private void Function(CommandArgs args)
    {
        if (args.Parameters.Count > 0)
        {
            args.Player.SendInfoMessage(GetString("输入 /zbpos  来返回上次死亡地点"));
            return;
        }

        if (config.AllowPlayerRespawnAtLastDeathPoint)
        {
            if (!args.Player.RealPlayer)
            {
                args.Player.SendInfoMessage(GetString("对象不正确，请检查您的状态，您是否为游戏内玩家？"));
                return;
            }

            var ex = edPlayers.Find(x => x.Name == args.Player.Name);
            if (ex == null)
            {
                args.Player.SendInfoMessage(GetString("数据异常，请重试"));
            }
            else if (ex.deadPos == Vector2.Zero)
            {
                args.Player.SendInfoMessage(GetString("你没有上次死亡的地点"));
            }
            else if (ex.point >= config.RespawnCostPoints || !config.EnablePointTracking)
            {
                if (config.EnablePointTracking)
                {
                    ex.point -= config.RespawnCostPoints > 0 ? config.RespawnCostPoints : 0;
                    TSPlayer.All.SendInfoMessage($"玩家 {args.Player.Name} 已传送到上次死亡地点{(config.RespawnCostPoints > 0 ? GetString($"，消耗点数 {config.RespawnCostPoints}") : "")}");
                }
                else
                {
                    TSPlayer.All.SendInfoMessage(GetString($"玩家 {args.Player.Name} 已传送到上次死亡地点"));
                }

                args.Player.Teleport(ex.deadPos.X, ex.deadPos.Y);
            }
            else
            {
                args.Player.SendInfoMessage(GetString("点数不足，无法回溯上次死亡地点"));
            }
        }
        else
        {
            args.Player.SendInfoMessage(GetString("未被允许死亡回调传送，请在配置文件中修改"));
        }
    }

    /*
    /// <summary>
    /// 查找当前哪些玩家拥有此物品
    /// </summary>
    /// <param name="args"></param>
    private void FindItem(CommandArgs args)
    {
        if (args.Parameters.Count < 2)
        {
            args.Player.SendInfoMessage("输入 /zfind [id]  来查找当前哪些玩家拥有此物品");
            return;
        }


        using (QueryResult queryResult = TShock.DB.QueryReader("SELECT * FROM tsCharacter"))
        {
            if (queryResult.Read())
            {
                List<NetItem> list = queryResult.Get<string>("Inventory").Split('~', StringSplitOptions.None).Select(new Func<string, NetItem>(NetItem.Parse)).ToList<NetItem>();
                if (list.Count < NetItem.MaxInventory)
                {
                    list.InsertRange(67, new NetItem[2]);
                    list.InsertRange(77, new NetItem[2]);
                    list.InsertRange(87, new NetItem[2]);
                    list.AddRange(new NetItem[NetItem.MaxInventory - list.Count]);
                }
                playerData.inventory = list.ToArray();
                playerData.extraSlot = new int?(queryResult.Get<int>("extraSlot"));
            }
        }

    }
    */


    /// <summary>
    ///     重新加载配置
    /// </summary>
    /// <param name="e"></param>
    private void OnReload(ReloadEventArgs e)
    {
        config = ZhipmConfig.LoadConfigFile();
        if (config.MaxBackupsPerPlayer < 1)
        {
            config.MaxBackupsPerPlayer = 5;
            e.Player.SendMessage(GetString("备份存档数目最小为 1 ，请不要输入无效值，已修改为默认 5"), new Color(255, 0, 0));
        }

        config.SaveConfigFile();
    }
}