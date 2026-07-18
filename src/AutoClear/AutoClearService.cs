using System;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using TShockAPI;

namespace AutoClear
{
    public enum AutoClearStartResult
    {
        Started,
        NoMatchingItems,
        Busy,
        InvalidRequest,
        MainThreadRequired,
    }

    internal readonly struct AutoClearCandidate
    {
        internal AutoClearCandidate(int slot, AutoClearItemCategory category)
        {
            Slot = slot;
            Category = category;
        }

        internal int Slot { get; }
        internal AutoClearItemCategory Category { get; }
    }

    internal sealed class AutoClearSummary
    {
        internal AutoClearSummary(
            int queuedItems,
            int deletedItems,
            int skippedItems,
            int throwableItems,
            int swingingItems,
            int regularItems,
            int equipmentItems,
            int vanityItems)
        {
            QueuedItems = queuedItems;
            DeletedItems = deletedItems;
            SkippedItems = skippedItems;
            ThrowableItems = throwableItems;
            SwingingItems = swingingItems;
            RegularItems = regularItems;
            EquipmentItems = equipmentItems;
            VanityItems = vanityItems;
        }

        internal int QueuedItems { get; }
        internal int DeletedItems { get; }
        internal int SkippedItems { get; }
        internal int ThrowableItems { get; }
        internal int SwingingItems { get; }
        internal int RegularItems { get; }
        internal int EquipmentItems { get; }
        internal int VanityItems { get; }
    }

    public static class AutoClearService
    {
        private static CleanupJob activeJob;
        private static int mainThreadId = -1;

        public static bool IsRunning => activeJob != null;
        public static int RemainingItems => activeJob?.Pending.Count ?? 0;

        /// <summary>
        /// Starts a paced world-item cleanup. Call this method from Terraria's main thread.
        /// </summary>
        public static AutoClearStartResult TryStart(
            TSPlayer initiator,
            int radiusTiles,
            bool silent,
            out int queuedItems)
        {
            queuedItems = 0;
            if (initiator == null || radiusTiles <= 0 || Main.item == null)
            {
                return AutoClearStartResult.InvalidRequest;
            }

            AutoClearStartResult stateResult = ValidateStartState();
            if (stateResult != AutoClearStartResult.Started)
            {
                return stateResult;
            }

            Queue<WorldItemSnapshot> pending = CaptureMatchingItems(initiator.X, initiator.Y, radiusTiles);
            queuedItems = pending.Count;
            if (pending.Count == 0)
            {
                SendCompletionMessage(initiator, initiator.Name, silent, radiusTiles, 0, 0);
                return AutoClearStartResult.NoMatchingItems;
            }

            activeJob = new CleanupJob(
                initiator,
                initiator.Name,
                silent,
                radiusTiles,
                pending,
                sendDefaultCompletion: true,
                completion: null);

            if (CanReceiveMessage(initiator))
            {
                initiator.SendInfoMessage(
                    $"已将 {queuedItems} 个世界物品加入安全清理队列，每 tick 最多处理 {AutoClearCommandRules.ItemsPerTick} 个。");
            }

            return AutoClearStartResult.Started;
        }

        internal static AutoClearStartResult TryStartAutomatic(
            IReadOnlyList<AutoClearCandidate> candidates,
            Action<AutoClearSummary> completion,
            out int queuedItems)
        {
            queuedItems = 0;
            if (candidates == null || Main.item == null)
            {
                return AutoClearStartResult.InvalidRequest;
            }

            AutoClearStartResult stateResult = ValidateStartState();
            if (stateResult != AutoClearStartResult.Started)
            {
                return stateResult;
            }

            Queue<WorldItemSnapshot> pending = CaptureCandidates(candidates);
            queuedItems = pending.Count;
            if (pending.Count == 0)
            {
                InvokeCompletion(completion, CreateEmptySummary());
                return AutoClearStartResult.NoMatchingItems;
            }

            activeJob = new CleanupJob(
                initiator: null,
                initiatorName: "AutoClear",
                silent: true,
                radiusTiles: 0,
                pending,
                sendDefaultCompletion: false,
                completion);
            return AutoClearStartResult.Started;
        }

