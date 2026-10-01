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
/// 原版的骷髅商人只在地牢里随机刷出，没有每日到访这回事。插件让他每天上午 4:30
/// 在出生点附近出现一次；离场条件：时间到了（天黑）而且附近没有玩家，他才走。
///   * 位置：出生点附近逐圈搜索出来的固定锚点（脚下实地、身体两格与两侧为空、不泡水、
///     不在地牢砖/蓝砖上）；种子按队伍分配出生点时（Main.teamBasedSpawnsSeed）从所有出生点里随机挑一个；
///   * 出生点站不住人时先按 40 格找，全都不行再放宽到 120 格；
///   * 货品完全沿用原版：客户端按 Main.moonPhase 现场计算（Chest.SetupShop + ShopHelper
///     .GetSkeletonMerchantPrices），而 moonPhase 在每天清晨 4:30 由 Main.UpdateTime 递增一次，
///     所以插件不缓存、不广播、不改写任何商店数据；
///   * 场上已经有骷髅商人（洞穴刷到的、别的插件召唤的）时当天不生成第二只；
///   * 一个游戏日只来一次，被打死也不补位。
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
        GetString("让旅商与骷髅商人每天到访：原版流动旅商每天只有约 22% 机会出现，这里改成每天必到；骷髅商人每天上午固定出现在出生点附近。");

    public DailyMerchantPlugin(Main game) : base(game) { }

    private const string LogPrefix = "[DailyMerchant] ";
    private const string CommandPermission = "tshock.admin";

    /// <summary>到访窗口：上午 4:30 起 450 分钟 = 中午 12 点，与原版一致。</summary>
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

    /// <summary>补位半径（像素）：出生点附近这个距离内有玩家，被卸载了才补回来。</summary>
    private const float RespawnRadiusPx = 6000f;

    /// <summary>两次补位之间的最小间隔（毫秒），防止来回抖动。</summary>
    private const long RespawnCooldownMs = 5000;

    /// <summary>骷髅商人离场半径（像素）：天黑之后，800 像素内没人他才走。</summary>
    private const float LeaveRadiusPx = 800f;

    /// <summary>锚点与地图边缘保持的距离。</summary>
    private const int MapMargin = 25;

    internal static DailyMerchantConfig Config = new();

    private Command? _command;
    private long _lastScanAt;
    private bool _worldReady;
    private long _scanCount;

    // ---- 旅商状态
    private bool _arrivedThisDay;
    private bool _wasDayTime = true;
    private bool _warnedNoHousing;

    // ---- 骷髅商人状态
    private bool _skeletonArrivedThisDay;
    private static int _skeletonIndex = -1;
    private static long _lastRespawnAt;
    private bool _skeletonAtDay;
    private double _skeletonAtTime;
    private int _skeletonAtMoonPhase;

    // ------------------------------------------------------------------ 生命周期

    public override void Initialize()
    {
        LoadConfig();
        GeneralHooks.ReloadEvent += ReloadConfig;

        RegisterCommand();
        ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);

        TShock.Log.ConsoleInfo(
            $"{LogPrefix}v{Version} 已加载：旅商 NPC ID = {NPCID.TravellingMerchant}（流动旅商），每天到访 100%（原版 22.12%）；" +
            $"骷髅商人 NPC ID = {NPCID.SkeletonMerchant}，每天上午 4:30 起在出生点附近出现一次、天黑离场。" +
            $"开关在 DailyMerchant.json（默认都开），命令 /merchant（权限 {CommandPermission}）。");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= ReloadConfig;
            ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
            RemoveCommand();
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
    /// 只有在"就绪状态发生变化"的那一次才重置当天状态，之后都是空操作——
    /// 否则空服时第一个判定来自 /merchant 这类命令，会把命令刚记下的东西顺手清掉。
    /// </summary>
    private bool EnsureWorldReady()
    {
        bool ready = !Main.gameMenu && Main.maxTilesX > 100;
        if (ready == _worldReady)
            return ready;

        _worldReady = ready;
        if (!ready)
            return false;

        _arrivedThisDay = false;
        _skeletonArrivedThisDay = false;
        TShock.Log.ConsoleDebug($"{LogPrefix}世界已载入（{(Main.dayTime ? "白天" : "夜晚")}），开始判定到访。");
        return true;
    }

    /// <summary>
    /// 一次到访判定。每秒自动跑一次，也可以用 /merchant check 手动跑一次
    /// （空服时世界不推进，自动判定不会触发，测试或排查时可用手动触发）。
    /// </summary>
    private void RunScan()
    {
        _scanCount++;

        // 天黑 = 今天结束，两个商人的名额一起重置。
        if (Main.dayTime != _wasDayTime)
        {
            if (!Main.dayTime)
            {
                _arrivedThisDay = false;
                _warnedNoHousing = false;
                _skeletonArrivedThisDay = false;
            }

            _wasDayTime = Main.dayTime;
        }

        UpdateSkeletonDay();

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
                TShock.Log.ConsoleDebug($"{LogPrefix}{LastVanillaBlock}；静默等待中…");
            }

            return;
        }

        // 到访/离场由游戏自己播报，这里不再往控制台刷常态输出。
        _arrivedThisDay = true;
        _warnedNoHousing = false;
    }

    /// <summary>把游戏内时刻换算成 HH:MM：白天 0 刻 = 4:30，夜晚 0 刻 = 19:30。</summary>
    private static string Clock()
    {
        double hours = (Main.dayTime ? 4.5 + Main.time / 3600.0 : 19.5 + Main.time / 3600.0) % 24.0;
        int h = (int)hours;
        int m = (int)((hours - h) * 60.0);
        return $"{h:00}:{m:00}";
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

    /// <summary>统计城镇 NPC：总数 / 已入住 / 虽无房但有空房可搬（仅用于 status 诊断）。</summary>
    private static void Town(out int town, out int housed, out int withRoom)
    {
        town = housed = withRoom = 0;

        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC? npc = Main.npc[i];
            if (npc == null || !npc.active || !npc.townNPC || npc.life <= 0)
                continue;

            int type = npc.netID;
            if (type == 37 || type == NPCID.SkeletonMerchant || type == NPCID.TravellingMerchant)
                continue;

            town++;

            if (!npc.homeless && (npc.homeTileX > 0 || npc.homeTileY > 0))
            {
                housed++;
                continue;
            }

            if (WorldGen.TownManager.HasRoom(type, out _))
                withRoom++;
        }
    }

    /// <summary>调用原版生成。返回 false 表示原版拒绝（条件不满足）。</summary>
    private static bool TryVanillaSpawn()
    {
        int before = CountMerchants();
        WorldGen.SpawnTravelNPC();
        bool ok = CountMerchants() > before;
        LastVanillaBlock = ok ? GetString("原版已放行") : ExplainVanillaBlock();
        return ok;
    }

    /// <summary>按原版 SpawnTravelNPC 的判断顺序逐条复算，说明它到底卡在哪一步。</summary>
    private static string ExplainVanillaBlock()
    {
        if (Main.eclipse)
            return GetString("原版拒绝：月食天不生成旅商");

        if (!Main.dayTime)
            return GetString("原版拒绝：夜晚不生成旅商");

        if (Main.invasionType > 0 && Main.invasionDelay == 0 && Main.invasionSize > 0)
            return GetString("原版拒绝：入侵进行中");

        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC? npc = Main.npc[i];
            if (npc != null && npc.active && npc.type == NPCID.TravellingMerchant)
                return Format(GetString("原版拒绝：场上已有旅商（槽位 {0}，生命 {1}）"), i, npc.life);
        }

        int homes = 0;
        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC? npc = Main.npc[i];
            if (npc != null && npc.active && npc.townNPC && npc.type != 37 && !npc.homeless)
                homes++;
        }

        if (homes == 0)
        {
            int rooms = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC? npc = Main.npc[i];
                if (npc != null && npc.active && npc.townNPC && npc.type != 37 && npc.homeless &&
                    WorldGen.TownManager.HasRoom(npc.type, out _))
                    rooms++;
            }

            return rooms == 0
                ? GetString("原版拒绝：既没有已入住的城镇 NPC，也没有可搬入的空房")
                : GetString("原版拒绝：TownManager 找不到可用的落脚点");
        }

        // 走到这里原版一定会调 NPC.NewNPC；如果仍然没出现，多半是生成点被占或槽位耗尽。
        int active = 0;
        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC? npc = Main.npc[i];
            if (npc != null && npc.active)
                active++;
        }

        return Format(GetString("原版已通过全部条件（有 {0} 位已入住 NPC）但仍没生成：活跃 NPC {1}/{2}"), homes, active, Main.maxNPCs);
    }

    /// <summary>最近一次原版生成的结果说明，status 里会显示。</summary>
    private static string LastVanillaBlock { get; set; } = "尚未尝试";

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

    private static string Where(int netId)
    {
        var spots = new List<string>();

        for (int i = 0; i < Main.maxNPCs && spots.Count < 5; i++)
        {
            NPC? npc = Main.npc[i];
            if (npc == null || !npc.active || npc.life <= 0 || npc.netID != netId)
                continue;

            spots.Add($"({(int)(npc.position.X / 16f)}, {(int)(npc.position.Y / 16f)})");
        }

        return spots.Count == 0 ? GetString("当前不在场") : string.Join("、", spots);
    }

    /// <summary>当前"能不能来"的诊断文案，供 status 使用。</summary>
    private static bool InWindow() =>
        Main.dayTime && Main.time < ArrivalWindowMinutes * 60.0 && !Main.IsFastForwardingTime();

    private static string BlockReason(bool inWindow)
    {
        if (!Main.dayTime)
            return GetString("现在是夜晚（原版入夜后不再来）");
        if (Main.IsFastForwardingTime())
            return GetString("日晷 / Moondial 生效中（原版此时不掷骰）");
        if (!inWindow)
            return Format(GetString("已过上午4:30 + {0} 分钟"), ArrivalWindowMinutes);
        if (Main.eclipse)
            return GetString("月食天不生成旅商");
        if (Main.invasionType > 0 && Main.invasionDelay == 0 && Main.invasionSize > 0)
            return GetString("入侵进行中（原版此时不生成旅商）");

        int town = CountTownNpcs();
        if (town < 2)
            return Format(GetString("城镇里只有 {0} 位 NPC，原版要求至少 2 位"), town);

        Town(out _, out int housed, out int withRoom);
        if (housed == 0 && withRoom == 0)
            return Format(GetString("城镇里有 {0} 位 NPC，但都没住进房子，原版没有落脚点（静默等待中）"), town);

        return GetString("条件已满足，正在按原版生成");
    }

    // ============================================================== 骷髅商人（NPC 453）

    /// <summary>
    /// 标记"骷髅商人今天的名额已用掉"，并记住当时的昼夜、时钟与月相。
    /// </summary>
    private void MarkSkeletonHandled()
    {
        _skeletonArrivedThisDay = true;
        _skeletonAtDay = Main.dayTime;
        _skeletonAtTime = Main.time;
        _skeletonAtMoonPhase = Main.moonPhase;
    }

    /// <summary>
    /// 判断骷髅商人是否已经进入新的一天，三个信号任一变化就算新的一天：
    /// 1. 昼夜不同 —— 采样到的翻转（正常游玩时每秒都会采到）；
    /// 2. 月相不同 —— 原版 Main.UpdateTime 在每天清晨 4:30 自己 +1，这是游戏自己的日计数器；
    /// 3. 时钟明显倒退 —— 管理员用 /time 跳回清晨。
    /// 必须在每次判定里跑，不能只靠 GameUpdate：空服时它根本不触发。
    /// </summary>
    private void UpdateSkeletonDay()
    {
        if (!_skeletonArrivedThisDay)
            return;

        if (Main.dayTime != _skeletonAtDay ||
            Main.moonPhase != _skeletonAtMoonPhase ||
            Main.time < _skeletonAtTime - 120.0)
            _skeletonArrivedThisDay = false;
    }

    private void SkeletonScan()
    {
        if (!Config.SkeletonMerchantEnabled)
            return;

        SkeletonUpdate();

        if (_skeletonArrivedThisDay)
            return;

        if (!Main.dayTime)
            return;

        if (Main.time >= ArrivalWindowMinutes * 60.0)
            return;

        if (Main.IsFastForwardingTime())   // 日晷 / Moondial 生效时原版不刷怪
            return;

        // 场上已经有骷髅商人（洞穴里刷到的、别的插件召唤的）：
        // 按需求"插件不加干预"，今天就让游戏自己来。
        if (CountSkeletons() > 0)
        {
            MarkSkeletonHandled();
            TShock.Log.ConsoleDebug($"{LogPrefix}场上已有骷髅商人，插件今天不干预。");
            return;
        }

        // 没人在线就不占用今天的机会，等有人进服再判定。
        if (!AnyPlayerOnline())
            return;

        if (!TryFindAnchor(out Point tile, out Point spawn))
        {
            TShock.Log.ConsoleDebug($"{LogPrefix}出生点附近找不到能站立的位置（常规半径 {AnchorSearchRadius} 格、兜底半径 {AnchorSearchFallbackRadius} 格都试过），本次跳过。");
            return;
        }

        if (!SpawnSkeleton(tile, out int index))
        {
            TShock.Log.ConsoleDebug($"{LogPrefix}生成失败（NPC 槽位不足或位置被占），稍后重试。");
            return;
        }

        _skeletonIndex = index;
        MarkSkeletonHandled();
        TShock.Log.ConsoleDebug($"{LogPrefix}骷髅商人已出现：出生点 ({spawn.X}, {spawn.Y}) → 位置 ({tile.X}, {tile.Y})。");
    }

    /// <summary>
    /// 每秒调用一次：维护跟踪状态 + 判定离场。
    ///
    /// 离场要同时满足两个条件：时间到了（天黑）**并且**附近 800 像素没有玩家。
    /// 只是天黑、身边还有人的时候他会照常站着，不会突然消失。
    ///
    /// 另外原版还有一套"按距离卸载"：NPC 离所有玩家 2000 像素（125 格）之外，
    /// 原版就会给他 timeLeft 倒计时，计时走完就把他清掉。骷髅商人不是城镇 NPC，照样适用，
    /// 所以玩家一走远他就没了（洞穴里原版的骷髅商人也是这样）。两个对策：
    ///   1. 每秒把 timeLeft 按回 0 —— 他就一直留在出生点；
    ///   2. 万一还是被清掉了（别的插件动过、换了游戏版本），只要有玩家在出生点附近就补回原位，
    ///      最多每 5 秒补一次，不会来回抖动；被打死的（血量归零）不补。
    /// </summary>
    private void SkeletonUpdate()
    {
        // 先看跟踪的那只还在不在。
        if (_skeletonIndex >= 0)
        {
            NPC? tracked = Main.npc[_skeletonIndex];

            if (tracked == null || !tracked.active || tracked.netID != NPCID.SkeletonMerchant)
            {
                bool killed = tracked != null && tracked.life <= 0;   // active=false 但血还在 = 被原版卸载
                _skeletonIndex = -1;

                if (!killed)
                    RespawnSkeletonIfNeeded();
            }
            else if (tracked.timeLeft > 0)
            {
                tracked.timeLeft = 0;   // 别让原版按距离把他卸载掉
            }
        }

        if (Main.dayTime)   // 时间没到，他照常站着
            return;

        if (_skeletonIndex < 0)
            return;

        NPC? npc = Main.npc[_skeletonIndex];

        // 时间到了，但附近还有人，就先不消失。
        if (CountPlayersNear(npc!.position) > 0)
            return;

        DespawnNpc(_skeletonIndex);
        _skeletonIndex = -1;
        TShock.Log.ConsoleDebug($"{LogPrefix}天黑且附近无人，骷髅商人离场。");
    }

    /// <summary>
    /// 兜底补位：他被原版按距离清掉了、而出生点附近还有玩家时，在同一个锚点重新生成。
    /// 打死的不补位。补位后玩家离锚点 2000 像素以上还会再被清，所以这里不无限补：
    /// 一是必须有玩家在附近才补，二是两次补位至少隔 5 秒。
    /// </summary>
    private void RespawnSkeletonIfNeeded()
    {
        if (!_skeletonArrivedThisDay || Main.dayTime)
            return;

        long now = Environment.TickCount64;
        if (now - _lastRespawnAt < RespawnCooldownMs)
            return;

        if (!TryFindAnchor(out Point tile, out Point spawn))
            return;

        Vector2 world = new(tile.X * 16, tile.Y * 16);
        if (CountPlayersNear(world, RespawnRadiusPx) == 0)
            return;   // 没人看着就不刷，避免来回抖动

        if (!SpawnSkeleton(tile, out int index))
            return;

        _skeletonIndex = index;
        _lastRespawnAt = now;
        TShock.Log.ConsoleDebug($"{LogPrefix}他被原版按距离清掉了，已在原位补回：出生点 ({spawn.X}, {spawn.Y}) → 位置 ({tile.X}, {tile.Y})。");
    }

    /// <summary>离场半径 800 像素（50 格），与原版 NPC 反卸载距离一致。</summary>
    private static int CountPlayersNear(Vector2 position, float radius = LeaveRadiusPx)
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

    private static void DespawnNpc(int index)
    {
        if (index < 0 || index >= Main.maxNPCs)
            return;

        NPC npc = Main.npc[index];
        npc.active = false;
        npc.life = 0;

        if (Main.netMode == 2)
            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, index);
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

        // 原版会把离所有玩家 2000 像素（125 格）之外的 NPC 慢慢卸载掉，骷髅商人不是城镇 NPC，
        // 照常适用——玩家一走远他就没了。timeLeft 就是原版那个卸载倒计时，
        // 每秒由 SkeletonUpdate 把它按回 0，他就能一直留在出生点待到天黑。
        npc.timeLeft = 0;

        if (Main.netMode == 2)
            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, index);

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

    private static bool AnyPlayerOnline()
    {
        for (int i = 0; i < Main.maxPlayers; i++)
        {
            Player? player = Main.player[i];
            if (player != null && player.active)
                return true;
        }

        return false;
    }

    /// <summary>骷髅商人当前"能不能来"的诊断文案，供 status 使用。</summary>
    private string SkeletonBlockReason()
    {
        if (Main.dayTime && CountSkeletons() > 0)
            return GetString("已在场（今天不再生成第二只）");

        if (_skeletonArrivedThisDay)
            return GetString("今天已经安排过了（不会再出现）");

        if (!Main.dayTime)
            return GetString("现在是夜晚（明天上午再来）");

        if (Main.IsFastForwardingTime())
            return GetString("日晷 / Moondial 生效中");

        if (Main.time >= ArrivalWindowMinutes * 60.0)
            return Format(GetString("已过上午4:30 + {0} 分钟"), ArrivalWindowMinutes);

        if (!AnyPlayerOnline())
            return GetString("暂无玩家在线，等有人进服再判定");

        return GetString("条件已满足，将出现在出生点附近");
    }

    /// <summary>
    /// 带占位符的文案统一这样拼：先取译文（没有译文时返回原文模板），再自己填值。
    /// 不能写成 GetString($"…{0}…", args) —— GetText.NET 8 的 FormattableStringAdapter 会先把
    /// 插值算好再查表，位置参数 {0} 会被当成"没有这个实参"填成 0。
    /// 也不能直接写 GetString($"…{expr}…") —— 这样查表用的键是渲染后的文本，译文永远命中不了。
    /// </summary>
    private static string Format(string template, params object?[] args)
        => string.Format(template, args);

    // ------------------------------------------------------------------ 命令

    private void RegisterCommand()
    {
        RemoveCommand();
        _command = new Command(CommandPermission, OnCommand, "merchant", "旅商", "dailymerchant")
        {
            HelpText = GetString("旅商与骷髅商人每日到访（/merchant summon|despawn|check|status）"),
            AllowServer = true
        };
        Commands.ChatCommands.Add(_command);
    }

    private void RemoveCommand()
    {
        if (_command == null)
            return;

        Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == OnCommand);
        _command = null;
    }

    private void OnCommand(CommandArgs args)
    {
        // 命令也算一次"世界已载入"的确认，否则首个命令的操作会被世界初始化顺手清掉。
        EnsureWorldReady();

        TSPlayer player = args.Player;
        string sub = args.Parameters.Count > 0 ? args.Parameters[0].ToLowerInvariant() : "help";

        switch (sub)
        {
            case "help" or "帮助" or "?":
                SendHelp(player);
                break;

            case "summon" or "s" or "召唤" or "来":
                if (!Config.TravellingMerchantEnabled)
                {
                    player.SendInfoMessage(GetString("旅商每日到访已在配置里关闭。"));
                }
                else if (CountMerchants() > 0)
                {
                    player.SendInfoMessage(GetString("场上已经有旅商了。"));
                }
                else if (TryVanillaSpawn())
                {
                    player.SendInfoMessage($"§e[旅商] §7{GetString("已召唤，旅商现在在城镇里。")}");
                }
                else
                {
                    // 召唤是管理员显式操作，这里必须说清楚为什么没成。
                    player.SendErrorMessage($"§e[旅商] §7{GetString("召唤失败：")}{BlockReason(InWindow())}。");
                }

                break;

            case "despawn" or "d" or "离开" or "送走":
                if (CountMerchants() == 0)
                {
                    player.SendInfoMessage(GetString("场上本来就没有旅商。"));
                    break;
                }

                WorldGen.UnspawnTravelNPC();   // 原版自己播报"xxx 离开了"
                _arrivedThisDay = false;
                player.SendInfoMessage($"§e[旅商] §7{GetString("已请旅商离场。")}");
                break;

            case "status" or "st" or "状态":
                SendStatus(player);
                break;

            case "check" or "tick" or "检查" or "判定":
                // 空服时世界不推进、自动判定不会触发，这里手动跑一次（也是自动化测试的入口）。
                _lastScanAt = Environment.TickCount64;
                RunScan();
                player.SendInfoMessage($"§e[旅商] §7{GetString("已手动判定一次：")}§f{BlockReason(InWindow())}§7");
                player.SendInfoMessage($"§e[骷髅商人] §7{GetString("已手动判定一次：")}§f{SkeletonBlockReason()}§7");
                break;

            default:
                SendHelp(player);
                break;
        }
    }

    private static void SendHelp(TSPlayer player)
    {
        player.SendInfoMessage("§e[旅商] §7" + GetString("指令用法："));
        player.SendInfoMessage("§f/merchant summon §7— " + GetString("立刻召唤旅商（/merchant 召唤）"));
        player.SendInfoMessage("§f/merchant despawn §7— " + GetString("请旅商离场（/merchant 离开）"));
        player.SendInfoMessage("§f/merchant check §7— " + GetString("手动跑一次到访判定（/merchant 检查）"));
        player.SendInfoMessage("§f/merchant status §7— " + GetString("查看到访状态与城镇条件（/merchant 状态）"));
    }

    private void SendStatus(TSPlayer player)
    {
        DailyMerchantConfig config = Config;
        bool travelOn = config.TravellingMerchantEnabled;
        bool skeletonOn = config.SkeletonMerchantEnabled;
        bool inWindow = InWindow();
        Town(out int town, out int housed, out int withRoom);

        player.SendInfoMessage($"§e[旅商] §7{GetString("启用中")} · " +
                               $"{GetString("旅商 NPC ID =")} §f{NPCID.TravellingMerchant}§7 · " +
                               $"{GetString("每天到访")} §f{(travelOn ? "100%" : GetString("已关闭"))}§7（{GetString("原版 22.12%")}）· " +
                               $"{GetString("窗口")} §f{GetString("上午4:30 起")} {ArrivalWindowMinutes} {GetString("分钟")}§7");
        player.SendInfoMessage($"§e[旅商] §7{GetString("当前：")}{(Main.dayTime ? "白天" : "夜晚")} §f{Clock()}§7，" +
                               $"{GetString("场上旅商")} §f{CountMerchants()}§7 {GetString("位")}，{GetString("位置")} §f{Where(NPCID.TravellingMerchant)}§7");
        player.SendInfoMessage($"§e[旅商] §7{GetString("城镇：")}§f{town}§7 {GetString("位 NPC（已入住")} §f{housed}§7，" +
                               $"{GetString("待搬入空房")} §f{withRoom}§7）· {GetString("判定已跑")} §f{_scanCount}§7 " +
                               $"{GetString("次（每秒 1 次自动，空服不跑）")}");
        player.SendInfoMessage($"§e[旅商] §7{GetString("判定：")}§f{(travelOn ? BlockReason(inWindow) : GetString("已在配置里关闭"))}§7");
        player.SendInfoMessage($"§e[旅商] §7{GetString("原版生成：")}§f{LastVanillaBlock}§7");

        player.SendInfoMessage($"§e[骷髅商人] §7{GetString("启用中")} · " +
                               $"{GetString("骷髅商人 NPC ID =")} §f{NPCID.SkeletonMerchant}§7 · " +
                               $"{GetString("每天上午 4:30 起一次")} · " +
                               $"{GetString("离场条件")} §f{Format(GetString("天黑且 {0} 像素内无人"), LeaveRadiusPx)}§7");
        player.SendInfoMessage($"§e[骷髅商人] §7{GetString("当前：")}{(Main.dayTime ? "白天" : "夜晚")} §f{Clock()}§7，" +
                               $"{GetString("月相")} §f{Main.moonPhase}§7，" +
                               $"{GetString("场上骷髅商人")} §f{CountSkeletons()}§7 {GetString("位")}，{GetString("位置")} §f{Where(NPCID.SkeletonMerchant)}§7");
        player.SendInfoMessage($"§e[骷髅商人] §7{GetString("出生点：")}§f({Main.spawnTileX}, {Main.spawnTileY})§7，" +
                               $"{GetString("判定：")}§f{(skeletonOn ? SkeletonBlockReason() : GetString("已在配置里关闭"))}§7");
    }
}