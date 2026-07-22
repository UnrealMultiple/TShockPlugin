using LazyAPI;
using Terraria;
using Terraria.GameContent.Events;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace AutoClear;

[ApiVersion(2, 1)]
public class AutoClear(Main game) : LazyPlugin(game)
{
    private const int ClearBatchSize = 7;
    private const int ClearBatchIntervalTicks = 1;

    public override string Author => "大豆子[Mute适配1447]，肝帝熙恩十七更新";
    public override string Description => GetString("智能扫地机");
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 2, 0);

    private bool _sweepScheduled;
    private DateTime _sweepScheduledAt;
    private long _updateCounter;
    private SweepJob? _sweepJob;

    public override void Initialize()
    {
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        PlayerHooks.PlayerCommand += this.OnPlayerCommand;
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
            PlayerHooks.PlayerCommand -= this.OnPlayerCommand;
        }
        base.Dispose(disposing);
    }

    private void OnUpdate(EventArgs args)
    {
        this._updateCounter++;
        this.ProcessSweepJob();

        if (this._sweepJob is not null)
        {
            return;
        }

        if (this._sweepScheduled)
        {
            if (DateTime.UtcNow < this._sweepScheduledAt)
            {
                return;
            }
            // 超时，立即执行清理
            this._sweepScheduled = false;
            if (CanSweep())
            {
                this.QueueSmartSweep();
            }
            else
            {
                TSPlayer.All.SendSuccessMessage(GetString("智能扫地机: 由于存在事件或BOSS，已跳过自动清理"));
            }

            return;
        }

        var detectionIntervalTicks = 60L * Math.Max(1, Configuration.Instance.DetectionIntervalSeconds);
        if (this._updateCounter % detectionIntervalTicks != 0 ||
            Main.item.Count(i => i is { active: true } && !Configuration.Instance.NonSweepableItemIDs.Contains(i.type)) < Configuration.Instance.SmartSweepThreshold)
        {
            return;
        }

        this._sweepScheduled = true;
        this._sweepScheduledAt = DateTime.UtcNow.AddSeconds(Configuration.Instance.DelayedSweepTimeoutSeconds);
        TSPlayer.All.SendSuccessMessage($"{Configuration.Instance.DelayedSweepCustomMessage}");
    }

    private void OnPlayerCommand(PlayerCommandEventArgs args)
    {
        if (!args.CommandName.Equals("clear", StringComparison.OrdinalIgnoreCase) ||
            args.Parameters.Count != 1 ||
            !args.Parameters[0].Equals("item", StringComparison.OrdinalIgnoreCase) ||
            !args.Player.HasPermission("tshock.clear"))
        {
            return;
        }

        args.Handled = true;
        if (this._sweepJob is not null)
        {
            args.Player.SendInfoMessage("已有物品清理任务正在执行。");
            return;
        }

        this._sweepScheduled = false;
        this.QueueManualItemSweep(args.Player);
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

    private void QueueSmartSweep()
    {
        var items = new Queue<SweepItem>();

        for (var i = 0; i < Main.item.Length; i++)
        {
            if (Main.item[i].active && !Configuration.Instance.NonSweepableItemIDs.Contains(Main.item[i].type))
            {
                var category = GetSweepCategory(Main.item[i]);
                if (category is not null && CanSweepCategory(category.Value))
                {
                    items.Enqueue(SweepItem.FromItem(i, Main.item[i], category.Value));
                }
            }
        }

        this.StartSweep(items, SweepSource.Smart, null);
    }

    private void QueueManualItemSweep(TSPlayer requester)
    {
        var items = new Queue<SweepItem>();
        for (var i = 0; i < Main.item.Length; i++)
        {
            if (Main.item[i].active)
            {
                items.Enqueue(SweepItem.FromItem(i, Main.item[i], SweepCategory.None));
            }
        }

        this.StartSweep(items, SweepSource.Manual, requester);
    }

    private void StartSweep(Queue<SweepItem> items, SweepSource source, TSPlayer? requester)
    {
        if (items.Count == 0)
        {
            if (source == SweepSource.Manual)
            {
                requester!.SendSuccessMessage("没有可清理的掉落物。");
            }

            return;
        }

        this._sweepJob = new SweepJob(items, source, requester);
        if (source == SweepSource.Manual)
        {
            requester!.SendInfoMessage($"已加入 {items.Count} 个掉落物，正在分批清理。");
        }
    }

    private void ProcessSweepJob()
    {
        var sweepJob = this._sweepJob;
        if (sweepJob is null || this._updateCounter % ClearBatchIntervalTicks != 0)
        {
            return;
        }

        for (var processed = 0; processed < ClearBatchSize && sweepJob.Items.TryDequeue(out var pendingItem); processed++)
        {
            var item = Main.item[pendingItem.Index];
            // 如果物品已经被清理或者类型或堆叠数量不匹配，则跳过
            if (!item.active || item.type != pendingItem.Type || item.stack != pendingItem.Stack)
            {
                continue;
            }

            item.TurnToAir();
            TSPlayer.All.SendData(PacketTypes.ItemDrop, null, pendingItem.Index);
            sweepJob.AddClearedItem(pendingItem.Category);
        }

        if (sweepJob.Items.Count != 0)
        {
            return;
        }

        this._sweepJob = null;
        this.CompleteSweep(sweepJob);
    }

    private void CompleteSweep(SweepJob sweepJob)
    {   // 如果是手动清理，则只给请求者发送消息
        if (sweepJob.Source == SweepSource.Manual)
        {
            sweepJob.Requester!.SendSuccessMessage($"已分批清理 {sweepJob.TotalItems} 个掉落物。");
            return;
        }
        // 如果是智能清理，则给所有玩家发送消息
        if (sweepJob.TotalItems > 0)
        {
            if (!string.IsNullOrEmpty(Configuration.Instance.CustomMessage))
            {
                TSPlayer.All.SendSuccessMessage($"{Configuration.Instance.CustomMessage}");
            }

            if (Configuration.Instance.SpecificMessage)
            {
                TSPlayer.All.SendSuccessMessage(GetString($"智能扫地机已清扫：[c/FFFFFF:{sweepJob.TotalItems}]种物品"));
                TSPlayer.All.SendSuccessMessage(GetString($"包含：【投掷武器[c/FFFFFF:{sweepJob.TotalThrowable}]】-【挥动武器[c/FFFFFF:{sweepJob.TotalSwinging}]】-【普通物品[c/FFFFFF:{sweepJob.TotalRegular}]】-【装备[c/FFFFFF:{sweepJob.TotalEquipment}]】-【时装[c/FFFFFF:{sweepJob.TotalVanity}]】"));
            }
        }
    }

    private static SweepCategory? GetSweepCategory(WorldItem item)
    {
        if (item.damage > 0)
        {
            return item.maxStack > 1 ? SweepCategory.Throwable : item.maxStack == 1 ? SweepCategory.Swinging : null;
        }

        if (item.damage < 0)
        {
            return item.maxStack > 1 ? SweepCategory.Regular : item.maxStack == 1 ? SweepCategory.Vanity : null;
        }

        return item.maxStack == 1 ? SweepCategory.Equipment : null;
    }

    private static bool CanSweepCategory(SweepCategory category)
    {
        return category switch
        {
            SweepCategory.Throwable => Configuration.Instance.SweepThrowable,
            SweepCategory.Swinging => Configuration.Instance.SweepSwinging,
            SweepCategory.Regular => Configuration.Instance.SweepRegular,
            SweepCategory.Equipment => Configuration.Instance.SweepEquipment,
            SweepCategory.Vanity => Configuration.Instance.SweepVanity,
            _ => false
        };
    }

    private enum SweepSource
    {
        Smart,
        Manual
    }

    private enum SweepCategory
    {
        None,
        Throwable,
        Swinging,
        Regular,
        Equipment,
        Vanity
    }

    private readonly record struct SweepItem(int Index, int Type, int Stack, SweepCategory Category)
    {
        public static SweepItem FromItem(int index, WorldItem item, SweepCategory category)
        {
            return new SweepItem(index, item.type, item.stack, category);
        }
    }

    private sealed class SweepJob(Queue<SweepItem> items, SweepSource source, TSPlayer? requester)
    {
        public Queue<SweepItem> Items { get; } = items;
        public SweepSource Source { get; } = source;
        public TSPlayer? Requester { get; } = requester;
        public int TotalItems { get; private set; }
        public int TotalThrowable { get; private set; }
        public int TotalSwinging { get; private set; }
        public int TotalRegular { get; private set; }
        public int TotalEquipment { get; private set; }
        public int TotalVanity { get; private set; }

        public void AddClearedItem(SweepCategory category)
        {
            this.TotalItems++;
            switch (category)
            {
                case SweepCategory.Throwable:
                    this.TotalThrowable++;
                    break;
                case SweepCategory.Swinging:
                    this.TotalSwinging++;
                    break;
                case SweepCategory.Regular:
                    this.TotalRegular++;
                    break;
                case SweepCategory.Equipment:
                    this.TotalEquipment++;
                    break;
                case SweepCategory.Vanity:
                    this.TotalVanity++;
                    break;
            }
        }
    }
}