        internal static void Update()
        {
            Interlocked.CompareExchange(
                ref mainThreadId,
                Thread.CurrentThread.ManagedThreadId,
                comparand: -1);

            CleanupJob job = activeJob;
            if (job == null)
            {
                return;
            }

            int batchSize = AutoClearCommandRules.GetBatchSize(job.Pending.Count);
            for (int i = 0; i < batchSize; i++)
            {
                WorldItemSnapshot snapshot = job.Pending.Dequeue();
                if (!snapshot.TryResolve(out WorldItem item))
                {
                    job.SkippedItems++;
                    continue;
                }

                item.TurnToAir(false);
                TSPlayer.All.SendData(PacketTypes.SyncItemDespawn, "", snapshot.Slot);
                job.RecordDeleted(snapshot.Category);
            }

            if (job.Pending.Count != 0)
            {
                return;
            }

            activeJob = null;
            AutoClearSummary summary = job.CreateSummary();
            if (job.SendDefaultCompletion)
            {
                SendCompletionMessage(
                    job.Initiator,
                    job.InitiatorName,
                    job.Silent,
                    job.RadiusTiles,
                    summary.DeletedItems,
                    summary.SkippedItems);
            }
            else
            {
#if DEBUG
                TShock.Log.ConsoleDebug(
                    $"[AutoClear] Automatic cleanup completed deleted={summary.DeletedItems} skipped={summary.SkippedItems}");
#endif
            }

            InvokeCompletion(job.Completion, summary);
        }

        internal static void Reset()
        {
            if (activeJob != null)
            {
                TShock.Log.ConsoleInfo(
                    $"[AutoClear] Cancelled pending cleanup during plugin unload. remaining={activeJob.Pending.Count}");
            }

            activeJob = null;
            Volatile.Write(ref mainThreadId, -1);
        }

        private static AutoClearStartResult ValidateStartState()
        {
            if (Thread.CurrentThread.ManagedThreadId != Volatile.Read(ref mainThreadId))
            {
                return AutoClearStartResult.MainThreadRequired;
            }

            return activeJob == null
                ? AutoClearStartResult.Started
                : AutoClearStartResult.Busy;
        }

        private static Queue<WorldItemSnapshot> CaptureMatchingItems(
            float centerX,
            float centerY,
            int radiusTiles)
        {
            int slotCount = Math.Min(AutoClearCommandRules.WorldItemSlotCount, Main.item.Length);
            Queue<WorldItemSnapshot> pending = new Queue<WorldItemSnapshot>(slotCount);
            for (int slot = 0; slot < slotCount; slot++)
            {
                WorldItem item = Main.item[slot];
                if (item == null
                    || !item.active
                    || !AutoClearCommandRules.IsWithinRadius(
                        item.position.X,
                        item.position.Y,
                        centerX,
                        centerY,
                        radiusTiles))
                {
                    continue;
                }

                pending.Enqueue(new WorldItemSnapshot(slot, item, AutoClearItemCategory.None));
            }

            return pending;
        }

        private static Queue<WorldItemSnapshot> CaptureCandidates(
            IReadOnlyList<AutoClearCandidate> candidates)
        {
            Queue<WorldItemSnapshot> pending = new Queue<WorldItemSnapshot>(candidates.Count);
            HashSet<int> capturedSlots = new HashSet<int>();
            foreach (AutoClearCandidate candidate in candidates)
            {
                if ((uint)candidate.Slot >= (uint)Main.item.Length
                    || candidate.Category == AutoClearItemCategory.None
                    || !capturedSlots.Add(candidate.Slot))
                {
                    continue;
                }

                WorldItem item = Main.item[candidate.Slot];
                if (item != null && item.active)
                {
                    pending.Enqueue(new WorldItemSnapshot(candidate.Slot, item, candidate.Category));
                }
            }

            return pending;
        }

        private static void SendCompletionMessage(
            TSPlayer initiator,
            string initiatorName,
            bool silent,
            int radiusTiles,
            int deletedItems,
            int skippedItems)
        {
            string skippedSuffix = skippedItems > 0
                ? $"，另有 {skippedItems} 个因槽位状态变化而跳过"
                : string.Empty;

            if (silent)
            {
                if (CanReceiveMessage(initiator))
                {
                    initiator.SendSuccessMessage(
                        $"已在半径 {radiusTiles} 格内安全清理 {deletedItems} 个物品{skippedSuffix}。");
                }
            }
            else
            {
                TSPlayer.All.SendInfoMessage(
                    $"{initiatorName} 在半径 {radiusTiles} 格内安全清理了 {deletedItems} 个物品{skippedSuffix}。");
            }

            TShock.Log.ConsoleInfo(
                $"[AutoClear] Completed requester={initiatorName} radius={radiusTiles} deleted={deletedItems} skipped={skippedItems}");
        }

