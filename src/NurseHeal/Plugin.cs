using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace NurseHeal;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    /// <summary>玩家与护士的最大交互距离(像素)。原版客户端只能在近距离内右键护士,这里放宽到 50 格,防改包客户端远程治疗。</summary>
    private const float MaxTalkDistance = 800f;

    public override string Author => "不是现在";

    public override string Description => "与护士对话:按原版定价公式扣费,回满血并移除可移除的减益";

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name;

    public override Version Version => new Version(1, 0, 0, 0);

    public Plugin(Main game)
        : base(game)
    {
        Order = 3;
    }

    public override void Initialize()
    {
        GetDataHandlers.NpcTalk.Register(OnNpcTalk, HandlerPriority.Normal, false);
        TShock.Log.ConsoleInfo($"[护士治疗 v{Version}] 已加载(原版定价公式 · 移除减益 · 找零不足即回滚)");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GetDataHandlers.NpcTalk.UnRegister(OnNpcTalk);
        }
        base.Dispose(disposing);
    }

    private void OnNpcTalk(object? sender, GetDataHandlers.NpcTalkEventArgs e)
    {
        if (e.Handled || e.NPCTalkTarget < 0 || e.NPCTalkTarget >= Main.npc.Length)
        {
            return;
        }
        NPC nurse = Main.npc[e.NPCTalkTarget];
        if (nurse is not { active: true, netID: NPCID.Nurse })
        {
            return;
        }
        TSPlayer player = e.Player;
        if (player is not { Active: true, Dead: false, TPlayer: not null }
            || player.IsBeingDisabled() || player.IsBouncerThrottled())
        {
            return;
        }

        // 吞掉原包并回发 NpcTalk(-1) 关闭客户端原版护士界面(与 TShock 对 disabled 玩家的处理一致),
        // 之后由本插件统一结算,避免玩家再手动点原版“治疗”按钮、由客户端决定扣多少钱。
        e.Handled = true;
        player.SendData(PacketTypes.NpcTalk, "", player.Index, -1f);

        // 原版客户端只能在交互距离内右键护士;服务端补上距离校验(README 未覆盖、旧版缺失)
        if (DistanceSq(player.TPlayer, nurse) > MaxTalkDistance * MaxTalkDistance)
        {
            player.SendErrorMessage("你距离护士太远,无法接受治疗。");
            return;
        }

        ServeNurse(player, nurse);
    }

    private static float DistanceSq(Player p, NPC nurse)
    {
        float dx = p.Center.X - nurse.Center.X;
        float dy = p.Center.Y - nurse.Center.Y;
        return dx * dx + dy * dy;
    }

    private static void ServeNurse(TSPlayer player, NPC nurse)
    {
        Player p = player.TPlayer;
        int missing = Math.Max(0, p.statLifeMax2 - p.statLife);
        List<int> debuffSlots = VanillaPricing.CollectRemovableDebuffSlots(p);
        long price = VanillaPricing.ComputePrice(p, nurse);

        if (missing <= 0 && debuffSlots.Count == 0)
        {
            player.SendInfoMessage("你现在生命值已满,也没有可清除的减益,无需治疗。");
            return;
        }

        if (PaymentService.GetBalance(p) < price)
        {
            player.SendErrorMessage($"治疗需要 {price} 铜币,你的余额不足!");
            return;
        }

        DeductResult pay = PaymentService.Deduct(p, price);
        if (!pay.Success)
        {
            player.SendErrorMessage("治疗失败:" + (pay.Reason ?? "扣款失败"));
            return;
        }

        // 扣款成功后才同步被改动的物品槽(纯内存操作,不走 GiveItem,避免 SSC/掉落分支竞态)
        foreach (int slot in pay.ChangedSlots)
        {
            player.SendData(PacketTypes.PlayerSlot, "", player.Index, slot, 0f, 0f, 0);
        }

        // 回血:TShock 6.2.1 的 TSPlayer.Heal 只广播 PlayerHealOther(66) 包、不改服务端 statLife,
        // 所以服务端必须先手动落账(若未来 Heal 语义变化需回看此处)
        if (missing > 0)
        {
            p.statLife = Math.Min(p.statLifeMax2, p.statLife + missing);
            player.Heal(missing);
        }

        // 移除减益并全量同步 buff 列表(msg 50),客户端收到后同步清掉本地减益
        if (debuffSlots.Count > 0)
        {
            foreach (int i in debuffSlots)
            {
                p.buffType[i] = 0;
                p.buffTime[i] = 0;
            }
            player.SendData(PacketTypes.PlayerBuff, "", player.Index);
        }

        string lifePart = missing > 0 ? $"回复 {missing} 点生命" : "";
        string buffPart = debuffSlots.Count > 0 ? $"清除 {debuffSlots.Count} 个减益" : "";
        string join = lifePart.Length > 0 && buffPart.Length > 0 ? "," : "";
        player.SendSuccessMessage($"护士治疗完成:{lifePart}{join}{buffPart},花费 {price} 铜币。");
        TShock.Log.Write($"[护士治疗] {player.Name}:{lifePart}{join}{buffPart},花费 {price} 铜币", TraceLevel.Info);
    }
}
