using System.Diagnostics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace BossLock;

internal static class SpawnGuard
{
    private static readonly Dictionary<int, long> LastNotice = new();
    private static readonly object NoticeGate = new();

    internal static void Register(TerrariaPlugin plugin)
    {
        ServerApi.Hooks.NpcSpawn.Register(plugin, OnSpawn);
        On.Terraria.NPC.SpawnSkeletron += SpawnSkeletron;
        On.Terraria.NPC.SpawnWOF += SpawnWof;
        On.Terraria.NPC.SpawnBoss += SpawnBoss;
        On.Terraria.NPC.SpawnMechQueen += SpawnMechQueen;
    }

    internal static void Deregister(TerrariaPlugin plugin)
    {
        ServerApi.Hooks.NpcSpawn.Deregister(plugin, OnSpawn);
        On.Terraria.NPC.SpawnSkeletron -= SpawnSkeletron;
        On.Terraria.NPC.SpawnWOF -= SpawnWof;
        On.Terraria.NPC.SpawnBoss -= SpawnBoss;
        On.Terraria.NPC.SpawnMechQueen -= SpawnMechQueen;
        lock (NoticeGate) { LastNotice.Clear(); }
    }

    private static bool IsBlocked(int id, out string? deadline)
    {
        var root = BossGroup.Root(id);
        deadline = null;
        if (!Config.Settings.Locks.Any(entry => BossGroup.Root(entry.NpcId) == root))
        {
            return false;
        }

        deadline = Database.LoadBoss(root);
        // 有配置但缺少时间时不默默放行，也不擅自重置已有进度。
        return deadline == null || !DateTime.TryParseExact(deadline, "yyyy-MM-dd-HH:mm:ss",
            null, System.Globalization.DateTimeStyles.None, out var time) || time > DateTime.Now;
    }

    private static void Notify(int root, string? deadline)
    {
        lock (NoticeGate)
        {
            var now = Stopwatch.GetTimestamp();
            if (LastNotice.TryGetValue(root, out var previous) &&
                Stopwatch.GetElapsedTime(previous, now) < TimeSpan.FromSeconds(2))
            {
                return;
            }
            LastNotice[root] = now;
        }

        var name = Lang.GetNPCNameValue(root);
        var detail = DateTime.TryParseExact(deadline, "yyyy-MM-dd-HH:mm:ss", null,
            System.Globalization.DateTimeStyles.None, out _)
            ? GetString($"解锁时间: {Utils.TimeFormat(deadline!)}")
            : GetString("缺少有效的进度时间记录，请管理员使用 /bl edit 设置该 Boss 的解锁时间。");
        TSPlayer.All.SendErrorMessage(GetString($"[BossLock] {name} 被禁止生成。") + detail);
    }

    private static bool Block(int id)
    {
        if (!IsBlocked(id, out var deadline)) { return false; }
        Notify(BossGroup.Root(id), deadline);
        return true;
    }

    private static void OnSpawn(NpcSpawnEventArgs args)
    {
        var npc = Main.npc[args.NpcId];
        if (!npc.active || !IsBlocked(npc.netID, out var deadline)) { return; }
        var root = BossGroup.Root(npc.netID);
        // 不触发死亡、掉落或击败进度；一并移除已生成的同组主体/部件。
        foreach (var part in Main.npc.Take(Main.maxNPCs))
        {
            if (part.active && BossGroup.Root(part.netID) == root)
            {
                part.active = false;
                TSPlayer.All.SendData(PacketTypes.NpcUpdate, null, part.whoAmI);
            }
        }
        args.Handled = true;
        Notify(root, deadline);
    }

    private static void SpawnSkeletron(On.Terraria.NPC.orig_SpawnSkeletron orig, int player, bool redHat)
    {
        if (!Block(NPCID.SkeletronHead)) { orig(player, redHat); }
    }

    private static void SpawnWof(On.Terraria.NPC.orig_SpawnWOF orig, Vector2 position)
    {
        if (!Block(NPCID.WallofFlesh)) { orig(position); }
    }

    private static void SpawnBoss(On.Terraria.NPC.orig_SpawnBoss orig, int x, int y, int type,
        int player, float ai0, float ai1, float ai2, float ai3)
    {
        if (!Block(type)) { orig(x, y, type, player, ai0, ai1, ai2, ai3); }
    }

    private static bool SpawnMechQueen(On.Terraria.NPC.orig_SpawnMechQueen orig, int player)
    {
        // 在原版查找机械骷髅王索引前检查整场组合召唤，避免删掉主体后访问 -1。
        foreach (var root in new[] { NPCID.SkeletronPrime, NPCID.Retinazer, NPCID.TheDestroyer })
        {
            if (Block(root)) { return false; }
        }
        return orig(player);
    }
}
