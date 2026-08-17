namespace RandomFishingLoot;

public sealed partial class RandomFishingLoot
{
    private sealed record PendingLootChoice(
        int ItemId,
        string DisplayName,
        int Weight,
        int MinStack,
        int MaxStack,
        int ItemMaxStack,
        string StageId,
        string StageName);

    private sealed record PendingNpcChoice(
        int NpcId,
        string DisplayName);

    private readonly record struct ProjectileKey(byte Owner, short Identity);

    private sealed class PendingNetworkCatch(PendingLootChoice choice, int stack)
    {
        public PendingLootChoice Choice { get; } = choice;
        public int Stack { get; } = stack;
        public bool BonusGranted { get; set; }
    }

    private sealed class LootPool
    {
        public static readonly LootPool Empty = new(Array.Empty<PendingLootChoice>(), 0);

        private readonly PendingLootChoice[] _items;
        private readonly int _totalWeight;

        private LootPool(PendingLootChoice[] items, int totalWeight)
        {
            _items = items;
            _totalWeight = totalWeight;
        }

        public int Count => _items.Length;

        public PendingLootChoice? Next()
        {
            if (_items.Length == 0 || _totalWeight <= 0)
                return null;

            int roll = Random.Shared.Next(_totalWeight);
            int sum = 0;
            foreach (PendingLootChoice item in _items)
            {
                sum += item.Weight;
                if (roll < sum)
                    return item;
            }

            return _items[^1];
        }

        public static LootPool Create(IEnumerable<PendingLootChoice> items)
        {
            PendingLootChoice[] values = items.ToArray();
            if (values.Length == 0)
                return Empty;

            int totalWeight = 0;
            foreach (PendingLootChoice item in values)
                totalWeight += Math.Max(1, item.Weight);

            return new LootPool(values, totalWeight);
        }
    }

    private sealed class NpcPool
    {
        public static readonly NpcPool Empty = new(Array.Empty<PendingNpcChoice>());

        private readonly PendingNpcChoice[] _items;

        private NpcPool(PendingNpcChoice[] items)
        {
            _items = items;
        }

        public int Count => _items.Length;

        public PendingNpcChoice? Next()
        {
            if (_items.Length == 0)
                return null;

            return _items[Random.Shared.Next(_items.Length)];
        }

        public static NpcPool Create(IEnumerable<PendingNpcChoice> items)
        {
            PendingNpcChoice[] values = items.ToArray();
            return values.Length == 0 ? Empty : new NpcPool(values);
        }
    }
}
