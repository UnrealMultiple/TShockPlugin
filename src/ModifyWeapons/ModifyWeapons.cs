using LazyAPI;
using Microsoft.Xna.Framework;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace ModifyWeapons;

[ApiVersion(2, 1)]
public class Plugin : LazyPlugin
{
    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Author => "羽学";
    public override Version Version => new Version(1, 2, 7, 6);
    public override string Description => GetString("修改玩家物品数据并自动储存重读,可使用/mw指令给予玩家指定属性的物品");
    #endregion

    #region 注册与释放
    public static Database DB = new();
    public Plugin(Main game) : base(game) { }
    public override void Initialize()
    {
        GetDataHandlers.ItemDrop.Register(this.OnItemDrop);
        GetDataHandlers.PlayerUpdate.Register(this.OnPlayerUpdate);
        ServerApi.Hooks.ServerChat.Register(this, this.OnChat);
        GetDataHandlers.ChestItemChange.Register(this.OnChestItemChange);
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreetPlayer);
        TShockAPI.Commands.ChatCommands.Add(new Command("mw.use", Commands.CMD, "mw", "修改武器"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GetDataHandlers.ItemDrop.UnRegister(this.OnItemDrop);
            ServerApi.Hooks.ServerChat.Deregister(this, this.OnChat);
            GetDataHandlers.PlayerUpdate.UnRegister(this.OnPlayerUpdate);
            GetDataHandlers.ChestItemChange.UnRegister(this.OnChestItemChange);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreetPlayer);
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.CMD);
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 进服自动创建玩家数据方法
    private void OnGreetPlayer(GreetPlayerEventArgs args)
    {
        var plr = TShock.Players[args.Who];
        var data = DB.GetData(plr.Name);
        var adamin = plr.HasPermission("mw.admin");
        if (!Configuration.Instance.Enabled || plr == null)
        {
            return;
        }

        // 检查并初始化 Config.AllData
        if (data == null)
        {
            if (Configuration.Instance.OnlyAdminCreateData && !adamin)
            {
                return;
            }

            var newData = new Database.PlayerData
            {
                Name = plr.Name,
                Hand = true,
                Join = true,
                Alone = false,
                ReadCount = Configuration.Instance.ReadCount,
                Process = 0,
                AloneTime = default,
                ReadTime = DateTime.UtcNow,
                SyncTime = DateTime.UtcNow,
                Dict = new Dictionary<string, List<Database.PlayerData.ItemData>>()
            };
            DB.AddData(newData);
        }
        else if (data.Join)
        {
            //自动更新模式每次进服都会加1次重读次数
            if (Configuration.Instance.Auto == 1)
            {
                DB.UpReadCount(plr.Name, 1);
            }

            data.Process = 2;
            DB.UpdateData(data);

            Commands.UpdataRead(plr, data);
        }
    }
    #endregion

