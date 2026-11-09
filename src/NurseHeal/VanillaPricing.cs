using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace NurseHeal;

/// <summary>
/// 逐行复刻原版护士定价(1.4.4 OTAPI 服务端 Main.GetNurseHealCost):
/// cost = (缺失生命 + 可计费减益数×100) × 世界进度(石巨人后封顶 ×200) × 专家/大师(×2)
///        × 折扣卡(×0.8) × 快乐度(ShopHelper,0.75~1.5,拒绝服务时为 1000)。
/// 快乐度直接调用原版 ShopHelper.GetShoppingSettings,不再依赖不存在的 NPC.happy/unhappy 字段。
/// </summary>
public static class VanillaPricing
{
    private static readonly object ShopLock = new();
    private static ShopHelper? _shopHelper;

    /// <summary>原版计费口径:Main.GetNurseHealCost(debuff 且 buffTime &gt; 60 且非 NurseCannotRemoveDebuff)。</summary>
    public static int CountChargedDebuffs(Player p)
    {
        int num = 0;
        for (int i = 0; i < Player.maxBuffs && i < p.buffType.Length && i < p.buffTime.Length; i++)
        {
            int type = p.buffType[i];
            if (type <= 0 || type >= Main.debuff.Length || p.buffTime[i] <= 60)
            {
                continue;
            }
            if (Main.debuff[type] && !BuffID.Sets.NurseCannotRemoveDebuff[type])
            {
                num++;
            }
        }
        return num;
    }

    /// <summary>收集可被护士移除的减益槽位(原版移除口径:Main.NPCChatText_DoNurseHeal,debuff 且 buffTime &gt; 0)。</summary>
    public static List<int> CollectRemovableDebuffSlots(Player p)
    {
        var slots = new List<int>();
        for (int i = 0; i < Player.maxBuffs && i < p.buffType.Length && i < p.buffTime.Length; i++)
        {
            int type = p.buffType[i];
            if (type <= 0 || type >= Main.debuff.Length || p.buffTime[i] <= 0)
            {
                continue;
            }
            if (Main.debuff[type] && !BuffID.Sets.NurseCannotRemoveDebuff[type])
            {
                slots.Add(i);
            }
        }
        return slots;
    }

    public static long ComputePrice(Player p, NPC nurse)
    {
        int num = Math.Max(0, p.statLifeMax2 - p.statLife) + CountChargedDebuffs(p) * 100;

        // 世界进度倍率:与原版一致的 if/else 链(互相独立取一档,石巨人后即最高档 ×200;
        // 月球领主档在原版公式中不存在)
        if (NPC.downedGolemBoss) num *= 200;
        else if (NPC.downedPlantBoss) num *= 150;
        else if (NPC.downedMechBossAny) num *= 100;
        else if (Main.hardMode) num *= 60;
        else if (NPC.downedBoss3 || NPC.downedQueenBee) num *= 25;
        else if (NPC.downedBoss2) num *= 10;
        else if (NPC.downedBoss1) num *= 3;

        // 专家/大师 ×2:OTAPI 中 Main.expertMode 是 hook 属性(非字段),必须直接读
        if (Main.expertMode) num *= 2;

        // 折扣卡(物品 854 / 指环 3035 生效时原版 ×0.8)
        if (p.discountAvailable)
        {
            num = (int)((float)num * 0.8f);
        }

        num = (int)((float)num * GetPriceAdjustment(p, nurse));
        return Math.Max(0, num);
    }

    private static float GetPriceAdjustment(Player p, NPC nurse)
    {
        try
        {
            lock (ShopLock)
            {
                _shopHelper ??= new ShopHelper();
                return _shopHelper.GetShoppingSettings(p, nurse).PriceAdjustment;
            }
        }
        catch
        {
            return 1f; // 任一环节异常时回退中性值,永不抛出
        }
    }
}
