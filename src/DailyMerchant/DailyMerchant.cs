using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace DM;

/// <summary>
/// 商人每日到访（DailyMerchant）
///
/// 一、旅商（NPC 368，流动旅商）
/// 原版里旅商每 tick 掷一次 1/108000，窗口是上午 4:30–12:00（27000 tick），
/// 所以一天只有 1-(107999/108000)^27000 ≈ 22.12% 的机会；入睡时概率 ×5，日晷 / Moondial 生效时永不出现。
/// 插件不自己造 NPC，只是把"每天必然到访"做成默认：
///   * 每秒扫一次，判定条件与原版逐条对齐（白天、窗口内、城镇 NPC 数量、日晷不生效）；
///   * 条件满足就直接调用游戏自己的 WorldGen.SpawnTravelNPC()，
///     商品池（Chest.SetupTravelShop + SendTravelShop）、客户端同步、到访/离开播报全部走原版；
///   * 条件不满足（例如城镇 NPC 还没房子）就静默跳过，不做任何强行补位。
/// 黄昏离场、月食/入侵暂停、天黑后原版自动请他离开，这些都是原版行为，插件不重复实现。
///
/// 二、骷髅商人（NPC 453）
/// 原版的骷髅商人只在地牢里随机刷出，没有每日到访这回事。插件只做一件事：
/// 白天、出生点附近有玩家、场上又没有骷髅商人的时候，在出生点附近的固定锚点生成一只。
/// 除此之外一概不管：
///   * 玩家走远了、晚上没人的时候，他会不会消失——由原版自己的"离所有玩家 2000 像素就卸载"决定，
///     插件不干预、也不补位；
///   * 场上已经有骷髅商人（洞穴刷到的、别的插件召唤的、昨天留下的）时不再生成第二只；
///   * 被打死了也不管，等白天有人走近出生点时自然再来一只。
///   * 位置：出生点附近逐圈搜索出来的固定锚点（脚下实地、身体两格与两侧为空、不泡水、
///     不在地牢砖/蓝砖上）；种子按队伍分配出生点时（Main.teamBasedSpawnsSeed）从所有出生点里随机挑一个；
///   * 货品完全沿用原版：客户端按 Main.moonPhase 现场计算（Chest.SetupShop + ShopHelper
///     .GetSkeletonMerchantPrices），插件不缓存、不广播、不改写任何商店数据。
///
/// 两者各有一个开关（配置文件 DailyMerchant.json，默认都开），关掉哪个就完全不做哪边的事。
/// </summary>
[ApiVersion(2, 1)]
public class DailyMerchantPlugin : TerrariaPlugin
{
    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "不是现在";
    public override Version Version => new(1, 1);
    public override string Description =>
        GetString("让旅商与骷髅商人每天到访：原版流动旅商每天只有约 22% 机会出现，这里改成每天必到；骷髅商人白天在出生点附近有人时就来一只。");

    public DailyMerchantPlugin(Main game) : base(game) { }

    private const string LogPrefix = "[DailyMerchant] ";

    /// <summary>到访窗口：上午 4:30 起 450 分钟 = 中午 12 点，与原版一致（只用于旅商）。</summary>
    private const int ArrivalWindowMinutes = 450;

    /// <summary>
    /// 判定间隔（毫秒）。用真实时间而不是"每 N 帧"：GameUpdate 每帧触发一次，
    /// 60 FPS 下每秒 60 次，按 20 帧算只有 0.33 秒。
    /// </summary>
    private const long ScanIntervalMs = 1000;

    /// <summary>骷髅商人锚点搜索：以出生点为圆心向外找可站立的位置，起始圈数与常规最大圈数。</summary>
    private const int AnchorSearchStart = 2;
    private const int AnchorSearchRadius = 40;

    /// <summary>所有出生点都找不到位置时的兜底搜索半径。</summary>
    private const int AnchorSearchFallbackRadius = 120;

