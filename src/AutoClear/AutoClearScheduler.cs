using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Events;
using TShockAPI;

namespace AutoClear
{
    internal sealed class AutoClearScheduler
    {
        private AutoClearConfiguration configuration;
        private readonly Func<bool> externalCleanupIsRunning;
        private bool sweepScheduled;
        private DateTime sweepScheduledAtUtc;
        private DateTime nextDetectionAtUtc;

        internal AutoClearScheduler(
            AutoClearConfiguration configuration,
            Func<bool> externalCleanupIsRunning)
        {
            this.externalCleanupIsRunning = externalCleanupIsRunning;
            Reload(configuration);
        }

        internal void Reload(AutoClearConfiguration newConfiguration)
        {
            configuration = newConfiguration ?? new AutoClearConfiguration();
            configuration.Normalize();
            sweepScheduled = false;
            nextDetectionAtUtc = DateTime.MinValue;
        }

        internal void Update()
        {
            DateTime now = DateTime.UtcNow;
            if (!configuration.EnableAutomaticSweep)
            {
                sweepScheduled = false;
                nextDetectionAtUtc = now.AddSeconds(configuration.DetectionIntervalSeconds);
                return;
            }

            if (sweepScheduled)
            {
                if (now >= sweepScheduledAtUtc)
                {
                    TryExecuteScheduledSweep(now);
                }
                return;
            }

            if (now < nextDetectionAtUtc)
            {
                return;
            }

            nextDetectionAtUtc = now.AddSeconds(configuration.DetectionIntervalSeconds);
            List<AutoClearCandidate> candidates = CaptureCandidates(configuration);
            if (candidates.Count < configuration.SmartSweepThreshold)
            {
                return;
            }

            sweepScheduled = true;
            sweepScheduledAtUtc = now.AddSeconds(configuration.DelayedSweepTimeoutSeconds);
            if (!string.IsNullOrWhiteSpace(configuration.DelayedSweepCustomMessage))
            {
                string message = configuration.DelayedSweepCustomMessage.Replace(
                    "{0}",
                    configuration.DelayedSweepTimeoutSeconds.ToString());
                TSPlayer.All.SendSuccessMessage(message);
            }

            if (configuration.DelayedSweepTimeoutSeconds == 0)
            {
                TryExecuteScheduledSweep(now);
            }
        }

        private void TryExecuteScheduledSweep(DateTime now)
        {
            if (AutoClearService.IsRunning || externalCleanupIsRunning?.Invoke() == true)
            {
                sweepScheduledAtUtc = now.AddSeconds(1);
                return;
            }

            List<AutoClearCandidate> candidates = CaptureCandidates(configuration);
            if (candidates.Count < configuration.SmartSweepThreshold)
            {
                ResetSchedule(now);
                return;
            }

            if (!CanSweep(out string blockedReason))
            {
                TSPlayer.All.SendWarningMessage(
                    $"智能扫地机：由于{blockedReason}，已跳过本次自动清理。");
                ResetSchedule(now);
                return;
            }

            AutoClearConfiguration completionConfiguration = configuration;
            AutoClearStartResult result = AutoClearService.TryStartAutomatic(
                candidates,
                summary => SendCompletionMessages(completionConfiguration, summary),
                out int queuedItems);

            switch (result)
            {
                case AutoClearStartResult.Started:
#if DEBUG
                    TShock.Log.ConsoleDebug(
                        $"[AutoClear] Automatic cleanup queued items={queuedItems}");
#endif
                    ResetSchedule(now);
                    return;

                case AutoClearStartResult.NoMatchingItems:
                    ResetSchedule(now);
                    return;

                case AutoClearStartResult.Busy:
                    sweepScheduledAtUtc = now.AddSeconds(1);
                    return;

                default:
                    TShock.Log.ConsoleError(
                        $"[AutoClear] Unable to start automatic cleanup: {result}");
                    ResetSchedule(now);
                    return;
            }
        }

        private void ResetSchedule(DateTime now)
        {
            sweepScheduled = false;
            nextDetectionAtUtc = now.AddSeconds(configuration.DetectionIntervalSeconds);
        }

        private static List<AutoClearCandidate> CaptureCandidates(
            AutoClearConfiguration configuration)
        {
            int slotCount = Math.Min(AutoClearCommandRules.WorldItemSlotCount, Main.item.Length);
            List<AutoClearCandidate> candidates = new List<AutoClearCandidate>();
            for (int slot = 0; slot < slotCount; slot++)
            {
                WorldItem item = Main.item[slot];
                if (item == null
                    || !item.active
                    || configuration.ExcludedItemIdSet.Contains(item.type))
                {
                    continue;
                }

                AutoClearItemCategory category = AutoClearItemRules.Classify(item.damage, item.maxStack);
                if (AutoClearItemRules.IsEnabled(category, configuration))
                {
                    candidates.Add(new AutoClearCandidate(slot, category));
                }
            }

            return candidates;
        }

        private static bool CanSweep(out string blockedReason)
        {
            if (Main.invasionType != 0 && Main.invasionSize != 0)
            {
                blockedReason = "入侵事件正在进行";
                return false;
            }

            if (Main.bloodMoon)
            {
                blockedReason = "血月正在进行";
                return false;
            }

            if (DD2Event.Ongoing)
            {
                blockedReason = "旧日军团正在进行";
                return false;
            }

            if (Main.eclipse)
            {
                blockedReason = "日食正在进行";
                return false;
            }

            if (Main.pumpkinMoon || Main.snowMoon)
            {
                blockedReason = "月亮事件正在进行";
                return false;
            }

            if (Main.npc.Any(npc => npc != null && npc.active && npc.boss))
            {
                blockedReason = "Boss 战正在进行";
                return false;
            }

            blockedReason = string.Empty;
            return true;
        }

        private static void SendCompletionMessages(
            AutoClearConfiguration configuration,
            AutoClearSummary summary)
        {
            if (summary.DeletedItems <= 0)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(configuration.CustomMessage))
            {
                TSPlayer.All.SendSuccessMessage(configuration.CustomMessage);
            }

            if (!configuration.SpecificMessage)
            {
                return;
            }

            TSPlayer.All.SendSuccessMessage(
                $"智能扫地机已安全清扫：[c/FFFFFF:{summary.DeletedItems}] 个物品");
            TSPlayer.All.SendSuccessMessage(
                $"包含：【投掷武器[c/FFFFFF:{summary.ThrowableItems}]】-" +
                $"【挥动武器[c/FFFFFF:{summary.SwingingItems}]】-" +
                $"【普通物品[c/FFFFFF:{summary.RegularItems}]】-" +
                $"【装备[c/FFFFFF:{summary.EquipmentItems}]】-" +
                $"【时装[c/FFFFFF:{summary.VanityItems}]】");

            if (summary.SkippedItems > 0)
            {
                TSPlayer.All.SendInfoMessage(
                    $"另有 {summary.SkippedItems} 个物品因被拾取、合并或槽位变化而跳过。");
            }
        }
    }
}