        private static void InvokeCompletion(
            Action<AutoClearSummary> completion,
            AutoClearSummary summary)
        {
            if (completion == null)
            {
                return;
            }

            try
            {
                completion(summary);
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[AutoClear] Cleanup completion callback failed: {ex}");
            }
        }

        private static AutoClearSummary CreateEmptySummary()
        {
            return new AutoClearSummary(0, 0, 0, 0, 0, 0, 0, 0);
        }

        private static bool CanReceiveMessage(TSPlayer player)
        {
            return player != null && (!player.RealPlayer || player.Active);
        }

        private sealed class CleanupJob
        {
            private readonly int[] deletedCategoryCounts = new int[6];

            internal CleanupJob(
                TSPlayer initiator,
                string initiatorName,
                bool silent,
                int radiusTiles,
                Queue<WorldItemSnapshot> pending,
                bool sendDefaultCompletion,
                Action<AutoClearSummary> completion)
            {
                Initiator = initiator;
                InitiatorName = initiatorName;
                Silent = silent;
                RadiusTiles = radiusTiles;
                Pending = pending;
                InitialItems = pending.Count;
                SendDefaultCompletion = sendDefaultCompletion;
                Completion = completion;
            }

            internal TSPlayer Initiator { get; }
            internal string InitiatorName { get; }
            internal bool Silent { get; }
            internal int RadiusTiles { get; }
            internal Queue<WorldItemSnapshot> Pending { get; }
            internal int InitialItems { get; }
            internal int DeletedItems { get; private set; }
            internal int SkippedItems { get; set; }
            internal bool SendDefaultCompletion { get; }
            internal Action<AutoClearSummary> Completion { get; }

            internal void RecordDeleted(AutoClearItemCategory category)
            {
                DeletedItems++;
                int categoryIndex = (int)category;
                if ((uint)categoryIndex < (uint)deletedCategoryCounts.Length)
                {
                    deletedCategoryCounts[categoryIndex]++;
                }
            }

            internal AutoClearSummary CreateSummary()
            {
                return new AutoClearSummary(
                    InitialItems,
                    DeletedItems,
                    SkippedItems,
                    deletedCategoryCounts[(int)AutoClearItemCategory.Throwable],
                    deletedCategoryCounts[(int)AutoClearItemCategory.Swinging],
                    deletedCategoryCounts[(int)AutoClearItemCategory.Regular],
                    deletedCategoryCounts[(int)AutoClearItemCategory.Equipment],
                    deletedCategoryCounts[(int)AutoClearItemCategory.Vanity]);
            }
        }

        private readonly struct WorldItemSnapshot
        {
            private readonly WorldItem itemReference;
            private readonly int itemType;
            private readonly int stack;
            private readonly byte prefix;
            private readonly int reservedForPlayer;
            private readonly int timeSinceItemSpawned;

            internal WorldItemSnapshot(
                int slot,
                WorldItem item,
                AutoClearItemCategory category)
            {
                Slot = slot;
                Category = category;
                itemReference = item;
                itemType = item.type;
                stack = item.stack;
                prefix = item.inner.prefix;
                reservedForPlayer = item.playerIndexTheItemIsReservedFor;
                timeSinceItemSpawned = item.timeSinceItemSpawned;
            }

            internal int Slot { get; }
            internal AutoClearItemCategory Category { get; }

            internal bool TryResolve(out WorldItem item)
            {
                item = null;
                if ((uint)Slot >= (uint)Main.item.Length)
                {
                    return false;
                }

                WorldItem current = Main.item[Slot];
                if (current == null
                    || !current.active
                    || !ReferenceEquals(current, itemReference)
                    || current.type != itemType
                    || current.stack != stack
                    || current.inner.prefix != prefix
                    || current.playerIndexTheItemIsReservedFor != reservedForPlayer
                    || current.timeSinceItemSpawned < timeSinceItemSpawned)
                {
                    return false;
                }

                item = current;
                return true;
            }
        }
    }
}
