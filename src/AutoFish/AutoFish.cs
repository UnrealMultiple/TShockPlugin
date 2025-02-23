using AutoFish.Utils;
using LazyAPI;
using Microsoft.Xna.Framework;
using System.Text;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace AutoFish;

[ApiVersion(2, 1)]
public class AutoFish : LazyPlugin
{

    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Author => "羽学 少司命";
    public override Version Version => new (1, 3, 7);
    public override string Description => GetString("自动钓鱼");
    #endregion

    #region 注册与释放
    public AutoFish(Main game) : base(game) { }
    public override void Initialize()
    {
        GetDataHandlers.NewProjectile.Register(this.ProjectNew);
        GetDataHandlers.NewProjectile.Register(this.BuffUpdate);
        ServerApi.Hooks.ServerJoin.Register(this, this.OnJoin);
        GetDataHandlers.PlayerUpdate.Register(this.OnPlayerUpdate);
        ServerApi.Hooks.ProjectileAIUpdate.Register(this, this.ProjectAiUpdate);
        TShockAPI.Commands.ChatCommands.Add(new Command("autofish", Commands.Afs, "af", "autofish"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GetDataHandlers.NewProjectile.UnRegister(this.ProjectNew);
            GetDataHandlers.NewProjectile.UnRegister(this.BuffUpdate);
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnJoin);
            GetDataHandlers.PlayerUpdate.UnRegister(this.OnPlayerUpdate);
            ServerApi.Hooks.ProjectileAIUpdate.Deregister(this, this.ProjectAiUpdate);
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.Afs);
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 玩家更新配置方法（创建配置结构）
    internal static MyData Data = new();
    private void OnJoin(JoinEventArgs args)
    {
        if (args == null || !Configuration.Instance.Enabled)
        {
            return;
        }

        var plr = TShock.Players[args.Who];

        if (plr == null)
        {
            return;
        }

        // 如果玩家不在数据表中，则创建新的数据条目
        if (!Data.Items.Any(item => item.Name == plr.Name))
        {
            Data.Items.Add(new MyData.ItemData()
            {
                Name = plr.Name,
                Enabled = true,
                Buff = true,
                Mod = false,
            });
        }
    }
    #endregion

    #region 触发自动钓鱼方法
    private void ProjectAiUpdate(ProjectileAiUpdateEventArgs args)
    {
        if (args.Projectile.owner is < 0 or > Main.maxPlayers ||
            !args.Projectile.active ||
            !args.Projectile.bobber ||
            !Configuration.Instance.Enabled)
        {
            return;
        }

        var plr = TShock.Players[args.Projectile.owner];
        if (plr == null || !plr.Active || !plr.HasPermission("autofish"))
        {
            return;
        }

        // 从数据表中获取与玩家名字匹配的配置项
        var list = Data.Items.FirstOrDefault(x => x.Name == plr.Name);
        // 如果数据表为空，开关没有开启则返回
        if (list == null || !list.Enabled)
        {
            return;
        }

        // 正常状态下 与消耗模式下启用自动钓鱼
        if (!Configuration.Instance.ConMod || (Configuration.Instance.ConMod && list.Mod))
        {
            if (args.Projectile.ai[1] < 0)
            {
                args.Projectile.ai[0] = 1.0f;

                var baitItem = new Item();

                // 检查并选择消耗饵料
                plr.TPlayer.ItemCheck_CheckFishingBobber_PickAndConsumeBait(args.Projectile, out var pullTheBobber, out var baitTypeUsed);
                if (pullTheBobber)
                {
                    plr.TPlayer.ItemCheck_CheckFishingBobber_PullBobber(args.Projectile, baitTypeUsed);

                    // 更新玩家背包 使用饵料信息
                    for (var i = 0; i < plr.TPlayer.inventory.Length; i++)
                    {
                        var inv = plr.TPlayer.inventory[i];

                        //玩家饵料（指的是你手上鱼竿上的那个数字），使用的饵料是背包里的物品
                        if (inv.bait > 0 && baitTypeUsed == inv.type)
                        {
                            //当物品数量正常则开始进入钓鱼检查
                            if (inv.stack > 1)
                            {
                                //发包到对应饵料的格子内
                                plr.SendData(PacketTypes.PlayerSlot, "", plr.Index, i);
                                break;
                            }

                            //当前物品数量为1则移除（避免选中的饵不会主动消失 变成无限饵 或 卡住线程）
                            if (inv.stack == 1 || inv.bait == 1)
                            {
                                inv.TurnToAir();
                                plr.SendData(PacketTypes.PlayerSlot, "", plr.Index, i);
                                break;
                            }
                        }
                    }
                }

                //松露虫 判断一下玩家是否在海边
                if (baitItem.type == 2673 && plr.X / 16 == Main.oceanBG && plr.Y / 16 == Main.oceanBG)
                {
                    args.Projectile.ai[1] = 0;
                    plr.SendData(PacketTypes.ProjectileNew, "", args.Projectile.whoAmI);
                    return;
                }

                //获得钓鱼物品方法
                var flag = false;
                var ActiveCount = TShock.Players.Where(plr => plr != null && plr.Active && plr.IsLoggedIn).Count();
                var Limit = Tools.GetLimit(ActiveCount); //根据人数动态调整Limit
                for (var count = 0; !flag && count < Limit; count++)
                {
                    args.Projectile.FishingCheck();

                    if (Configuration.Instance.Random)
                    {
                        args.Projectile.localAI[1] = Random.Shared.Next(1, ItemID.Count);
                    }

                    args.Projectile.ai[1] = args.Projectile.localAI[1];

                    // 如果额外渔获有任何1个物品ID，则参与AI[1]
                    if (Configuration.Instance.DoorItems.Any())
                    {
                        if (args.Projectile.ai[1] <= 0)
                        {
                            args.Projectile.ai[1] = Configuration.Instance.DoorItems[Main.rand.Next(Configuration.Instance.DoorItems.Count)];
                        }
                    }

                    flag = args.Projectile.ai[1] > 0;
                }

                if (!flag)
                {
                    return;
                }


                //这里发的是连续弹幕 避免线断 因为弹幕是不需要玩家物理点击来触发收杆的
                plr.SendData(PacketTypes.ProjectileNew, "", args.Projectile.whoAmI);
                
                var index = Projectile.NewProjectile(
                    args.Projectile.GetProjectileSource_FromThis(),
                    // NewProjectile uses centred position
                    args.Projectile.position + new Vector2((float)args.Projectile.width / 2, (float)args.Projectile.height / 2),
                    args.Projectile.velocity,
                    args.Projectile.type,
                    0,
                    0,
                    args.Projectile.owner);

                plr.SendData(PacketTypes.ProjectileNew, "", index);
            }
        }
    }
    #endregion

    #region 多线钓鱼
    public void ProjectNew(object? sender, GetDataHandlers.NewProjectileEventArgs e)
    {
        var plr = e.Player;
        var bobberCount = Main.projectile.Count(p => p.active && p.owner == e.Owner && p.bobber); // 浮漂计数

        if (plr == null ||
            !plr.Active ||
            !plr.IsLoggedIn ||
            !Configuration.Instance.Enabled ||
            !Configuration.Instance.MoreHook ||
            !plr.HasPermission("autofish") ||
            bobberCount > Configuration.Instance.HookMax - 1)
        {
            return;
        }

        // 从数据表中获取与玩家名字匹配的配置项
        var playerData = Data.Items.FirstOrDefault(x => x.Name == plr.Name);
        // 如果没有找到配置项，或者自动钓鱼功能或启用状态未设置，则返回
        if (playerData is not { Enabled: true })
        {
            return;
        }

        // 正常状态下与消耗模式下启用多线钓鱼
        if (!Configuration.Instance.ConMod || (Configuration.Instance.ConMod && playerData.Mod))
        {
            // 检查是否上钩
            if (Tools.BobbersActive(e.Owner))
            {
                // since there is no `ProjectileID.Sets.IsBobber`, I can only determine in this way
                var dummyProjectile = new Projectile();
                dummyProjectile.SetDefaults(e.Type);
                if (!dummyProjectile.bobber)
                {
                    return;
                }
                
                var index = Projectile.NewProjectile(
                    Main.projectile[e.Index].GetProjectileSource_FromThis(),
                    e.Position,
                    e.Velocity,
                    e.Type,
                    e.Damage,
                    e.Knockback,
                    e.Owner);
                

                plr.SendData(PacketTypes.ProjectileNew, "", index);
            }
        }
    }
    #endregion

    #region Buff更新方法
    public void BuffUpdate(object? sender, GetDataHandlers.NewProjectileEventArgs e)
    {
        var plr = e.Player;

        if (plr == null || !plr.Active || !plr.IsLoggedIn || !Configuration.Instance.Enabled || !plr.HasPermission("autofish"))
        {
            return;
        }

        // 从数据表中获取与玩家名字匹配的配置项
        var list = Data.Items.FirstOrDefault(x => x.Name == plr.Name);
        if (list == null || !list.Buff)
        {
            return;
        }

        //出现鱼钩摆动就给玩家施加buff
        if (list.Enabled)
        {
            if (Tools.BobbersActive(e.Owner))
            {
                foreach (var buff in Configuration.Instance.BuffID)
                {
                    plr.SetBuff(buff.Key, buff.Value);
                }
            }
        }
    }
    #endregion

    #region 消耗模式:消耗物品开启自动钓鱼方法
    private void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
    {
        var plr = e.Player;
        if (!Configuration.Instance.Enabled || !Configuration.Instance.ConMod || e == null ||
            plr == null || !plr.IsLoggedIn || !plr.Active ||
            !plr.HasPermission("autofish"))
        {
            return;
        }

        var data = Data.Items.FirstOrDefault(x => x.Name == plr.Name);

        if (data == null || !data.Enabled)
        {
            return;
        }

        // 播报玩家消耗鱼饵用的
        var mess = new StringBuilder();

        //当玩家的自动钓鱼没开启时
        if (!data.Mod)
        {
            //初始化一个消耗值
            var sun = Configuration.Instance.BaitStack;

            // 统计背包中指定鱼饵的总数量(不包含手上物品)
            var TotalBait = plr.TPlayer.inventory.Sum(inv =>
            (Configuration.Instance.BaitType.Contains(inv.type) &&
            inv.type != plr.TPlayer.inventory[plr.TPlayer.selectedItem].type) ?
            inv.stack : 0);

            // 如果背包中有足够的鱼饵数量 和消耗值相等
            if (TotalBait >= sun)
            {
                // 遍历背包58格
                for (var i = 0; i < plr.TPlayer.inventory.Length && sun > 0; i++)
                {
                    var inv = plr.TPlayer.inventory[i];

                    // 是Config里指定的鱼饵,不是手上的物品
                    if (Configuration.Instance.BaitType.Contains(inv.type))
                    {
                        var BaitStack = Math.Min(sun, inv.stack); // 计算需要消耗的鱼饵数量

                        inv.stack -= BaitStack; // 从背包中扣除鱼饵
                        sun -= BaitStack; // 减少消耗值

                        // 记录消耗的鱼饵数量到播报
                        mess.AppendFormat(" [c/F25156:{0}]([c/AECDD1:{1}]) ", TShock.Utils.GetItemById(inv.type).Name, BaitStack);

                        // 如果背包中的鱼饵数量为0，清空该格子
                        if (inv.stack < 1)
                        {
                            inv.TurnToAir();
                        }

                        // 发包给背包里对应格子的鱼饵
                        plr.SendData(PacketTypes.PlayerSlot, "", plr.Index, PlayerItemSlotID.Inventory0 + i);
                    }
                }

                // 消耗值清空时，开启自动钓鱼开关
                if (sun <= 0)
                {
                    data.Mod = true;
                    data.LogTime = DateTime.Now;
                    plr.SendMessage(GetString($"玩家 [c/46C2D4:{plr.Name}] 已开启[c/F5F251:自动钓鱼] 消耗物品为:{mess}"), 247, 244, 150);
                }
            }
        }

        else //当 data.Mod 开启时
        {
            //由它判断关闭自动钓鱼
            ExitMod(plr, data);
        }
    }
    #endregion

    #region 消耗模式:过期后关闭自动钓鱼方法
    private static int ClearCount = 0; //需要关闭钓鱼权限的玩家计数
    private static void ExitMod(TSPlayer plr, MyData.ItemData data)
    {
        var mess2 = new StringBuilder();
        mess2.AppendLine(GetString($"[i:3455][c/AD89D5:自][c/D68ACA:动][c/DF909A:钓][c/E5A894:鱼][i:3454]"));
        mess2.AppendLine(GetString($"以下玩家超过 [c/E17D8C:{Configuration.Instance.timer}] 分钟 已关闭[c/76D5B4:自动钓鱼]权限："));

        // 只显示分钟
        var Minutes = (DateTime.Now - data.LogTime).TotalMinutes;

        // 时间过期 关闭自动钓鱼权限
        if (Minutes >= Configuration.Instance.timer)
        {
            ClearCount++;
            data.Mod = false;
            data.LogTime = default; // 清空记录时间
            mess2.AppendFormat(GetString("[c/A7DDF0:{0}]:[c/74F3C9:{1}分钟]"), data.Name, Math.Floor(Minutes));
        }

        // 确保有一个玩家计数，只播报一次
        if (ClearCount > 0 && mess2.Length > 0)
        {
            plr.SendMessage(mess2.ToString(), 247, 244, 150);
            ClearCount = 0;
        }
    }
    #endregion
}