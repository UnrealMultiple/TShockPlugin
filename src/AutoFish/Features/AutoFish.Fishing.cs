using AutoFish.Utils;
using Terraria;
using Terraria.ID;
using TShockAPI;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace AutoFish;

public partial class Plugin
{
    private void OnAI_061_FishingBobber(Projectile projectile,
        HookEvents.Terraria.Projectile.AI_061_FishingBobberEventArgs args)
    {
        this.HookUpdate(projectile);
        args.ContinueExecution = false;
    }

    /// <summary>
    ///     触发自动钓鱼，处理浮漂 AI 更新与收杆逻辑。原理：每次AI更新后尝试为玩家把鱼钓起来，并生成一个新的同样的弹射物
    /// </summary>
    private void HookUpdate(Projectile hook)
    {

        if (hook.ai[0] >= 1f)
        {
            return;
        }
        if (hook.owner < 0)
        {
            Uitls.DebugInfoLog($"[AutoFishR-DEBUG] hook.owner < 0");
            return;
        }

        if (hook.owner > Main.maxPlayers)
        {
            Uitls.DebugInfoLog($"[AutoFishR-DEBUG] hook.owner > Main.maxPlayers");
            return;
        }

        if (!hook.active)
        {
            
           Uitls.DebugInfoLog($"[AutoFishR-DEBUG] hook is not active");
            return;
        }

        if (!hook.bobber)
        {
            Uitls.DebugInfoLog($"[AutoFishR-DEBUG] hook is not bobber");
            return;
        }

        if (!Configuration.Instance.Enabled)
        {
            Uitls.DebugInfoLog($"[AutoFishR-DEBUG] Plugin not enabled");
            return;
        }

        if (!Configuration.Instance.GlobalAutoFishFeatureEnabled)
        {
            Uitls.DebugInfoLog($"[AutoFishR-DEBUG] Global auto fish feature not enabled");
            return;
        }

        var player = TShock.Players[hook.owner];
        if (player == null)
        {
            Uitls.DebugInfoLog($"[AutoFishR-DEBUG] Player is null (owner: {hook.owner})");
            return;
        }

        //if (!player.Active)
        //{
        //    if (DebugMode) player.SendInfoMessage($"[DEBUG] Player not active");
        //    return;
        //}

        var blockMonsterCatch = Configuration.Instance.GlobalBlockMonsterCatch &&
                                HasFeaturePermission(player, "filter.monster");
        var skipFishingAnimation = Configuration.Instance.GlobalSkipFishingAnimation &&
                                   HasFeaturePermission(player, "skipanimation");
        var blockQuestFish = Configuration.Instance.GlobalBlockQuestFish &&
                             HasFeaturePermission(player, "filter.quest");
        var protectValuableBait = Configuration.Instance.GlobalProtectValuableBaitEnabled &&
                                  HasFeaturePermission(player, "bait.protect");

        var playerData = PlayerData.GetOrCreatePlayerData(player.Name, CreateDefaultPlayerData);
        if (!playerData.AutoFishEnabled)
        {
            if (!HasFeaturePermission(player, "fish"))
            {
                if (DebugMode)
                {
                    player.SendInfoMessage($"[DEBUG] No permission for fish feature");
                }
                return;
            }

            if (playerData.FirstFishHintShown)
            {
                if (DebugMode)
                {
                    player.SendInfoMessage($"[DEBUG] First fish hint already shown, auto fish not enabled");
                }
                return;
            }

            playerData.FirstFishHintShown = true;
            player.SendInfoMessage(GetString("检测到你正在钓鱼，可使用 /af fish 开启自动钓鱼。"));
            return;
        }

        blockMonsterCatch &= playerData.BlockMonsterCatch;
        skipFishingAnimation &= playerData.SkipFishingAnimation;
        blockQuestFish &= playerData.BlockQuestFish;
        protectValuableBait &= playerData.ProtectValuableBaitEnabled;

        //负数时候为咬钩倒计时，说明上鱼了
        if (!(hook.ai[1] < 0))
        {
            // if (DebugMode) player.SendInfoMessage($"[DEBUG] Fish not hooked yet (ai[1]: {hook.ai[1]:F2})");
            return;
        }

        player.TPlayer.Fishing_GetBait(out var baitPower, out var baitType);
        if (baitType == 0) //没有鱼饵，不要继续
        {
            player.SendErrorMessage(GetString("没有鱼饵了！"));
            player.SendInfoMessage(GetString("自动钓鱼已停止，请补充鱼饵后重新抛竿。"));
            ResetHook(hook);
            return;
        }

        // 保护贵重鱼饵：将其移到背包末尾以避免被消耗
        if (protectValuableBait && Configuration.Instance.ValuableBaitItemIds.Contains(baitType))
        {
            if (Uitls.TrySwapValuableBaitToBack(player, baitType, Configuration.Instance.ValuableBaitItemIds,
                    out var fromSlot, out var toSlot, out var fromType, out var toType))
            {
                player.SendData(PacketTypes.PlayerSlot, "", player.Index, fromSlot);
                player.SendData(PacketTypes.PlayerSlot, "", player.Index, toSlot);
                var fromName = TShock.Utils.GetItemById(fromType).Name;
                var toName = TShock.Utils.GetItemById(toType).Name;
                Uitls.SendGradientMessage(player,
                    GetString("检测到贵重鱼饵，已与背包末尾鱼饵交换：{0} -> {1} (槽位 {2} ↔ {3})").SFormat(fromName, toName, fromSlot, toSlot));
                ResetHook(hook);
                return;
            }
            else //就剩下一个了
            {
                var baitName = TShock.Utils.GetItemById(baitType).Name;
                Uitls.SendGradientMessage(player, GetString("已保护贵重鱼饵 [{0}]，这是最后一个，已停止钓鱼。").SFormat(baitName));
                ResetHook(hook);
                player.SendData(PacketTypes.ProjectileDestroy, "", hook.whoAmI);
                return;
            }
        }


        // 正常状态下与消耗模式下启用自动钓鱼
        if (Configuration.Instance.GlobalConsumptionModeEnabled)
        {
            //消耗模式判定
            if (!CanConsumeFish(player, playerData))
            {
                player.SendInfoMessage(GetString("服务器已开启消耗模式，你缺少指定物品无法自动钓鱼！使用 /af list 查看需要的物品。"));
                ResetHook(hook);
                player.SendData(PacketTypes.ProjectileDestroy, "", hook.whoAmI);
                return;
            }
        }

        //修改钓鱼得到的东西
        //获得钓鱼物品方法
        var noCatch = true;
        var activePlayerCount = TShock.Players.Count(p => p != null && p.Active && p.IsLoggedIn);
        var dropLimit = Uitls.GetLimit(activePlayerCount); //根据人数动态调整Limit
        var catchMonster = false;
        for (var count = 0; noCatch && count < dropLimit; count++)
        {
            var context = Projectile._context;
            if (hook.TryBuildFishingContext(context))
            {
                //屏蔽任务鱼
                if (blockQuestFish)
                {
                    context.Fisher.questFish = -1;
                }

                hook.SetFishingCheckResults(ref context.Fisher);
            }


            var catchId = hook.localAI[1];

            if (context.Fisher.rolledEnemySpawn > 0) //抓到怪物
            {
                if (blockMonsterCatch)
                {
                    continue; //不想抓怪物
                }

                catchMonster = true;
                noCatch = false;
            }

            // 怪物生成使用localAI[1]，而物品则使用ai[1]，小于0情况无需处理，是刷血月怪
            if (context.Fisher.rolledItemDrop > 0)
            {
                noCatch = false;
            }

            if (noCatch)
            {
                continue; //真没抓到
            }

            hook.localAI[1] = catchId; //数值置回
            break; //抓到就不应该继续判断
        }

        if (noCatch)
        {
            if (DebugMode)
            {
                player.SendInfoMessage($"[DEBUG] No catch after {dropLimit} attempts");
            }

            ResetHook(hook);
            return; //没抓到，不抬杆
        }

        //设置为收杆状态
        hook.ai[0] = 1.0f;

        var nowBaitType = (int) hook.localAI[2];

        // 让服务器扣饵料
        var locate = LocateBait(player, nowBaitType);
        var pull = player.TPlayer.ItemCheck_CheckFishingBobber_ConsumeBait(hook,
            out var baitUsed);
        if (nowBaitType != baitUsed)
        {
            if (DebugMode) player.SendInfoMessage($"[DEBUG] Bait mismatch: now={nowBaitType}, used={baitUsed}");
            player.SendMessage(GetString("鱼饵不一致"), Colors.CurrentLiquidColor);
            return;
        }

        if (!pull)
        {
            if (DebugMode) player.SendInfoMessage($"[DEBUG] Cannot pull, bait may be depleted");
            return; //说明鱼饵没了，不能继续，否则可能会卡bug
        }

        //Buff更新
        if (playerData.BuffEnabled)
        {
            BuffUpdate(player);
        }

        //原版收杆函数，这里会使得  bobber.ai[1] = bobber.localAI[1];，必须调用此函数，否则杆子会爆一堆弹幕，并且鱼饵会全不见，也是刷怪的函数
        player.TPlayer.ItemCheck_CheckFishingBobber_PullBobber(hook, baitUsed);
        // 同步玩家背包
        player.SendData(PacketTypes.PlayerSlot, "", player.Index, locate);

        var origPos = hook.position;
        if (!catchMonster) //抓到怪物触发会导致刷鱼漂，这里是重新设置溅射物
        {
            SpawnHook(player, hook, origPos);
            //多钩钓鱼代码
            this.AddMultiHook(player, hook, origPos);
        }

        // 原版给东西的代码，在kill函数，会把ai[1]给玩家
        // 这里发的是连续弹幕 避免线断 因为弹幕是不需要玩家物理点击来触发收杆的，但是服务端和客户端概率测算不一样，会导致服务器扣了饵料，但是客户端没扣
        player.SendData(PacketTypes.ProjectileNew, "", hook.whoAmI);
        //服务器的netMod为2

        if (skipFishingAnimation) //跳过上鱼动画
        {
            player.SendData(PacketTypes.ProjectileDestroy, "", hook.whoAmI);
        }
    }

