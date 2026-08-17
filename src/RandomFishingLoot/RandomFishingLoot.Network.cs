using System.Text;

using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace RandomFishingLoot;

public sealed partial class RandomFishingLoot
{
    private void OnNewProjectilePacket(object? sender, GetDataHandlers.NewProjectileEventArgs args)
    {
        if (args.Handled
            || !_config.Enabled
            || !UseProgressionItemMode()
            || args.Player?.Active != true
            || args.Owner != args.Player.Index
            || !IsFishingBobber(args.Type))
        {
            return;
        }

        ProjectileKey key = new(args.Owner, args.Identity);

        if (args.Ai[0] == 3f)
        {
            _networkCatches.Remove(key);
            return;
        }

        if (args.Ai[0] == 0f)
        {
            EnsureNetworkCatch(key, args.Owner);
            return;
        }

        if (args.Ai[0] != 1f || args.Ai[1] <= 0f)
            return;

        // 掉落判定发生在客户端，客户端上报的这一杆若是宝匣，直接放行不改写。
        // 这样木匣/铁匣/金匣及各生物群系宝匣不再被自定义渔获表吞掉。
        if (_config.AllowCrates && IsCrateItem((int)args.Ai[1]))
        {
            _networkCatches.Remove(key);
            return;
        }

        if (!_networkCatches.TryGetValue(key, out PendingNetworkCatch? pending))
            return;

        if (!TryRewriteAi1(args.Data, pending.Choice.ItemId))
            return;

        args.Ai[1] = pending.Choice.ItemId;
        SyncCatchToOwner(args, pending.Choice.ItemId);
        if (pending.BonusGranted)
            return;

        pending.BonusGranted = true;
        int bonusStack = pending.Stack - 1;
        if (bonusStack > 0)
            args.Player.GiveItem(pending.Choice.ItemId, bonusStack, 0);

        if (_config.AnnounceToPlayer)
        {
            args.Player.SendInfoMessage(
                $"本次渔获：{pending.Choice.DisplayName} x{pending.Stack} [{pending.Choice.StageName}]");
        }
    }

    private void EnsureNetworkCatch(ProjectileKey key, int ownerIndex)
    {
        if (_networkCatches.ContainsKey(key))
            return;

        // 掉落判定发生在客户端，服务端只能通过网络包改写生效。
        // 这里复刻原版任务鱼判定：玩家处于任务中、未持有该鱼、渔夫存在且任务未完成时，
        // 按原版 "uncommon" 层级概率让这一杆钓到任务鱼。
        PendingLootChoice? choice;
        if (_config.AllowQuestFish && TryGetQuestFish(ownerIndex, out int questFish))
        {
            int fishingLevel = GetFinalFishingLevel(ownerIndex);
            choice = RollVanillaQuestFishChance(fishingLevel)
                ? MakeQuestFishChoice(questFish)
                : RollCurrentLootChoice();
        }
        else
        {
            choice = RollCurrentLootChoice();
        }

        if (choice == null)
            return;

        int stack = RollStack(choice.MinStack, choice.MaxStack, choice.ItemMaxStack);
        _networkCatches[key] = new PendingNetworkCatch(choice, stack);
    }

    private void OnProjectileKillPacket(object? sender, GetDataHandlers.ProjectileKillEventArgs args)
    {
        _networkCatches.Remove(new ProjectileKey(args.ProjectileOwner, (short)args.ProjectileIdentity));
    }

    private static void SyncCatchToOwner(GetDataHandlers.NewProjectileEventArgs args, int itemId)
    {
        if (args.Index < 0 || args.Index >= Main.maxProjectiles)
            return;

        Terraria.Projectile bobber = Main.projectile[args.Index];
        if (!bobber.active || bobber.owner != args.Owner || !bobber.bobber)
            return;

        bobber.position = args.Position;
        bobber.velocity = args.Velocity;
        bobber.ai[0] = 1f;
        bobber.ai[1] = itemId;
        if (bobber.ai.Length > 2 && args.Ai.Length > 2)
            bobber.ai[2] = args.Ai[2];

        args.Player.SendData(PacketTypes.ProjectileNew, "", args.Index);
    }

    private void OnServerLeave(LeaveEventArgs args)
    {
        byte owner = (byte)args.Who;
        foreach (ProjectileKey key in _networkCatches.Keys.Where(key => key.Owner == owner).ToArray())
            _networkCatches.Remove(key);
    }

    private static bool TryRewriteAi1(MemoryStream packet, float itemId)
    {
        if (!packet.CanRead || !packet.CanWrite)
            return false;

        long originalPosition = packet.Position;
        try
        {
            packet.Position = 0;
            using BinaryReader reader = new(packet, Encoding.UTF8, leaveOpen: true);

            const int fixedHeaderLength = sizeof(short) + sizeof(float) * 4 + sizeof(byte) + sizeof(short);
            if (packet.Length < fixedHeaderLength + sizeof(byte))
                return false;

            packet.Position = fixedHeaderLength;
            byte flags = reader.ReadByte();
            if ((flags & 0b0000_0100) != 0)
                reader.ReadByte();

            if ((flags & 0b0000_0001) != 0)
                reader.ReadSingle();

            if ((flags & 0b0000_0010) == 0 || packet.Position + sizeof(float) > packet.Length)
                return false;

            long ai1Offset = packet.Position;
            using BinaryWriter writer = new(packet, Encoding.UTF8, leaveOpen: true);
            packet.Position = ai1Offset;
            writer.Write(itemId);
            writer.Flush();
            return true;
        }
        finally
        {
            packet.Position = originalPosition;
        }
    }

    private static bool IsFishingBobber(int projectileType)
    {
        return projectileType is >= 360 and <= 366
            or 381
            or 382
            or 760
            or 775
            or >= 986 and <= 993;
    }
}