    #region 自动模式与公用武器数据写入方法
    private void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
    {
        var plr = e.Player;
        var tplr = plr.TPlayer;
        var datas = DB.GetData(plr.Name);
        var Sel = plr.SelectedItem;
        if (plr == null || !plr.IsLoggedIn || !plr.Active || datas == null || !Configuration.Instance.Enabled)
        {
            return;
        }
        var now = DateTime.UtcNow;

        //触发延迟指令
        if (Configuration.Instance.Alone)
        {
            var last = 0f;
            if (datas.AloneTime != default)
            {
                last = (float) Math.Round((now - datas.AloneTime).TotalMilliseconds, 2);
            }
            if (datas.Alone && last >= Configuration.Instance.AloneTimer)
            {
                this.Cmd(plr);
                datas.Alone = false;
                DB.UpdateData(datas);
            }
        }

        //自动模式
        if (Configuration.Instance.Auto == 1)
        {
            var flag = false;

            if (datas.Dict.TryGetValue(plr.Name, out var DataList))
            {
                foreach (var data in DataList)
                {
                    if (datas.Process == 0)
                    {
                        if (!tplr.controlUseItem)
                        {
                            continue;
                        }

                        if (Sel.type == data.type)
                        {
                            if (Sel.ammo != data.ammo)
                            {
                                plr.SendInfoMessage(GetString($"《[c/AD89D5:自][c/D68ACA:动][c/DF909A:重][c/E5A894:读]》 玩家:{plr.Name}"));
                                plr.SendMessage(GetString($"《[c/FCFE63:弹药转换]》[c/15EDDB:{Lang.GetItemName(Sel.type)}] ") +
                                    GetString($"[c/FF6863:{Sel.ammo}] => [c/8AD0EA:{data.ammo}]"), 255, 244, 155);

                                flag = true;
                            }

                            if (Sel.prefix != data.prefix && Sel.prefix != 0)
                            {
                                var pr = TShock.Utils.GetPrefixById(data.prefix);
                                if (string.IsNullOrEmpty(pr))
                                {
                                    pr = GetString("无");
                                }
                                plr.SendInfoMessage(GetString($"《[c/AD89D5:自][c/D68ACA:动][c/DF909A:重][c/E5A894:读]》 玩家:{plr.Name}"));
                                plr.SendMessage(GetString($"《[c/FCFE63:词缀转换]》[c/15EDDB:{Lang.GetItemName(Sel.type)}] ") +
                                    GetString($"[c/FF6863:{pr}] => ") +
                                    GetString($"[c/8AD0EA:{TShock.Utils.GetPrefixById(Sel.prefix)}]"), 255, 244, 155);

                                data.prefix = Sel.prefix;
                                flag = true;
                            }

                            if (flag)
                            {
                                datas.Process = 1;
                                break;
                            }
                        }
                    }
                }
            }

            if (datas.Process == 1)
            {
                for (var i = 0; i < plr.TPlayer.inventory.Length; i++)
                {
                    var inv = plr.TPlayer.inventory[i];
                    if (inv.type == 4346)
                    {
                        inv.TurnToAir();
                        plr.SendData(PacketTypes.PlayerSlot, null, plr.Index, i);
                        plr.GiveItem(5391, 1);
                    }
                }

                datas.Process = 2;
                DB.UpdateData(datas);
                Commands.UpdataRead(plr, datas);
            }
        }

        //公用武器
        if (Configuration.Instance.PublicWeapons)
        {
            if ((now - datas.SyncTime).TotalSeconds >= Configuration.Instance.SyncTime)
            {
                if (!datas.Dict.ContainsKey(plr.Name))
                {
                    datas.Dict[plr.Name] = new List<Database.PlayerData.ItemData>();
                }

                foreach (var item in Configuration.Instance.ItemDatas!)
                {
                    var data = datas.Dict[plr.Name].FirstOrDefault(d => d.type == item.type);

                    if (data == null)
                    {
                        datas.Dict[plr.Name].Add(new Database.PlayerData.ItemData(item.type, item.stack, item.prefix, item.damage, item.scale, item.knockBack, item.useTime, item.useAnimation, item.shoot, item.shootSpeed, item.ammo, item.useAmmo, item.color));
                    }
                    else
                    {
                        data.stack = item.stack;
                        data.prefix = item.prefix;
                        data.damage = item.damage;
                        data.scale = item.scale;
                        data.knockBack = item.knockBack;
                        data.useTime = item.useTime;
                        data.useAnimation = item.useAnimation;
                        data.shoot = item.shoot;
                        data.shootSpeed = item.shootSpeed;
                        data.ammo = item.ammo;
                        data.useAmmo = item.useAmmo;
                        data.color = item.color;
                    }
                }

                datas.SyncTime = now;
                DB.UpdateData(datas);
            }
        }
    }
    #endregion

    #region 获取公用武器的物品中文名
    public static void WriteName()
    {
        foreach (var item in Configuration.Instance.ItemDatas!)
        {
            var name = Lang.GetItemNameValue(item.type);
            if (item.Name == "")
            {
                item.Name = name;
            }
        }
    }
    #endregion

