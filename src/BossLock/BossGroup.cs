namespace BossLock;

internal static class BossGroup
{
    // 主体、肢体和专属附属单位共用同一进度；保留旧配置/数据库中的部件 ID 兼容性。
    private static readonly int[][] Groups =
    [
        [13, 14, 15], [35, 36], [113, 114, 115, 116],
        [125, 126], [127, 128, 129, 130, 131], [134, 135, 136, 139],
        [262, 263, 264], [266, 267], [245, 246, 247, 248, 249],
        [327, 328], [395, 392, 393, 394], [398, 396, 397, 400],
        [439, 440], [491, 492]
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