    /// <summary>
    /// "出生点附近有人"的判定半径（像素）。
    /// 取原版按距离卸载 NPC 的同一个半径（2000 像素 = 125 格）：这么近的玩家在，
    /// 原版就不会把刚生成的骷髅商人卸载掉——所以我们只在这种时候生成，不会"刚出现就自己没了"。
    /// </summary>
    private const float PresenceRadiusPx = 2000f;

    /// <summary>锚点与地图边缘保持的距离。</summary>
    private const int MapMargin = 25;

    internal static DailyMerchantConfig Config = new();

    private long _lastScanAt;
    private bool _worldReady;

    // ---- 旅商状态
    private bool _arrivedThisDay;
    private bool _wasDayTime = true;
    private bool _warnedNoHousing;

    // ---- 骷髅商人锚点（算一次就固定下来，避免每秒重算）
    private static bool _anchorCached;
    private static Point _anchorTile;
    private static Point _anchorSpawn;

    // ------------------------------------------------------------------ 生命周期

    public override void Initialize()
    {
        LoadConfig();
        GeneralHooks.ReloadEvent += ReloadConfig;

        ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);

        TShock.Log.ConsoleInfo(
            $"{LogPrefix}v{Version} 已加载：旅商 NPC ID = {NPCID.TravellingMerchant}（流动旅商），每天到访 100%（原版 22.12%）；" +
            $"骷髅商人 NPC ID = {NPCID.SkeletonMerchant}，白天出生点附近有人时在固定锚点生成一只，走远/入夜后的去留由原版决定。" +
            $"开关在 DailyMerchant.json（默认都开）。");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= ReloadConfig;
            ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
        }

        base.Dispose(disposing);
    }

    // ------------------------------------------------------------------ 配置

    private static void LoadConfig() => Config = DailyMerchantConfig.Read();

    private static void ReloadConfig(ReloadEventArgs args)
    {
        LoadConfig();
        args.Player.SendInfoMessage($"§e[旅商] §7{Format(GetString("配置已重载：旅商 {0}，骷髅商人 {1}"),
            Config.TravellingMerchantEnabled ? GetString("开") : GetString("关"),
            Config.SkeletonMerchantEnabled ? GetString("开") : GetString("关"))}");
    }

    // ------------------------------------------------------------------ 每秒扫描

    private void OnUpdate(EventArgs args)
    {
        // Terraria 空服不推进世界（GameUpdate 也不触发），有人进服后这里会自动补上判定。
        if (!EnsureWorldReady())
            return;

        if (Environment.TickCount64 - _lastScanAt < ScanIntervalMs)
            return;
        _lastScanAt = Environment.TickCount64;

        RunScan();
    }

    /// <summary>
    /// 世界没载入（主菜单、世界数据还没准备好）时返回 false，此时不做任何判定。
    /// 只有在"就绪状态发生变化"的那一次才重置状态，之后都是空操作——
    /// 否则空服时第一个判定来自 /merchant 这类命令，会把命令刚记下的东西顺手清掉。
    /// </summary>
    private bool EnsureWorldReady()
    {
        bool ready = !Main.gameMenu && Main.maxTilesX > 100;
        if (ready == _worldReady)
            return ready;

        _worldReady = ready;
        if (!ready)
        {
            _anchorCached = false;
            return false;
        }

        _arrivedThisDay = false;
        _anchorCached = false;
        TShock.Log.ConsoleDebug($"{LogPrefix}世界已载入（{(Main.dayTime ? "白天" : "夜晚")}），开始判定到访。");
        return true;
    }

    /// <summary>
    /// 一次到访判定。每秒自动跑一次，也可以用 /merchant check 手动跑一次
    /// （空服时世界不推进，自动判定不会触发，测试或排查时可用手动触发）。
    /// </summary>
    private void RunScan()
    {
        // 天黑 = 今天结束，旅商名额重置。
        if (Main.dayTime != _wasDayTime)
        {
            if (!Main.dayTime)
            {
                _arrivedThisDay = false;
                _warnedNoHousing = false;
            }

            _wasDayTime = Main.dayTime;
        }

        TravelScan();
        SkeletonScan();
    }

    // ============================================================== 旅商（NPC 368）

    private void TravelScan()
    {
        if (!Config.TravellingMerchantEnabled)
            return;

        if (_arrivedThisDay || CountMerchants() > 0)
            return;

        // ---- 与原版逐条对齐的条件；任何一条不满足就静默跳过，等下一次判定。
        if (!Main.dayTime)
            return;

        if (Main.time >= ArrivalWindowMinutes * 60.0)
            return;

        if (Main.IsFastForwardingTime())   // 日晷 / Moondial 生效时原版不掷骰
            return;

        if (CountTownNpcs() < 2)
            return;

        // 最后一步交给原版：它自己判断月食/入侵/有没有已入住的 NPC。
        // 不满足就什么都不做（静默跳过），等城镇条件变好了自然会来。
        if (!TryVanillaSpawn())
        {
            if (!_warnedNoHousing)
            {
                _warnedNoHousing = true;
                TShock.Log.ConsoleDebug($"{LogPrefix}原版暂未放行旅商（城镇里还没有落脚点），静默等待中…");
            }

            return;
        }

        // 到访/离场由游戏自己播报，这里不再往控制台刷常态输出。
        _arrivedThisDay = true;
        _warnedNoHousing = false;
    }

    /// <summary>原版的城镇 NPC 数量要求：不含护士(37)、骷髅旅商(453)、旅商自己。</summary>
    private static int CountTownNpcs()
    {
        int count = 0;

        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC? npc = Main.npc[i];
            if (npc == null || !npc.active || !npc.townNPC || npc.life <= 0)
                continue;

            int type = npc.netID;
            if (type == 37 || type == NPCID.SkeletonMerchant || type == NPCID.TravellingMerchant)
                continue;

            count++;
        }

        return count;
    }

    /// <summary>调用原版生成。返回 false 表示原版拒绝（条件不满足）。</summary>
    private static bool TryVanillaSpawn()
    {
        int before = CountMerchants();
        WorldGen.SpawnTravelNPC();
        return CountMerchants() > before;
    }

    private static int CountMerchants()
    {
        int count = 0;

        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC? npc = Main.npc[i];
            if (npc != null && npc.active && npc.life > 0 && npc.netID == NPCID.TravellingMerchant)
                count++;
        }

        return count;
    }

    // ============================================================== 骷髅商人（NPC 453）

    /// <summary>
    /// 骷髅商人的全部判定——就这一条：
    ///
    ///   白天  +  场上没有骷髅商人  +  锚点附近（2000 像素）有玩家  →  在固定锚点生成一只
    ///
    /// 其余情况插件一律什么都不做：
    ///   * 玩家走远了他会不会消失：原版"离所有玩家 2000 像素就卸载"，插件不管；
    ///   * 晚上有人就留着、没人就让他走：同样是原版按距离卸载，插件不管，也不补位。
    ///
    /// "有人才生成"这一条同时保证不会刚出现就被原版清掉——这么近的玩家在，原版不会卸载他。
    /// </summary>
    private void SkeletonScan()
    {
        if (!Config.SkeletonMerchantEnabled)
            return;

        if (!Main.dayTime)          // 晚上不生成
            return;

        if (CountSkeletons() > 0)   // 已经有一个了（洞穴刷到的、昨天留下的、别的插件召唤的）
            return;

        if (!TryGetAnchor(out Point tile, out Point spawn))
        {
            TShock.Log.ConsoleDebug($"{LogPrefix}出生点附近找不到能站立的位置（常规半径 {AnchorSearchRadius} 格、兜底半径 {AnchorSearchFallbackRadius} 格都试过），本次跳过。");
            return;
        }

        // 锚点附近没人就先不生成：没人看着他，生成出来也只会被原版卸载。
        if (CountPlayersNear(AnchorWorld(tile), PresenceRadiusPx) == 0)
            return;

        if (!SpawnSkeleton(tile, out _))
        {
            TShock.Log.ConsoleDebug($"{LogPrefix}生成失败（NPC 槽位不足或位置被占），稍后重试。");
            return;
        }

        Event($"骷髅商人已出现：出生点 ({spawn.X}, {spawn.Y}) → 位置 ({tile.X}, {tile.Y})。");
    }

    /// <summary>
    /// 留一条排查用的线索，但只走 ConsoleDebug：它既不写日志文件也不往控制台刷，
    /// 插件在服务器上保持安静。加载时那行横幅之外，任何运行期事件都不输出。
    /// </summary>
    private static void Event(string message) => TShock.Log.ConsoleDebug($"{LogPrefix}{message}");

    private static Vector2 AnchorWorld(Point tile) => new(tile.X * 16, tile.Y * 16);

    private static int CountPlayersNear(Vector2 position, float radius)
    {
        int count = 0;

        for (int i = 0; i < Main.maxPlayers; i++)
        {
            Player? player = Main.player[i];
            if (player == null || !player.active)
                continue;

            if (Vector2.Distance(player.position, position) <= radius)
                count++;
        }

        return count;
    }

    private static bool SpawnSkeleton(Point tile, out int index)
    {
        index = NPC.NewNPC(new EntitySource_SpawnNPC(), tile.X * 16 + 8, tile.Y * 16, NPCID.SkeletonMerchant);
        if (index < 0 || index >= Main.maxNPCs)
        {
            index = -1;
            return false;
        }

        NPC npc = Main.npc[index];

        // 他不是城镇 NPC（townNPC = false），但跑城镇 AI；
        // 按无家 NPC 处理才不会去寻路回"家"，与原版造旅商时的做法一致。
        npc.homeless = true;
        npc.netUpdate = true;

        if (Main.netMode == 2)
            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, index);

        return true;
    }

    /// <summary>
    /// 锚点只算一次就固定下来：同一个出生点每次算出来的位置是同一个格子，缓存起来也省得每秒重搜一圈。
    /// 世界切换（载入 / 退出）时由 EnsureWorldReady 作废。
    /// </summary>
    private static bool TryGetAnchor(out Point tile, out Point spawn)
    {
        if (_anchorCached)
        {
            tile = _anchorTile;
            spawn = _anchorSpawn;
            return true;
        }

        if (!TryFindAnchor(out tile, out spawn))
        {
            tile = Point.Zero;
            spawn = Point.Zero;
            return false;
        }

        _anchorTile = tile;
        _anchorSpawn = spawn;
        _anchorCached = true;
        return true;
    }

    /// <summary>
    /// 找一个能站人的出现位置。返回实际选中的出生点，便于排查。
    ///
    /// 步骤：收集候选出生点 → 随机排序 → 每个出生点先按常规半径找 → 全都不行再放宽半径重来一次。
    /// 候选出生点本身往往是站不住人的（世界生成只保证"大致能站"），所以每个都要再搜一圈。
    /// </summary>
    private static bool TryFindAnchor(out Point tile, out Point spawn)
    {
        tile = Point.Zero;
        spawn = Point.Zero;

        List<Point> candidates = SpawnCandidates();
        if (candidates.Count == 0)
            return false;

        foreach (Point candidate in candidates)
        {
            if (SearchNear(candidate, AnchorSearchRadius, out tile))
            {
                spawn = candidate;
                return true;
            }
        }

        // 兜底：有些种子的出生点周围几十格都站不住人，放宽到 120 格再试一遍。
        foreach (Point candidate in candidates)
        {
            if (SearchNear(candidate, AnchorSearchFallbackRadius, out tile))
            {
                spawn = candidate;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 出生点候选：
    /// - "按队伍分配出生点"的种子（Main.teamBasedSpawnsSeed）有多个出生点，
    ///   存在 ExtraSpawnPointManager.extraSpawnPoints 里（下标 0 是主出生点），随机挑一个；
    /// - 普通种子只有世界出生点 Main.spawnTileX / Main.spawnTileY。
    /// 返回的顺序是随机的，调用方按顺序试即可。
    /// </summary>
    private static List<Point> SpawnCandidates()
    {
        var list = new List<Point>();

        if (Main.teamBasedSpawnsSeed)
        {
            Point[] extra = ExtraSpawnPointManager.extraSpawnPoints;
            for (int i = 0; i < extra.Length; i++)
            {
                Point p = extra[i];
                if (p.X <= 0 || p.Y <= 0 || p.X >= Main.maxTilesX || p.Y >= Main.maxTilesY)
                    continue;

                if (!list.Contains(p))
                    list.Add(p);
            }
        }

        if (list.Count == 0)
        {
            Point main = new(Main.spawnTileX, Main.spawnTileY);
            if (main.X > 0 && main.Y > 0 && main.X < Main.maxTilesX && main.Y < Main.maxTilesY)
                list.Add(main);
        }

        // Fisher-Yates：多出生点种子每天随机落在其中一个。
        Random rng = Random.Shared;
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }

        return list;
    }

    /// <summary>
    /// 以某个出生点为圆心，由近及远逐圈查找能站立的位置。
    /// 顺序只取决于出生点坐标，所以同一个出生点每次算出来的锚点都是同一个格子。
    /// </summary>
    private static bool SearchNear(Point center, int maxRadius, out Point tile)
    {
        tile = Point.Zero;

        for (int r = AnchorSearchStart; r <= maxRadius; r++)
        {
            for (int dy = -r; dy <= r; dy++)
            {
                for (int dx = -r; dx <= r; dx++)
                {
                    if (Math.Abs(dx) != r && Math.Abs(dy) != r)
                        continue;   // 只看当前这一圈

                    if (!CanStand(center.X + dx, center.Y + dy))
                        continue;

                    tile = new Point(center.X + dx, center.Y + dy);
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 能不能站人：脚下是实地、脚下没有液体、自己和两侧身体位置没有方块也没有水。
    /// 额外排掉地牢砖和 226 号砖（蓝砖）——原版挑队伍出生点时同样排掉它们。
    /// </summary>
    private static bool CanStand(int x, int y)
    {
        if (x < MapMargin || x >= Main.maxTilesX - MapMargin ||
            y < 20 || y >= Main.maxTilesY - MapMargin)
            return false;

        if (!WorldGen.SolidTile(x, y + 1))
            return false;

        // 脚下这块不能是地牢 / 蓝砖。
        ITile floor = Main.tile[x, y + 1];
        if (Main.tileDungeon[floor.type] || floor.type == 226)
            return false;

        if (WorldGen.SolidTile(x, y) || WorldGen.SolidTile(x, y - 1))
            return false;

        if (WorldGen.SolidTile(x - 1, y) || WorldGen.SolidTile(x + 1, y))
            return false;

        // 脚下、身体、两侧都不能泡在液体里（地面方块也算：被水淹没的实心方块不能当落脚点）。
        if (floor.liquid > 0 || Main.tile[x, y].liquid > 0 || Main.tile[x, y - 1].liquid > 0)
            return false;

        if (Main.tile[x - 1, y].liquid > 0 || Main.tile[x + 1, y].liquid > 0)
            return false;

        return true;
    }

    private static int CountSkeletons()
    {
        int count = 0;

        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC? npc = Main.npc[i];
            if (npc != null && npc.active && npc.life > 0 && npc.netID == NPCID.SkeletonMerchant)
                count++;
        }

        return count;
    }

    /// <summary>
    /// 带占位符的文案统一这样拼：先取译文（没有译文时返回原文模板），再自己填值。
    /// 不能写成 GetString($"…{0}…", args) —— GetText.NET 8 的 FormattableStringAdapter 会先把
    /// 插值算好再查表，位置参数 {0} 会被当成"没有这个实参"填成 0。
    /// 也不能直接写 GetString($"…{expr}…") —— 这样查表用的键是渲染后的文本，译文永远命中不了。
    /// </summary>
    private static string Format(string template, params object?[] args)
        => string.Format(template, args);
}