    #region 播报公用武器变动数值方法
    public static void PublicWeaponsMess()
    {
        var itemName = new HashSet<int>();
        var flag = false;
        var MessToPlayer = new List<Tuple<TSPlayer, string>>(); // 用于存储要发送给每个玩家的消息

        foreach (var item in Configuration.Instance.ItemDatas!)
        {
            if (!itemName.Contains(item.type))
            {
                itemName.Add(item.type);
            }

            var user = TShock.UserAccounts.GetUserAccounts();
            foreach (var acc in user)
            {
                var plr = TShock.Players.FirstOrDefault(p => p != null && p.IsLoggedIn && p.Active && p.Name == acc.Name);
                if (plr == null)
                {
                    continue;
                }

                var datas = DB.GetData(plr.Name);
                if (datas == null)
                {
                    continue;
                }

                var data = datas.Dict[plr.Name].FirstOrDefault(d => d.type == item.type);
                if (data == null)
                {
                    continue;
                }

                else if (data != null)
                {
                    var diffs = CompareItem2(item, data.type, data.stack, data.prefix, data.damage, data.scale,
                        data.knockBack, data.useTime, data.useAnimation, data.shoot, data.shootSpeed,
                        data.ammo, data.useAmmo, data.color);

                    if (diffs.Count > 0)
                    {
                        var mess = new StringBuilder($"[c/92C5EC:{Lang.GetItemName(item.type)}] ");
                        foreach (var diff in diffs)
                        {
                            mess.Append($" {diff.Key}{diff.Value}");
                        }
                        MessToPlayer.Add(Tuple.Create(plr, mess.ToString())); // 将消息存入列表
                        flag = true;
                    }
                }
            }
        }

        if (Configuration.Instance.Title.Any() && flag)
        {
            // 首先进行广播
            TShock.Utils.Broadcast(Configuration.Instance.Title + GetString("[c/AD89D5:公][c/D68ACA:用][c/DF909A:武][c/E5A894:器] 已更新!"), 240, 255, 150);

            // 立即发送玩家消息
            foreach (var tuple in MessToPlayer)
            {
                tuple.Item1.SendMessage(tuple.Item2, 244, 255, 150);
            }
        }
    }
    #endregion

    #region 对比公用武器配置文件与玩家数据差异 列出给玩家看
    internal static Dictionary<string, object> CompareItem2(Configuration.ItemData item, int type, int stack, byte prefix, int damage, float scale, float knockBack, int useTime, int useAnimation, int shoot, float shootSpeed, int ammo, int useAmmo, Color color)
    {
        string ColorToHex(Color color) => $"{color.R:X2}{color.G:X2}{color.B:X2}";

        var pr = TShock.Utils.GetPrefixById(item.prefix);
        if (string.IsNullOrEmpty(pr))
        {
            pr = GetString("无");
        }

        var diff = new Dictionary<string, object>();
        if (item.type != type)
        {
            diff.Add($"{Lang.GetItemNameValue(item.type)}", item.type);
        }

        if (item.stack != stack)
        {
            diff.Add(GetString("数量"), item.stack);
        }

        if (item.prefix != prefix)
        {
            diff.Add(GetString("前缀"), pr);
        }

        if (item.damage != damage)
        {
            diff.Add(GetString("伤害"), item.damage);
        }

        if (item.scale != scale)
        {
            diff.Add(GetString("大小"), item.scale);
        }

        if (item.knockBack != knockBack)
        {
            diff.Add(GetString("击退"), item.knockBack);
        }

        if (item.useTime != useTime)
        {
            diff.Add(GetString("用速"), item.useTime);
        }

        if (item.useAnimation != useAnimation)
        {
            diff.Add(GetString("攻速"), item.useAnimation);
        }

        if (item.shoot != shoot)
        {
            diff.Add(GetString("弹幕"), item.shoot);
        }

        if (item.shootSpeed != shootSpeed)
        {
            diff.Add(GetString("射速"), item.shootSpeed);
        }

        if (item.ammo != ammo)
        {
            diff.Add(GetString("弹药"), item.ammo);
        }

        if (item.useAmmo != useAmmo)
        {
            diff.Add(GetString("发射器"), item.useAmmo);
        }

        if (item.color != color)
        {
            diff.Add(GetString("颜色"), ColorToHex(item.color));
        }

        return diff;
    }
    #endregion