    private static void SpawnHook(TSPlayer player, Projectile hook, Vector2 pos, string uuid = "")
    {
        var velocity = new Vector2(0, 0);
        // var pos = new Vector2(hook.position.X, hook.position.Y + 3);
        var index = SpawnProjectile.NewProjectile(
            hook.GetProjectileSource_FromThis(),
            pos, velocity, hook.type, 0, 0,
            player.Index, 0, 0, 0, -1, uuid);
        player.SendData(PacketTypes.ProjectileNew, "", index);
    }

    private static void ResetHook(Projectile projectile)
    {
        //设置成没上鱼，无状态
        projectile.ai[1] = 0;
        //清空进度
        projectile.localAI[1] = 0;
        //原版岩浆类似逻辑，加点进度
        projectile.localAI[1] += 240f;
    }

    private static int LocateBait(TSPlayer player, int baitUsed)
    {
        // 更新玩家背包 使用饵料信息
        for (var i = 0; i < player.TPlayer.inventory.Length; i++)
        {
            var inventorySlot = player.TPlayer.inventory[i];
            // 玩家饵料（指的是你手上鱼竿上的那个数字），使用的饵料是背包里的物品
            if (inventorySlot.bait <= 0 || baitUsed != inventorySlot.type)
            {
                continue;
            }
            return i;
        }
        return 0;
    }

    private static void BuffUpdate(TSPlayer player)
    {
        if (!Configuration.Instance.GlobalBuffFeatureEnabled)
        {
            return;
        }

        foreach (var buff in Configuration.Instance.BuffDurations)
        {
            player.SetBuff(buff.Key, buff.Value);
        }
    }
}