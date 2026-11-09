using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace NurseHeal;

/// <summary>
/// 扣钱:扫背包 0–57(含 50–53 钱币槽)、存钱罐、保险箱里的原版钱币,
/// 先整体清空 → 用 999 大堆叠从铂金到铜币重发找零 → 槽位不足时整体回滚并拒绝本次治疗。
/// 所有改动只动内存,成功后由调用方按 ChangedSlots 发 PlayerSlot 同步包。
/// </summary>
public static class PaymentService
{
    private sealed record CoinSlot(Item Item, int SlotId);

    private const int CoinStackMax = 999; // 原版钱币堆叠上限(旧版误用 100,白白多占 10 倍槽位)

    private static readonly int[] CoinIds = { 71, 72, 73, 74 };

    private static readonly long[] CoinValues = { 1L, 100L, 10000L, 1000000L };

    public static long GetBalance(Player p)
    {
        return SumCoins(GetCoinSlots(p).Where(c => IsCoin(c.Item)));
    }

    public static DeductResult Deduct(Player p, long price)
    {
        if (price <= 0)
        {
            return DeductResult.Ok(new List<int>());
        }

        List<CoinSlot> coinSlots = GetCoinSlots(p);
        List<CoinSlot> coins = coinSlots.Where(c => IsCoin(c.Item)).ToList();
        long balance = SumCoins(coins);
        if (balance < price)
        {
            return DeductResult.Fail("背包钱币不足");
        }

        long change = balance - price;

        // 1) 快照原有钱币,失败时可整体回滚
        List<Snapshot> snapshot = coins
            .Select(c => new Snapshot(c.SlotId, c.Item, c.Item.type, c.Item.stack, c.Item.prefix))
            .ToList();

        // 2) 清空全部钱币
        foreach (CoinSlot c in coins)
        {
            c.Item.type = 0;
            c.Item.stack = 0;
            c.Item.prefix = 0;
        }

        // 3) 可用于找零的空槽:被清空的钱币槽 + 原本就空的槽(按引用去重)
        var free = new List<(Item Item, int SlotId)>();
        var seen = new HashSet<Item>();
        foreach (CoinSlot c in coins)
        {
            if (seen.Add(c.Item))
            {
                free.Add((c.Item, c.SlotId));
            }
        }
        foreach (CoinSlot cs in coinSlots)
        {
            Item it = cs.Item;
            if (it != null && seen.Add(it) && it.type == 0 && it.stack == 0)
            {
                free.Add((it, cs.SlotId));
            }
        }

        // 4) 从铂金到铜币贪心分配找零;槽位不足则回滚并拒绝(旧版会静默吞掉找零)
        var filled = new List<int>();
        if (!AllocateChange(change, free, filled))
        {
            foreach (Snapshot s in snapshot)
            {
                s.Item.type = s.Type;
                s.Item.stack = s.Stack;
                s.Item.prefix = s.Prefix;
            }
            return DeductResult.Fail("背包无法容纳找零(空间不足),本次未扣费");
        }

        // 5) 需要同步的槽位 = 被清空的钱币槽 ∪ 被写入找零的槽
        var touched = new SortedSet<int>();
        foreach (Snapshot s in snapshot)
        {
            touched.Add(s.SlotId);
        }
        foreach (int slot in filled)
        {
            touched.Add(slot);
        }
        return DeductResult.Ok(touched.ToList());
    }

    private static bool AllocateChange(long change, List<(Item Item, int SlotId)> freeSlots, List<int> filledSlots)
    {
        int idx = 0;
        long remaining = change;
        for (int d = CoinIds.Length - 1; d >= 0 && remaining > 0; d--)
        {
            long denom = CoinValues[d];
            long count = remaining / denom;
            remaining -= count * denom;
            while (count > 0)
            {
                if (idx >= freeSlots.Count)
                {
                    return false;
                }
                long stack = Math.Min(count, CoinStackMax);
                (Item Item, int SlotId) e = freeSlots[idx++];
                e.Item.type = CoinIds[d];
                e.Item.stack = (int)stack;
                e.Item.prefix = 0;
                filledSlots.Add(e.SlotId);
                count -= stack;
            }
        }
        return true;
    }

    private static long SumCoins(IEnumerable<CoinSlot> slots)
    {
        return slots.Sum(c => (long)c.Item.stack * CoinValueOf(c.Item.type));
    }

    private static List<CoinSlot> GetCoinSlots(Player p)
    {
        var list = new List<CoinSlot>();
        for (int i = 0; i < p.inventory.Length && i < 58; i++)
        {
            list.Add(new CoinSlot(p.inventory[i], PlayerItemSlotID.Inventory0 + i));
        }
        for (int j = 0; j < p.bank.item.Length; j++)
        {
            list.Add(new CoinSlot(p.bank.item[j], PlayerItemSlotID.Bank1_0 + j));
        }
        for (int k = 0; k < p.bank2.item.Length; k++)
        {
            list.Add(new CoinSlot(p.bank2.item[k], PlayerItemSlotID.Bank2_0 + k));
        }
        return list;
    }

    private static bool IsCoin(Item? item)
    {
        return item != null && item.stack > 0 && Array.IndexOf(CoinIds, item.type) >= 0;
    }

    private static long CoinValueOf(int type)
    {
        int i = Array.IndexOf(CoinIds, type);
        return i < 0 ? 0L : CoinValues[i];
    }

    private sealed record Snapshot(int SlotId, Item Item, int Type, int Stack, byte Prefix);
}