    #region 发送指令：检查玩家背包是否有修改物品 有则重读
    private void OnChat(ServerChatEventArgs e)
    {
        var plr = TShock.Players[e.Who];
        var datas = DB.GetData(plr.Name);
        if (plr == null || !plr.IsLoggedIn || !plr.Active || datas == null || !Configuration.Instance.Enabled || Configuration.Instance.Auto != 1)
        {
            return;
        }

        var flag = false;
        var flag2 = false;

        if (e.Text.StartsWith(TShock.Config.Settings.CommandSpecifier) || e.Text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier))
        {
            flag = true;
        }

        if (Configuration.Instance.Text.Any(text => e.Text.Contains(text)))
        {
            flag2 = true;
        }

        if (flag && flag2)
        {
            if (datas.Dict.TryGetValue(plr.Name, out var DataList))
            {
                foreach (var data in DataList)
                {
                    for (var i = 0; i < plr.TPlayer.inventory.Length; i++)
                    {
                        var inv = plr.TPlayer.inventory[i];
                        if (inv.type == data.type)
                        {
                            plr.SendInfoMessage(GetString($"《[c/AD89D5:自][c/D68ACA:动][c/DF909A:重][c/E5A894:读]》 玩家:{plr.Name}\n") +
                            GetString($"检测到经济指令 重读物品:[c/4C95DD:{Lang.GetItemNameValue(data.type)}]!"));

                            datas.Process = 2;
                            DB.UpdateData(datas);
                            Commands.UpdataRead(plr, datas);
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region 修改武器掉落的清理方法
    private void OnItemDrop(object? sender, GetDataHandlers.ItemDropEventArgs e)
    {
        var plr = e.Player;
        var datas = DB.GetData(plr.Name);

        if (plr == null || !plr.IsLoggedIn || !plr.Active ||
            datas == null || !Configuration.Instance.Enabled ||
            !Configuration.Instance.ClearItem ||
            datas.Process == 2 || plr.HasPermission("mw.admin") ||
            Configuration.Instance.ExemptItems.Contains(e.ID))
        {
            return;
        }

        if (datas.Dict.TryGetValue(plr.Name, out var DataList))
        {
            foreach (var data in DataList)
            {
                if (data.type == e.Type)
                {
                    plr.SendInfoMessage(GetString($"《[c/AD89D5:清][c/D68ACA:理][c/DF909A:警][c/E5A894:告]》 玩家:{plr.Name}\n") +
                        GetString($"禁止乱丢修改物品:[c/4C95DD:{Lang.GetItemNameValue(e.Type)}]!"));
                    e.Handled = true;
                    break;
                }
            }
        }
    }
    #endregion

    #region 箱子内出现修改武器的清理方法
    private void OnChestItemChange(object? sender, GetDataHandlers.ChestItemEventArgs e)
    {
        var plr = e.Player;
        var datas = DB.GetData(plr.Name);

        if (plr == null || !plr.IsLoggedIn || !plr.Active ||
            datas == null || !Configuration.Instance.Enabled ||
            !Configuration.Instance.ClearItem ||
            plr.HasPermission("mw.admin") ||
            Configuration.Instance.ExemptItems.Contains(e.ID))
        {
            return;
        }

        if (datas.Dict.TryGetValue(plr.Name, out var DataList))
        {
            foreach (var data in DataList)
            {
                if (data.type == e.Type)
                {
                    plr.SendInfoMessage(GetString($"《[c/AD89D5:清][c/D68ACA:理][c/DF909A:警][c/E5A894:告]》 玩家:{plr.Name}\n") +
                        GetString($"修改物品禁止放箱子:[c/4C95DD:{Lang.GetItemNameValue(e.Type)}]!"));
                    e.Handled = true;
                    break;
                }
            }
        }
    }
    #endregion

    #region 用临时超管权限让玩家执行命令
    private void Cmd(TSPlayer plr)
    {
        var mess = new StringBuilder();
        mess.Append(GetString("触发延时指令:"));
        var group = plr.Group;
        try
        {
            plr.Group = new SuperAdminGroup();
            foreach (var cmd in Configuration.Instance.AloneList)
            {
                TShockAPI.Commands.HandleCommand(plr, cmd);
                mess.Append($" [c/91DFBB:{cmd}]");
            }
        }
        finally
        {
            plr.Group = group;
        }
        plr.SendMessage($"{mess}", 0, 196, 177);
    }
    #endregion

}