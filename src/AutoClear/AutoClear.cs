using LazyAPI;
using Terraria;
using Terraria.GameContent.Events;
using TerrariaApi.Server;
using TShockAPI;

namespace AutoClear;

[ApiVersion(2, 1)]
public class AutoClear(Main game) : LazyPlugin(game)
{
    public override string Author => "大豆子[Mute适配1447]，肝帝熙恩更新";
    public override string Description => GetString("智能扫地机");
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 1, 0);

    private bool _sweepScheduled;
    private DateTime _sweepScheduledAt;
    private long _updateCounter;

    public override void Initialize()
    {
        base.Initialize();
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
        }
        base.Dispose(disposing);
    }

    private void OnUpdate(EventArgs args)
    {
        this._updateCounter++;

        if (this._updateCounter % (60 * Configuration.Instance.DetectionIntervalSeconds) != 0)
        {
            return;
        }

        if (Main.item.Count(i => i is { active: true } && !Configuration.Instance.NonSweepableItemIDs.Contains(i.type)) < Configuration.Instance.SmartSweepThreshold)
        {
            return;
        }
        
        if (!this._sweepScheduled)
        {
            this._sweepScheduled = true;
            this._sweepScheduledAt = DateTime.UtcNow.AddSeconds(Configuration.Instance.DelayedSweepTimeoutSeconds);
            TSPlayer.All.SendSuccessMessage($"{Configuration.Instance.DelayedSweepCustomMessage}");
        }
        
        if (this._sweepScheduled && DateTime.UtcNow >= this._sweepScheduledAt)
        {
            // 到达清扫时间，执行清扫任务
            this._sweepScheduled = false;
            if (CanSweep())
            {
                PerformSmartSweep();
            }
            else
            {
                TSPlayer.All.SendSuccessMessage(GetString($"智能扫地机: 由于存在事件或BOSS，已跳过自动清理"));
            }
            
        }
    }

    private static bool CanSweep()
    {
        // 入侵
        if (Main.invasionType != 0 && Main.invasionSize != 0)
        {
            return false;
        }

        // 血月 | 旧日军团 | 日食
        if (Main.bloodMoon || DD2Event.Ongoing || Main.eclipse)
        {
            return false;
        }

        // BOSS
        if (Main.npc.Any(npc => npc is { active: true, boss: true }))
        {
            return false;
        }
        
        return true;

    }

    private static void PerformSmartSweep()
    {

        var totalItems = 0;
        var totalThrowable = 0;
        var totalSwinging = 0;
        var totalRegular = 0;
        var totalEquipment = 0;
        var totalVanity = 0;

        for (var i = 0; i < Main.item.Length; i++)
        {
            if (Main.item[i].active && !Configuration.Instance.NonSweepableItemIDs.Contains(Main.item[i].type))
            {
                var isThrowable = Main.item[i].damage > 0 && Main.item[i].maxStack > 1;
                var isSwinging = Main.item[i].damage > 0 && Main.item[i].maxStack == 1;
                var isRegular = Main.item[i].damage < 0 && Main.item[i].maxStack > 1;
                var isEquipment = Main.item[i].damage == 0 && Main.item[i].maxStack == 1;
                var isVanity = Main.item[i].damage < 0 && Main.item[i].maxStack == 1;

                if ((Configuration.Instance.SweepThrowable && isThrowable) ||
                    (Configuration.Instance.SweepSwinging && isSwinging) ||
                    (Configuration.Instance.SweepRegular && isRegular) ||
                    (Configuration.Instance.SweepEquipment && isEquipment) ||
                    (Configuration.Instance.SweepVanity && isVanity))
                {
                    Main.item[i].active = false;
                    TSPlayer.All.SendData(PacketTypes.ItemDrop, null, i);
                    totalItems++;

                    if (isThrowable)
                    {
                        totalThrowable++;
                    }

                    if (isSwinging)
                    {
                        totalSwinging++;
                    }

                    if (isRegular)
                    {
                        totalRegular++;
                    }

                    if (isEquipment)
                    {
                        totalEquipment++;
                    }

                    if (isVanity)
                    {
                        totalVanity++;
                    }
                }
            }
        }

        if (totalItems > 0)
        {
            if (!string.IsNullOrEmpty(Configuration.Instance.CustomMessage))
            {
                TSPlayer.All.SendSuccessMessage($"{Configuration.Instance.CustomMessage}");
            }

            if (Configuration.Instance.SpecificMessage)
            {
                TSPlayer.All.SendSuccessMessage(GetString($"智能扫地机已清扫：[c/FFFFFF:{totalItems}]种物品"));
                TSPlayer.All.SendSuccessMessage(GetString($"包含：【投掷武器[c/FFFFFF:{totalThrowable}]】-【挥动武器[c/FFFFFF:{totalSwinging}]】-【普通物品[c/FFFFFF:{totalRegular}]】-【装备[c/FFFFFF:{totalEquipment}]】-【时装[c/FFFFFF:{totalVanity}]】"));
            }
        }
    }
}