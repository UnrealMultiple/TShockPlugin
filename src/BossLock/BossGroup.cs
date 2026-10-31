using Terraria.ID;

namespace BossLock;

internal static class BossGroup
{
    // 主体、肢体和专属附属单位共用同一进度；保留旧配置/数据库中的部件 ID 兼容性。
    private static readonly int[][] Groups =
    [
        [NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail],
        [NPCID.SkeletronHead, NPCID.SkeletronHand],
        [NPCID.WallofFlesh, NPCID.WallofFleshEye, NPCID.TheHungry, NPCID.TheHungryII],
        [NPCID.Retinazer, NPCID.Spazmatism],
        [NPCID.SkeletronPrime, NPCID.PrimeCannon, NPCID.PrimeSaw, NPCID.PrimeVice, NPCID.PrimeLaser],
        [NPCID.TheDestroyer, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail, NPCID.Probe],
        [NPCID.Plantera, NPCID.PlanterasHook, NPCID.PlanterasTentacle],
        [NPCID.BrainofCthulhu, NPCID.Creeper],
        [NPCID.Golem, NPCID.GolemHead, NPCID.GolemFistLeft, NPCID.GolemFistRight, NPCID.GolemHeadFree],
        [NPCID.Pumpking, NPCID.PumpkingBlade],
        [NPCID.MartianSaucerCore, NPCID.MartianSaucer, NPCID.MartianSaucerTurret, NPCID.MartianSaucerCannon],
        [NPCID.MoonLordCore, NPCID.MoonLordHead, NPCID.MoonLordHand, NPCID.MoonLordFreeEye],
        [NPCID.CultistBoss, NPCID.CultistBossClone],
        [NPCID.PirateShip, NPCID.PirateShipCannon]
    ];

    internal static int Root(int id) => Groups.FirstOrDefault(group => group.Contains(id))?[0] ?? id;

    internal static int[] Members(int id) =>
        Groups.FirstOrDefault(group => group.Contains(id)) ?? [id];

    internal static IEnumerable<Lock> Configured() => Config.Settings.Locks
        .GroupBy(entry => Root(entry.NpcId))
        .Select(group => new Lock
        {
            NpcId = group.Key,
            LockSeconds = group.Any(entry => entry.LockSeconds == -1)
                ? -1 : group.Max(entry => entry.LockSeconds)
        });
}
